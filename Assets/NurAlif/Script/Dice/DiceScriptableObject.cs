using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceScriptableObject", menuName = "Scriptable Objects/DiceScriptableObject")]
public class DiceScriptableObject : ScriptableObject
{
    public string id;
    /*Note ID:
    - "D" 
        +
    - Tipe dadu : 
    1 : skill
    2 : condition
    3 : special
        +
    - urutan pembuatan dadu
    
    contoh : pembuatan ke 10 special dice -> "D310"
     */
    public EnumDiceType diceType;
    public List<EnumFaceType> faceType;
    public List<BaseDiceFaceTypeValue> _baseDiceFaceTypeValues;
}
