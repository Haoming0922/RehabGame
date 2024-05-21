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
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Game.Bicycle
{
    public class GameManager : MonoBehaviour{
        public AvatarLoader avatarLoader;
        public SensorManager sensorManager;
        
        private float timeCount = 0;
        private float timePeriod = 0;

        private float avgSpeed;
        private float leftPerformance;
        private int coins;
        
        public GameObject gameEndUI;
        
        public Action gameEndEvent;

        public bool playerWin;
        public bool ghostWin;
        public GameState state;
        public BicycleUIUpdater UIUpdater;

        public bool fiveMinReminded = false;
        public bool finishReminded = false;

        public Transform player;
        public Transform ghost;
        
                
        public InputActionReference Reload;
        public InputActionReference PauseGame;
        public InputActionReference Exit;
        private bool isPause;

        private float sensorInput;

        private string sensorInputString;
        
        // [Header("level Generator")] 
        // public int seed;
        // public List<GameObject> trackPrefs;
        // public GameObject slopePref;
        // public Transform firstTrack;
        // public int totalSlopes;
        // public int totalTracks;
        //
        // private int currentTrack = 0;
        // private int lastTrack = 0;
        // private List<Transform> tracks = new List<Transform>();
        //
        // public float MaxDistance { get; private set; }


        
        void Start()
        {
            if (DBManager.Instance.currentPatient != null)
            {
                avatarLoader.LoadAvatar(DBManager.Instance.currentPatient.UnityAvatar);
            }
            
            StartCoroutine(MoveToStart());
            gameEndEvent += OnGameEnd;
        }

        private void OnDestroy()
        {
            gameEndEvent -= OnGameEnd;
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
        

        private void Update()
        {
            TimeCounter();
            AvatarSpeak();
            ControllerInput();
        }


        private void ControllerInput()
        {
            if (Exit.action.IsPressed()) SceneManager.LoadScene("Home");
            if (Reload.action.IsPressed()) SceneManager.LoadScene("Cycle");
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
        
        private void TimeCounter()
        {
            if (state == GameState.PLAY)
            {
                timePeriod += Time.deltaTime;
                timeCount += Time.deltaTime;
            }
        }
        
        private void AvatarSpeak()
        {
            if (timeCount > 180 && fiveMinReminded)
            {
                fiveMinReminded = true;
                avatarLoader.Speak("three_min_reminder", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            }
            else if (timeCount > 300 && finishReminded)
            {
                finishReminded = true;
                avatarLoader.Speak("end_game_reminder", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            }
            else if (timePeriod > 100)
            {
                timePeriod = 0;
                if (player.position.z > ghost.position.z)
                {
                    avatarLoader.Speak("competition_status_faster", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
                }
                else
                {
                    avatarLoader.Speak("competition_status_slower", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
                }
            }
            else if (timeCount > 60) 
            {
                avatarLoader.Speak("come_on", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            }

        }

        public void UpdatePlayerData(float speed, float input)
        {
            avgSpeed = avgSpeed ==  0 ? speed : (avgSpeed + speed) / 2;
            leftPerformance = leftPerformance == 0 ? leftPerformance : (leftPerformance + input) / 2; // TODO: Performance = current AvgInput / past AvgInput
            sensorInput = input;
        }

        private IEnumerator RecordInput()
        {
            while (true)
            {
                // sensorManager.localPatient.CyclePerformanceList.Add(sensorInput);
                sensorInputString += sensorInput.ToString("0.0");
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void OnGameEnd()
        {
            state = GameState.END;
            StartCoroutine(UIUpdater.ShowGameEndUI(timeCount, avgSpeed, leftPerformance, coins));
            if (playerWin)
            {
                avatarLoader.Speak("finish_round_win_competition", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            }
            else
            {
                avatarLoader.Speak("finish_round_lose_competition", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            }
            // DBManager.Instance.currentPatient.UpdateGameTask("Cycle", timeCount);
            // DBManager.Instance.currentPatient.UpdateGamePerformance("Cycle", leftPerformance, 0f);

            if (DBManager.Instance.currentPatient != null)
            {
                LocalPatientData newData =
                    new LocalPatientData(DBManager.Instance.currentPatient.Name, sensorInputString);
                DataSaver.SaveData(DBManager.Instance.currentPatient.Name + ".userInfo", newData);
            }
        }
        
        private IEnumerator MoveToStart()
        {
            gameEndUI.SetActive(false);
            state = GameState.PREPARE;
            
            yield return new WaitForSeconds(3f);
            avatarLoader.Speak("intro_hand_paddle", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            yield return new WaitForSeconds(12f);
            UIUpdater.ShowMoveToStartUI();
            yield return new WaitForSeconds(4f);
            
            // TODO
            float move = sensorManager.GetData(SensorPosition.LEFT);
            while (move < 15)
            {
                move += sensorManager.GetData(SensorPosition.LEFT);
                yield return null;
            }
            
            yield return StartCoroutine(UIUpdater.ShowGameStartUI());
            state = GameState.PLAY;
            timeCount = 0;
            StartCoroutine(RecordInput());
            if(sensorManager.localPatient != null) StartCoroutine(ghost.gameObject.GetComponent<BicycleVehicle>().PastDrive());
            else StartCoroutine(ghost.gameObject.GetComponent<BicycleVehicle>().DefaultDrive());
        }
        
        }
        
        
        #region Archive

        
        // public Transform GetTrack(int idx)
        // {
        //     return idx < tracks.Count ? tracks[idx] : null;
        // }
        //
        // public Vector3 GetCurrentPosition()
        // {
        //     Vector3 currentPosition = tracks[currentTrack].position;
        //     currentPosition.y += tracks[currentTrack].lossyScale.y + 1;
        //     return currentPosition;
        // }
        //
        // public Vector3 GetTargetPosition()
        // {
        //     Vector3 targetPosition = tracks[currentTrack + 1].transform.GetChild(0).position;
        //     targetPosition.y += 1;
        //     return targetPosition;
        // }
        //
        // public void UpdateCurrentCube(int idx)
        // {
        //     if (GameStateManager.State == GameState.PLAY)
        //     {
        //         currentTrack = idx;
        //         // Debug.Log(currentCube);
        //
        //         if (lastTrack < currentTrack)
        //         {
        //             lastTrack = currentTrack;
        //         }
        //     }
        // }
        //
        // private void GenerateLevels()
        // {
        //     foreach (GameObject go in trackPrefs)
        //     {
        //         tracks.Add(go.transform);
        //     }
        //     
        //     Random.InitState(seed);
        //
        //     Transform lastCube = firstTrack;
        //     int trackIdx = tracks.Count;
        //     float lastTotalRotation = 0f;
        //     int slopeCount = 0;
        //         
        //     while (trackIdx < totalTracks)
        //     {
        //         int trackInGroup = Random.Range(15, 25);
        //         float totalRotation = Random.Range(-50f, 50f);
        //         for (int i = 0; i < trackInGroup; i++)
        //         {
        //             if (trackIdx == totalTracks) return;
        //
        //             if (i > 17 && slopeCount < totalSlopes)
        //             {
        //                 slopeCount++;
        //                 GameObject newSlope = Instantiate(slopePref);
        //                 
        //                 newSlope.transform.position = lastCube.position;
        //                 newSlope.transform.Rotate(0f, totalRotation / trackInGroup * i + lastTotalRotation, 0);
        //                 
        //                 lastCube.position = newSlope.transform.position + lastCube.forward * 23f;
        //                 lastCube.rotation = newSlope.transform.rotation;
        //                 
        //                 newSlope.transform.position += newSlope.transform.forward * 5f;
        //                 newSlope.transform.position += newSlope.transform.right * 3f;
        //                 
        //                 break;
        //             }
        //
        //             GameObject cubePref = trackPrefs[Random.Range(0, trackPrefs.Count)];
        //             GameObject newCube = Instantiate(cubePref, cubePref.transform.parent);
        //             trackIdx++;
        //
        //             newCube.transform.position = lastCube.position +
        //                                          lastCube.forward * 4f;
        //             newCube.transform.Rotate(0f, totalRotation / trackInGroup * i + lastTotalRotation, 0);
        //             newCube.name = "Track" + trackIdx;
        //
        //             tracks.Add(newCube.transform);
        //
        //             // if ((lastCube.transform.GetChild(0).position - newCube.transform.GetChild(0).position).magnitude > MaxDistance)
        //             // {
        //             //     MaxDistance = (lastCube.transform.GetChild(0).position - newCube.transform.GetChild(0).position).magnitude;
        //             // }
        //             //
        //             lastCube = newCube.transform;
        //         }
        //         
        //         lastTotalRotation = lastCube.transform.rotation.eulerAngles.y;
        //     }
        // }
        //
        //
        //
        // private void WinCheck()
        // {
        //     if (currentTrack == tracks.Count - 1)
        //     {
        //         gameStateManager.SwitchGameState(GameState.END);
        //     }
        // }
        #endregion
    }