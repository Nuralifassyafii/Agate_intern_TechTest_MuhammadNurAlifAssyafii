using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public GameState CurrentState
    {
        get;
        private set;
    }

    void Awake()
    {
        Instance = this;
        ChangeState(GameState.Exploration);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"Game State: {newState}");
    }

    public bool IsState(GameState state)
    {
        return CurrentState == state;
    }
}