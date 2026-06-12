using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class FaceTypeManager : MonoBehaviour
{
    [SerializeField] private PlayerDiceScriptableObject _instantiatePlayerSO;
    private Dictionary<EnumPlayerStat,string> listStringCondition = new Dictionary<EnumPlayerStat,string>();

    private void Start()
    {
        _instantiatePlayerSO =FindFirstObjectByType<PlayerManager>().GetPlayerDiceSO();
        AddStringToListStat(EnumPlayerStat.health, "HP");
        AddStringToListStat(EnumPlayerStat.sanity, "SN");
    }

    //modified value untuk add & subtract
    public void ModifiedPlayerRollValue(int value, EnumPlayerStat modifiedStat, bool isAdder)
    {
        int fixedValue = 0;
        FieldInfo playerStatField = _instantiatePlayerSO.GetType().GetField(modifiedStat.ToString());
        if (isAdder)
        {
            fixedValue = 1 * value;
        }
        else
        {
            fixedValue = -1 * value;
        }
        fixedValue += (int)playerStatField.GetValue(_instantiatePlayerSO);
        _instantiatePlayerSO.GetType().GetField(modifiedStat.ToString()).SetValue(_instantiatePlayerSO, fixedValue);
    }

    //modified data UI face untuk adder & subtractor
    public string UIFaceModifier(EnumPlayerStat modifiedStat, int value)
    {
        return value.ToString() + GetStringForModifiedStat(modifiedStat);
    }

    public string GetStringForModifiedStat(EnumPlayerStat modifiedStat)
    {
        if(listStringCondition.TryGetValue(modifiedStat,out string getString))
        {
            return getString;
        }
        else
        {
            return "XXX"; //kalau belum ada
        }
    }

    //add string setelah modified value contoh health => HP
    private void AddStringToListStat(EnumPlayerStat modifiedStat, string stringStat)
    {
        listStringCondition.Add(modifiedStat, stringStat);
    }

    //function reroll
    public void Reroll(int rerollCount)
    {
        rerollCount += 1;
    }

    //function untuk lock dadu
    public bool GetLocked(bool islocked)
    {
        return islocked = !islocked;
    }
}
