using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RollableConditionDice : RollableDice
{
    [SerializeField] public ConditionDiceScriptableObject diceData;

    public override void SetData(DiceScriptableObject data)
    {
        diceData = data as ConditionDiceScriptableObject;
        faceTextsParent.SetActive(true);
        faceSpritesParent.SetActive(true);
        SetFaces(diceData.faceDiceValues);
        for (int i = 0; i<6; i++)
        {
            switch (diceData.faceType[i])
            {
                case EnumFaceType.integerValue:
                    _faceTexts[i].text = diceData.faceDiceValues[i].ToString();
                    _faceSprites[i].enabled = false;
                    break;
                case EnumFaceType.opponentLock:
                    _faceTexts[i].text = "Lock";
                    // _faceSprites[i].sprite = availableFaceSprites[0];
                    break;
                case EnumFaceType.reRoll:
                    _faceTexts[i].text = "Roll";
                    // _faceSprites[i].sprite = availableFaceSprites[1];
                    break;
                case EnumFaceType.health:
                    _faceTexts[i].text = diceData.faceDiceValues[i].ToString() + " HP";
                    _faceSprites[i].enabled = false;
                    break;
                case EnumFaceType.exp:
                    _faceTexts[i].text = diceData.faceDiceValues[i].ToString() + " XP";
                    _faceSprites[i].enabled = false;
                    break;
                case EnumFaceType.sanity:
                    _faceTexts[i].text = diceData.faceDiceValues[i].ToString() + " San";
                    _faceSprites[i].enabled = false;
                    break;
                default:
                    break;
            }
        }
    }
    
    public override DiceResult CalculateTopFace()
    {
        int topFaceIndex = GetTopFaceIndex();

        switch (diceData.faceType[topFaceIndex])
        {
            case EnumFaceType.integerValue:
                Debug.Log("Score " + diceData.faceDiceValues[topFaceIndex]);
                return new DiceResult{Score = diceData.faceDiceValues[topFaceIndex]};
            case EnumFaceType.opponentLock:
                Debug.Log("OpponentLock");
                return new DiceResult{OpponentLock = diceData.faceDiceValues[topFaceIndex]};
            case EnumFaceType.reRoll:
                Debug.Log("Reroll");
                return new DiceResult{Reroll = diceData.faceDiceValues[topFaceIndex]};
            case EnumFaceType.health:
                Debug.Log("Health " + diceData.faceDiceValues[topFaceIndex]);
                return new DiceResult{Health = diceData.faceDiceValues[topFaceIndex]};
            case EnumFaceType.exp:
                Debug.Log("EXP " + diceData.faceDiceValues[topFaceIndex]);
                return new DiceResult{EXP = diceData.faceDiceValues[topFaceIndex]};
            case EnumFaceType.sanity:
                Debug.Log("Sanity " + diceData.faceDiceValues[topFaceIndex]);
                return new DiceResult{Sanity = diceData.faceDiceValues[topFaceIndex]};
            default:
                return new DiceResult{};
        }
    }
}
