using UnityEngine;

[CreateAssetMenu(fileName = "MaterialSpecialDice", menuName = "Scriptable Objects/MaterialSpecialDice")]
public class MaterialSpecialDice : ScriptableObject
{
    public string id;
    /*
     NOTE ID :
    - "M" 
        +
    - Urutan pembuatan material
    contoh : pembuatan material special dice ke 2 -> "M02"
     */
    public EnumFaceType face;
    public int value;
    public EnumPlayerStat modifiedValue;
    public int amount;
    public string alias;
}
