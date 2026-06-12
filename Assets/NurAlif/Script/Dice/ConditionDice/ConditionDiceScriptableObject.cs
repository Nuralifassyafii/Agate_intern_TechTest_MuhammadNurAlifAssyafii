using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionDiceScriptableObject", menuName = "Scriptable Objects/ConditionDiceScriptableObject")]
public class ConditionDiceScriptableObject : DiceScriptableObject
{
    public string conditionName;
    public string conditionDescription;
    public EnumConditionDice conditions;
    public EnumBuffNerfCondition buffNerfCondition;
    public List<EnumSkillDice> affectedSkill;
    public List<int> faceDiceValues;
}
