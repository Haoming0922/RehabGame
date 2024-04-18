using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Util;

public class UserConfig
{
    public float maxLeftArmRotationAngle;
    public float maxRightArmRotationAngle;
    
    public UserConfig() {}
    
    public void SetArmRotationAngle(SensorPosition position, float data)
    {
        switch (position)
        {
            case SensorPosition.LEFT:
                maxLeftArmRotationAngle = data;
                break;
            case SensorPosition.RIGHT:
                maxRightArmRotationAngle = data;
                break;
            default: break;
        }
    }
}
