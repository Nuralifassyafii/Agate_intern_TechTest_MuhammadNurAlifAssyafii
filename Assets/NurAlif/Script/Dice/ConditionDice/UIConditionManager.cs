using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIConditionManager : MonoBehaviour
{
    [Header("Title & Desc")]
    public TMP_Text titleTextField;
    public TMP_Text descriptionTextField;

    [Header("Condition")]
    [Header("Condition 1")]
    public TMP_Text titleTextFieldCondition1;
    public List<TMP_Text> conditionFaceTextFields1;

    [Header("Condition 2")]
    public TMP_Text titleTextFieldCondition2;
    public List<TMP_Text> conditionFaceTextFields2;

    [Header("Condition 3")]
    public TMP_Text titleTextFieldCondition3;
    public List<TMP_Text> conditionFaceTextFields3;

    [Header("is conditon filled list")]
    public List<bool> isFilled;

    [Header("Affected Skill")]
    public List<TMP_Text> AffectedSkillTextFields;

    private PlayerDiceScriptableObject _playerScriptableObject;
    [SerializeField] private FaceTypeManager _faceTypeManager;
    public void ShowConditionDetail(int index)
    {
        ShowConditionTitle(index);
        ShowConditionDescription(index);
        ShowAffectedSkill(index);
    }
    public void ShowConditionTitle(int index)
    {
        if (index <= _playerScriptableObject.playerCondition.Count)
        {
            titleTextField.text = _playerScriptableObject.playerCondition[index].conditionName;
        }
        else
        {
            Debug.Log("Index Tidak Ditemukan");
        }
    }

    //show condition description untuk deskripsi kondisi
    public void ShowConditionDescription(int index)
    {
        if (index <= _playerScriptableObject.playerCondition.Count)
        {
            descriptionTextField.text = _playerScriptableObject.playerCondition[index].conditionDescription;
        }
        else
        {
            Debug.Log("Index Tidak Ditemukan");
        }
    }

    public void ShowAffectedSkill(int index)
    {
        for (int i = 0; i < AffectedSkillTextFields.Count; i++)
        {
            if (i < _playerScriptableObject.playerCondition[index].affectedSkill.Count)
            {
                AffectedSkillTextFields[i].text = _playerScriptableObject.playerCondition[index].affectedSkill[i].ToString();
            }
            else
            {
                AffectedSkillTextFields[i].text = "-";
            }
        }
    }

    //show condition dice yang dimiliki karakter di UI menu condition dice (maksimal 3)
    //index
    public void ShowConditionIndex(int skillIndex)
    {
        ShowConditionIndexTitle(skillIndex);
        ShowFaceDiceCondition(skillIndex);
    }

    //index
    public void ShowConditionIndexTitle(int index)
    {
        TMP_Text pickedTextField = index == 0 ? titleTextFieldCondition1 : index == 1 ? titleTextFieldCondition2 : titleTextFieldCondition3;
        pickedTextField.text = _playerScriptableObject.playerCondition[index].conditionName;

    }

    //get face dan memanggil fungsi di face yang terpilih
    public void ShowFaceDiceCondition(int index)
    {
        List<TMP_Text> pickedTextField = index == 0 ? conditionFaceTextFields1 : index == 1 ? conditionFaceTextFields2 : conditionFaceTextFields3;
        EnumFaceType faceType;
        for (int i = 0; i < pickedTextField.Count; i++)
        {
            pickedTextField[i].text = _playerScriptableObject.playerCondition[index]._baseDiceFaceTypeValues[i].faceType == EnumFaceType.reRoll || _playerScriptableObject.playerCondition[index]._baseDiceFaceTypeValues[i].faceType == EnumFaceType.opponentLock ?
                _playerScriptableObject.playerCondition[index]._baseDiceFaceTypeValues[i].alias : 
                _playerScriptableObject.playerCondition[index]._baseDiceFaceTypeValues[i].value.ToString() + " "+_playerScriptableObject.playerCondition[index]._baseDiceFaceTypeValues[i].alias;
        }
    }

    private void Start()
    {
        _playerScriptableObject = FindFirstObjectByType<PlayerManager>().GetPlayerDiceSO();
    }

    private void Update()
    {
        for (int i = 0; i < _playerScriptableObject.playerCondition.Count; i++)
        {
            if (_playerScriptableObject.playerCondition[i] != null)
            {
                ShowConditionIndex(i);
            }
        }
    }
}
