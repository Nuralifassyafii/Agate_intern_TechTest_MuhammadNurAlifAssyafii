using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AccountDataPersistenceManager : MonoBehaviour
{
    private AccountGameData gameData;
    private List<IAccountDataPersistent> dataPersistence;
    private AccountFileDataHandler _fileDataHandler;
    private int accountId = 0;
    private string fileName = "accountProfile";
    private string fileExtension = ".game";
    private string accountFilePath = "";
    string accountFileName = "";
    private UISaveManager _uiSaveManager;

    [SerializeField] private bool useEncryption;
    public static AccountDataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        accountFileName = fileName + accountId.ToString();
        //accountFileName = fileName + "0";
        accountFilePath = Application.persistentDataPath + "/ListAccount";
        _uiSaveManager = FindFirstObjectByType<UISaveManager>();
        if (instance != null)
        {
            _uiSaveManager.SetAlertAccount("Found more than 1 data persistence in this file");
        }
        instance = this;
    }

    private void Start()
    {
        try
        {
            accountId = FindFirstObjectByType<IDMakerManager>().GetCurrentGeneratedId();
            this._fileDataHandler = new AccountFileDataHandler(accountFilePath, accountFileName + fileExtension, useEncryption, _uiSaveManager);
            this.dataPersistence = FindAllDataPersistent();
            LoadGame();
        }
        catch(Exception e)
        {
            _uiSaveManager.SetAlertAccount("ada yang salah saat membuat akun : " + e.Message);
        }
    }

    public string GetListAccountName()
    {
        return fileName;
    }

    public string GetAccountFilePath()
    {
        return this.accountFilePath;
    }

    public void ChangeAccount(int id)
    {
        accountFileName = fileName + id.ToString();
        try
        {
            this._fileDataHandler = null;
            this._fileDataHandler = new AccountFileDataHandler(accountFilePath, accountFileName + fileExtension, useEncryption, _uiSaveManager);
            this.dataPersistence = FindAllDataPersistent();
            SaveGame();
            LoadGame();
        }
        catch (Exception e)
        {
            _uiSaveManager.SetAlertAccount("ada yang salah saat mengganti akun : " + e.Message);
        }
    }

    public string GetFileName()
    {
        return accountFileName;
    }

    public void NewGame()
    {
        this.gameData = new AccountGameData();
        _uiSaveManager.SetAlertAccount("Creating New Account...");
    }

    public void LoadGame()
    {
        // search game data dari file yang ada
        this.gameData = _fileDataHandler.LoadAccount();

        // kalau belum ketemu game data maka initialize new game
        if (this.gameData == null)
        {
            _uiSaveManager.SetAlertAccount("No save file found, creating new account");
            NewGame();
        }

        // atur game data lalu dimasukan ke game sekarang (this gamedata)
        foreach (IAccountDataPersistent dataPersistentObj in dataPersistence)
        {
            dataPersistentObj.LoadAccountData(gameData);
        }

    }

    public void SaveGame()
    {
        // cek data lalu store ke game data
        foreach (IAccountDataPersistent dataPersistentObj in dataPersistence)
        {
            dataPersistentObj.SaveAccountData(ref gameData);
            _uiSaveManager.SetAlertAccount("Save Account Completed");
        }

        _fileDataHandler.SaveAccount(gameData);

        // cek game data lalu store ke file di application
    }

    public void DeleteAccount()
    {
        string pathAccount = Path.Combine(accountFilePath, accountFileName);
        if (File.Exists(pathAccount))
        {
            File.Delete(Path.Combine(accountFilePath, accountFileName));
        }
        else
        {
            _uiSaveManager.SetAlertAccount("File tidak ditemukan");
        }
    }

    public void DeleteFolderAccount()
    {
        string pathFolder = Path.Combine(Application.persistentDataPath, accountFileName);
        if (Directory.Exists(pathFolder))
        {
            Directory.Delete(pathFolder,true);
        }
        else
        {
            _uiSaveManager.SetAlertAccount("Folder Tidak Ditemukan");
        }
    }

    public void FullDeleteAccount()
    {
        try
        {
            DeleteAccount();
            DeleteFolderAccount();
        }
        catch(Exception e)
        {
            _uiSaveManager.SetAlertAccount("Ada yang salah saat menghapus akun : " + e.Message);
        }
    }

    private List<IAccountDataPersistent> FindAllDataPersistent()
    {
        IEnumerable<IAccountDataPersistent> dataPersistents = FindObjectsOfType<MonoBehaviour>().OfType<IAccountDataPersistent>();

        return new List<IAccountDataPersistent>(dataPersistents);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
