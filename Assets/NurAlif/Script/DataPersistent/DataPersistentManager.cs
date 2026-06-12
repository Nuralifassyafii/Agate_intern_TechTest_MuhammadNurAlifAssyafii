using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DataPersistentManager :MonoBehaviour
{
    private GameData gameData;
    private List<IDataPersistent> dataPersistence;
    private FileDataHandler _fileDataHandler;
    private string fileName = "saveFile1.game";
    [SerializeField] private bool useEncryption;
    public static DataPersistentManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Found more than 1 data persistence in this file");
        }
        instance = this;
    }

    private void Start()
    {
        this._fileDataHandler = new FileDataHandler(Application.persistentDataPath,fileName,useEncryption);
        this.dataPersistence = FindAllDataPersistent();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
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
