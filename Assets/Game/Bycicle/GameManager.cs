using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Game.Sensor;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using Game.Util;

namespace Game.Bicycle
{
    public class GameManager : MonoBehaviour
    {
        public SensorManager sensorManager;
        public GameStateManager gameStateManager;
        
        [Header("level Generator")] 
        public int seed;
        public List<GameObject> trackPrefs;
        public GameObject slopePref;
        public Transform firstTrack;
        public int totalSlopes;
        public int totalTracks;
        
        private int currentTrack = 0;
        private int lastTrack = 0;
        private List<Transform> tracks = new List<Transform>();
        
        public float MaxDistance { get; private set; }

        
        void Start()
        {
            gameStateManager.PrepareEvent += OnGameStart;
            gameStateManager.PrepareEvent += DisablePlayerControl;

            gameStateManager.PlayEvent += EnablePlayerControl;

            gameStateManager.EndEvent += DisablePlayerControl;
            
            gameStateManager.SwitchGameState(GameState.PREPARE);
            
        }

        private void OnDestroy()
        {
            gameStateManager.PrepareEvent -= OnGameStart;
            gameStateManager.PrepareEvent -= DisablePlayerControl;
            
            gameStateManager.PlayEvent -= EnablePlayerControl;
            
            gameStateManager.EndEvent -= DisablePlayerControl;
        }

        // Update is called once per frame
        void Update()
        {
            WinCheck();
        }
        
        
        public void EnableCameraFollow()
        {
            GameObject.Find("CameraOffset").GetComponent<CameraFollow>().enabled = true;
        }

        public void DisableCameraFollow()
        {
            GameObject.Find("CameraOffset").GetComponent<CameraFollow>().enabled = false;
        }

        
        public void EnablePlayerControl()
        {
            GameObject.Find("Bike").GetComponent<BicycleVehicle>().enabled = true;
        }

        public void DisablePlayerControl()
        {
            GameObject.Find("Bike").GetComponent<BicycleVehicle>().enabled = false;
        }

        
        
        private void OnGameStart()
        {
            StartCoroutine(Prepare());
        }
        
        private IEnumerator Prepare()
        {

            GenerateLevels();
            
            gameStateManager.SwitchGameState(GameState.PLAY);
            
            yield return new WaitForSeconds(.5f);
        }
        
        
        public Transform GetTrack(int idx)
        {
            return idx < tracks.Count ? tracks[idx] : null;
        }

        public Vector3 GetCurrentPosition()
        {
            Vector3 currentPosition = tracks[currentTrack].position;
            currentPosition.y += tracks[currentTrack].lossyScale.y + 1;
            return currentPosition;
        }
        
        public Vector3 GetTargetPosition()
        {
            Vector3 targetPosition = tracks[currentTrack + 1].transform.GetChild(0).position;
            targetPosition.y += 1;
            return targetPosition;
        }

        public void UpdateCurrentCube(int idx)
        {
            if (GameStateManager.State == GameState.PLAY)
            {
                currentTrack = idx;
                // Debug.Log(currentCube);

                if (lastTrack < currentTrack)
                {
                    lastTrack = currentTrack;
                }
            }
        }
        
        private void GenerateLevels()
        {
            foreach (GameObject go in trackPrefs)
            {
                tracks.Add(go.transform);
            }
            
            Random.InitState(seed);

            Transform lastCube = firstTrack;
            int trackIdx = tracks.Count;
            float lastTotalRotation = 0f;
            int slopeCount = 0;
                
            while (trackIdx < totalTracks)
            {
                int trackInGroup = Random.Range(15, 25);
                float totalRotation = Random.Range(-50f, 50f);
                for (int i = 0; i < trackInGroup; i++)
                {
                    if (trackIdx == totalTracks) return;

                    if (i > 17 && slopeCount < totalSlopes)
                    {
                        slopeCount++;
                        GameObject newSlope = Instantiate(slopePref);
                        
                        newSlope.transform.position = lastCube.position;
                        newSlope.transform.Rotate(0f, totalRotation / trackInGroup * i + lastTotalRotation, 0);
                        
                        lastCube.position = newSlope.transform.position + lastCube.forward * 23f;
                        lastCube.rotation = newSlope.transform.rotation;
                        
                        newSlope.transform.position += newSlope.transform.forward * 5f;
                        newSlope.transform.position += newSlope.transform.right * 3f;
                        
                        break;
                    }

                    GameObject cubePref = trackPrefs[Random.Range(0, trackPrefs.Count)];
                    GameObject newCube = Instantiate(cubePref, cubePref.transform.parent);
                    trackIdx++;

                    newCube.transform.position = lastCube.position +
                                                 lastCube.forward * 4f;
                    newCube.transform.Rotate(0f, totalRotation / trackInGroup * i + lastTotalRotation, 0);
                    newCube.name = "Track" + trackIdx;

                    tracks.Add(newCube.transform);

                    // if ((lastCube.transform.GetChild(0).position - newCube.transform.GetChild(0).position).magnitude > MaxDistance)
                    // {
                    //     MaxDistance = (lastCube.transform.GetChild(0).position - newCube.transform.GetChild(0).position).magnitude;
                    // }
                    //
                    lastCube = newCube.transform;
                }
                
                lastTotalRotation = lastCube.transform.rotation.eulerAngles.y;
            }
        }
        
        
        
        private void WinCheck()
        {
            if (currentTrack == tracks.Count - 1)
            {
                gameStateManager.SwitchGameState(GameState.END);
            }
        }
        
    }
}