using UnityEngine;

public interface IDataLastId
{
    void SaveLastID(ref IDGameData gameData);
    void LoadLastID(IDGameData gameData);
}
