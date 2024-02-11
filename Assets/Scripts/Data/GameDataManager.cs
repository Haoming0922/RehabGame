using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{
    public List<string> gameControllerList;
    
    public IDictionary<string, string> sensorMapping = new Dictionary<string, string>(); // address -> name

    private IDictionary<string, Queue<SensorDataReceived>> gameDataWindowDict = new Dictionary<string, Queue<SensorDataReceived>>(); // name -> data

    private IDictionary<string, SensorDataReceived> gameDataDict = new Dictionary<string, SensorDataReceived>(); // name -> data

    private IDictionary<string, RotationType> rotationCalibrationDict = new Dictionary<string, RotationType>();
    
    public delegate float DataTransform(SensorDataReceived data);
    
    private int windowSize = 10;
    private void Start()
    {
        SyncsenseSensorManager.OnSensorDataReceivedEvent -= LowPassFiler;
        SyncsenseSensorManager.OnSensorDataReceivedEvent += LowPassFiler;
    }

    private void OnDestroy()
    {
        SyncsenseSensorManager.OnSensorDataReceivedEvent -= LowPassFiler;
    }

    public float GetData(string controller, DataTransform dataTransform)
    {
        return dataTransform(gameDataDict[controller]);
    }
    

    public void LowPassFiler(SensorDataReceived data)
    {
        if (!sensorMapping.ContainsKey(data.deviceAddress)) { return; };

        if (!gameDataWindowDict.ContainsKey(data.deviceAddress))
        {
            gameDataWindowDict.Add(data.deviceAddress, new Queue<SensorDataReceived>(windowSize));
            gameDataWindowDict[data.deviceAddress].Enqueue(data);
        }

        if (gameDataWindowDict[data.deviceAddress].Count < windowSize)
        {
            gameDataWindowDict[data.deviceAddress].Enqueue(data);
        }
        else
        {
            gameDataWindowDict[data.deviceAddress].Dequeue();
            gameDataWindowDict[data.deviceAddress].Enqueue(data);
        }

        SetGameData(data.deviceAddress);
    }
    
    private void SetGameData(string deviceAddress)
    {
        string gameConroller = sensorMapping[deviceAddress];
        SensorDataReceived filterData = Calculation.AverageQueue(gameDataWindowDict[deviceAddress]);
        gameDataDict[gameConroller] = filterData;
    }

    public void SetRotationCalibration(string deviceAddress, RotationType rotation)
    {
        if(!rotationCalibrationDict.ContainsKey(deviceAddress)) rotationCalibrationDict.Add(deviceAddress,rotation);
    }


    public RotationType GetRotationCalibration(string deviceAddress)
    {
        return rotationCalibrationDict[deviceAddress];
    }

}
