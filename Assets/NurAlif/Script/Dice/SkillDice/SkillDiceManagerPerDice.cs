using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SkillDiceManagerPerDice : MonoBehaviour, IDataPersistent
{
    private float clickInterval = 3f;
    private PlayerManager _playerManager;
    private PlayerDiceScriptableObject _playerDice;
    private int skillCost = 3;
    public List<TMP_Text> faceTextField;
    public SkillDiceScriptableObject _pickedSkillDiceScriptableObject;
    private SkillDiceScriptableObject _instantiatePickedSkillDiceSO;
    private SkilllDiceUIManager _skillDiceUIManager;
    private float sisaWaktuCountdown;
    [SerializeField] private int indexSkillForSaveData;


    public void ShowValue(TMP_Text textfield, int skillFaceIndex)
    {
        textfield.text = _instantiatePickedSkillDiceSO.diceValue[skillFaceIndex].ToString();
    }
    public void IsUpgradePressed(int skillIndex)
    {
        StartCoroutine(ClickUpgrade(_instantiatePickedSkillDiceSO, skillIndex, false));
    }
    public void IsAllUpgradePressed(int skillIndex)
    {
        StartCoroutine(ClickUpgrade(_instantiatePickedSkillDiceSO, skillIndex, true));
    }

    //fungsi yang digunakan saat button upgrade diklik player
    public IEnumerator ClickUpgrade(SkillDiceScriptableObject _skillDiceScriptableObject, int faceNumber, bool isAllUpgrade)
    {
        sisaWaktuCountdown = clickInterval;
        while (sisaWaktuCountdown > 0)
        {
            _skillDiceUIManager.ShowCountDown(sisaWaktuCountdown, clickInterval);

            yield return null;

            sisaWaktuCountdown -= Time.deltaTime;
        }
        sisaWaktuCountdown = 0;
        _skillDiceUIManager.ShowCountDown(sisaWaktuCountdown, clickInterval);
        if (!isAllUpgrade)
        {
            UpgradeDiceSkill(_skillDiceScriptableObject, faceNumber);
        }
        else
        {
            UpgradeAllDiceSkill(_skillDiceScriptableObject);
        }
    }

    public int GetCost(int faceDiceIndex)
    {
        return skillCost * GetPickedDiceValue().diceValue[faceDiceIndex];
    }

    //fungsi yang dipanggil untuk upgrade skill dice
    public void UpgradeDiceSkill(SkillDiceScriptableObject _skillDiceScriptableObject, int faceNumber)
    {
        int fixedCost = skillCost * _skillDiceScriptableObject.diceValue[faceNumber];
        if (_playerDice.exp >= fixedCost)
        {
            if (!IsSkillMax(_skillDiceScriptableObject, faceNumber))
            {
                _playerDice.exp -= fixedCost;
                _skillDiceScriptableObject.diceValue[faceNumber] += 1;
                StopAllCoroutines();
                StartCoroutine(_skillDiceUIManager.ShowNotification(GetPickedDiceValue().skillType.ToString() + " Berhasil Diupgrade"));
            }
        }
        else
        {
            _skillDiceUIManager.SetSkillDiceAlert("EXP tidak mencukupi / level sudah maksimal");
            _skillDiceUIManager.SetCountdownImageFailed(); //nanti diganti (masih belum)
        }
    }

    //fungsi yang dipanggil untuk cek total cost dari skill dice yang ingin diupgrade
    public int GetTotalCost(SkillDiceScriptableObject _skillDiceScriptableObject)
    {
        int totalCost = 0;
        for (int i = 0; i < _skillDiceScriptableObject.diceValue.Count; i++)
        {
            totalCost += _skillDiceScriptableObject.diceValue[i] * skillCost;
        }

        return totalCost;
    }

    //fungsi yang dipanggil untuk upgrade seluruh dice skill yang dimiliki player
    public void UpgradeAllDiceSkill(SkillDiceScriptableObject _skillDiceScriptableObject)
    {
        int totalCost = GetTotalCost(_skillDiceScriptableObject);

        if (_playerDice.exp >= totalCost)
        {
            for (int i = 0; i < _skillDiceScriptableObject.diceValue.Count; i++)
            {
                UpgradeDiceSkill(_skillDiceScriptableObject, i);
            }
        }
        else
        {
            _skillDiceUIManager.SetSkillDiceAlert("Xp kurang / Level maksimal");
            _skillDiceUIManager.SetCountdownImageFailed(); //nanti diganti (masih belum)
        }
    }

    //cek apakah skill sudah max atau belum (sekarang 13)
    public bool IsSkillMax(SkillDiceScriptableObject _skillDiceScriptableObject, int faceNumber)
    {
        int skillNow = _skillDiceScriptableObject.diceValue[faceNumber];
        bool isSkillMax = skillNow < 13 ? false : true;

        return isSkillMax;
    }
    public void SaveData(ref GameData gameData)
    {
        _playerManager.SaveData(ref gameData);
        for (int i = 0; i < _instantiatePickedSkillDiceSO.diceValue.Count; i++)
        {
            gameData.skillDiceList[indexSkillForSaveData][i] = this._instantiatePickedSkillDiceSO.diceValue[i];
        }
    }

    public void LoadData(GameData gameData)
    {
        for (int i = 0; i < _instantiatePickedSkillDiceSO.diceValue.Count; i++)
        {
            this._instantiatePickedSkillDiceSO.diceValue[i] = gameData.skillDiceList[indexSkillForSaveData][i];
        }
    }

    public void SetPickedDiceValue(SkillDiceScriptableObject _skillDice)
    {
        this._instantiatePickedSkillDiceSO = _skillDice;
    }

    public SkillDiceScriptableObject GetPickedDiceValue()
    {
        return this._instantiatePickedSkillDiceSO;
    }

    private void Update()
    {
        for (int i = 0; i < faceTextField.Count; i++)
        {
            ShowValue(faceTextField[i], i);
        }
    }

    public void SetInstantiadeSkillDice()
    {
        _instantiatePickedSkillDiceSO = ScriptableObject.Instantiate(_pickedSkillDiceScriptableObject);
    }

    private void Awake()
    {
        _playerManager = FindFirstObjectByType<PlayerManager>();
        _instantiatePickedSkillDiceSO = ScriptableObject.Instantiate(_pickedSkillDiceScriptableObject);
    }

    private void Start()
    {
        _skillDiceUIManager = FindFirstObjectByType<SkilllDiceUIManager>();
        if (!_playerManager)
        {
            if (!GameContext.PersistentServices.TryGet(out _playerManager))
            {
                Debug.LogError("PlayerManager Not Found!");
            }
        }
        if (_playerManager) _playerDice = _playerManager.GetPlayerDiceSO();
    }
}
