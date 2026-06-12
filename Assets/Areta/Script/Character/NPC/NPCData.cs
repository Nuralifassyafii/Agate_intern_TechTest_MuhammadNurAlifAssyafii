using UnityEngine;

[CreateAssetMenu(menuName = "NPC/Data")]
public class NPCData : CharacterData
{
    public string npcId => characterId;
    public string npcName => characterName;

    public NarrativeNode startNode;
    public NarrativeNode currentNode;
}