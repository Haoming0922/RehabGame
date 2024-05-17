using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Game.Sensor;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using Game.Util;
using RehabDB;
using UnityEngine.InputSystem;
using Pico;
using Unity.XR.PXR;
using UnityEngine.SceneManagement;

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
        
        private int currentCube = 0;
        private int lastCube = 0;
        private List<Transform> cubes = new List<Transform>();
        
        public float MaxDistance { get; private set; }
        public SensorPosition currentController;
        
        public GameObject gameEndUI;

        public Action gameEndEvent;

        public GameState state;

        public JumpUIUpdater UIUpdater;

        public AvatarLoader avatarLoader;
        
                
        private float timeCount = 0;
        private float timePeriod = 0;

        private float leftPerformance;
        private float rightPerformance;
        private int coins;

        public Transform player;
        
        public InputActionReference Reload;
        public InputActionReference PauseGame;
        public InputActionReference Exit;
        private bool isPause;
        
        void Start()
        {
            seed = Random.Range(1, 100);
            GenerateLevels();
            if (DBManager.Instance.currentPatient != null)
            {
                avatarLoader.LoadAvatar(DBManager.Instance.currentPatient.UnityAvatar);
                // avatarLoader.SetupAvatarGame();
            }
            player.gameObject.SetActive(false);
            player.gameObject.SetActive(true);
            StartCoroutine(MoveToStart()); // TODO
            gameEndEvent += OnGameEnd;
        }

        private void OnEnable()
        {
            PauseGame.action.Enable();
            Reload.action.Enable();
            Exit.action.Enable();
        }
        
        private void OnDisable()
        {
            PauseGame.action.Disable();
            Reload.action.Disable();
            Exit.action.Disable();
        }
        
        private void OnDestroy()
        {
            gameEndEvent -= OnGameEnd;
        }

        // Update is called once per frame
        void Update()
        {
            TimeCounter();
            AvatarSpeak();
            ControllerInput();
        }
        

        private void TimeCounter()
        {
            if (state == GameState.PLAY)
            {
                timePeriod += Time.deltaTime;
                timeCount += Time.deltaTime;
            }
        }

        private void ControllerInput()
        {
            if (Exit.action.IsPressed()) SceneManager.LoadScene("Home");
            if (Reload.action.IsPressed()) SceneManager.LoadScene("Jump");
            if (PauseGame.action.IsPressed())
            {
                if (isPause)
                {
                    isPause = false;
                    Time.timeScale = 1;
                }
                else
                {
                    isPause = true;
                    Time.timeScale = 0;
                }
            }
        }


        private void ReloadScene(InputAction.CallbackContext ctx)
        {
            SceneManager.LoadScene("Jump");
        }

        
        private void AvatarSpeak()
        {
            if (currentCube == totalCubes / 2)
            {
                avatarLoader.Speak("mid_game_reminder", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            }
            else if (currentCube == totalCubes - 15)
            {
                timePeriod = 0;
                avatarLoader.Speak("middle_game",new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            }
            else if (currentCube == 10) 
            {
                avatarLoader.Speak("come_on", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            }
        }
        

        public void UpdatePlayerData(float input)
        {
            if(input < 0.2f) return;
            leftPerformance = leftPerformance == 0 ? leftPerformance : (leftPerformance + input) / 2; 
            rightPerformance = rightPerformance == 0 ? rightPerformance : (rightPerformance + input) / 2; 

            // if (position == SensorPosition.LEFT)
            // {
            //     leftPerformance = leftPerformance == 0 ? leftPerformance : (leftPerformance + input) / 2; 
            // }
            // else
            // {
            //     rightPerformance = rightPerformance == 0 ? rightPerformance : (rightPerformance + input) / 2; 
            // }
        }
        
        public void SetControlSensor(SensorPosition? position)
        {
            if (position != null) currentController = position.Value;
            else currentController =  currentCube % 2 == 0 ? SensorPosition.LEFT : SensorPosition.RIGHT;
            // UIUpdater.DisplayOneSide(currentController);
        }
        
        
        public Transform GetCube(int idx)
        {
            return idx < cubes.Count ? cubes[idx] : null;
        }

        public Vector3 GetCurrentPosition()
        {
            Vector3 currentPosition = cubes[currentCube].position;
            currentPosition.y += cubes[currentCube].lossyScale.y + 1;
            return currentPosition;
        }
        
        public Vector3 GetTargetPosition()
        {
            if (currentCube + 1 < cubes.Count)
            {
                Vector3 targetPosition = cubes[currentCube + 1].transform.GetChild(0).position;
                // targetPosition.y += 1f;
                return targetPosition;
            }
            else return player.position + player.forward;
        }

        public void UpdateCurrentCube(int idx)
        {
            currentCube = idx;
            // SetControlSensor(null);
            // Debug.Log(currentCube);

            if (lastCube < currentCube)
            {
                lastCube = currentCube;
            }
        
        }
        
        private void GenerateLevels()
        {
            foreach (GameObject go in cubePrefs)
            {
                cubes.Add(go.transform);
            }
            
            Random.InitState(seed);

            Transform lastCube = firstCube;
            int cubeIdx = cubes.Count-1;
                
            while (cubeIdx < totalCubes)
            {
                int cubeInGroup = Random.Range(2, 10);
                float totalRotation = Random.Range(-50f, 50f);
                
                for (int i = 0; i < cubeInGroup; i++)
                {
                    if (cubeIdx == totalCubes) break;

                    GameObject cubePref = cubePrefs[Random.Range(0, cubePrefs.Count)];
                    GameObject newCube = Instantiate(cubePref, cubePref.transform.parent);
                    cubeIdx++;

                    newCube.transform.position = lastCube.position +
                                                 lastCube.right * Random.Range(-4f, 4f) +
                                                 lastCube.forward * Random.Range(4f, 7f) +
                                                 new Vector3(0, Random.Range(-.4f, .4f), 0);
                    // newCube.transform.Rotate(Random.Range(-0.5f, 0.5f), totalRotation / cubeInGroup * i, Random.Range(-0.5f, 0.5f));
                    newCube.name = "Cube" + cubeIdx;

                    cubes.Add(newCube.transform);

                    if ((lastCube.transform.GetChild(0).position - newCube.transform.GetChild(0).position).magnitude > MaxDistance)
                    {
                        MaxDistance = (lastCube.transform.GetChild(0).position - newCube.transform.GetChild(0).position).magnitude;
                    }
                    
                    lastCube = newCube.transform;
                }

            }

            cubes[^1].gameObject.tag = "Finish";
            Debug.Log(cubes[^1].gameObject.tag);
        }

        public void DisableCurrentLastCube(int idx)
        {
            if(idx < 0 || idx >= cubes.Count) return;
            cubes[idx].gameObject.SetActive(false);
        }
        
        private IEnumerator MoveToStart()
        {
            gameEndUI.SetActive(false);
            state = GameState.PREPARE;
            
            yield return new WaitForSeconds(3f);
            avatarLoader.Speak("intro_jump_jump", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            yield return new WaitForSeconds(6f);
            UIUpdater.ShowMoveToStartUI();
            yield return new WaitForSeconds(3f);
            
            float move = sensorManager.GetData(SensorPosition.LEFT);
            while (move < 5)
            {
                move += Mathf.Max(sensorManager.GetData(SensorPosition.LEFT), sensorManager.GetData(SensorPosition.RIGHT));
                yield return null;
            }
            
            yield return StartCoroutine(UIUpdater.ShowGameStartUI());
            // SetControlSensor(null);
            state = GameState.PLAY;
        }
        
        private void OnGameEnd()
        {
            state = GameState.END;
            StartCoroutine(UIUpdater.ShowGameEndUI(timeCount, (rightPerformance/sensorManager.GetBaseline(SensorPosition.RIGHT) + leftPerformance/sensorManager.GetBaseline(SensorPosition.LEFT)) / 2 ,(rightPerformance  + leftPerformance) / 2 , coins));
            avatarLoader.Speak("finish_round_without_competition", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            
            // DBManager.Instance.currentPatient.UpdateGameTask("Jump Jump", timeCount);
            DBManager.Instance.currentPatient.UpdateGamePerformance("Jump Jump", leftPerformance, rightPerformance);
        }
        
        
    }
}