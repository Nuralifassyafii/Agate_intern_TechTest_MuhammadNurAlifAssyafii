using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NPC/Database")]
public class NPCDatabase : ScriptableObject
{
    public List<NPCData> npcs;
    private Dictionary<string, NPCData> npcDict;

    public void Init()
    {
        npcDict = new Dictionary<string, NPCData>();
        foreach (var npc in npcs)
        {
            if (npc == null) continue;
            npcDict[npc.npcId] = npc;
            npc.currentNode = npc.startNode;
        }
    }

    private void OnEnable()
    {
        Init();
    }

    public NPCData GetNPC(string id)
    {
        if (npcDict == null) Init();
        return npcDict.ContainsKey(id) ? npcDict[id] : null;
    }
}