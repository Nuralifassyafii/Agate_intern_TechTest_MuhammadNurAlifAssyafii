using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Assets/Databases/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> allItem;

    public ItemData GetItem(string id)
    {
        foreach (ItemData item in allItem)
        {
            if (item.itemID == id)
                return item;
        }

        return null;
    }

}
