using UnityEngine;

[System.Serializable]
public class ResponseData
{
    public string responseId;
    public string text;
    public ResponseType type;
    public EnumSkillDice skillType;
    public PrerequisiteData prerequisite;
    public OutcomeSet outcomes;
}