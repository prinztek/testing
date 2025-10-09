using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    // Ensures only one instance of GameManager exists in the scene
    private static GameManager instance;
    public GameState State;
    public static event Action<GameState> OnGameStateChanged;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindFirstObjectByType<GameManager>();
            return instance;
        }
    }

    public void updateGameState(GameState newState)
    {
        State = newState;
        switch (newState)
        {
            case GameState.Playing:
                // Implement pause logic here
                break;
            case GameState.Paused:
                // Implement pause logic here
                break;
            case GameState.Victory:
                // Implement victory logic here
                break;
            case GameState.Lose:
                // Implement game over logic here
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        // Implement state change logic here (e.g., pause game, show UI, etc.)
        // notify other systems of state change if necessary
        OnGameStateChanged?.Invoke(newState);
    }

    // HANDLE GAME STATES (PAUSE, GAME OVER, ETC.) HERE

    public enum GameState
    {
        Playing,
        Paused,
        Victory,
        Lose
    }
}
