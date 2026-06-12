using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    [SerializeField] GameObject dialogueContainer;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] Transform responseBox;
    [SerializeField] GameObject responseButtonPrefab;
    [SerializeField] private DialoguePresenter presenter;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private ConditionDiceManager conditionManager;

    [Header("Dice Preview UI")]
    [SerializeField] private GameObject dicePreviewPanel;
    [SerializeField] private TMP_Text playerDiceText;
    [SerializeField] private TMP_Text opponentDiceText;

    private NarrativeNode currentNode;

    private bool isHasResponse = false;
    private TMP_Text[] playerFaceTexts;
    private TMP_Text[] opponentFaceTexts;
    private TMP_Text playerDiceNameText;
    private TMP_Text opponentDiceNameText;

    private bool isLayoutInitialized = false;
    private GameObject dialogueBubblePanel;
    private GameObject currentNPCGameObject;
    private Vector3 originalCamPosition;
    private Quaternion originalCamRotation;
    private bool hasSavedCamTransform = false;
    private MonoBehaviour cinemachineBrain = null;

    void Start()
    {
        if (GameContext.SceneEvents == null) return;
        GameContext.SceneEvents.Subscribe<NarrativeNodeChangedEvent>(OnNodeChanged);
        GameContext.SceneEvents.Subscribe<DialogueEndedEvent>(OnDialogueEnded);
        GameContext.SceneEvents.Subscribe<SkillCheckRequestedEvent>(OnSkillCheckRequested);
        GameContext.SceneEvents.Subscribe<SkillCheckCompletedEvent>(OnSkillCheckCompleted);
        GameContext.SceneEvents.Subscribe<NPCInteractionStartedEvent>(OnNPCInteractionStarted);
    }

    void OnDestroy()
    {
        if (GameContext.SceneEvents == null) return;
        GameContext.SceneEvents.Unsubscribe<NarrativeNodeChangedEvent>(OnNodeChanged);
        GameContext.SceneEvents.Unsubscribe<DialogueEndedEvent>(OnDialogueEnded);
        GameContext.SceneEvents.Unsubscribe<SkillCheckRequestedEvent>(OnSkillCheckRequested);
        GameContext.SceneEvents.Unsubscribe<SkillCheckCompletedEvent>(OnSkillCheckCompleted);
        GameContext.SceneEvents.Unsubscribe<NPCInteractionStartedEvent>(OnNPCInteractionStarted);
    }

    void OnNPCInteractionStarted(NPCInteractionStartedEvent signal)
    {
        currentNPCGameObject = signal.npcGameObject;
    }

    void OnNodeChanged(NarrativeNodeChangedEvent signal)
    {
        ShowNode(signal.node);
    }

    private void MoveCameraToDialoguePerspective(GameObject npcObj)
    {
        Debug.Log($"[DialogueUIController] MoveCameraToDialoguePerspective: npcObj = {(npcObj != null ? npcObj.name : "null")}");
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null || npcObj == null)
        {
            Debug.LogWarning($"[DialogueUIController] MoveCameraToDialoguePerspective early exit! Player: {(playerObj != null ? "ok" : "null")}, NPC: {(npcObj != null ? "ok" : "null")}");
            return;
        }

        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogWarning("[DialogueUIController] MoveCameraToDialoguePerspective: Main Camera is null!");
            return;
        }

        if (!hasSavedCamTransform)
        {
            originalCamPosition = mainCam.transform.position;
            originalCamRotation = mainCam.transform.rotation;
            hasSavedCamTransform = true;
        }

        var brainComponent = mainCam.GetComponent("CinemachineBrain") as MonoBehaviour;
        if (brainComponent != null)
        {
            cinemachineBrain = brainComponent;
            cinemachineBrain.enabled = false;
        }

        Vector3 playerPos = playerObj.transform.position;
        Vector3 npcPos = npcObj.transform.position;

        Vector3 midpoint = (playerPos + npcPos) / 2f;
        midpoint.y += 1.2f;

        Vector3 playerToNpc = npcPos - playerPos;
        playerToNpc.y = 0;
        
        Vector3 sideDirection = Vector3.Cross(playerToNpc, Vector3.up).normalized;

        float distance = Mathf.Max(playerToNpc.magnitude * 1.5f, 3.5f);
        mainCam.transform.position = midpoint + sideDirection * distance + Vector3.up * 0.3f;
        mainCam.transform.LookAt(midpoint);
    }

    private void RestoreCameraPerspective()
    {
        Debug.Log($"[DialogueUIController] RestoreCameraPerspective: hasSavedCamTransform = {hasSavedCamTransform}, cinemachineBrain = {(cinemachineBrain != null ? "ok" : "null")}");
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        if (hasSavedCamTransform)
        {
            mainCam.transform.position = originalCamPosition;
            mainCam.transform.rotation = originalCamRotation;
            hasSavedCamTransform = false;
        }

        if (cinemachineBrain != null)
        {
            cinemachineBrain.enabled = true;
            cinemachineBrain = null;
        }
    }

    private void InitializeDialogueLayout()
    {
        if (dialogueContainer == null) return;

        RectTransform containerRect = dialogueContainer.GetComponent<RectTransform>();
        if (containerRect != null)
        {
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.pivot = new Vector2(0.5f, 0.5f);
            containerRect.anchoredPosition = Vector2.zero;
            containerRect.sizeDelta = Vector2.zero;
        }

        Image bgImage = dialogueContainer.GetComponent<Image>();
        if (bgImage == null) bgImage = dialogueContainer.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.5f);
        
        // dialogue text
        if (dialogueBubblePanel == null)
        {
            dialogueBubblePanel = new GameObject("SpeechBubble", typeof(RectTransform), typeof(Image));
            dialogueBubblePanel.transform.SetParent(dialogueContainer.transform, false);
            
            dialogueBubblePanel.AddComponent<Outline>();
            dialogueBubblePanel.AddComponent<VerticalLayoutGroup>();
            dialogueBubblePanel.AddComponent<LayoutElement>();

            if (dialogueText != null)
            {
                dialogueText.transform.SetParent(dialogueBubblePanel.transform, false);
                dialogueText.color = Color.white;
                dialogueText.fontSize = 22f;
                dialogueText.alignment = TextAlignmentOptions.Center;
                dialogueText.enableWordWrapping = true;
            }
        }

        RectTransform bubbleRect = dialogueBubblePanel.GetComponent<RectTransform>();
        bubbleRect.anchorMin = new Vector2(0.8f, 0.5f);
        bubbleRect.anchorMax = new Vector2(0.8f, 0.5f);
        bubbleRect.pivot = new Vector2(0.5f, 0.5f);
        bubbleRect.anchoredPosition = new Vector2(0f, 150f);
        bubbleRect.sizeDelta = new Vector2(350f, 180f);

        Image bubbleImg = dialogueBubblePanel.GetComponent<Image>();
        bubbleImg.color = new Color(0.12f, 0.12f, 0.14f, 0.95f);

        Outline outline = dialogueBubblePanel.GetComponent<Outline>();
        if (outline != null)
        {
            outline.effectColor = new Color(1f, 1f, 1f, 0.7f);
            outline.effectDistance = new Vector2(2f, 2f);
        }

        VerticalLayoutGroup bubbleVlg = dialogueBubblePanel.GetComponent<VerticalLayoutGroup>();
        if (bubbleVlg != null)
        {
            bubbleVlg.padding = new RectOffset(15, 15, 15, 15);
            bubbleVlg.childAlignment = TextAnchor.MiddleCenter;
            bubbleVlg.childControlWidth = true;
            bubbleVlg.childControlHeight = true;
            bubbleVlg.childForceExpandWidth = true;
            bubbleVlg.childForceExpandHeight = true;
        }

        LayoutElement bubbleLe = dialogueBubblePanel.GetComponent<LayoutElement>();
        if (bubbleLe != null)
        {
            bubbleLe.ignoreLayout = true;
        }

        // responseBox (Choices)
        if (responseBox != null)
        {
            responseBox.SetParent(dialogueContainer.transform, true);
            RectTransform responseBoxRect = responseBox.GetComponent<RectTransform>();
            if (responseBoxRect != null)
            {
                responseBoxRect.anchorMin = new Vector2(0.2f, 0.5f);
                responseBoxRect.anchorMax = new Vector2(0.2f, 0.5f);
                responseBoxRect.pivot = new Vector2(0.5f, 0.5f);
                responseBoxRect.anchoredPosition = Vector2.zero;
                responseBoxRect.sizeDelta = new Vector2(300f, 400f);
            }

            Image responseBoxBg = responseBox.GetComponent<Image>();
            if (responseBoxBg == null) responseBoxBg = responseBox.gameObject.AddComponent<Image>();
            responseBoxBg.color = new Color(0.12f, 0.12f, 0.14f, 0.95f);

            Outline responseBoxOutline = responseBox.GetComponent<Outline>();
            if (responseBoxOutline == null) responseBoxOutline = responseBox.gameObject.AddComponent<Outline>();
            responseBoxOutline.effectColor = new Color(1f, 1f, 1f, 0.7f);
            responseBoxOutline.effectDistance = new Vector2(2f, 2f);

            VerticalLayoutGroup vlg = responseBox.GetComponent<VerticalLayoutGroup>();
            if (vlg == null) vlg = responseBox.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(15, 15, 15, 15);
            vlg.spacing = 15f;
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;

            ContentSizeFitter csf = responseBox.GetComponent<ContentSizeFitter>();
            if (csf == null) csf = responseBox.gameObject.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            LayoutElement le = responseBox.GetComponent<LayoutElement>();
            if (le == null) le = responseBox.gameObject.AddComponent<LayoutElement>();
            le.ignoreLayout = true;
        }

        for (int i = dialogueContainer.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = dialogueContainer.transform.GetChild(i);
            if (child != responseBox && 
                child.name != "SpeechBubble")
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    void ShowNode(NarrativeNode node)
    {
        if (!isLayoutInitialized)
        {
            InitializeDialogueLayout();
            isLayoutInitialized = true;
        }

        GameObject npcObj = currentNPCGameObject;
        if (npcObj == null)
        {
            npcObj = NPCInteraction.ActiveNPCGameObject;
        }

        MoveCameraToDialoguePerspective(npcObj);

        dialogueContainer.SetActive(true);
        dialogueText.text = node.exposition;
        ClearResponses();

        int activeResponseCount = 0;
        foreach (ResponseData response in node.responses)
        {
            if (response.prerequisite != null && response.prerequisite.type != PrerequisiteType.None)
            {
                bool prereqMet = false;
                switch (response.prerequisite.type)
                {
                    case PrerequisiteType.QuestCompleted:
                        prereqMet = questManager != null && questManager.IsQuestCompleted(response.prerequisite.questId);
                        break;
                    case PrerequisiteType.ItemOwned:
                        prereqMet = inventoryManager != null && inventoryManager.HasItem(response.prerequisite.itemId);
                        break;
                    case PrerequisiteType.HasCondition:
                        prereqMet = conditionManager != null && conditionManager.CheckDuplicateForConditionDice(response.prerequisite.conditionId);
                        break;
                }
                if (!prereqMet) continue;
            }

            CreateResponse(response);
            activeResponseCount++;
        }

        isHasResponse = activeResponseCount > 0;
        if (responseBox != null)
        {
            responseBox.gameObject.SetActive(isHasResponse);
        }
    }

    private void CheckHasResponse(ResponseData[] responsesData)
    {
        isHasResponse = responsesData.Length > 0;
    }

    public void CloseDialogueUI()
    {
        HideDicePreview();
        if (!isHasResponse)
        {
            RestoreCameraPerspective();
            ClearResponses();
            dialogueContainer.SetActive(false);
            GameStateManager.Instance.ChangeState(GameState.Exploration);
            currentNPCGameObject = null;
            NPCInteraction.ActiveNPCGameObject = null;
        }
    }

    void CreateResponse(ResponseData response)
    {
        GameObject buttonObj = Instantiate(responseButtonPrefab, responseBox);
        TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
        buttonText.text = response.text;

        Image btnImg = buttonObj.GetComponent<Image>();
        if (btnImg != null)
        {
            btnImg.color = Color.white;
        }
        if (buttonText != null)
        {
            buttonText.color = Color.white;
            buttonText.fontSize = 18f;
        }
        LayoutElement layout = buttonObj.GetComponent<LayoutElement>();
        if (layout == null) layout = buttonObj.AddComponent<LayoutElement>();
        layout.minWidth = 280f;
        layout.minHeight = 60f;
        layout.preferredWidth = 280f;
        layout.preferredHeight = 60f;

        Outline btnOutline = buttonObj.GetComponent<Outline>();
        if (btnOutline == null) btnOutline = buttonObj.AddComponent<Outline>();
        btnOutline.effectColor = new Color(1f, 1f, 1f, 0.4f);
        btnOutline.effectDistance = new Vector2(1f, 1f);

        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = btnImg;

            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.2f, 0.2f, 0.22f, 1.0f);
            colors.highlightedColor = new Color(0.8f, 0.6f, 0.15f, 1.0f);
            colors.pressedColor = new Color(1.0f, 0.8f, 0.2f, 1.0f);
            colors.selectedColor = new Color(0.2f, 0.2f, 0.22f, 1.0f);
            colors.disabledColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            button.colors = colors;
        }

        button.onClick.AddListener(() =>
        {
            GameContext.SceneEvents.Publish(new ResponseSelectedEvent
                {
                    response = response
                });
        });
        ResponseHoverHandler hoverHandler = buttonObj.AddComponent<ResponseHoverHandler>();
        hoverHandler.Response = response;
        hoverHandler.OnHoverEnter = ShowDicePreview;
        hoverHandler.OnHoverExit = HideDicePreview;
    }

    void ClearResponses()
    {
        foreach (Transform child in responseBox)
        {
            Destroy(child.gameObject);
        }
        if (responseBox != null)
        {
            responseBox.gameObject.SetActive(false);
        }
    }

    public void OnDialogueEnded(DialogueEndedEvent evt)
    {
       HideDicePreview();
       RestoreCameraPerspective();
       CloseDialogueUI();
       dialogueContainer.SetActive(false);
       currentNPCGameObject = null;
       NPCInteraction.ActiveNPCGameObject = null;
    }

    public void OnSkillCheckRequested(SkillCheckRequestedEvent evt)
    {
       HideDicePreview();
       dialogueContainer.SetActive(false);
    }
    
    public void OnSkillCheckCompleted(SkillCheckCompletedEvent evt)
    {
       Debug.Log("[DialogueUIController] OnSkillCheckCompleted fired.");
       if (!GameStateManager.Instance.IsState(GameState.Exploration))
       {
           dialogueContainer.SetActive(true);
           GameObject npcObj = currentNPCGameObject != null ? currentNPCGameObject : NPCInteraction.ActiveNPCGameObject;
           if (npcObj != null)
           {
               MoveCameraToDialoguePerspective(npcObj);
           }
           else
           {
               Debug.LogWarning("[DialogueUIController] OnSkillCheckCompleted: active NPC object is null!");
           }
       }
       else
       {
           Debug.Log("[DialogueUIController] OnSkillCheckCompleted: State is Exploration, keeping UI closed.");
       }
    }

    private void InitializeFallbackPreviewUI()
    {
        if (dicePreviewPanel != null) return;
        Transform finalParent = dialogueContainer.transform.parent != null ? dialogueContainer.transform.parent : dialogueContainer.transform;
        TMP_FontAsset customFont = dialogueText != null ? dialogueText.font : null;
        Material customMaterial = dialogueText != null ? dialogueText.fontSharedMaterial : null;
        GameObject panelObj = new GameObject("DicePreviewPanel", typeof(RectTransform));
        panelObj.transform.SetParent(finalParent, false);
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0f, -60f);
        panelRect.sizeDelta = new Vector2(850f, 130f);

        Image bgImage = panelObj.AddComponent<Image>();
        bgImage.color = new Color(0.12f, 0.12f, 0.14f, 0.95f);
        Outline outline = panelObj.AddComponent<Outline>();
        outline.effectColor = new Color(1f, 1f, 1f, 0.7f);
        outline.effectDistance = new Vector2(2f, 2f);

        // Opponent Section (Kiri)
        var opponentSect = CreateDiceRow(panelObj.transform, "Opponent", customFont, customMaterial);
        opponentFaceTexts = opponentSect.faceTexts;
        opponentDiceNameText = opponentSect.label;
        
        RectTransform oppRect = opponentSect.container.GetComponent<RectTransform>();
        oppRect.anchorMin = new Vector2(0f, 0.5f);
        oppRect.anchorMax = new Vector2(0f, 0.5f);
        oppRect.pivot = new Vector2(0f, 0.5f);
        oppRect.anchoredPosition = new Vector2(30f, 0f);

        // Teks VS (Tengah)
        GameObject vsObj = new GameObject("VSText", typeof(RectTransform));
        vsObj.transform.SetParent(panelObj.transform, false);
        TMP_Text vsText = vsObj.AddComponent<TextMeshProUGUI>();
        if (customFont != null)
        {
            vsText.font = customFont;
            vsText.fontSharedMaterial = customMaterial;
        }
        vsText.fontSize = 72f;
        vsText.fontStyle = FontStyles.Bold;
        vsText.color = new Color(0.9f, 0.7f, 0.15f, 1.0f);
        vsText.text = "VS";
        vsText.alignment = TextAlignmentOptions.Center;
        vsText.enableWordWrapping = false;

        RectTransform vsRect = vsObj.GetComponent<RectTransform>();
        vsRect.anchorMin = new Vector2(0.5f, 0.5f);
        vsRect.anchorMax = new Vector2(0.5f, 0.5f);
        vsRect.pivot = new Vector2(0.5f, 0.5f);
        vsRect.anchoredPosition = Vector2.zero;
        vsRect.sizeDelta = new Vector2(200f, 100f);

        // Player Section (Kanan)
        var playerSect = CreateDiceRow(panelObj.transform, "Player", customFont, customMaterial);
        playerFaceTexts = playerSect.faceTexts;
        playerDiceNameText = playerSect.label;
        
        RectTransform plyRect = playerSect.container.GetComponent<RectTransform>();
        plyRect.anchorMin = new Vector2(1f, 0.5f);
        plyRect.anchorMax = new Vector2(1f, 0.5f);
        plyRect.pivot = new Vector2(1f, 0.5f);
        plyRect.anchoredPosition = new Vector2(-30f, 0f);

        dicePreviewPanel = panelObj;
        dicePreviewPanel.SetActive(false);
    }

    private (GameObject container, TMP_Text[] faceTexts, TMP_Text label) CreateDiceRow(Transform parent, string name, TMP_FontAsset customFont, Material customMaterial)
    {
        GameObject containerObj = new GameObject(name + "Section", typeof(RectTransform));
        containerObj.transform.SetParent(parent, false);
        
        VerticalLayoutGroup vertLayout = containerObj.AddComponent<VerticalLayoutGroup>();
        vertLayout.spacing = 4f;
        vertLayout.childAlignment = TextAnchor.MiddleCenter;
        vertLayout.childControlHeight = true;
        vertLayout.childControlWidth = true;
        vertLayout.childForceExpandHeight = false;
        vertLayout.childForceExpandWidth = true;

        RectTransform sectRect = containerObj.GetComponent<RectTransform>();
        sectRect.sizeDelta = new Vector2(320f, 110f);

        // Row dadu horizontal
        GameObject facesObj = new GameObject("FacesRow", typeof(RectTransform));
        facesObj.transform.SetParent(containerObj.transform, false);
        
        HorizontalLayoutGroup horizLayout = facesObj.AddComponent<HorizontalLayoutGroup>();
        horizLayout.spacing = 8f;
        horizLayout.childAlignment = TextAnchor.MiddleCenter;
        horizLayout.childControlHeight = true;
        horizLayout.childControlWidth = true;
        horizLayout.childForceExpandHeight = false;
        horizLayout.childForceExpandWidth = false;

        LayoutElement facesLayout = facesObj.AddComponent<LayoutElement>();
        facesLayout.preferredHeight = 45f;
        facesLayout.preferredWidth = 320f;

        TMP_Text[] faceTexts = new TMP_Text[6];

        for (int i = 0; i < 6; i++)
        {
            GameObject faceBox = new GameObject($"Face_{i}", typeof(RectTransform));
            faceBox.transform.SetParent(facesObj.transform, false);
            
            LayoutElement boxLayout = faceBox.AddComponent<LayoutElement>();
            boxLayout.minWidth = 42f;
            boxLayout.minHeight = 42f;
            boxLayout.preferredWidth = 42f;
            boxLayout.preferredHeight = 42f;

            Image boxImg = faceBox.AddComponent<Image>();
            boxImg.color = new Color(0.2f, 0.2f, 0.22f, 1.0f);

            Outline boxOutline = faceBox.AddComponent<Outline>();
            boxOutline.effectColor = new Color(1f, 1f, 1f, 0.5f);
            boxOutline.effectDistance = new Vector2(1f, 1f);

            GameObject textObj = new GameObject("Text", typeof(RectTransform));
            textObj.transform.SetParent(faceBox.transform, false);
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            TMP_Text txt = textObj.AddComponent<TextMeshProUGUI>();
            if (customFont != null)
            {
                txt.font = customFont;
                txt.fontSharedMaterial = customMaterial;
            }
            txt.fontSize = 24f;
            txt.color = Color.white;
            txt.fontStyle = FontStyles.Bold;
            txt.alignment = TextAlignmentOptions.Center;
            txt.text = "x";

            faceTexts[i] = txt;
        }

        // Label nama skill
        GameObject labelObj = new GameObject("LabelText", typeof(RectTransform));
        labelObj.transform.SetParent(containerObj.transform, false);
        
        LayoutElement labelLayout = labelObj.AddComponent<LayoutElement>();
        labelLayout.preferredHeight = 30f;
        labelLayout.preferredWidth = 320f;

        TMP_Text labelTxt = labelObj.AddComponent<TextMeshProUGUI>();
        if (customFont != null)
        {
            labelTxt.font = customFont;
            labelTxt.fontSharedMaterial = customMaterial;
        }
        labelTxt.fontSize = 20f;
        labelTxt.color = Color.white;
        labelTxt.fontStyle = FontStyles.Normal;
        labelTxt.alignment = TextAlignmentOptions.Center;
        labelTxt.text = "Nama Skill";

        return (containerObj, faceTexts, labelTxt);
    }

    private void ShowDicePreview(ResponseData response)
    {
        Debug.Log($"[DialogueUIController] ShowDicePreview called for: {response.text}, Type: {response.type}, SkillType: {response.skillType}");
        if (response.type != ResponseType.SkillCheck)
        {
            return;
        }

        InitializeFallbackPreviewUI();

        if (dicePreviewPanel == null) return;

        SkillDiceManager skillDiceManager;
        NPCManager npcManager;

        if (GameContext.SceneServices.TryGet(out skillDiceManager) && GameContext.SceneServices.TryGet(out npcManager))
        {
            // Ambil dadu Player
            int index = skillDiceManager._managerPerDice.FindIndex(item => response.skillType == item.GetPickedDiceValue().skillType);
            SkillDiceScriptableObject playerSkillDice = null;
            if (index >= 0)
            {
                playerSkillDice = skillDiceManager._managerPerDice[index].GetPickedDiceValue();
            }

            // Ambil dadu NPC
            SkillDiceScriptableObject npcSkillDice = npcManager.GetValueDiceNPC((int)response.skillType);

            // Tampilkan dadu Player
            if (playerSkillDice != null && playerSkillDice.diceValue != null && playerFaceTexts != null)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (i < playerSkillDice.diceValue.Count && i < playerFaceTexts.Length)
                    {
                        playerFaceTexts[i].text = playerSkillDice.diceValue[i].ToString();
                    }
                    else
                    {
                        playerFaceTexts[i].text = "x";
                    }
                }
                if (playerDiceNameText != null)
                {
                    playerDiceNameText.text = "Skill Player: " + response.skillType.ToString().ToUpper();
                }
            }

            // Tampilkan dadu NPC
            if (npcSkillDice != null && npcSkillDice.diceValue != null && opponentFaceTexts != null)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (i < npcSkillDice.diceValue.Count && i < opponentFaceTexts.Length)
                    {
                        opponentFaceTexts[i].text = npcSkillDice.diceValue[i].ToString();
                    }
                    else
                    {
                        opponentFaceTexts[i].text = "x";
                    }
                }
                if (opponentDiceNameText != null)
                {
                    opponentDiceNameText.text = "Aksi Opponent: " + response.skillType.ToString().ToUpper();
                }
            }

            dicePreviewPanel.SetActive(true);
        }
    }

    private void HideDicePreview()
    {
        if (dicePreviewPanel != null)
        {
            dicePreviewPanel.SetActive(false);
        }
    }
}