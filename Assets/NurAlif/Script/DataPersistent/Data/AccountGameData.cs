using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AccountGameData
{
    public string accountName;
    public int accountId;

    public AccountGameData()
    {
        this.accountName = "test";
        this.accountId = 0;
    }
}
