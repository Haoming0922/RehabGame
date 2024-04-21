using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Util;

namespace Game.Util
{
    public class UserConfig
    {
        public Dictionary<MiniGame, float> leftInputPerformance = new Dictionary<MiniGame, float>();
        public Dictionary<MiniGame, float> rightInputPerformance = new Dictionary<MiniGame, float>();

        // JumpJump : maxArmRotationAngle

        public UserConfig()
        {
            foreach (MiniGame game in Enum.GetValues(typeof(MiniGame)))
            {
                leftInputPerformance.Add(game, 1);
                rightInputPerformance.Add(game, 1);
            }
        }
    }
    
}