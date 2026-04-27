using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public JSONSaveData currentData; // now includes badges and one-time triggers
    public JSONPlayerData playerData;
    public JSONUsedMathQuestionData usedMathQuestionData;
    public PlayerDataHandler playerDataHandler;

    public BadgeManager badgeManager;
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

    [Header("Loading Settings")]
    [SerializeField] public GameObject loaderCanvas;
    [SerializeField] public Slider progressBar;

    [Header("Fade Settings")]
    [SerializeField] public UI_Fade uiFade;

    // Level Stage 
    [SerializeField] public AudioClip levelStageMusic;
    [SerializeField] public AudioClip mainMenuMusic;
    public bool IsLoading { get; private set; }
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

        if (badgeManager != null && currentData != null)
        {
            badgeManager.Initialize(currentData);
        }

        settingsGlobalData = JSONSaveSystem.LoadSettingsGlobal();

    }

    private void Start()
    {
        PlayMainMenuMusic();
        // Try to register to existing UIManager
        if (UIManager.Instance != null)
            RegisterUIManager(UIManager.Instance);
        ApplySaveSettings();
    }

    // Called by UIManager once it exists
    public void RegisterUIManager(UIManager ui)
    {
        // Prevent multiple event bindings
        ui.OnModalToggled -= HandleModalToggled;
        ui.OnModalToggled += HandleModalToggled;
        // Debug.Log("GameManager linked with UIManager modal toggle event (pause game when a modal is open)");
    }

    private void HandleModalToggled(bool isOpen)
    {
        if (isOpen)
        {
            UpdateGameState(GameState.Paused);
            // InputGate.BlockInput();
            // Debug.Log("Paused");
        }
        else
        {
            UpdateGameState(GameState.Playing);
            // InputGate.AllowInput();
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

    public void ContinueGame()
    {
        // Find most recently updated profile
        string profileId = JSONSaveSystem.GetMostRecentlyUpdatedProfileId();

        if (string.IsNullOrEmpty(profileId))
        {
            Debug.LogWarning("No save data exists to continue.");
            return;
        }

        // Select and load that profile
        selectedProfileID = profileId;
        LoadGame2();

        // Find latest unlocked level
        int latestChapter = 0;
        int latestLevel = 0;

        for (int c = 0; c < currentData.chapters.Length; c++)
        {
            for (int l = 0; l < currentData.chapters[c].levels.Length; l++)
            {
                if (currentData.chapters[c].levels[l].isUnlocked)
                {
                    latestChapter = c;
                    latestLevel = l;
                }
            }
        }

        // Load that level
        LoadLevel(latestChapter, latestLevel);
    }

    public void SelectChapter()
    {
        // Find most recently updated or used profile
        string profileId = JSONSaveSystem.GetMostRecentlyUpdatedProfileId();

        if (string.IsNullOrEmpty(profileId))
        {
            Debug.LogWarning("No save data exists to continue.");
            return;
        }

        // Select and load that profile
        selectedProfileID = profileId;
        LoadGame2();

        // this loads the latest save data, so the chapter selection screen can display the correct unlocked levels for that profile.
    }

    public void ViewBadges()
    {
        // Find most recently updated or used profile
        string profileId = JSONSaveSystem.GetMostRecentlyUpdatedProfileId();

        if (string.IsNullOrEmpty(profileId))
        {
            Debug.LogWarning("No save data exists to continue.");
            return;
        }

        // Select and load that profile
        selectedProfileID = profileId;
        LoadGame2();

        // this loads the latest save data, so the badge panel can display the correct unlocked badges for that profile.
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
        badgeManager.Initialize(currentData);
        playerData = gameData.player ?? new JSONPlayerData();
        usedMathQuestionData = gameData.questions ?? new JSONUsedMathQuestionData();

        // Debug.Log("Combined GameData loaded.");
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

        // Debug.Log("Combined GameData saved.");
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
        if (IsLoading) return;

        IsLoading = true;

        string sceneName = $"Level{chapterIndex + 1}_{levelIndex + 1}"; // Example: your level scenes could be named "Level_1_1", "Level_1_2", etc.
        // StartCoroutine(LoadLevelAsync(sceneName)); // commented out for async/await version to add loading screen
        StartCoroutine(LoadLevelAsyncWithLoader(sceneName));
    }

    // commented out for async/await version to add loading screen
    // private IEnumerator LoadLevelAsync(string sceneName)
    // {
    //     // Optional: show loading screen here
    //     AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
    //     yield return new WaitUntil(() => load.isDone);

    //     // Spawn player once scene is loaded
    //     SpawnPlayer();
    // }

    public IEnumerator LoadLevelAsyncWithLoader(string sceneName)
    {
        // Fade gameplay → black
        yield return uiFade.FadeOut();

        // Show loader (still black)
        // Activate the loader canvas and set the slider to 0% initially
        loaderCanvas.SetActive(true);
        progressBar.value = 0f;

        // Fade black → loader
        yield return uiFade.FadeIn();

        // Start loading the scene asynchronously
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        load.allowSceneActivation = false; // Prevents the scene from activating automatically

        float fakeProgress = 0f;

        while (!load.isDone)
        {
            // Update progress slider value
            float realProgress = Mathf.Clamp01(load.progress / 0.9f);

            // Smoothly move fake progress toward real progress
            fakeProgress = Mathf.MoveTowards(fakeProgress, realProgress, Time.deltaTime * 0.5f);

            progressBar.value = fakeProgress;

            // When Unity loading finished (0.9), finish smoothly to 1
            if (fakeProgress >= 0.99f && load.progress >= 0.9f)
            {
                progressBar.value = 1f;
                load.allowSceneActivation = true;
            }

            yield return null;
        }
        // Fade loader → black
        yield return uiFade.FadeOut();

        PlayLevelMusic();

        // Spawn player once the scene is fully loaded
        SpawnPlayer();
        loaderCanvas.SetActive(false); // hide the loading screen once the scene is loaded

        // Fade black → gameplay
        yield return uiFade.FadeIn();

        IsLoading = false;
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

        // LOAD PLAYER DATA
        var handler = currentPlayerInstance.GetComponent<PlayerDataHandler>();
        if (handler != null)
        {
            handler.LoadPlayerFromData(playerData);
        }

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
            // Debug.Log("Player state saved.");
        }
        else
        {
            Debug.LogWarning("PlayerDataHandler component not found on player instance.");
        }
    }

    void Update()
    {
        // ------------------ LEVEL LOADING TESTING ------------------
        //  CHAPTER 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadLevel(0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadLevel(0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadLevel(0, 2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            LoadLevel(0, 3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            LoadLevel(0, 4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            LoadLevel(0, 5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            LoadLevel(0, 6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            LoadLevel(0, 7);
        }


        // CHAPTER 2
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     LoadLevel(1, 0);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     LoadLevel(1, 1);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha3))
        // {
        //     LoadLevel(1, 2);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha4))
        // {
        //     LoadLevel(1, 3);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha5))
        // {
        //     LoadLevel(1, 4);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha6))
        // {
        //     LoadLevel(1, 5);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha7))
        // {
        //     LoadLevel(1, 6);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha8))
        // {
        //     LoadLevel(1, 7);
        // }


        // CHAPTER 3
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     LoadLevel(2, 0);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     LoadLevel(2, 1);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha3))
        // {
        //     LoadLevel(2, 2);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha4))
        // {
        //     LoadLevel(2, 3);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha5))
        // {
        //     LoadLevel(2, 4);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha6))
        // {
        //     LoadLevel(2, 5);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha7))
        // {
        //     LoadLevel(2, 6);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha8))
        // {
        //     LoadLevel(2, 7);
        // }
    }

    public void ResetSave()
    {
        currentData = new JSONSaveData();
        playerData = new JSONPlayerData();
        usedMathQuestionData = new JSONUsedMathQuestionData();
        SaveGame();
    }

    // ------------------ Math Question Helper ------------------
    public List<MathQuestionJSON> GetUnusedQuestions(MathTopic topic, QuestionDifficulty difficulty)
    {
        return MathQuestionLoaderJSON.LoadByTopic(
            topic,
            new HashSet<int>(usedMathQuestionData.UsedMathQuestionIds)
        );
    }

    public void MarkQuestionAsUsed(MathQuestionJSON question)
    {
        if (!usedMathQuestionData.UsedMathQuestionIds.Contains(question.id))
        {
            usedMathQuestionData.UsedMathQuestionIds.Add(question.id);
        }
    }

    // ----------------- Settings Related ----------------------------------
    public void ApplySaveSettings()
    {
        if (SoundMixerManager.Instance == null) return;
        if (ScreenShakeManager.Instance == null) return;

        var settings = settingsGlobalData;

        ScreenShakeManager.Instance.SetShakeMultiplier(settings.shakeMultiplier);
        SoundMixerManager.Instance.SetMasterVolume(settings.masterVolume);
        SoundMixerManager.Instance.SetMusicVolume(settings.musicVolume);
        SoundMixerManager.Instance.SetSFXVolume(settings.sfxVolume);
    }

    // Intro and Outro sequences can call these functions to ensure the game is in the correct state
    public void TriggerIntro()
    {
        // this scene is played first before the first level
        SceneManager.LoadScene("Intro");
    }

    // ----------------- True Ending Trigger -------------------------------
    public void TriggerTrueEnding()
    {
        // This function can be called when the player completes the final level (2-7 or 3-8) to trigger the true ending instead of just a victory screen
        Debug.Log("True Ending Triggered!");
        // Implement your true ending logic here, such as loading a special true ending scene or showing unique UI
        SceneManager.LoadScene("Outro");
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

    public void BlockInput()
    {
        InputGate.BlockInput();
    }

    public void AllowInput()
    {
        InputGate.AllowInput();
    }

    public void PlayMainMenuMusic()
    {
        MusicManager.Instance.PlayMusic(mainMenuMusic, true);
    }

    // For Level or Stage Music
    public void PlayLevelMusic()
    {
        MusicManager.Instance.PlayMusic(levelStageMusic, true);
    }

    public void StopLevelMusic()
    {
        MusicManager.Instance.StopMusic();
    }
}