using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Database")]
    public QuestDatabase questDatabase;


    [Header("Quest Progression")]
    public List<QuestProgress> questProgressList = new();


    [System.Serializable]
    public class QuestProgress
    {
        public QuestData questData;

        public QuestState state;

        public List<string> completedObjectives = new();
    }

    public enum QuestState
    {
        Backlog,
        Ongoing,
        Completed
    }

    void OnEnable()
    {
        GlobalEventBus.Subscribe<ItemAddedEvent>(OnItemAdded);
        GlobalEventBus.Subscribe<SkillCheckCompletedEvent>(OnSkillCheckCompleted);
        GlobalEventBus.Subscribe<ResponseSelectedEvent>(OnResponseSelected);
    }

    void OnDisable()
    {
        GlobalEventBus.Unsubscribe<ItemAddedEvent>(OnItemAdded);
        GlobalEventBus.Unsubscribe<SkillCheckCompletedEvent>(OnSkillCheckCompleted);
        GlobalEventBus.Unsubscribe<ResponseSelectedEvent>(OnResponseSelected);
    }

    private void Start()
    {
        InitializeQuestProgress();
    }

    QuestProgress GetQuestByID(string questID)
    {
        foreach (var progress in questProgressList)
        {
            if (progress.questData.questID == questID)
                return progress;
        }
        return null;
    }

    public void ChangeQuestState(string questID, QuestState newState) 
    {
        QuestProgress progress = GetQuestByID(questID);
        if (progress == null) return;

        if (progress.state == newState) return;

        QuestState oldState = progress.state;
        progress.state = newState;

        HandleNPCLocations(progress.questData, newState);

        if (newState == QuestState.Ongoing)
        {
            GlobalEventBus.Publish(new QuestStartedEvent(questID));

            if (QuestNotificationUI.Instance != null)
            {
                QuestNotificationUI.Instance.ShowNotification($"{progress.questData.questName} Started");
            }
        }
        else if (newState == QuestState.Completed)
        {
            GlobalEventBus.Publish(new QuestCompletedEvent(questID));

            if (QuestNotificationUI.Instance != null)
            {
                QuestNotificationUI.Instance.ShowNotification($"{progress.questData.questName} Completed");
            }
        }

    }

    void InitializeQuestProgress()
    {
        foreach (QuestData quest in questDatabase.allQuest)
        {
            QuestProgress progress = new QuestProgress
            {
                questData = quest,
                state = QuestState.Backlog
            };

            questProgressList.Add(progress);

            if (quest.prerequisiteType == QuestData.PrerequisiteType.None)
            {
                ChangeQuestState(quest.questID, QuestState.Ongoing);
            }
        }
    }

    public List<QuestData> GetOngoingQuests()
    {
        List<QuestData> ongoingQuests = new();

        foreach (var progress in questProgressList)
        {
            if (progress.state == QuestState.Ongoing)
            {
                ongoingQuests.Add(progress.questData);
            }
        }

        return ongoingQuests;
    }

    public QuestState GetQuestState(string questID)
    {
        var quest = GetQuestByID(questID);
        return quest != null ? quest.state : QuestState.Backlog;
    }

    public bool IsQuestCompleted(string questID)
    {
        var quest = GetQuestByID(questID);
        return quest != null && quest.state == QuestState.Completed;
    }

    public bool IsQuestOngoing(string questID)
    {
        var quest = GetQuestByID(questID);
        return quest != null && quest.state == QuestState.Ongoing;
    }

    public void MarkQuestCompleted(string questID)
    {
        ChangeQuestState(questID, QuestState.Completed);

        CheckQuestUnlocks();
    }

    void CheckQuestUnlocks()
    {
        foreach (var progress in questProgressList)
        {
            if (progress.state != QuestState.Backlog)
                continue;

            QuestData quest = progress.questData;

            if (quest.prerequisiteType == QuestData.PrerequisiteType.Quest)
            {
                if (IsQuestCompleted(quest.prerequisiteID))
                {
                    ChangeQuestState(quest.questID, QuestState.Ongoing);
                }
            }
        }
    }

    void OnItemAdded(ItemAddedEvent evt)
    {
        CheckItemQuestUnlocks(evt.itemID);

        CheckItemObjective(evt.itemID);
    }

    void CheckItemQuestUnlocks(string itemID)
    {
        foreach (var progress in questProgressList)
        {
            if (progress.state != QuestState.Backlog)
                continue;

            QuestData quest = progress.questData;

            if (quest.prerequisiteType == QuestData.PrerequisiteType.Item &&
                quest.prerequisiteID == itemID)
            {
                ChangeQuestState(quest.questID, QuestState.Ongoing);
            }
        }
    }

    void CheckItemObjective(string itemID)
    {
        foreach (QuestProgress progress in questProgressList)
        {
            if (progress.state != QuestState.Ongoing)
                continue;

            QuestData quest = progress.questData;

            if (quest.objective == null)
                continue;

            QuestData.QuestObjective obj = quest.objective;

            if (obj.objectiveType != QuestData.ObjectiveType.Item)
                continue;

            if (obj.objectiveID == itemID)
            {
                MarkQuestCompleted(quest.questID);
            }
        }
    }

    void CheckSkillCheckObjective()
    {
        foreach (QuestProgress progress in questProgressList)
        {
            if (progress.state != QuestState.Ongoing)
                continue;

            QuestData quest = progress.questData;

            if (quest.objective == null)
                continue;

            QuestData.QuestObjective obj = quest.objective;

            if (obj.objectiveType != QuestData.ObjectiveType.SkillCheck)
                continue;

            MarkQuestCompleted(quest.questID);
        }
    }


    void OnSkillCheckCompleted(SkillCheckCompletedEvent evt)
    {
        if (evt.success != true)
            return;

        CheckSkillCheckObjective();
    }

    void OnResponseSelected(ResponseSelectedEvent evt)
    {
        CheckResponseObjective(evt.response.responseId);
    }

    void CheckResponseObjective(
        string responseId
    )
    {
        foreach (QuestProgress progress
            in questProgressList)
        {
            if (progress.state != QuestState.Ongoing)
                continue;

            QuestData quest = progress.questData;

            if (quest.objective == null)
                continue;

            QuestData.QuestObjective obj = quest.objective;

            if (obj.objectiveType != QuestData.ObjectiveType.Response)
                continue;

            if (obj.objectiveID == responseId)
            {
                MarkQuestCompleted(quest.questID);
            }
        }
    }

    void HandleNPCLocations(QuestData questData, QuestState newState)
    {
        foreach (var npcLocation in questData.npcLocations)
        {
            if (npcLocation.triggerState != newState)
                continue;

            QuestNPCLocation[] npcs = FindObjectsByType<QuestNPCLocation>(FindObjectsSortMode.None);

            foreach (var npc in npcs)
            {
                if (npc.npcID == npcLocation.npcID)
                {
                    npc.transform.position = npcLocation.newLocation;

                    break;
                }
            }
        }
    }
}