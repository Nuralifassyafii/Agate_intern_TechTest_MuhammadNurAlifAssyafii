using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterData : ScriptableObject
{
    public string characterId;
    public string characterName;
    
    public Vector3 location;
    
    public string animationState;
    
    public List<DiceScriptableObject> dice;
    public List<ConditionDiceScriptableObject> conditionDice;

    public List<string> inventoryItems;
}
