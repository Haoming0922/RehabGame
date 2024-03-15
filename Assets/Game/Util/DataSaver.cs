using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Game.Util
{

public class DataSaver
{
    // public static Action OnNullDataLoad;
    
    public static void SaveData(string fileName, object ob)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path)) File.Delete(path);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string data = JsonUtility.ToJson(ob, true);
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

    public static object LoadData(string fileName, Type type)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        object ob = null;
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
                ob = JsonUtility.FromJson(dataString, type);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when reading data " + path + e.ToString());
            }
        }
        
        return ob;
    }
}

}