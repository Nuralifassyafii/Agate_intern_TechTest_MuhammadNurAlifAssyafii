public struct ItemRemovedEvent
{
    public string itemID;

    public ItemRemovedEvent(string itemID)
    {
        this.itemID = itemID;
    }
}