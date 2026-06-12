using UnityEngine;

public interface IDataPersistent
{
    void SaveData(ref GameData gameData);
    void LoadData(GameData gameData);
}
