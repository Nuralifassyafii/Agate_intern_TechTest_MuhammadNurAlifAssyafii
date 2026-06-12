using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Assets/Quest")]
public class QuestData : ScriptableObject
{

    public string questID;
    public QuestType questType;
    public PrerequisiteType prerequisiteType;
    public string prerequisiteID;
    [TextArea]
    public string questName;
    [TextArea] 
    public string questDescription;

    public QuestObjective objective;

    public List<QuestHint> hints;


    public enum QuestType
    {
        Main,
        Side
    }

    public enum PrerequisiteType
    {
        None,
        Item,
        Quest
    }

    public enum ObjectiveType
    {
        Item,
        SkillCheck,
        Response
    }

    [System.Serializable]
    public class QuestObjective
    {
        public ObjectiveType objectiveType;
        public string objectiveID;
    }

    [System.Serializable]
    public class QuestHint
    {
        [TextArea]
        public string hintText;
        public bool unlockedAtStart;
        public string requiredItemID;
    }

    [System.Serializable]
    public class NPCQuestLocation
    {
        public QuestManager.QuestState triggerState;

        public string npcID;

        public Vector3 newLocation;
    }

    public List<NPCQuestLocation> npcLocations = new();
}
