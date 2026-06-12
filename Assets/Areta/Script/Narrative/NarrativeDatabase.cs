using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/Database")]
public class NarrativeDatabase : ScriptableObject
{
    public List<NarrativeNode> nodes;
    private Dictionary<string, NarrativeNode> nodeDict;

    public void Init()
    {
        nodeDict = new Dictionary<string, NarrativeNode>();
        foreach (var node in nodes)
        {
            if (node == null) continue;
            if (nodeDict.ContainsKey(node.nodeId))
            {
                Debug.LogError($"Duplicate Node ID: {node.nodeId}");
                continue;
            }
            nodeDict[node.nodeId] = node;
            node.isCompleted = false;
        }
    }

    private void OnEnable()
    {
        Init();
    }

    public NarrativeNode GetNode(string id)
    {
        if (nodeDict == null) Init();
        return nodeDict.ContainsKey(id) ? nodeDict[id] : null;
    }
}