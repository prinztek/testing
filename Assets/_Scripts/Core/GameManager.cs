using System;
using System.Collections;
using System.Collections.Generic;
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

    // SEPARATE SAVE DATA MANAGEMENT
    public JSONSaveData currentData;
    public JSONPlayerData playerData;
    public JSONUsedMathQuestionData usedMathQuestionData;
    public PlayerDataHandler playerDataHandler;

    // CLASS WRAPPER GAME DATA (HOLDS SAVE DATA, PLAYER DATA, QUESTION DATA)
    public GameData gameData;
    public JSONSettingsGlobalData settingsGlobalData; // saved separately non specific to save slot
    public ItemDatabase itemDatabase;

    [Header("Player Management")]
    public GameObject playerPrefab;
    private GameObject currentPlayerInstance;
    public GameObject CurrentPlayer => currentPlayerInstance;

    // TESTING FOR MULTIPLE SAVE FILES
    private string selectedProfileID = "";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;

        JSONSaveSystem.LoadSettingsGlobal();
        JSONSaveSystem.GetMostRecentlyUpdatedProfileId();
    }

    private void Start()
    {
        // Try to register to existing UIManager
        if (UIManager.Instance != null)
            RegisterUIManager(UIManager.Instance);

        ApplySavedAudioSettings();
    }

    // Called by UIManager once it exists
    public void RegisterUIManager(UIManager ui)
    {
        // Prevent multiple event bindings
        ui.OnModalToggled -= HandleModalToggled;
        ui.OnModalToggled += HandleModalToggled;
        // Debug.Log("✅ GameManager linked with UIManager modal toggle event");
    }

    private void HandleModalToggled(bool isOpen)
    {
        if (isOpen)
        {
            UpdateGameState(GameState.Paused);
            Debug.Log("Paused");
        }
        else
        {
            UpdateGameState(GameState.Playing);
            Debug.Log("Resumed");
        }

    }

    // Update the current game state and notify listeners
    public void UpdateGameState(GameState newState)
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
        // JSONSaveSystem.SaveGame(currentData);
        // SaveCurrentPlayerState(); // Save player-specific data
        // JSONSaveSystem.SaveUsedMathQuestions(usedMathQuestionData); // Save used math question data
        // -------------------------------- FOR TESTING --------------------------------
        SaveCurrentPlayerState(); // Save player-specific data
        SaveGame2();
    }

    public void SaveSettingsGlobal()
    {
        JSONSaveSystem.SaveSettingsGlobal(settingsGlobalData);
    }

    public void LoadGame()
    {
        // currentData = JSONSaveSystem.LoadGame();
        // playerData = JSONSaveSystem.LoadPlayer();
        // usedMathQuestionData = JSONSaveSystem.LoadUsedMathQuestions();
        // settingsGlobalData = JSONSaveSystem.LoadSettingsGlobal();

        // if (currentData == null)
        // {
        //     currentData = new JSONSaveData();
        // }

        // if (playerData == null)
        // {
        //     playerData = new JSONPlayerData();
        // }

        // if (usedMathQuestionData == null)
        // {
        //     usedMathQuestionData = new JSONUsedMathQuestionData();
        // }


        // SaveGame();

        // -------------------------------- FOR TESTING --------------------------------
        LoadGame2();
        SaveGame2();
    }

    public void NewGame(string name)
    {
        gameData = new GameData(name);
    }

    public void LoadGame2()
    {
        gameData = JSONSaveSystem.LoadSlot(selectedProfileID);

        if (gameData == null)
        {
            gameData = new GameData(selectedProfileID);
        }

        // push combined data back into your old separate system
        currentData = gameData.save ?? new JSONSaveData();
        playerData = gameData.player ?? new JSONPlayerData();
        usedMathQuestionData = gameData.questions ?? new JSONUsedMathQuestionData();

        Debug.Log("Combined GameData loaded.");
    }

    public void SaveGame2()
    {
        // fill gamedata from your existing separate data
        gameData.save = currentData;
        gameData.player = playerData;
        gameData.questions = usedMathQuestionData;
        // timestamp the data so we know when it was last saved
        gameData.lastUpdated = DateTime.Now.ToBinary();

        JSONSaveSystem.SaveSlot(gameData, selectedProfileID);

        Debug.Log("Combined GameData saved.");
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
        string sceneName = $"Level{chapterIndex + 1}_{levelIndex + 1}"; // Example: your level scenes could be named "Level_1_1", "Level_1_2", etc.
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
        if (playerPrefab == null)
        {
            Debug.LogWarning("No playerPrefab assigned!");
            return;
        }
        var spawn = GameObject.FindWithTag("PlayerSpawn");
        Vector3 spawnPos = spawn != null ? spawn.transform.position : Vector3.zero;

        // Instantiate player prefab
        currentPlayerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        currentPlayerInstance.name = "Player";
        OnPlayerSpawned?.Invoke(currentPlayerInstance);
    }

    public void SaveCurrentPlayerState()
    {
        if (currentPlayerInstance == null) return;
        var handler = currentPlayerInstance.GetComponent<PlayerDataHandler>();
        if (handler != null)
        {
            handler.SavePlayerToData(playerData);
            // JSONSaveSystem.SavePlayer(playerData);// write JSON to file
            // Debug.Log("💾 Player state saved.");
        }
        else
        {
            Debug.LogWarning("⚠️ PlayerDataHandler component not found on player instance.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSave(); // Reset all saved data
        }
    }

    public void ResetSave()
    {
        currentData = new JSONSaveData();
        playerData = new JSONPlayerData();
        usedMathQuestionData = new JSONUsedMathQuestionData();
        SaveGame();
    }

    // ------------------ Math Question Helper ------------------
    public List<MathQuestion> GetUnusedQuestions(MathTopic topic, QuestionDifficulty difficulty)
    {
        return MathQuestionLoaderJSON.LoadByTopic(
            topic,
            new HashSet<int>(usedMathQuestionData.UsedMathQuestionIds)
        );
    }

    public void MarkQuestionAsUsed(MathQuestion question)
    {
        if (!usedMathQuestionData.UsedMathQuestionIds.Contains(question.id))
        {
            usedMathQuestionData.UsedMathQuestionIds.Add(question.id);
        }
    }

    // ----------------- Audio Related ----------------------------------
    public void ApplySavedAudioSettings()
    {
        if (SoundMixerManager.Instance == null) return;

        var settings = settingsGlobalData;

        SoundMixerManager.Instance.SetMasterVolume(settings.masterVolume);
        SoundMixerManager.Instance.SetMusicVolume(settings.musicVolume);
        SoundMixerManager.Instance.SetSFXVolume(settings.sfxVolume);
    }


    // ---------------------------------- For Multiple Save Slots ----------------------------------
    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return JSONSaveSystem.LoadAllProfiles();
    }

    public void ChangeSelectedProfileId(string profileId)
    {
        // update profile to use for saving and loading
        selectedProfileID = profileId;
        // Load Game - will use the profile, updating our game data accordingly
    }
}



// Based on Shaped by Rain Studios - How to Implement Save Slots to Manage Multiple Saved Games in Unity | Tutorial
// Since this GameManager handles saving - This acts as the DataPersistenceManager in the tutorial?
// holds selectedProfileID