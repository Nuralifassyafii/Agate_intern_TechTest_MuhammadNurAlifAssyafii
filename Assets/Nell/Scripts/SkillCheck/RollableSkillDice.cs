using UnityEngine;

public class RollableSkillDice : RollableDice
{
    [SerializeField] public SkillDiceScriptableObject diceData;

    public override void SetData(DiceScriptableObject data)
    {
        diceData = data as SkillDiceScriptableObject;
        faceTextsParent.SetActive(true);
        SetFaces(diceData.diceValue);
    }
    
    public override DiceResult CalculateTopFace()
    {
        int topFaceIndex = GetTopFaceIndex();

        return new DiceResult{Score = diceData.diceValue[topFaceIndex]};
    }
}
