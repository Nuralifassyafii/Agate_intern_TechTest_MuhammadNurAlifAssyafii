using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AmountUI
{
    public TMP_Text amountText;
    public string amountId;
}

public class SpecialDiceUIManager : MonoBehaviour
{
    [SerializeField] private List<AmountUI> listAmountText;
    [SerializeField] private TMP_Text alertText;
    [SerializeField] private List<Image> listPriceSpecialDice;
    [SerializeField] private List<Image> listIconSpecialDice;
    [SerializeField] private Image baseIconDice;
    [SerializeField] private Image baseIconPriceSpecialDice;
    [SerializeField] private List<SpecialDiceVessel> iconVessel;
    private PlayerManager _playerManager;

    public void SetMaterialAmount(string materialId, string text)
    {
        foreach (AmountUI item in listAmountText)
        {
            if (item.amountId.Equals(materialId))
            {
                item.amountText.text = "X" + text;
            }
        }
    }

    public void SetAlertText(string text)
    {
        alertText.text = text;
    }

    public void SetBaseIconSpecialDice(int specialDiceIndex, Image baseIconImg, Image basePriceImg)
    {
        iconVessel[specialDiceIndex].specialDiceIconVessel.sprite = baseIconImg.sprite;
        for (int i = 0; i < iconVessel[specialDiceIndex].specialDicePriceVessel.Count; i++)
        {
            iconVessel[specialDiceIndex].specialDicePriceVessel[i].sprite = basePriceImg.sprite;
        }
    }

    public void SetIconSpecialDice(int specialDiceIndex)
    {
        List<Image> stockPrice = listIconSpecialDice;
        int randomNumber = Random.Range(0, stockPrice.Count);
        if (_playerManager.GetPlayerDiceSO().playerSpecialDice[specialDiceIndex] != null)
        {
            iconVessel[specialDiceIndex].specialDiceIconVessel.sprite = listIconSpecialDice[Random.Range(0, listIconSpecialDice.Count - 1)].sprite;
        }
    }

    public void SetPriceSpecialDice(int specialDiceIndex)
    {
        if (_playerManager.GetPlayerDiceSO().playerSpecialDice[specialDiceIndex] != null)
        {
            SetFaceDiceSpecialDiceUI(iconVessel[specialDiceIndex].specialDicePriceVessel);
        }
    }

    public void SetFaceDiceSpecialDiceUI(List<Image> specialDiceFace)
    {
        List<Sprite> stockPrice = listPriceSpecialDice.Select(item => item.sprite).ToList();
        for (int i = 0;i<specialDiceFace.Count;i++)
        {
            int randomNumber = Random.Range(0, stockPrice.Count);

            if (stockPrice[randomNumber] != null)
            {
                specialDiceFace[i].sprite = stockPrice[randomNumber];
                stockPrice[randomNumber] = null;
            }
            else
            {
                i--;
            }
        }
    }

    public void SetSpecialDiceUI(int index)
    {
        SetIconSpecialDice(index);
        SetPriceSpecialDice(index);
    }

    private void Start()
    {
        _playerManager = FindFirstObjectByType<PlayerManager>();
        SetAlertText("Alert will show in here");
        for(int i = 0; i < iconVessel.Count; i++)
        {
            SetBaseIconSpecialDice(i,baseIconDice,baseIconPriceSpecialDice);
        }
    }
}

