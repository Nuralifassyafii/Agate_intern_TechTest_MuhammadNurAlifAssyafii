using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCDiceConfigurator : MonoBehaviour
{
    [SerializeField] public List<NPCDiceList> usedDice;
    [SerializeField] public NPCManager _npcManager;

    void Start()
    {
        if (!GameContext.SceneServices.TryGet(out _npcManager))
        {
            Debug.LogError("NPCManager Not Found!");
        }
    }

    public int GetIndexUsedSkillDiceNPC(EnumSkillDice enumUsedSkill)
    {
        int indexSkillDice = -1;
        if (_npcManager == null)
        {
            _npcManager = FindObjectOfType<NPCManager>();
        }

        if (_npcManager == null)
        {
            Debug.LogError("NPCManager is null and could not be found in the scene.");
            return indexSkillDice;
        }

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
        if (_npcManager == null)
        {
            _npcManager = FindObjectOfType<NPCManager>();
        }

        if (_npcManager == null)
        {
            Debug.LogError("NPCManager is null and could not be found in the scene.");
            return;
        }

        for (int i = 0; i < usedDice.Count; i++)
        {
            _npcManager.MakeDiceNPC(GetIndexUsedSkillDiceNPC(usedDice[i].skillType), usedDice[i].diceValue);
        }
    }
}
