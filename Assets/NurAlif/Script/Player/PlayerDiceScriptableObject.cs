using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDiceScriptableObject", menuName = "Scriptable Objects/PlayerDiceScriptableObject")]
public class PlayerDiceScriptableObject : ScriptableObject
{
    public List<DiceScriptableObject> dice;
    public int health;
    public int sanity;
    public int exp;
    public List<ConditionDiceScriptableObject> playerCondition;
    public List<SpecialDicePlayer> playerSpecialDice;
    //public List<>

    private void OnEnable()
    {
        health = 4;
        sanity = 4;
        exp = 0;
        //playerCondition = new List<ConditionDiceScriptableObject>(3);
    }
}
