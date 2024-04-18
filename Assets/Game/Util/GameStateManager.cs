using System;
using System.Collections;
using System.Collections.Generic;
using Game.Sensor;
using UnityEngine;
using Game.Util;

namespace Game.Sensor
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameState State { get; set; }

        public Action PrepareEvent;
        public Action PlayEvent;
        public Action EndEvent;
        
        
        public void SwitchGameState(GameState state)
        {
            State = state;
            switch (State)
            {
                case GameState.PREPARE:
                    PrepareEvent?.Invoke();
                    break;
                case GameState.PLAY:
                    PlayEvent?.Invoke();
                    break;
                case GameState.END:
                    EndEvent?.Invoke();
                    break;
                case GameState.NULL:
                default: break;
            }
        }
    }

}

