using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SkillDiceScriptableObject", menuName = "Scriptable Objects/SkillDiceScriptableObject")]
public class SkillDiceScriptableObject : DiceScriptableObject
{
    public bool lockUpgrade;
    public EnumSkillDice skillType;
    public List<int> diceValue;

    private void OnEnable()
    {
        //for(int i = 0; i < diceValue.Count; i++)
        //{
        //    diceValue[i] = 1;
        //}
    }
}
