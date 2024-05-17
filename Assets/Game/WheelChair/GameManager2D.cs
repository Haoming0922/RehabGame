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


namespace Game.Wheelchair
{
    public class GameManager2D : MonoBehaviour
    {
        public AvatarLoader avatarLoader;
        public SensorManager sensorManager;
        
        private float timeCount = 0;
        private float timePeriod = 0;

        private float avgSpeed;
        private float leftPerformance;
        private float rightPerformance;
        private int coins;
        
        public GameObject gameEndUI;
        
        public Action gameEndEvent;

        public GameState state;

        public WheelchairUIUpdater2D UIUpdater;
        
        public bool fiveMinReminded = false;
        public bool finishReminded = false;
        
        
        public InputActionReference Reload;
        public InputActionReference PauseGame;
        public InputActionReference Exit;
        private bool isPause;
        
        void Start()
        {
            if (DBManager.Instance.currentPatient != null)
            {
                avatarLoader.LoadAvatar(DBManager.Instance.currentPatient.UnityAvatar);
            }
            StartCoroutine(MoveToStart());
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

        private void Update()
        {
            TimeCounter();
            AvatarSpeak();
            ControllerInput();
        }
        
        private void ControllerInput()
        {
            if (Exit.action.IsPressed()) SceneManager.LoadScene("Home");
            if (Reload.action.IsPressed()) SceneManager.LoadScene("Wheelchair2D");
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
            if (timeCount > 150 && fiveMinReminded)
            {
                fiveMinReminded = true;
                avatarLoader.Speak("three_min_reminder", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            }
            else if (timeCount > 300 && finishReminded)
            {
                finishReminded = true;
                avatarLoader.Speak("end_game_reminder", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            }
            else if (timePeriod > 200)
            {
                timePeriod = 0;
                avatarLoader.Speak("middle_game", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            }
            else if (timeCount > 60) 
            {
                avatarLoader.Speak("come_on", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            }
        }

        public void UpdatePlayerData(float leftInput, float rightInput)
        {
            if(leftInput > 0.2f) leftPerformance = leftPerformance == 0 ? leftPerformance : (leftPerformance + leftInput) / 2; 
            if(rightInput > 0.2f) rightPerformance = rightPerformance == 0 ? rightPerformance : (rightPerformance + rightInput) / 2; 
        }
        
        private void OnGameEnd()
        {
            state = GameState.END;
            StartCoroutine(UIUpdater.ShowGameEndUI(timeCount, avgSpeed, (rightPerformance  + leftPerformance) / 2, coins));
            avatarLoader.Speak("finish_round_without_competition", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            
            // DBManager.Instance.currentPatient.UpdateGameTask("Wheelchair football", timeCount);
            DBManager.Instance.currentPatient.UpdateGamePerformance("Wheelchair football", leftPerformance, rightPerformance);

        }
        
        private IEnumerator MoveToStart()
        {
            gameEndUI.SetActive(false);
            state = GameState.PREPARE;
            
            yield return new WaitForSeconds(3f);
            avatarLoader.Speak("intro_wheelchair_football", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            yield return new WaitForSeconds(7f);
            UIUpdater.ShowMoveToStartUI();
            yield return new WaitForSeconds(2f);
            
            // TODO
            float move = sensorManager.GetData(SensorPosition.LEFT);
            while (move < 10)
            {
                move += Mathf.Max(sensorManager.GetData(SensorPosition.LEFT), sensorManager.GetData(SensorPosition.RIGHT));
                yield return null;
            }
            
            yield return StartCoroutine(UIUpdater.ShowGameStartUI());
            state = GameState.PLAY;
        }
        
    }
}