using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Classes : MonoBehaviour
{
    
}

[System.Serializable]
public class BaseDiceFaceTypeValue
{
    public EnumFaceType faceType;
    public int value;
    public string alias;

    public BaseDiceFaceTypeValue(EnumFaceType faceType, int value, string alias)
    {
        this.faceType = faceType;
        this.value = value;
        this.alias = alias;
    }
}

[System.Serializable]
public class SpecialDiceVessel
{
    public Image specialDiceIconVessel;
    public List<Image> specialDicePriceVessel;
}

[System.Serializable]
public class SpecialDicePlayer
{
    public bool isActive;
    public SpecialDiceScriptable specialDicePriceVessel;

    public SpecialDicePlayer(bool isActive, SpecialDiceScriptable specialDiceForPlayer)
    {
        this.isActive = isActive;
        this.specialDicePriceVessel = specialDiceForPlayer;
    }
}

[System.Serializable]
public class CostUI
{
    public GameObject CostUIObject;
    public TMP_Text CostUIText;

    public CostUI(GameObject CostUIObject, TMP_Text CostUIText)
    {
        this.CostUIObject = CostUIObject;
        this.CostUIText = CostUIText;
    }
}
