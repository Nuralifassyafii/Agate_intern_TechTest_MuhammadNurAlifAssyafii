using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Outcome")]
public class OutcomeData : ScriptableObject
{
    public string outcomeId;
    [TextArea] public string outcomeText;
    public List<OutcomeEffect> effects;
    public NarrativeNode nextNode;
}