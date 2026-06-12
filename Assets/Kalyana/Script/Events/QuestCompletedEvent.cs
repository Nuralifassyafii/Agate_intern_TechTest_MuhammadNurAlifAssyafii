public struct QuestCompletedEvent
{
    public string questID;

    public QuestCompletedEvent(string id)
    {
        questID = id;
    }
}