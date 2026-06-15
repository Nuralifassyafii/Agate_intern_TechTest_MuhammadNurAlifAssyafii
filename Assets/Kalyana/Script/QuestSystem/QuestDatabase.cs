using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Database", menuName = "Assets/Databases/Quest Database")]
public class QuestDatabase : ScriptableObject
{
    public List<QuestData> allQuest;

}