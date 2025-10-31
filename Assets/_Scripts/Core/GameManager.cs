using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{// HANDLE GAME STATES (PAUSE, GAME OVER, ETC.) HERE
    public enum GameState
    {
        Playing,
        Paused, // or any Canvas is Displayed,
        Victory,
        Lose
    }
    // Singleton instance
    // Ensures only one instance of GameManager exists in the scene
    private static GameManager instance;

    // Public accessor for the singleton instance
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindFirstObjectByType<GameManager>();
            return instance;
        }
    }

    // GAME STATE MANAGEMENT
    public GameState State;
    public static event Action<GameState> OnGameStateChanged;

    // SAVE DATA MANAGEMENT
    public JSONSaveData currentData;

    private void Awake()
    {
        // Singleton pattern setup
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
        LoadGame(); // Load saved progress on startup
    }

    private void Start()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.OnModalToggled += HandleModalToggled;
    }

    // Update the current game state and notify listeners
    public void updateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Playing:
                // Implement resume logic here
                Time.timeScale = 1f;
                break;

            case GameState.Paused:
                // Implement pause logic here
                Time.timeScale = 0f;
                break;

            case GameState.Victory:
                // Implement victory logic here
                break;

            case GameState.Lose:
                FailLevel();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        // Implement state change logic here (e.g., pause game, show UI, etc.)
        // Notify other systems of state change if necessary
        OnGameStateChanged?.Invoke(newState);
    }

    // SAVE/LOAD SYSTEM
    public void SaveGame()
    {
        JSONSaveSystem.SaveGame(currentData);
    }

    public void LoadGame()
    {
        currentData = JSONSaveSystem.LoadGame();

        // Optionally initialize a new SaveData if none exists
        if (currentData == null)
        {
            Debug.Log("No save file found — creating new SaveData.");
            currentData = new JSONSaveData();
            SaveGame();
        }
    }

    // Example helper function to complete a level and save progress
    public void CompleteLevel(int chapterIndex, int levelIndex, float time)
    {
        var level = currentData.chapters[chapterIndex].levels[levelIndex];
        level.isCompleted = true;

        // Store best time
        if (level.bestTime == 0 || time < level.bestTime)
            level.bestTime = time;

        // Unlock next level
        if (levelIndex < 7) // if not last in chapter
            currentData.chapters[chapterIndex].levels[levelIndex + 1].isUnlocked = true;
        else if (chapterIndex < 2) // if last level in chapter
            currentData.chapters[chapterIndex + 1].levels[0].isUnlocked = true;

        SaveGame();
    }

    public void LoadLevel(int chapterIndex, int levelIndex)
    {
        // Example: your level scenes could be named "Level_1_1", "Level_1_2", etc.
        string sceneName = $"Level{chapterIndex + 1}_{levelIndex + 1}";
        Debug.Log(sceneName);
        SceneManager.LoadScene(sceneName);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSave();
        }
    }

    public void ResetSave()
    {
        currentData = new JSONSaveData();
        SaveGame();
    }

    private void HandleModalToggled(bool isOpen)
    {
        Time.timeScale = isOpen ? 0 : 1;
        // Optionally disable player input here
    }

    // Example: call these when level ends
    public void CompleteLevel()
    {
        UIManager.Instance.ShowLevelComplete();
    }

    public void FailLevel()
    {
        UIManager.Instance.ShowLevelFailed();
    }
}
