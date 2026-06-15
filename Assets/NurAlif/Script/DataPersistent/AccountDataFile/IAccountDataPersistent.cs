using UnityEngine;

public interface IAccountDataPersistent
{
    void SaveAccountData(ref AccountGameData gameData);
    void LoadAccountData(AccountGameData gameData);
}
