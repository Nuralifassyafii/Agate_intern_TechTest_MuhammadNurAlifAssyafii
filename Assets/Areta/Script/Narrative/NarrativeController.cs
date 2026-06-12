using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeController : MonoBehaviour
{
    [SerializeField] private PlayerDiceScriptableObject playerData;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private ConditionDiceManager conditionManager;
    [SerializeField] private NarrativeDatabase narrativeDatabase;
    [SerializeField] private NPCDatabase npcDatabase;
    private NarrativeNode currentNode;
    private NPCData activeNPC;
    private GameObject activeNPCGameObject;
    private PlayerManager playerManager;
    
    void Start()
    {
        if (narrativeDatabase != null) narrativeDatabase.Init();
        if (npcDatabase != null) npcDatabase.Init();
        if (!playerManager)
        {
            if (!GameContext.PersistentServices.TryGet(out playerManager))
            {
                Debug.LogError("PlayerManager Not Found!");
            }
        }
        if (playerManager) playerData = playerManager.GetPlayerDiceSO();
        if (GameContext.SceneEvents == null) return;
        GameContext.SceneEvents.Subscribe<NPCInteractionStartedEvent>(OnNPCClicked);
        GameContext.SceneEvents.Subscribe<ResponseSelectedEvent>(OnResponseSelected);
        GameContext.SceneEvents.Subscribe<SkillCheckCompletedEvent>(OnSkillCheckCompleted);
    }

    void OnDestroy()
    {
        if (narrativeDatabase != null) narrativeDatabase.Init();
        if (npcDatabase != null) npcDatabase.Init();
        if (GameContext.SceneEvents == null) return;
        GameContext.SceneEvents.Unsubscribe<NPCInteractionStartedEvent>(OnNPCClicked);
        GameContext.SceneEvents.Unsubscribe<ResponseSelectedEvent>(OnResponseSelected);
        GameContext.SceneEvents.Unsubscribe<SkillCheckCompletedEvent>(OnSkillCheckCompleted);
    }

    public void StartNarrative(NarrativeNode node)
    {
        GameStateManager.Instance.ChangeState(GameState.Dialogue);
        currentNode = node;
        GameContext.SceneEvents.Publish(new DialogueStartedEvent
            {
                node = currentNode
            });
        EnterNode(currentNode);
    }

    void EnterNode(NarrativeNode node)
    {      
        currentNode = node;
        if (activeNPC != null)
        {
            activeNPC.currentNode = node;
        }
        GameContext.SceneEvents.Publish(new NarrativeNodeChangedEvent
            {
                node = currentNode
            });
    }

    void OnResponseSelected(ResponseSelectedEvent signal)
    {
        ResponseData response = signal.response;
        Debug.Log(response.type);
        Debug.Log(response.text);
        switch (response.type)
        {
            case ResponseType.NarrativeOnly:
                OutcomeData neutralOutcome = response.outcomes.neutralOutcome;
                ResolveOutcome(neutralOutcome);
                break;

            case ResponseType.SkillCheck:
                if (activeNPCGameObject != null)
                {
                    var configurator = activeNPCGameObject.GetComponent<NPCDiceConfigurator>();
                    if (configurator != null)
                    {
                        configurator.SetUsedDiceForNPC();
                    }
                    else
                    {
                        var npcAbstract = activeNPCGameObject.GetComponent<NPCAbstract>();
                        if (npcAbstract != null)
                        {
                            npcAbstract.SetUsedDiceForNPC();
                        }
                    }
                }
                GameStateManager.Instance.ChangeState(GameState.SkillCheck);
                GameContext.SceneEvents.Publish(new SkillCheckRequestedEvent
                    {
                        response = response,
                        npc = activeNPC
                    });
                break;
            case ResponseType.EndDialogue:
                if (currentNode != null)
                {
                    currentNode.isCompleted = true;
                }
                Debug.Log("END DIALOGUE");
                EndNarrative();
                break;
        }
    }

    void OnSkillCheckCompleted(SkillCheckCompletedEvent signal)
    {
        GameStateManager.Instance.ChangeState(GameState.Dialogue);
        OutcomeData outcome;
        if (signal.success == true)
        {
            outcome = signal.response.outcomes.successOutcome;
        }
        else if (signal.success == false)
        {
            outcome = signal.response.outcomes.failedOutcome;
        }
        else
        {
            outcome = signal.response.outcomes.neutralOutcome;
        }

        ResolveOutcome(outcome);
    }

    void ResolveOutcome(OutcomeData outcome)
    {
        if (currentNode != null)
        {
            currentNode.isCompleted = true;
        }

        if (outcome == null)
        {
            EndNarrative();
            return;
        }

        Debug.Log(outcome.nextNode == null ? "NEXT NODE NULL" : outcome.nextNode.nodeId);

        ApplyEffects(outcome.effects);

        GameContext.SceneEvents.Publish(
            new OutcomeResolvedEvent
            {
                outcome = outcome
            });

        if (outcome.nextNode == null)
        {
            EndNarrative();
            return;
        }

        EnterNode(outcome.nextNode);
    }

    void ApplyEffects(List<OutcomeEffect> effects)
    {
        if (effects == null) return;

        foreach (OutcomeEffect effect in effects)
        {
            switch (effect.effectType)
            {

                // EXP
                case EffectType.ChangeExp:
                    playerData.exp += effect.intValue;
                    Debug.Log($"CHANGE EXP: {effect.intValue}");
                    break;

                // Health
                case EffectType.ChangeHealth:
                    playerData.health += effect.intValue;
                    Debug.Log($"CHANGE HEALTH: {effect.intValue}");
                    break;

                // Sanity
                case EffectType.ChangeSanity:
                    playerData.sanity += effect.intValue;
                    Debug.Log($"CHANGE SANITY: {effect.intValue}");
                    break;

                // Condition
                case EffectType.AddCondition:
                    conditionManager.AddConditionToPlayerByName(effect.stringValue);
                    Debug.Log($"ADD CONDITION: {effect.stringValue}");
                    break;

                case EffectType.RemoveCondition:
                    conditionManager.RemoveConditionFromPlayerByName(effect.stringValue);
                    Debug.Log($"REMOVE CONDITION: {effect.stringValue}");
                    break;

                // Item
                case EffectType.AddItem:
                    inventoryManager.AddItem(effect.stringValue);
                    Debug.Log($"ADD ITEM: {effect.stringValue}");
                    break;

                case EffectType.RemoveItem:
                    inventoryManager.RemoveItem(effect.stringValue);
                    Debug.Log($"REMOVE ITEM: {effect.stringValue}");
                    break;

                // Quest
                case EffectType.ShowQuest:
                    questManager.ChangeQuestState(effect.stringValue, QuestManager.QuestState.Ongoing);
                    Debug.Log($"SHOW QUEST: {effect.stringValue}");
                    break;

                case EffectType.ChangeQuestStatus:
                    questManager.ChangeQuestState(effect.stringValue, QuestManager.QuestState.Completed);
                    Debug.Log($"CHANGE QUEST STATUS: {effect.stringValue}");
                    break;

                // Relationship
                case EffectType.ChangeRelationship:
                    Debug.Log($"CHANGE RELATIONSHIP: " + $"{effect.stringValue} " + $"{effect.intValue}");
                    break;

            }
        }
    }

    void EndNarrative()
    {
        GameStateManager.Instance.ChangeState(GameState.Exploration);
        GameContext.SceneEvents.Publish(new DialogueEndedEvent());
    }

    void OnNPCClicked(NPCInteractionStartedEvent signal)
    {
        activeNPC = Instantiate(signal.npc);
        activeNPCGameObject = signal.npcGameObject;
        Debug.Log($"Start Dialogue With: " + $"{activeNPC.npcName}");
        NarrativeNode nodeToStart = activeNPC.currentNode != null ? activeNPC.currentNode : activeNPC.startNode;
        if (nodeToStart != null)
        {
            StartNarrative(nodeToStart);
        }
    }
}