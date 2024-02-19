using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Calculation
{
    public static float AverageMotion(SensorDataReceived data)
    {
        return (Mathf.Abs(data.accX) + Mathf.Abs(data.accY) + Mathf.Abs(data.accZ));
    }


    public static float ToRacingData(SensorDataReceived data)
    {
        float threshold = 12f;
        if (AverageMotion(data) > threshold) return Mathf.Log10(AverageMotion(data) / threshold);
        else return 0;
    }

    public static float ToRotationData(SensorDataReceived data)
    {
        float threshold = 20f;
        switch (GameDataManager.Instance.GetRotationCalibration(data.deviceAddress))    
        {
            case RotationType.XPositive:
                if (Mathf.Abs(data.gyroX) > threshold)
                {
                    return data.gyroX > 0 ? 1 : -1;
                }
                else return 0;
            case RotationType.XNegative:
                if (Mathf.Abs(data.gyroX) > threshold)
                {
                    return data.gyroX > 0 ? -1 : 1;
                }
                else return 0;
            case RotationType.YPositive:
                if (Mathf.Abs(data.gyroY) > threshold)
                {
                    return data.gyroY > 0 ? 1 : -1;
                }
                else return 0;
            case RotationType.YNegative:
                if (Mathf.Abs(data.gyroY) > threshold)
                {
                    return data.gyroY > 0 ? -1 : 1;
                }
                else return 0;
            case RotationType.ZPositive:
                if (Mathf.Abs(data.gyroZ) > threshold)
                {
                    return data.gyroZ > 0 ? 1 : -1;
                }
                else return 0;
            case RotationType.ZNegative:
                if (Mathf.Abs(data.gyroZ) > threshold)
                {
                    return data.gyroZ > 0 ? -1 : 1;
                }
                else return 0;
            default: return 0;
        }
    }

    public static SensorDataReceived AverageQueue(Queue<SensorDataReceived> dataQueue)
    {
        SensorDataReceived sum = new SensorDataReceived();
        foreach (var data in dataQueue)
        {
            sum += data;
        }
        return sum / dataQueue.Count;
    }
    

}
