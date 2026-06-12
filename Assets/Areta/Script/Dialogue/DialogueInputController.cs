using UnityEngine;

public class DialogueInputController : MonoBehaviour
{
    [SerializeField] private DialoguePresenter presenter;

    void Update()
    {
        HandleNumericInput();
    }

    void HandleNumericInput()
    {
        if (!GameStateManager.Instance.IsState(GameState.Dialogue)) return;

        if (!presenter.HasResponses) return;

        int responseCount = presenter.ResponseCount;

        for (int responseIndex = 0; responseIndex < responseCount; responseIndex++)
        {
            KeyCode key = KeyCode.Alpha1 + responseIndex;

            if (Input.GetKeyDown(key))
            {
                ResponseData response = presenter.GetResponse(responseIndex);
                GameContext.SceneEvents.Publish(new ResponseSelectedEvent
                    {
                        response = response
                    });
                return;
            }
        }
    }
}