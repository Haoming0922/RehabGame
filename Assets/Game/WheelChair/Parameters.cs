using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Wheelchair
{
    [Serializable]
    public class Parameters
    {
        public float mass;
        public float drag;
        public float angularDrag;
        public float forwardFactor;
        public float turningFactor;
        public float dynamicFriction;
        public float staticFriction;
        public int windowSize;

        private static string fileName = "Wheelchair.setting";

        public Parameters()
        {
            mass = 0;
            drag = 0;
            angularDrag = 0;
            forwardFactor = 0;
            turningFactor = 0;
            dynamicFriction = 0;
            staticFriction = 0;
            windowSize = 0;
        }

        public void SaveData()
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
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
                Debug.LogError("Error occured when saving data " + path);
            }
        }

        public static Parameters LoadData()
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            Parameters newData = null;
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

                    newData = JsonUtility.FromJson<Parameters>(dataString);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error occured when reading data " + path);
                }
            }

            return newData;
        }
    }

}