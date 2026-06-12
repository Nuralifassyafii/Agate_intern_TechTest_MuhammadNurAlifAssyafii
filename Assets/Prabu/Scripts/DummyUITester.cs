using UnityEngine;
using UnityEngine.UI;

public class DummyUITester : MonoBehaviour
{
    [SerializeField] GameObject _uiPanel;
    [SerializeField] Button _openSkillCheckBtn;
    [SerializeField] Button _openDialogueBtn;
    [SerializeField] Button _closeBtn;
    [SerializeField] Button _openUIBtn; 


    void Start()
    {
        _uiPanel.SetActive(false);

        //_openUIBtn.onClick.AddListener(() => _uiPanel.SetActive(true)); 
        _openUIBtn.onClick.AddListener(() => OpenUI(GameState.SkillCheck));
        _openSkillCheckBtn.onClick.AddListener(() => OpenUI(GameState.SkillCheck));
        _openDialogueBtn.onClick.AddListener(() => OpenUI(GameState.Dialogue));
        _closeBtn.onClick.AddListener(CloseUI);
    }

    void OpenUI(GameState state)
    {
        GameStateManager.Instance.ChangeState(state);
        _uiPanel.SetActive(true);
    }

    void CloseUI()
    {
        GameStateManager.Instance.ChangeState(GameState.Exploration);
        _uiPanel.SetActive(false);
    }
}