
using TMPro;
using UnityEngine;

public class SkillCheckUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public TextMeshProUGUI playerRollScoreText;
    [SerializeField] public TextMeshProUGUI npcRollScoreText;
    [SerializeField] public TextMeshProUGUI rerollCountText;
    [SerializeField] private TextMeshProUGUI npcActionText;
    [SerializeField] private TextMeshProUGUI playerSkillText;
    [SerializeField] public TextMeshProUGUI resultsText;
    [SerializeField] public TextMeshProUGUI resultsAcceptPreviewText;
    [SerializeField] public GameObject rollButton;
    [SerializeField] public GameObject preview;
    [SerializeField] public TextMeshProUGUI hpChangeText;
    [SerializeField] public TextMeshProUGUI sanityChangeText;
    [SerializeField] public TextMeshProUGUI expChangeText;

    public void Start()
    {
        Initialize();
    }

    void Initialize(){
        GameContext.SceneEvents.Subscribe<SkillCheckUpdateUIEvent>(UpdateUI);
        GameContext.SceneEvents.Subscribe<SkillCheckResetUIEvent>(ResetUI);
    }

    void Dispose(){
        GameContext.SceneEvents.Unsubscribe<SkillCheckUpdateUIEvent>(UpdateUI);
        GameContext.SceneEvents.Unsubscribe<SkillCheckResetUIEvent>(ResetUI);
    }

    void UpdateUI(SkillCheckUpdateUIEvent evt)
    {
        playerRollScoreText.text = evt.PlayerScore.ToString();
        npcRollScoreText.text = evt.NPCScore.ToString();
        rerollCountText.text = "Re-roll count: " + evt.RerollCount;
        if (evt.Results == true) {
            resultsText.text = "Successing";
            resultsAcceptPreviewText.text = "Successing";
        }
        else if (evt.Results == false) {
            resultsText.text = "Failing";
            resultsAcceptPreviewText.text = "Failing";
        }
        else {
            resultsText.text = "Draw";
            resultsAcceptPreviewText.text = "Draw";
        }

        // Stat Change & Accept Preview
        preview.SetActive(evt.PreviewEnabled);
        if(evt.HPChange == 0) hpChangeText.enabled = false;
        else hpChangeText.enabled = true;
        if(evt.SanityChange == 0) sanityChangeText.enabled = false;
        else sanityChangeText.enabled = true;
        hpChangeText.text = (evt.HPChange > 0 ? "+" : "-") + evt.HPChange.ToString();
        sanityChangeText.text = (evt.SanityChange > 0 ? "+" : "-") + evt.SanityChange.ToString();
        expChangeText.text = "EXP " + (evt.EXPChange >= 0 ? "+" : "-") + evt.EXPChange.ToString();

        rollButton.SetActive(evt.RollButtonEnabled);
    }

    void ResetUI(SkillCheckResetUIEvent evt)
    {
        playerRollScoreText.text = "?";
        npcRollScoreText.text = "?";
        rerollCountText.text = "";
        resultsText.text = "Undecided";
        resultsAcceptPreviewText.text = "Undecided";
        expChangeText.text = "";
        playerSkillText.text = evt.PlayerSkillText;
        npcActionText.text = evt.NPCActionText;
        rollButton.SetActive(true);
        preview.SetActive(false);
    }
}