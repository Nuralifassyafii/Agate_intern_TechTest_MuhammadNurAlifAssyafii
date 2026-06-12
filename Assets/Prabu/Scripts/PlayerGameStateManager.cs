using UnityEngine;

public class PlayerGameStateManager : MonoBehaviour
{
    public static PlayerGameStateManager Instance;

    public PlayerGameState CurrentState
    {
        get;
        private set;
    }

    void Awake()
    {
        Instance = this;
        ChangeState(PlayerGameState.Exploration);
    }

    public void ChangeState(PlayerGameState newState)
    {
        CurrentState = newState;
        Debug.Log($"Game State: {newState}");
    }

    public bool IsState(PlayerGameState state)
    {
        return CurrentState == state;
    }
}
