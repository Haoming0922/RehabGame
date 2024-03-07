// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// public class GameDataManager : Singleton<GameDataManager>
// {
//     // General
//     public List<string> gameControllerList;
//     public IDictionary<string, string> sensorMapping = new Dictionary<string, string>(); // address -> name
//     private IDictionary<string, Queue<SensorDataReceived>> gameDataWindowDict = new Dictionary<string, Queue<SensorDataReceived>>(); // name -> data
//     private IDictionary<string, SensorDataReceived> gameDataDict = new Dictionary<string, SensorDataReceived>(); // name -> data
//     public delegate float DataTransform(SensorDataReceived data);
//     public DataTransform dataTransform;
//     // public static Action<string,string> sensorAdded;
//     // public static Action<string> sensorRemoved;
//     
//     public int windowSize = 5;
//     
//     private void Start()
//     {
//         SyncsenseSensorManager.OnSensorDataReceivedEvent -= LowPassFiler;
//         SyncsenseSensorManager.OnSensorDataReceivedEvent += LowPassFiler;
//     }
//
//     private void OnDestroy()
//     {
//         SyncsenseSensorManager.OnSensorDataReceivedEvent -= LowPassFiler;
//     }
//
//     public float GetData(string controller, DataTransform dataTransform)
//     {
//         return gameDataDict.ContainsKey(controller) ? dataTransform(gameDataDict[controller]) : 0;
//     }
//     
//     public SensorDataReceived GetData(string controller)
//     {
//         return gameDataDict.ContainsKey(controller) ? gameDataDict[controller] : new SensorDataReceived();
//     }
//     
//
//     public void LowPassFiler(SensorDataReceived data)
//     {
//         if (!sensorMapping.ContainsKey(data.deviceAddress)) { return; };
//
//         if (!gameDataWindowDict.ContainsKey(data.deviceAddress))
//         {
//             gameDataWindowDict.Add(data.deviceAddress, new Queue<SensorDataReceived>(windowSize));
//             gameDataWindowDict[data.deviceAddress].Enqueue(data);
//         }
//
//         if (gameDataWindowDict[data.deviceAddress].Count < windowSize)
//         {
//             gameDataWindowDict[data.deviceAddress].Enqueue(data);
//         }
//         else
//         {
//             gameDataWindowDict[data.deviceAddress].Dequeue();
//             gameDataWindowDict[data.deviceAddress].Enqueue(data);
//         }
//
//         SetGameData(data.deviceAddress);
//     }
//     
//     private void SetGameData(string deviceAddress)
//     {
//         string gameController = sensorMapping[deviceAddress];
//         SensorDataReceived filterData = Calculation.AverageQueue(gameDataWindowDict[deviceAddress]);
//         gameDataDict[gameController] = filterData;
//     }
//
//
//     
//
// }
