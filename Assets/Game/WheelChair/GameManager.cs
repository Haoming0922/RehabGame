using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Game.Sensor;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using Game.Util;

namespace Game.Wheelchair
{
    public class GameManager : MonoBehaviour
    {
        public GameState state;
        public ReHybAvatarController AvatarController;
        public SensorManager sensorManager;
        
        private float timeCount = 0;
        private float timePeriod = 0;

        public GameObject gamePlayUI;
        public GameObject gameEndUI;
        
        public Action gameEndEvent;

        public bool playerWin;
        
        void Start()
        {
            StartCoroutine(MoveToStart());
            gameEndEvent += OnGameEnd;
        }

        private void OnDestroy()
        {
            gameEndEvent -= OnGameEnd;
        }

        private void Update()
        {
            if (state == GameState.PLAY)
            {
                timePeriod += Time.deltaTime;
                timeCount += Time.deltaTime;
            }

            if (timePeriod > 30 && state == GameState.PLAY)
            {
                timePeriod = 0;
                AvatarController.UserSpeak("middle_game");
            }
        }


        private void OnGameEnd()
        {
            state = GameState.END;
            gamePlayUI.SetActive(false);
            gameEndUI.SetActive(true);
            // TODO: evaluation page
            if (playerWin)
            {
                AvatarController.UserSpeak("game_win");
            }
            else
            {
                AvatarController.UserSpeak("game_lose");
            }
        }
        
        private IEnumerator MoveToStart()
        {
            gamePlayUI.SetActive(false);
            gameEndUI.SetActive(false);
            state = GameState.PREPARE;
            
            AvatarController.UserSpeak("ask_move_sensor");
            yield return new WaitForSeconds(5f);
            
            while (!(sensorManager.IsMove(SensorPosition.LEFT) && sensorManager.IsMove(SensorPosition.RIGHT)))
            {
                yield return null;
            }
            
            AvatarController.UserSpeak("start_game");
            yield return new WaitForSeconds(5f);
            
            state = GameState.PLAY;
            gamePlayUI.SetActive(true);
        }
        
    }
}