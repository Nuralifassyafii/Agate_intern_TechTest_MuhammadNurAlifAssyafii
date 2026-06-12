using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Assets/Item")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public string itemName;
    [TextArea] public string itemDescription;
}
