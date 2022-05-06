using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public TMP_InputField saveName;
    public GameObject loadButton;

    public void OnSave()
    {
        SaveSystem.Save(saveName.text, saveName.text, true);
    }
    public string[] saveFiles;
    public void GetLoadFiles()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves/")){
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves/");
        }
        saveFiles = Directory.GetFiles(Application.persistentDataPath + "/Saves/");
    }
    //public void ShowLoadScreen()
    //{
    //    GetLoadFiles();
    //    foreach (Transform button in collection)
    //    {

    //    }
    //}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
