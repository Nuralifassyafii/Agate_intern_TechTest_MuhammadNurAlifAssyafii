using UnityEngine;

public class DialoguePresenter : MonoBehaviour
{
    private NarrativeNode currentNode;

    public bool HasResponses
    {
        get
        {
            return currentNode != null && currentNode.responses != null && currentNode.responses.Length > 0;
        }
    }

    public int ResponseCount
    {
        get
        {
            if (currentNode == null) return 0;
            return currentNode.responses.Length;
        }
    }

    void OnEnable()
    {
        if (GameContext.SceneEvents == null) return;
        GameContext.SceneEvents.Subscribe<NarrativeNodeChangedEvent>(OnNodeChanged);
        //GameContext.SceneEvents.Subscribe<DialogueEndedEvent>(OnDialogueEnded);
    }

    void OnDisable()
    {
        if (GameContext.SceneEvents == null) return;
        GameContext.SceneEvents.Unsubscribe<NarrativeNodeChangedEvent>(OnNodeChanged);
        //GameContext.SceneEvents.Unsubscribe<DialogueEndedEvent>(OnDialogueEnded);
    }

    void OnNodeChanged(NarrativeNodeChangedEvent signal)
    {
        currentNode = signal.node;
    }

    public ResponseData GetResponse(int index)
    {
        return currentNode.responses[index];
    }
}