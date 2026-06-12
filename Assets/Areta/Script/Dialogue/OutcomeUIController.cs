using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OutcomeUIController : MonoBehaviour
{
    [SerializeField] private GameObject outcomeContainer;
    [SerializeField] private TMP_Text outcomeText;
    [SerializeField] private TMP_Text effectText;

    void Start()
    {
        InitializeOutcomeUIStyle();
        outcomeContainer.SetActive(false);

        if (GameContext.SceneEvents == null) return;

        GameContext.SceneEvents
            .Subscribe<OutcomeResolvedEvent>(OnOutcomeResolved);
    }

    private void InitializeOutcomeUIStyle()
    {
        if (outcomeText != null)
        {
            outcomeText.color = Color.white;
        }

        if (effectText != null)
        {
            effectText.color = new Color(0.9f, 0.7f, 0.15f, 1.0f);
        }
    }

    void OnDestroy()
    {
        if (GameContext.SceneEvents == null) return;

        GameContext.SceneEvents
            .Unsubscribe<OutcomeResolvedEvent>(OnOutcomeResolved);
    }

    void OnOutcomeResolved(OutcomeResolvedEvent signal)
    {
        StartCoroutine(ShowOutcome(signal.outcome));
    }

    IEnumerator ShowOutcome(OutcomeData outcome)
    {
        outcomeContainer.SetActive(true);

        outcomeText.text = outcome.outcomeText;
        ShowEffects(outcome.effects);

        yield return new WaitForSeconds(2f);

        outcomeContainer.SetActive(false);
    }

    void ShowEffects(List<OutcomeEffect> effects)
    {
        effectText.text = "";

        if (effects == null) return;

        foreach (OutcomeEffect effect in effects)
        {
            switch (effect.effectType)
            {
                case EffectType.ChangeExp:
                    effectText.text += $"{FormatValue(effect.intValue)} EXP\n";
                    break;

                case EffectType.ChangeHealth:
                    effectText.text += $"{FormatValue(effect.intValue)} Health\n";
                    break;

                case EffectType.ChangeSanity:
                    effectText.text += $"{FormatValue(effect.intValue)} Sanity\n";
                    break;

                case EffectType.AddItem:
                    effectText.text += $"Get Item: {effect.stringValue}\n";
                    break;

                case EffectType.RemoveItem:
                    effectText.text += $"Lose Item: {effect.stringValue}\n";
                    break;

                case EffectType.AddCondition:
                    effectText.text += $"Gain Condition: {effect.stringValue}\n";
                    break;

                case EffectType.RemoveCondition:
                    effectText.text += $"Remove Condition: {effect.stringValue}\n";
                    break;

                case EffectType.ShowQuest:
                    effectText.text += $"New Quest: {effect.stringValue}\n";
                    break;

                case EffectType.ChangeQuestStatus:
                    effectText.text += $"Quest Updated: {effect.stringValue}\n";
                    break;

                case EffectType.ChangeRelationship:
                    effectText.text +=
                        $"Relationship {effect.stringValue} {effect.intValue}\n";
                    break;
            }
        }
    }
    
    string FormatValue(int value)
    {
        return value >= 0 ? $"+{value}" : value.ToString();
    }
}