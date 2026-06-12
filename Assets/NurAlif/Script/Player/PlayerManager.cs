using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
//public class PlayerUIPlaceholder
//{
//    public EnumPlayerStat statName;
//    [Tooltip("Masukan TextUI dari scene")]
//    public TMP_Text _text;
//    public int value=0;
//}

public class PlayerManager : MonoBehaviour,IDataPersistent
{
    public PlayerDiceScriptableObject _playerDiceSO;
    private PlayerDiceScriptableObject _instantiatePlayerDiceSO;
    [SerializeField] private TMP_Text  healthText;
    [SerializeField] private TMP_Text sanityText;
    [SerializeField] private TMP_Text  expText;

    private void Awake()
    {
        _instantiatePlayerDiceSO = ScriptableObject.Instantiate(_playerDiceSO);
    }

    public PlayerDiceScriptableObject GetPlayerDiceSO()
    {
        return _instantiatePlayerDiceSO;
    }

    public void SetPlayerDiceSO(PlayerDiceScriptableObject playerSO)
    {
        this._instantiatePlayerDiceSO = playerSO;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.sanity = _instantiatePlayerDiceSO.sanity;
        gameData.exp = _instantiatePlayerDiceSO.exp;
        gameData.health = _instantiatePlayerDiceSO.health;
    }

    public void LoadData(GameData gameData)
    {
        _instantiatePlayerDiceSO.exp = gameData.exp;
        _instantiatePlayerDiceSO.health = gameData.health;
        _instantiatePlayerDiceSO.sanity = gameData.sanity;
    }

    private void Update()
    {
        healthText.text = "X"+_instantiatePlayerDiceSO.health.ToString();
        sanityText.text = "X" + _instantiatePlayerDiceSO.sanity.ToString();
        expText.text = "X" + _instantiatePlayerDiceSO.exp.ToString();
    }

    private void Start()
    {
        healthText = GameObject.Find("PlayerHealthText").GetComponent<TMP_Text>();
        sanityText = GameObject.Find("PlayerSanityText").GetComponent<TMP_Text>();
        expText = GameObject.Find("PlayerExpText").GetComponent<TMP_Text>();
    }
}
