using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IDGameData
{
    public int lastGeneratedId;

    public IDGameData()
    {
        this.lastGeneratedId = 0;
    }
}
