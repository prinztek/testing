using System;
using System.Collections;
using System.IO;
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
    public static event Action<GameObject> OnPlayerSpawned;


    // SAVE DATA MANAGEMENT
    public JSONSaveData currentData;
    public JSONPlayerData playerData;
    public PlayerDataHandler playerDataHandler;

    [Header("Player Management")]
    public GameObject playerPrefab;
    private GameObject currentPlayerInstance;
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
                // Implement lose logic here
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
        JSONSaveSystem.SavePlayer(playerData);
    }

    public void LoadGame()
    {
        currentData = JSONSaveSystem.LoadGame();
        playerData = JSONSaveSystem.LoadPlayer();

        if (currentData == null)
        {
            currentData = new JSONSaveData();
        }

        if (playerData == null)
        {
            playerData = new JSONPlayerData();
        }

        SaveGame();
    }


    // Example helper function to complete a level and save progress
    public void CompleteLevel(int chapterIndex, int levelIndex, float time)
    {
        var level = currentData.chapters[chapterIndex].levels[levelIndex];
        level.isCompleted = true;

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
        StartCoroutine(LoadLevelAsync(sceneName));
    }

    private IEnumerator LoadLevelAsync(string sceneName)
    {
        // Optional: show loading screen here
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => load.isDone);

        // Spawn player once scene is loaded
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // Find spawn point (place an empty GameObject tagged "PlayerSpawn" in each level)
        var spawn = GameObject.FindWithTag("PlayerSpawn");
        Vector3 spawnPos = spawn != null ? spawn.transform.position : Vector3.zero;

        // Instantiate player prefab
        currentPlayerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        OnPlayerSpawned?.Invoke(currentPlayerInstance);

        // PlayerDataHandler.Start() automatically loads data from GameManager.Instance.playerData
        Debug.Log("✅ Player spawned and loaded.");
    }

    public void SaveCurrentPlayerState()
    {
        if (currentPlayerInstance == null) return;

        var handler = currentPlayerInstance.GetComponent<PlayerDataHandler>();
        if (handler != null)
        {
            handler.SavePlayerToData(playerData);
            SaveGame(); // write JSON to file
            Debug.Log("💾 Player state saved.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSave();
        }

        if (Input.GetKeyDown(KeyCode.K))
            SaveCurrentPlayerState();

        if (Input.GetKeyDown(KeyCode.P))
            Debug.Log(JsonUtility.ToJson(playerData, true));

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
        Debug.Log($"GameManager received modal toggle: isOpen = {isOpen}");
    }


}
