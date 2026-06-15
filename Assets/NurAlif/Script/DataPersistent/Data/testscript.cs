using System.IO;
using UnityEngine;

public class testscript : MonoBehaviour,IAccountDataPersistent
{
    string accountName = "testingAccount";    
    private string filepath = "";
    [SerializeField] string testingSendedData;
    [SerializeField] int idSended;

    public void LoadAccountData(AccountGameData gameData)
    {
        testingSendedData = gameData.accountName;
        idSended = gameData.accountId;
    }

    public void SaveAccountData(ref AccountGameData gameData)
    {
        gameData.accountId = this.idSended;
        gameData.accountName = this.testingSendedData;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        filepath = Path.Combine(Application.persistentDataPath, accountName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
