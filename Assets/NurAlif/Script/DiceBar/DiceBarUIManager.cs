using UnityEngine;
using UnityEngine.UI;

public class DiceBarUIManager : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button skillMenuButton;
    [SerializeField] private Button conditionMenuButton;
    [SerializeField] private Button specialMenuButton;
    [SerializeField] private Button OpenMenuUIButton;
    [SerializeField] private Canvas thisCanvas;
    [SerializeField] private PlayerDiceUIManager _playerDiceUIManager;

    private void Start()
    {
        _playerDiceUIManager = Object.FindFirstObjectByType<PlayerDiceUIManager>();

        backButton = GameObject.Find("BackButton").GetComponent<Button>();
        skillMenuButton = GameObject.Find("SkillBar").GetComponent<Button>();
        conditionMenuButton = GameObject.Find("ConditionBar").GetComponent<Button>();
        specialMenuButton = GameObject.Find("SpecialBar").GetComponent<Button>();
        OpenMenuUIButton = GameObject.Find("DiceProgressionButton").GetComponent<Button>();

        backButton.onClick.AddListener(_playerDiceUIManager.CloseMenuDice);
        skillMenuButton.onClick.AddListener(() => _playerDiceUIManager.ShowMenuDice("CanvasSkillDice"));
        conditionMenuButton.onClick.AddListener(() => _playerDiceUIManager.ShowMenuDice("CanvasConditionDice"));
        specialMenuButton.onClick.AddListener(() => _playerDiceUIManager.ShowMenuDice("CanvasSpecialDice"));
        OpenMenuUIButton.onClick.AddListener(() => _playerDiceUIManager.ShowMenuDice("CanvasSkillDice"));
    }
}
