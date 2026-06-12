using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialDiceMaker : MonoBehaviour
{
    private List<DropManager> _listCraftingMaterial;
    private int specialDiceMaxLength = 6;
    private PlayerManager _playerManager;
    [SerializeField] private SpecialDiceScriptable _baseSpecialDice;
    private bool isDiceReady = false;
    private SpecialDiceUIManager _specialDiceUI;

    private void Start()
    {
        _listCraftingMaterial = new List<DropManager>(specialDiceMaxLength);
        _playerManager = FindFirstObjectByType<PlayerManager>();
        _specialDiceUI = FindFirstObjectByType<SpecialDiceUIManager>();
    }

    public void AddListMaterialSpecialDice(DropManager craftingMaterial)
    {
        try
        {
            if (_listCraftingMaterial.Count <= specialDiceMaxLength)
            {
                _listCraftingMaterial.Add(craftingMaterial);
            }
            else
            {
                //ui text alert ubah jadi max length
                _specialDiceUI.SetAlertText("Material sudah terpenuhi");
            }
        }
        catch (Exception e)
        {
            //ui text alert diganti jadi exception
            _specialDiceUI.SetAlertText("Ada masalah dengan pengaturan crafting : " + e);
        }
    }

    public SpecialDiceScriptable MakeSpecialDice()
    {
        SpecialDiceScriptable specialDice = ScriptableObject.Instantiate(_baseSpecialDice);
        specialDice.diceType = EnumDiceType.specialDice;
        if (IsCraftingMaterialEnough() && !IsCraftingMaterialNull())
        {
            for (int i = 0; i < _listCraftingMaterial.Count; i++)
            {
                CreateSpecialDiceBaseModifiedStat(specialDice, _listCraftingMaterial[i].GetPickedMaterial());
            }
            ChangeStatusDice();
        }
        else
        {
            //ui text alert ubah jadi kesalahan saat membuat special dice
            _specialDiceUI.SetAlertText("crafting material belum cukup");
        }
        return specialDice;
    }

    public void ChangeStatusDice()
    {
        isDiceReady = !isDiceReady;
    }

    public void AddSpecialDiceToPlayer()
    {
        SpecialDiceScriptable SpecialDiceForPlayer = MakeSpecialDice();
        try
        {
            if (isDiceReady && _playerManager.GetPlayerDiceSO().playerSpecialDice.Count<=3)
            {
                _playerManager.GetPlayerDiceSO().playerSpecialDice.Add(new SpecialDicePlayer(true,SpecialDiceForPlayer));
                _specialDiceUI.SetSpecialDiceUI(_playerManager.GetPlayerDiceSO().playerSpecialDice.Count - 1);
                ChangeStatusDice();
                DestroyAllMaterialSpecialDice();
            }
        }
        catch (Exception e)
        {
            //UI Alert 
            _specialDiceUI.SetAlertText("ada yang salah saat memasukan dice ke player : " + e);
        }
    }

    public bool IsCraftingMaterialEnough()
    {
        return _listCraftingMaterial.Count == specialDiceMaxLength;
    }

    public bool IsCraftingMaterialNull()
    {
        bool isNull = false;
        for (int i = 0; i < _listCraftingMaterial.Count; i++)
        {
            isNull = _listCraftingMaterial[i] == null ? true : isNull;
        }
        return isNull;
    }

    public bool IsHaveValue(EnumFaceType faceType)
    {
        return faceType == EnumFaceType.integerValue || faceType == EnumFaceType.health || faceType == EnumFaceType.exp;
    }

    public void CreateSpecialDiceBaseModifiedStat(SpecialDiceScriptable baseSpecialDice, MaterialSpecialDice materialSpecialDice)
    {
        if (IsHaveValue(materialSpecialDice.face))
        {
            baseSpecialDice._baseDiceFaceTypeValues.Add(new BaseDiceFaceTypeValue(materialSpecialDice.face,materialSpecialDice.value,materialSpecialDice.alias));
        }
        else
        {
            baseSpecialDice._baseDiceFaceTypeValues.Add(new BaseDiceFaceTypeValue(materialSpecialDice.face, 0, materialSpecialDice.alias));
        }
    }

    public void DestroyAllMaterialSpecialDice()
    {
        for(int i = 0; i < _listCraftingMaterial.Count; i++)
        {
            _listCraftingMaterial[i].DestroyMaterial();
        }
        _listCraftingMaterial.Clear();
    }
}
