using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public List<SkillDiceScriptableObject> _skillDiceSO;
    /*Note for list skill dice :
     * 0 : strength
     * 1 : reflex
     * 2 : artist
     * 3 : deception
     * 4 : knowledge
     * 5 : observation
     */
    private List<SkillDiceScriptableObject> _instantiateSkillDice = new List<SkillDiceScriptableObject>();

    private void Awake()
    {
        for (int i = 0; i < _skillDiceSO.Count; i++)
        {
            _instantiateSkillDice.Add(ScriptableObject.Instantiate(_skillDiceSO[i]));
        }
    }

    public void MakeDiceNPC(int skillDiceIndex, List<int> diceValue)
    {
        _instantiateSkillDice[skillDiceIndex].diceValue = diceValue;
    }

    public List<int> MakeDiceValue(List<int> diceValue)
    {
        List<int> fixedDiceValue = new List<int>();

            for (int i = 0; i < diceValue.Count; i++)
            {
                fixedDiceValue.Add(diceValue[i]);
            }
            return fixedDiceValue;
    }

    public SkillDiceScriptableObject GetValueDiceNPC(int skillDiceIndex)
    {
        return _instantiateSkillDice[skillDiceIndex];
    }

    public List<SkillDiceScriptableObject> GetAllSkillDiceNPC()
    {
        return _instantiateSkillDice;
    }
}
