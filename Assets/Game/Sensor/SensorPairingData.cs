using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Game.Util;

namespace Game.Sensor
{

    [Serializable]
    public class SensorPairingData
    {
        public Exercise exercise;
        public string cycleSensorAddress;
        public string leftSensorAddress;
        public RotationDirection leftSensorDirection;
        public float leftSensorGravity;
        public string rightSensorAddress;
        public RotationDirection rightSensorDirection;
        public float rightSensorGravity;


        public SensorPairingData()
        {
        }

        public SensorPairingData(Exercise e)
        {
            exercise = e;
        }


        #region SetData

        public void SetSensorAddress(SensorPosition position, string id)
        {
            switch (position)
            {
                case SensorPosition.LEFT:
                    leftSensorAddress = id;
                    break;
                case SensorPosition.RIGHT:
                    rightSensorAddress = id;
                    break;
                case SensorPosition.NULL:
                    cycleSensorAddress = id;
                    break;
                default: break;
            }
        }

        public void SetSensorDirection(SensorPosition position, RotationDirection direction)
        {
            switch (position)
            {
                case SensorPosition.LEFT:
                    leftSensorDirection = direction;
                    break;
                case SensorPosition.RIGHT:
                    rightSensorDirection = direction;
                    break;
                default: break;
            }
        }
        
        public void SetSensorGravity(SensorPosition position, float gravity)
        {
            switch (position)
            {
                case SensorPosition.LEFT:
                    leftSensorGravity = gravity;
                    break;
                case SensorPosition.RIGHT:
                    rightSensorGravity = gravity;
                    break;
                default: break;
            }
        }
        

        #endregion


        /*
        #region Save&Load

        public void SaveData()
        {
            string path = Path.Combine(Application.persistentDataPath, exercise.ToString() + ".sensorpair");
            if (File.Exists(path)) File.Delete(path);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                string data = JsonUtility.ToJson(this, true);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(data);
                        Debug.Log(path);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when saving data " + path + e.ToString());
            }
        }

        public void LoadData(Exercise ex)
        {
            string path = Path.Combine(Application.persistentDataPath, ex.ToString() + ".sensorpair");
            SensorPairingData newData = null;
            if (File.Exists(path))
            {
                try
                {
                    string dataString;
                    using (FileStream stream = new FileStream(path, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataString = reader.ReadToEnd();
                        }
                    }

                    newData = JsonUtility.FromJson<SensorPairingData>(dataString);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error occured when reading data " + path + e.ToString());
                }
            }

            if (newData == null)
            {
                Debug.LogError("Please pair the sensors first."); //TODO
                SceneManager.LoadScene("Home");
                return;
            }

            leftSensorAddress = newData.leftSensorAddress;
            leftSensorDirection = newData.leftSensorDirection;
            rightSensorAddress = newData.rightSensorAddress;
            rightSensorDirection = newData.rightSensorDirection;
        }

        #endregion          
        */

    }

}