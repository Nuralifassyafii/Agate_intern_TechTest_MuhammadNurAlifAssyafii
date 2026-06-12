using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "New Quest Database", menuName = "Assets/Databases/Quest Database")]
public class QuestDatabase : ScriptableObject
{
    public List<QuestData> allQuest;

}