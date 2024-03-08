using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Game.Core;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.JumpJump
{
    public class GameManager : MonoBehaviour
    {
        public SensorManager sensorManager;
        [Header("level Generator")] 
        public int seed;
        public List<GameObject> cubePrefs;
        public Transform firstCube;
        public int totalCubes;

        [Header("Left/Right Control UI")] 
        public TextMeshProUGUI textLeft;
        public TextMeshProUGUI textRight;
        public TextMeshProUGUI textGuide;
        
        private int currentCube = 0;
        private List<Transform> cubes = new List<Transform>();
        
        public bool IsStart { get; private set; }
        public bool IsWin { get; private set; }
        
        public SensorPosition currentController;
        
        // Start is called before the first frame update
        void Start()
        {
            textLeft.gameObject.SetActive(false);
            textRight.gameObject.SetActive(false);
            textGuide.gameObject.SetActive(false);
            IsStart = false;
            IsWin = false;
            sensorManager.gameStartEvent += GameStart;
        }

        private void OnDestroy()
        {
            sensorManager.gameStartEvent -= GameStart;
        }

        // Update is called once per frame
        void Update()
        {
            WinCheck();
        }
        
        private void GameStart()
        {
            StartCoroutine(Prepare());
        }
        
        private IEnumerator Prepare()
        {
            IsStart = true;
            GenerateLevels();
            yield return new WaitForSeconds(2f);
            
            textGuide.gameObject.SetActive(true);
            textGuide.text = "Move to Jump";
            yield return new WaitForSeconds(2f);
            textGuide.text = "Ready?";
            yield return new WaitForSeconds(1f);
            textGuide.text = "3";
            yield return new WaitForSeconds(1f);
            textGuide.text = "2";
            yield return new WaitForSeconds(1f);
            textGuide.text = "1";
            yield return new WaitForSeconds(1f);
            textGuide.text = "Go";
            yield return new WaitForSeconds(1f);
            
            textGuide.gameObject.SetActive(false);
            SetControlSensor(null);

            yield return new WaitForSeconds(.5f);
        }

        public void SetControlSensor(SensorPosition? position)
        {
            if (position != null) currentController = position.Value;
            else currentController = Random.Range(0, 1) == 0 ? SensorPosition.LEFT : SensorPosition.RIGHT;
            switch (currentController)
            {
                case SensorPosition.LEFT:
                    textLeft.gameObject.SetActive(true);
                    textRight.gameObject.SetActive(false);
                    break;
                case SensorPosition.RIGHT:
                    textLeft.gameObject.SetActive(false);
                    textRight.gameObject.SetActive(true);
                    break;
            }
        }
        
        public Transform GetCube(int idx)
        {
            return idx < cubes.Count ? cubes[idx] : null;
        }

        public Vector3 GetCurrentPosition()
        {
            Vector3 currentPosition = cubes[currentCube].position;
            currentPosition.y += cubes[currentCube].lossyScale.y +1;
            return currentPosition;
        }
        
        public Vector3 GetTargetPosition()
        {
            Vector3 targetPosition = cubes[currentCube + 1].position;
            targetPosition.y += cubes[currentCube + 1].lossyScale.y + 1;
            return targetPosition;
        }

        public void UpdateCurrentCube(int idx)
        {
            currentCube = idx;
            Debug.Log(currentCube);

            SetControlSensor(null);
        }
        
        private void GenerateLevels()
        {
            foreach (GameObject go in cubePrefs)
            {
                cubes.Add(go.transform);
            }
            
            Random.InitState(seed);

            Transform lastCube = firstCube;
            int cubeIdx = cubes.Count;
                
            while (cubeIdx < totalCubes)
            {
                int cubeInGroup = Random.Range(2, 10);
                float totalRotation = Random.Range(-100f, 100f);
                for (int i = 0; i < cubeInGroup; i++)
                {
                    if (cubeIdx == totalCubes) return;

                    GameObject cubePref = cubePrefs[Random.Range(0, cubePrefs.Count)];
                    GameObject newCube = Instantiate(cubePref, cubePref.transform.parent);
                    cubeIdx++;

                    newCube.transform.position = lastCube.position +
                                                 lastCube.right * Random.Range(-10f, 10f) +
                                                 lastCube.forward * Random.Range(6f, 15f) +
                                                 new Vector3(0, Random.Range(-5f, 5f), 0);
                    newCube.transform.Rotate(Random.Range(-10f, 10f), totalRotation / cubeInGroup * i, Random.Range(-5f, 5f));
                    newCube.name = "Cube" + cubeIdx;

                    cubes.Add(newCube.transform);
                    lastCube = newCube.transform;
                }
            }
        }
        
        private void WinCheck()
        {
            if (currentCube == cubes.Count - 1)
            {
                textGuide.gameObject.SetActive(true);
                textGuide.text = "Congratulations";

                IsWin = true;
            }
        }
        
    }
}