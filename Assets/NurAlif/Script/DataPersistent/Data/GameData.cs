using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int[] strengthDiceValue;
    public int[] artistryDiceValue;
    public int[] reflexDiceValue;
    public int[] deceptionDiceValue;
    public int[] knowledgeDiceValue;
    public int[] observationDiceValue;
    public List<int[]> skillDiceList;
    public int health;
    public int sanity;
    public int exp;

    public GameData()
    {
        //default value for skill dice
        this.strengthDiceValue = new int[6] {1,1,1,1,1,1};
        this.artistryDiceValue = new int [6] { 1,1,1,1,1,1};
        this.reflexDiceValue = new int[6] {1,1,1,1,1,1};
        this.deceptionDiceValue = new int[6] {1,1,1,1,1,1};
        this.knowledgeDiceValue = new int[6] {1,1,1,1,1,1};
        this.observationDiceValue = new int[6] {1,1,1,1,1,1};

        //default index for skill dice list
        this.skillDiceList = new List<int[]>()
        {
            strengthDiceValue,
            reflexDiceValue,
            artistryDiceValue,
            deceptionDiceValue,
            knowledgeDiceValue,
            observationDiceValue,
        };

        //default value for player
        this.health = 4;
        this.sanity = 4;
        this.exp = 0;
    }
}
