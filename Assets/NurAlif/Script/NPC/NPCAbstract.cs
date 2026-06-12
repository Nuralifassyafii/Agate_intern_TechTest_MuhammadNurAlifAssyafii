using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class NPCDiceList
{
    public EnumSkillDice skillType;
    public List<int> diceValue;
}
public class NPCAbstract : MonoBehaviour
{
    [SerializeField] public List<NPCDiceList> usedDice;

    public NPCManager _npcManager;

    public int GetIndexUsedSkillDiceNPC(EnumSkillDice enumUsedSkill)
    {
        int indexSkillDice = -1;
        try
        {
            indexSkillDice = _npcManager._skillDiceSO.FindIndex(x => x.skillType == enumUsedSkill);
        }
        catch (Exception e)
        {
            Debug.Log("ada masalah saat mencari index untuk skill dice yang dipakai NPC : " + e);
        }
        return indexSkillDice;
    }

    public void SetUsedDiceForNPC()
    {
        for (int i = 0; i < usedDice.Count; i++)
        {
            _npcManager.MakeDiceNPC(GetIndexUsedSkillDiceNPC(usedDice[i].skillType), usedDice[i].diceValue);
        }
    }

    //private void Start() //tester
    //{
    //    SetUsedDiceForNPC();
    //}
}
