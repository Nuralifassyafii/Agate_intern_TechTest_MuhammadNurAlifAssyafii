using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class IDMakerPersistentManager : MonoBehaviour
{
    private IDGameData gameData;
    private List<IDataLastId> dataPersistence;
    private IDMakerFileManager _fileDataHandler;
    private string fileName = "idMaker.game";
    [SerializeField] private bool useEncryption;
    public static IDMakerPersistentManager instance { get; private set; }

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
        this._fileDataHandler = new IDMakerFileManager(Application.persistentDataPath, fileName, useEncryption);
        this.dataPersistence = FindAllDataPersistent();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new IDGameData();
    }

    public void LoadGame()
    {
        // search game data dari file yang ada
        this.gameData = _fileDataHandler.LoadAccount();

        // kalau belum ketemu game data maka initialize new game
        if (this.gameData == null)
        {
            Debug.Log("No Id file found, creating new account");
            NewGame();
        }

        // atur game data lalu dimasukan ke game sekarang (this gamedata)
        foreach (IDataLastId dataPersistentObj in dataPersistence)
        {
            dataPersistentObj.LoadLastID(gameData);
        }

    }

    public void SaveGame()
    {
        // cek data lalu store ke game data
        foreach (IDataLastId dataPersistentObj in dataPersistence)
        {
            dataPersistentObj.SaveLastID(ref gameData);
            Debug.Log("Save id Completed");
        }

        _fileDataHandler.SaveAccount(gameData);

        // cek game data lalu store ke file di application
    }

    private List<IDataLastId> FindAllDataPersistent()
    {
        IEnumerable<IDataLastId> dataPersistents = FindObjectsOfType<MonoBehaviour>().OfType<IDataLastId>();

        return new List<IDataLastId>(dataPersistents);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
