using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private NPCData npcData;
    public NPCData NpcData => npcData;
    public static GameObject ActiveNPCGameObject;

    void OnMouseDown()
    {
        Debug.Log(GameStateManager.Instance.CurrentState);
        
        if (!GameStateManager.Instance.IsState(GameState.Exploration))
        {
            return;
        }

        Debug.Log($"Clicked NPC: {npcData.npcName}");
        ActiveNPCGameObject = this.gameObject;
        GameContext.SceneEvents.Publish(new NPCInteractionStartedEvent
            {
                npc = npcData,
                npcGameObject = this.gameObject
            });
    }
}