using UnityEngine;

public class IDMakerManager : MonoBehaviour, IDataLastId
{
    private int counterGeneratedId = 0;

    public int GetCurrentGeneratedId()
    {
        return counterGeneratedId;
    }

    public void LoadLastID(IDGameData gameData)
    {
        counterGeneratedId = gameData.lastGeneratedId;
    }

    public void SaveLastID(ref IDGameData gameData)
    {
        counterGeneratedId += 1;
        gameData.lastGeneratedId = this.counterGeneratedId;
    }
}
