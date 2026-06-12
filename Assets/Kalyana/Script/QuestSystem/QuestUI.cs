using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [Header("Data")]
    public QuestManager questManager;
    public QuestDatabase questDatabase;

    [Header("Icons")]
    public GameObject openButton;
    public GameObject closeButton;

    [Header("Quest Type")]
    public GameObject mainQuest;
    public GameObject sideQuest;

    [Header("Panels")]
    public GameObject mainQuestPanel;
    public GameObject sideQuestPanel;
    public GameObject questDetailPanel;

    [Header("Main Quest List")]
    public Transform mainQuestContainer;
    public GameObject mainQuestPrefab;

    [Header("Side Quest List")]
    public Transform sideQuestContainer;
    public GameObject sideQuestPrefab;

    [Header("quest Detail")]
    public TMP_Text questNameText;
    public TMP_Text questDescText;
    public Transform questHintContainer;
    public TMP_Text questHintPrefab;

    void Start()
    {
        mainQuestPanel.SetActive(false);
        sideQuestPanel.SetActive(false);
        questDetailPanel.SetActive(false);

        openButton.SetActive(true);
        closeButton.SetActive(false);

        mainQuest.SetActive(false);
        sideQuest.SetActive(false);
    }
    public void OpenQuestPanel()
    {
        mainQuestPanel.SetActive(true);
        sideQuestPanel.SetActive(false );
        questDetailPanel.SetActive(false);

        openButton.SetActive(false);
        closeButton.SetActive(true);

        mainQuest.SetActive(true);
        sideQuest.SetActive(true);

        RefreshQuestList();
    }

    public void OpenSideQuestPanel()
    {
        mainQuestPanel.SetActive(false);
        sideQuestPanel.SetActive(true );
        questDetailPanel.SetActive(false);

        openButton.SetActive(false);
        closeButton.SetActive(true);

        mainQuest.SetActive(true);
        sideQuest.SetActive(true);

        RefreshQuestList();
    }

    public void CloseQuestPanel()
    {
        mainQuestPanel.SetActive(false);
        sideQuestPanel.SetActive(false);
        questDetailPanel.SetActive(false);

        openButton.SetActive(true);
        closeButton.SetActive(false);

        mainQuest.SetActive(false);
        sideQuest.SetActive(false);
    }

    void RefreshQuestList()
    {
        foreach (Transform child in mainQuestContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in sideQuestContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (QuestManager.QuestProgress progress in questManager.questProgressList)
        {
            if (progress.state == QuestManager.QuestState.Backlog)
                continue;

            QuestData quest = progress.questData;

            GameObject prefab = null;
            Transform container = null;

            if (quest.questType == QuestData.QuestType.Main)
            {
                prefab = mainQuestPrefab;
                container = mainQuestContainer;
            }
            else if (quest.questType == QuestData.QuestType.Side)
            {
                prefab = sideQuestPrefab;
                container = sideQuestContainer;
            }

            if (prefab == null || container == null)
                continue;

            GameObject obj = Instantiate(prefab, container);

            TMP_Text text = obj.GetComponentInChildren<TMP_Text>(true);

            if (progress.state == QuestManager.QuestState.Completed)
            {
                text.text = "<s>" + quest.questName + "</s>";
                text.color = Color.gray;
            }
            else
            {
                text.text = quest.questName;
                text.color = Color.white;
            }

            Button btn = obj.GetComponent<Button>();

            btn.onClick.AddListener(() =>
            {
                ShowQuestDetail(quest);
            });
        }
    }

    void ShowQuestDetail(QuestData quest)
    {
        questDetailPanel.SetActive(true);

        questNameText.text = quest.questName;
        questDescText.text = quest.questDescription;

        foreach (Transform child in questHintContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (QuestData.QuestHint hint in quest.hints)
        {
            TMP_Text hintText = Instantiate(questHintPrefab, questHintContainer);

            hintText.text = "- " + hint.hintText;
        }
    }
}
