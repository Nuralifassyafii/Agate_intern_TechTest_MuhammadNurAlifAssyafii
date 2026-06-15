using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DataPersistentManager : MonoBehaviour
{
    private GameData gameData;
    private List<IDataPersistent> dataPersistence;
    private FileDataHandler _fileDataHandler;
    private string folderName;
    private int idSaveFile;
    string folderPath;
    private string fileName = "";
    private string extensionName = ".game";
    [SerializeField] private bool useEncryption;
    public Action ResetData;
    public static DataPersistentManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than 1 data persistence in this file");
        }
        instance = this;
    }

    private void Start()
    {
        try
        {
            folderName = FindFirstObjectByType<AccountDataPersistenceManager>().GetFileName();
            folderPath = Application.persistentDataPath + "/" + folderName;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            idSaveFile = Directory.GetFiles(folderPath).Length;
            fileName = GetFileName(2);
            this._fileDataHandler = new FileDataHandler(folderPath, fileName + extensionName, useEncryption);
            this.dataPersistence = FindAllDataPersistent();
            LoadGame();
        }
        catch (Exception e)
        {
            Debug.LogError("ada yang salah saat membuat save data : " + e.Message);
        }
    }

    public void ChangeAccount(int id)
    {
        try
        {
            folderName = FindFirstObjectByType<AccountDataPersistenceManager>().GetFileName();
            folderPath = Application.persistentDataPath + "/" + folderName;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            idSaveFile = Directory.GetFiles(folderPath).Length;
            fileName = GetFileName(id);
            this._fileDataHandler = null;
            this._fileDataHandler = new FileDataHandler(folderPath, fileName + extensionName, useEncryption);
            this.dataPersistence = FindAllDataPersistent();
            SaveGame();
            ResetData?.Invoke();
            LoadGame();
        }
        catch (Exception e)
        {
            Debug.LogError("ada yang salah saat mengganti save data : " + e.Message);
        }
    }

    public string GetFolderPath()
    {
        return this.folderPath;
    }

    public string GetFileName(int id)
    {
        return "saveFile" + folderName + id;
    }
    public void NewGame()
    {
        if (Directory.GetFiles(folderPath).Length < 3)
        {
            this.gameData = new GameData();
        }
        else
        {
            Debug.LogError("Save data untuk akun " + folderName + " sudah Maksimal");
        }
    }

    public void LoadGame()
    {
        // search game data dari file yang ada
        this.gameData = _fileDataHandler.Load();

        // kalau belum ketemu game data maka initialize new game
        if (this.gameData == null)
        {
            Debug.Log("No save file found, creating new game");
            NewGame();
        }
        // atur game data lalu dimasukan ke game sekarang (this gamedata)
        foreach (IDataPersistent dataPersistentObj in dataPersistence)
        {
            dataPersistentObj.LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        // cek data lalu store ke game data
        foreach (IDataPersistent dataPersistentObj in dataPersistence)
        {
            dataPersistentObj.SaveData(ref gameData);
            Debug.Log("Save Completed");
        }

        _fileDataHandler.Save(gameData);

        // cek game data lalu store ke file di application
    }

    private List<IDataPersistent> FindAllDataPersistent()
    {
        IEnumerable<IDataPersistent> dataPersistents = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistent>();

        return new List<IDataPersistent>(dataPersistents);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
