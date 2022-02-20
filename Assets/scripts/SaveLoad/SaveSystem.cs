using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private const string Save_EXTENSION = "txt";
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";
    private static bool isInit = false;

    //public static void SaveAgent(AgentManagerTest agent)
    //{
    //    BinaryFormatter formatter = new BinaryFormatter();
    //    string path = Application.persistentDataPath + "/player.fun";
    //    FileStream stream = new FileStream(path, FileMode.Create);

    //    AgentData data = new AgentData(agent);
    //    formatter.Serialize(stream, data);
    //    stream.Close();
    //}

    //public static AgentData LoadAgent()
    //{
    //    string path = Application.persistentDataPath + "/player.fun";
    //    if (File.Exists(path))
    //    {
    //        BinaryFormatter formatter = new BinaryFormatter();
    //        FileStream stream = new FileStream(path, FileMode.Open);

    //        AgentData data = formatter.Deserialize(stream) as AgentData;
    //        stream.Close();
    //        return data;
            
    //    }
    //    else
    //    {
    //        Debug.LogError("No se encontro en " + path);
    //        return null;
    //    }
    //}
    public static void Init()
    {
        if (!isInit)
        {
            isInit = true;
            if (!Directory.Exists(SAVE_FOLDER)){
                Directory.CreateDirectory(SAVE_FOLDER);
            }
        }
    }

    public static void Save(string fileName, string saveString, bool overwrite)
    {
        Init();
        string saveFileName = fileName;
        if (!overwrite)
        {
            int saveNumber = 1;
            while(File.Exists(SAVE_FOLDER + saveFileName + "." + Save_EXTENSION))
            {
                saveNumber++;
                saveFileName = fileName + "_" + saveNumber;
            }
        }
        File.WriteAllText(SAVE_FOLDER + saveFileName + "." + Save_EXTENSION, saveString);
    }

    public static string Load(string fileName)
    {
        Init();
        if(File.Exists(SAVE_FOLDER + fileName + "." + Save_EXTENSION))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + fileName + "." + Save_EXTENSION);
            return saveString;
            
        }
        else
        {
            return null;
        }
    }

    public static string LoadMostRecentFile()
    {
        Init();
        DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);
        FileInfo[] saveFiles = directoryInfo.GetFiles("*." + Save_EXTENSION);
        FileInfo mostRecentFile = null;
        foreach(FileInfo fileInfo in saveFiles)
        {
            if(mostRecentFile == null)
            {
                mostRecentFile = fileInfo;
            }
            else
            {
                if(fileInfo.LastWriteTime > mostRecentFile.LastWriteTime)
                {
                    mostRecentFile = fileInfo;
                }
            }
        }
        if(mostRecentFile != null)
        {
            string saveString = File.ReadAllText(mostRecentFile.FullName);
            return saveString;
        }
        else
        {
            return null;
        }

    }


    public static void SaveObject(string saveName,object saveObject)
    {
        SaveObject(saveName, saveObject, false);
    }

    public static void SaveObject(string fileName,object saveObject, bool overWrite)
    {
        Init();
        string json = JsonUtility.ToJson(saveObject);
        Save(fileName, json, overWrite);
    }

    public static TSaveObject LoadMostRecentObject<TSaveObject>()
    {
        Init();
        string saveString = LoadMostRecentFile();
        if(saveString != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObject;
        }
        else
        {
            return default(TSaveObject);
        }
    }

    public static TSaveObject LoadObject<TSaveObject>(string fileName)
    {
        Init();
        string saveSting = Load(fileName);
        if(saveSting != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveSting);
            return saveObject;
        }
        else
        {
            return default(TSaveObject);
        }
    }
}
