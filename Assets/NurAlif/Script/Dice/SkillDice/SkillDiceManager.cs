using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SkillDiceManager : MonoBehaviour
{
    public PlayerDiceScriptableObject _player;
    private List<SkillDiceScriptableObject> _skillDiceScriptableObject;
    /*Note for list skill dice :
     * 0 : strength
     * 1 : reflex
     * 2 : artist
     * 3 : deception
     * 4 : knowledge
     * 5 : observation
     */
    public List<SkillDiceManagerPerDice> _managerPerDice;
    /*Note for list skill dice :
     * 0 : strength
     * 1 : reflex
     * 2 : artist
     * 3 : deception
     * 4 : knowledge
     * 5 : observation
     */

    private SkilllDiceUIManager _skillDiceUIManager;
    private void Start()
    {
        _skillDiceUIManager = FindFirstObjectByType<SkilllDiceUIManager>();
        _skillDiceUIManager.SetViewSkillDiceCanvas(true);
    }

}
