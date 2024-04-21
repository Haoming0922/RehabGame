using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Util
{
    public class UserManager : Singleton<UserManager>
    {
        public UserConfig userConfig;

        private void Start()
        {
            userConfig = new UserConfig();
            DBLoader.Instance.LoadData();
        }
        
        public void SetPerformance(MiniGame game, SensorPosition position, float data)
        {
            switch (position)
            {
                case SensorPosition.LEFT:
                    userConfig.leftInputPerformance[game] = data;
                    break;
                case SensorPosition.RIGHT:
                    userConfig.rightInputPerformance[game] = data;
                    break;
                default: break;
            }
        }

        public float GetPerformance(MiniGame game, SensorPosition position)
        {
            switch (position)
            {
                case SensorPosition.LEFT:
                    return userConfig.leftInputPerformance[game];
                case SensorPosition.RIGHT:
                    return userConfig.rightInputPerformance[game];
                    break;
                default: break;
            }
            return userConfig.leftInputPerformance[game];
        }
        
    }
    
}