using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SkillCheckManager : MonoBehaviour
{
    [Header("Variables")]
    public int playerRollScore;
    public int npcRollScore;
    [SerializeField] public int rerollCount;
    private bool? results;
    [SerializeField] private int opponentLockCount;
    [SerializeField] public int hpChange;
    [SerializeField] public int sanityChange;
    [SerializeField] public int expChange;
    [SerializeField] public int outcomeExpChange;

    [Header("UI")]
    [SerializeField] public GameObject skillCheckProps;
    private bool rollButtonStatus = true;
    private bool previewStatus = false;

    [Header("Dice Setup")]
    [SerializeField] private List<RollableDice> playerDices;
    [SerializeField] private List<RollableDice> npcDices;
    [SerializeField] private SkillDiceScriptableObject playerSkillDice;
    [SerializeField] private SkillDiceScriptableObject npcSkillDice;
    [SerializeField] private List<ConditionDiceScriptableObject> playerConditionDices;
    [SerializeField] private List<ConditionDiceScriptableObject> npcConditionDices;
    [SerializeField] private List<SpecialDiceScriptable> playerSpecialDices;
    
    [Header("Dice Spawn")]
    [SerializeField] private GameObject skillDicePrefab;
    [SerializeField] private GameObject conditionDicePrefab;
    [SerializeField] private Transform playerDiceSpawn;
    [SerializeField] private Transform npcDiceSpawn;

    [Header("Dice Lock & Select")]
    [SerializeField] private List<RollableDice> chosenDices;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _skillCheckCamera;
    private UniversalAdditionalCameraData cameraData;
    [SerializeField] private LayerMask diceLayer;
    [SerializeField] private float upForce = 5f;
    [SerializeField] private float forwardForce = 2f;
    [SerializeField] private float rotationForce = 3f;
    public bool isRolling;
    private bool isFirstRoll;
    
    [Header("Dependency References")]
    [SerializeField] SkillDiceManager _skillDiceManager;
    [SerializeField] PlayerDiceScriptableObject _playerScriptableObject;
    [SerializeField] NPCManager _npcManager;
    private OutcomeSet skillCheckRequestOutcomes;

    void Start(){
        Initialize();
    }
    
    // ----- DICE SETUP -----

    // Setup variables, text, buttons, Events, References
    public void Initialize()
    {
        if(_mainCamera) cameraData = _mainCamera.GetUniversalAdditionalCameraData();
        if (!GameContext.SceneServices.TryGet(out _skillDiceManager))
        {
            Debug.LogError("SkillDiceManager Not Found!");
        }
        if (!GameContext.SceneServices.TryGet(out _npcManager))
        {
            Debug.LogError("NPCManager Not Found!");
        }
        
        PlayerManager playerManager;
        if (!GameContext.PersistentServices.TryGet(out playerManager))
        {
            Debug.LogError("NPCManager Not Found!");
        }
        
        skillCheckProps.SetActive(false);
        _playerScriptableObject = playerManager.GetPlayerDiceSO();
        GameContext.SceneEvents.Subscribe<SkillCheckRequestedEvent>(InitializeSkillCheck);
        
        Debug.Log("Called Initialize");
    }

    // Dispose Event Subscription
    public void Dispose()
    {
        GameContext.SceneEvents.Unsubscribe<SkillCheckRequestedEvent>(InitializeSkillCheck);
    }

    // Update Loop
    void Update()
    {
        if(isRolling) CheckRoll();
        
        if (Input.GetMouseButtonDown(0) && !isRolling && !isFirstRoll)
        {
            Ray _ray = _skillCheckCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;

            if(Physics.Raycast(_ray, out _hit, 100f, diceLayer))
            {
                OnChoose(_hit.collider.gameObject.GetComponent<RollableDice>());
            }
        }
    }

    void InitializeSkillCheck(SkillCheckRequestedEvent evt)
    {
        Debug.Log("Called InitializeSkillCheck");

        // Reset
        skillCheckRequestOutcomes = evt.response.outcomes;

        rerollCount = 3;
        isRolling = false;
        isFirstRoll = true;
        playerRollScore = 0;
        npcRollScore = 0;
        hpChange = 0;
        sanityChange = 0;
        expChange = 0;
        
        skillCheckProps.SetActive(true);

        GameContext.SceneEvents.Publish(new SkillCheckResetUIEvent{NPCActionText = evt.response.skillType.ToString(), PlayerSkillText = evt.response.skillType.ToString()});
        
        // add skill camera to main camera's stack
        cameraData.cameraStack.Add(_skillCheckCamera);
        
        // Get Dice Data
        EnumSkillDice enumSkillDice = evt.response.skillType;

        playerSkillDice =_skillDiceManager._managerPerDice[_skillDiceManager._managerPerDice.FindIndex(item => enumSkillDice == item.GetPickedDiceValue().skillType)].GetPickedDiceValue();

        npcSkillDice = _npcManager.GetValueDiceNPC((int)enumSkillDice);
        
        // -- Instantiate Dice --
        RollableDice diceObject;

        // Player Skill Dice
        diceObject = Instantiate(skillDicePrefab, playerDiceSpawn).GetComponent<RollableSkillDice>();
        diceObject.SetData(playerSkillDice);
        playerDices.Add(diceObject);
        chosenDices.Add(diceObject);

        // NPC Skill Dice
        diceObject = Instantiate(skillDicePrefab, npcDiceSpawn).GetComponent<RollableSkillDice>();
        diceObject.SetData(npcSkillDice);
        npcDices.Add(diceObject);
        chosenDices.Add(diceObject);

        // Player Condition Dice
        playerConditionDices = _playerScriptableObject.playerCondition;

        foreach(SpecialDicePlayer specialDicePlayer in _playerScriptableObject.playerSpecialDice.FindAll(item => item.isActive == true))
        {
            playerSpecialDices.Add(specialDicePlayer.specialDicePriceVessel);
        }

        Vector3 diceSpawnIncrement;
        foreach (ConditionDiceScriptableObject conditionDice in playerConditionDices)
        {
            if(conditionDice){
                Debug.Log("Condition " + conditionDice.name);
                if (conditionDice.affectedSkill.Contains(playerSkillDice.skillType))
                {
                    diceSpawnIncrement = playerDiceSpawn.position;
                    diceSpawnIncrement.x += 1.5f * playerDices.Count;
                    diceObject = Instantiate(conditionDicePrefab, diceSpawnIncrement, playerDiceSpawn.rotation, playerDiceSpawn).GetComponent<RollableConditionDice>();
                    diceObject.SetData(conditionDice);
                    playerDices.Add(diceObject);
                    chosenDices.Add(diceObject);
                }
            }
        }

        foreach (SpecialDiceScriptable specialDice in playerSpecialDices)
        {
            if(specialDice){
                Debug.Log("Special " + specialDice.name);
                diceSpawnIncrement = playerDiceSpawn.position;
                diceSpawnIncrement.x += 1.5f * playerDices.Count;
                diceObject = Instantiate(conditionDicePrefab, diceSpawnIncrement, playerDiceSpawn.rotation, playerDiceSpawn).GetComponent<RollableConditionDice>();
                diceObject.SetData(specialDice);
                playerDices.Add(diceObject);
                chosenDices.Add(diceObject);
            }
        }
        
    }

    // ----- DICE ROLL -----
    public void Roll()
    {
        if(rerollCount > 0 && chosenDices.Count > 0 && !isRolling)
        {
            isFirstRoll = false;
            rerollCount--;
            rollButtonStatus = false;
            previewStatus = false;

            UpdateUI();
            foreach(RollableDice dice in chosenDices)
            {                
                // Call roll method in every dice
                dice._diceRigidbody.isKinematic = false;
                StartCoroutine(dice.RollDice(upForce, rotationForce));
                
                if(playerDices.Contains(dice)) dice.isLocked = false;
            }
        }
    }
    
    // ----- DICE CHOOSING -----
    void OnChoose(RollableDice dice)
    {
        DiceResult diceResult;
        if (!npcDices.Contains(dice))
        {
            dice.ToggleChoose();
            diceResult = dice.CalculateTopFace();
            if (dice.isChosen)
            {
                playerRollScore -= diceResult.Score;
                hpChange -= diceResult.Health;
                sanityChange -= diceResult.Sanity;
                expChange -= diceResult.EXP;

                Debug.Log("Select");
                chosenDices.Add(dice);
            } else
            {
                // Deselect & refund opponent lock
                playerRollScore += diceResult.Score;
                hpChange += diceResult.Health;
                sanityChange += diceResult.Sanity;
                expChange += diceResult.EXP;

                Debug.Log("Deselect");
                if(chosenDices.Contains(dice)) chosenDices.Remove(dice);
            }
        } else
        {
            if (dice.isChosen)
            {
                // Deselect & refund opponent lock
                opponentLockCount++;
                dice.ToggleChoose();

                diceResult = dice.CalculateTopFace();
                npcRollScore += diceResult.Score;
                hpChange += diceResult.Health;
                sanityChange += diceResult.Sanity;
                expChange += diceResult.EXP;

                Debug.Log("Deselect");
                if(chosenDices.Contains(dice)) chosenDices.Remove(dice);
            } else if(opponentLockCount > 0)
            {
                // Select & use opponent lock
                opponentLockCount--;
                dice.ToggleChoose();

                diceResult = dice.CalculateTopFace();
                npcRollScore -= diceResult.Score;
                hpChange -= diceResult.Health;
                sanityChange -= diceResult.Sanity;
                expChange -= diceResult.EXP;
                
                Debug.Log("Select");
                chosenDices.Add(dice);
            }
        }
        UpdateUI();
    }

    // ----- DICE CHECK -----

    // Make sure every dice already stopped
    void CheckRoll()
    {
        bool allFinishedRolled = true;
        foreach (RollableDice dice in chosenDices){
            Rigidbody diceRigidbody = dice.gameObject.GetComponent<Rigidbody>();
            if (Mathf.Abs(diceRigidbody.angularVelocity.magnitude) < 0.001f && Mathf.Abs(diceRigidbody.linearVelocity.magnitude) < 0.001f) {
                diceRigidbody.isKinematic = true;
            } else
            {
                allFinishedRolled = false;
                break;
            }
        }

        if (allFinishedRolled)
        {
            isRolling = false;
            CalculateResults();
        }
    }

    // Call top face check in every dice and aggregate the results
    void CalculateResults()
    {
        DiceResult diceResult;
        foreach(RollableDice dice in chosenDices)
        {
            diceResult = dice.CalculateTopFace();
            if(playerDices.Contains(dice))
            {
                playerRollScore += diceResult.Score;
            } else
            {
                npcRollScore += diceResult.Score;
            }

            hpChange += diceResult.Health;
            sanityChange += diceResult.Sanity;
            expChange += diceResult.EXP;
            rerollCount += diceResult.Reroll;
            opponentLockCount += diceResult.OpponentLock;
        }

        if(playerRollScore > npcRollScore) {
            results = true;
            int index = skillCheckRequestOutcomes.successOutcome.effects.FindIndex(item => item.effectType == EffectType.ChangeExp);
            if(index > -1) outcomeExpChange = skillCheckRequestOutcomes.successOutcome.effects[index].intValue;
        } else if(playerRollScore < npcRollScore) {
            results = false;
            int index = skillCheckRequestOutcomes.failedOutcome.effects.FindIndex(item => item.effectType == EffectType.ChangeExp);
            if(index > -1) outcomeExpChange = skillCheckRequestOutcomes.failedOutcome.effects[index].intValue;
        } else
        {
            results = null;
            int index = skillCheckRequestOutcomes.neutralOutcome.effects.FindIndex(item => item.effectType == EffectType.ChangeExp);
            if(index > -1) outcomeExpChange = skillCheckRequestOutcomes.neutralOutcome.effects[index].intValue;
        }

        foreach(RollableDice dice in chosenDices)
        {
            dice.isChosen = false;
        }
        chosenDices.Clear();
    
        if(rerollCount > 0) rollButtonStatus = true;
        previewStatus = true;
        UpdateUI();
    }

    // ----- ACCEPT DICE CHECK -----
    public void AcceptResults()
    {
        // Setup Response Data to pass to Dialogue and Quest
        ResponseData skillCheckResults = new ResponseData {type = ResponseType.SkillCheck};
        List<OutcomeEffect> outcomeEffects = new List<OutcomeEffect>();

        if(sanityChange != 0){
            outcomeEffects.Add(new OutcomeEffect { effectType = EffectType.ChangeSanity, intValue = sanityChange});
        }
        if(expChange != 0){
            outcomeEffects.Add(new OutcomeEffect { effectType = EffectType.ChangeExp, intValue = expChange});
        }
        if(hpChange != 0){
            outcomeEffects.Add(new OutcomeEffect { effectType = EffectType.ChangeHealth, intValue = hpChange});
        }

        OutcomeData outcomeData;
        if(results == true){
            outcomeData = Instantiate(skillCheckRequestOutcomes.successOutcome);
        }
        if(results == false){
            outcomeData = Instantiate(skillCheckRequestOutcomes.failedOutcome);
        }
        else{
            outcomeData = Instantiate(skillCheckRequestOutcomes.neutralOutcome);
        }
        outcomeData.effects.AddRange(outcomeEffects);

        skillCheckResults.outcomes = new OutcomeSet{successOutcome = outcomeData, failedOutcome = outcomeData, neutralOutcome = outcomeData};

        GameContext.SceneEvents.Publish(new SkillCheckCompletedEvent{ success = results, response = skillCheckResults});

        // Remove camera stack and dices
        cameraData.cameraStack.Remove(_skillCheckCamera);

        foreach(RollableDice dice in playerDices.Concat(npcDices))
        {
            Destroy(dice.gameObject);
        }
        playerDices.Clear();
        npcDices.Clear();
        npcConditionDices.Clear();
        playerConditionDices.Clear();
        playerSpecialDices.Clear();

        // Disables skill check's props
        skillCheckProps.SetActive(false);
    }

    void UpdateUI()
    {
        GameContext.SceneEvents.Publish(new SkillCheckUpdateUIEvent{
            PlayerScore = playerRollScore,
            NPCScore = npcRollScore,
            Results = results,
            RollButtonEnabled = rollButtonStatus,
            PreviewEnabled = previewStatus,
            RerollCount = rerollCount,
            HPChange = hpChange,
            SanityChange = sanityChange,
            EXPChange = expChange + outcomeExpChange
        });
    }
}