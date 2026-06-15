using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AccountMenu
{
    public TMP_Text title;
    public AccountButton AccountButton;
    public AccountButton deleteButton;
}
[System.Serializable]
public class SaveFileMenu
{
    public TMP_Text title;
    public AccountButton saveFileButton;
    public AccountButton deleteButton;
}

[System.Serializable]
public class AccountButton
{
    public Button saveFileButton;
    public TMP_Text ButtonName;
}

//class Utama
public class UISaveManager : MonoBehaviour
{
    public TMP_Text titleAccount;
    public TMP_Text titleSaveFile;
    public List<AccountMenu> accountMenu;
    public List<SaveFileMenu> saveFileMenu;

    private string usedAccount = "Pick Account";
    private AccountDataPersistenceManager _accountDataPersistent;
    private DataPersistentManager _dataPersistent;
    private Coroutine intervalCoroutine;

    [SerializeField] private GameObject accountPanel;
    [SerializeField] private GameObject saveDataPanel;
    public void SetAlertAccount(string text)
    {
        titleAccount.text = text;
        if(intervalCoroutine != null)
        {
            StopAllCoroutines();
        }
        intervalCoroutine = StartCoroutine(waitSecond());
    }

    public void SetStatusAccountPanel(bool isActive)
    {
        accountPanel.SetActive(isActive);
    }

    public void SetSaveDataPanel(bool isActive)
    {
        saveDataPanel.SetActive(isActive);
    }

    private IEnumerator waitSecond()
    {
        yield return new WaitForSeconds(3f);
        titleAccount.text = usedAccount;
    }

    public void PauseButton()
    {
        if (accountPanel.activeSelf || saveDataPanel.activeSelf)
        {
            accountPanel.SetActive(false);
            saveDataPanel.SetActive(false);
        }
        else
        {
            accountPanel.SetActive(true);
        }
    }

    public void SetButtonAccountName(string path,string filename)
    {
        for (int i = 0; i < 3; i++)
        {
            string nama = Path.Combine(path, filename + i.ToString()+".game");
            if (File.Exists(nama))
            {
                accountMenu[i].AccountButton.ButtonName.text = filename + i.ToString();
            }
            else
            {
                accountMenu[i].AccountButton.ButtonName.text = "Empty";
            }
        }
    }

    public void SetPickedAccountUI(int pickedAccount)
    {
        titleAccount.text = _accountDataPersistent.GetListAccountName() + pickedAccount.ToString();
    }

    public void SetButtonSaveDataName(string path)
    {
        for (int i = 0; i < 3; i++)
        {
            string nama = Path.Combine(path, _dataPersistent.GetFileName(i) + ".game");
            if (File.Exists(nama))
            {
                saveFileMenu[i].saveFileButton.ButtonName.text = _dataPersistent.GetFileName(i) + i.ToString();
                //saveFileMenu[i].saveFileButton.saveFileButton.onClick.AddListener(() => _dataPersistent.ChangeAccount(i));
            }
            else
            {
                saveFileMenu[i].saveFileButton.ButtonName.text = "Empty";
            }
        }
    }

    private void Start()
    {
        _accountDataPersistent = FindFirstObjectByType<AccountDataPersistenceManager>();   
        _dataPersistent = FindFirstObjectByType<DataPersistentManager>();   
    }

    private void Update()
    {
        titleSaveFile.text = _accountDataPersistent.GetFileName();
        SetButtonAccountName(_accountDataPersistent.GetAccountFilePath(), _accountDataPersistent.GetListAccountName());
        SetButtonSaveDataName(_dataPersistent.GetFolderPath());
    }

}
