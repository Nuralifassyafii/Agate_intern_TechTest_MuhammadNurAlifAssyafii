using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/Node")]
public class NarrativeNode : ScriptableObject
{
    public string nodeId;
    public bool isCompleted;
    [TextArea] public string exposition;
    public ResponseData[] responses;
}