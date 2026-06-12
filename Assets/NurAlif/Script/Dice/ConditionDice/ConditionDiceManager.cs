using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class ConditionDiceManager : MonoBehaviour
{
    [SerializeField] private UIConditionManager _uiCondition;
    [SerializeField] private List<ConditionDiceScriptableObject> _listConditionDice;
    /*
     * NOTE FOR LIST CONDITION DICE :
     * 0 : AGILE
     * 1 : BORED
     * 2 : CONFIDENT
     * 3 : CUNNING
     * 4 : DISTRACTED
     * 5 : FATIGUE
     * 6 : FIT
     * 7 : FOCUSED
     * 8 : INSIPIRED
     * 9 : NAIVE
     * 10 : SLUGGISH
     * 11 : UNSURE
     */
    private List<ConditionDiceScriptableObject> _listInstantiateConditionDice = new List<ConditionDiceScriptableObject>();
    private PlayerDiceScriptableObject _instantiatePlayerSO;

    //menambahkan kondisi ke player dengan proses pemanggilan dari class parent utama (class ini) (butuh index kondisi ke berapa yang ingin dimasukan, dan index posisi kondisi player)
    public void AddConditionToPlayerByIndex(int playerConditionIndex, int conditionDiceIndex)
    {
        _instantiatePlayerSO.playerCondition[playerConditionIndex] = _listInstantiateConditionDice[conditionDiceIndex];
    }

    //Menambahkan kondisi ke player dengan proses pemanggilan dari class parent utama menggunakan string
    public void AddConditionToPlayerByName(string conditionString)
    {
        string modifiedString = conditionString.ToLower();
        for (int i = 0; i < _instantiatePlayerSO.playerCondition.Count; i++)
        {
            if (_instantiatePlayerSO.playerCondition[i] == null && !CheckDuplicateForConditionDice(conditionString))
            {
                _instantiatePlayerSO.playerCondition[i] = GetScriptableObjectByString(modifiedString);
                break;
            }
        }
    }

    //menghapus kondisi dari player menggunakan string sesuai dengan enum condition dice
    public void RemoveConditionFromPlayerByName(string conditionString)
    {
        string modifiedString = conditionString.ToLower();
        if (CheckDuplicateForConditionDice(conditionString))
        {
            _instantiatePlayerSO.playerCondition[GetRemovedConditionIndex(conditionString)] = null;
        }
    }

    //mendapatkan condition dice menggunakan string yg sesuai enum condition dice
    public ConditionDiceScriptableObject GetScriptableObjectByString(string conditionString)
    {
        string modifiedString = conditionString.ToLower();
        for (int j = 0; j < _listInstantiateConditionDice.Count; j++)
        {
            if (_listInstantiateConditionDice[j].conditions.ToString().Equals(modifiedString))
            {
                return _listInstantiateConditionDice[j];
                break;
            }
        }

        return null;
    }

    // mengecek duplicate di condition dice player
    public bool CheckDuplicateForConditionDice(string conditionString)
    {
        string modifiedString = conditionString.ToLower();
        bool isDuplicate = false;
        for (int i = 0; i < _instantiatePlayerSO.playerCondition.Count; i++)
        {
            if (_instantiatePlayerSO.playerCondition[i] != null && _instantiatePlayerSO.playerCondition[i].conditions.ToString().Equals(modifiedString))
            {
                isDuplicate = true;
            }
        }
        return isDuplicate;
    }

    //mendapatkan index condition yang ingin dihilangkan
    public int GetRemovedConditionIndex(string conditionString)
    {
        string modifiedConditionString = conditionString.ToLower();
        int index = -1; //untuk sementara (masih belum sempurna untuk menemukan default index saat tidak ada yang harus dibalikan)
        for (int i = 0; i < _instantiatePlayerSO.playerCondition.Count; i++)
        {
            if (_instantiatePlayerSO.playerCondition[i] != null && _instantiatePlayerSO.playerCondition[i].conditions.ToString().Equals(modifiedConditionString))
            {
                index = i;
                break;
            }
        }
        return index;
    }

    //get instantiate untuk player SO
    private void Awake()
    {
        for(int i = 0;i<_listConditionDice.Count;i++)
        {
            _listInstantiateConditionDice.Add(ScriptableObject.Instantiate(_listConditionDice[i]));
        }
        _instantiatePlayerSO = FindFirstObjectByType<PlayerManager>().GetPlayerDiceSO();
    }

    private void Start()
    {
        _instantiatePlayerSO.playerCondition.Add(_listInstantiateConditionDice[6]); //tester
        _instantiatePlayerSO.playerCondition.Add(_listInstantiateConditionDice[1]); //tester
    }
}
