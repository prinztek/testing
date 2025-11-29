using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 Instance;

    // ---------------- GAME STATE ----------------
    public enum GameState
    {
        Playing,
        Paused,
        Victory,
        Lose
    }

    public GameState State;
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<GameObject> OnPlayerSpawned;

    // ---------------- SAVE SLOT ----------------
    public int currentSaveSlot = 1;

    // ---------------- PLAYER ----------------
    [Header("Player Management")]
    public GameObject playerPrefab;
    private GameObject currentPlayerInstance;
    public GameObject CurrentPlayer => currentPlayerInstance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Application.targetFrameRate = 60;

        // Safety check: SaveLoadManager must exist
        if (SaveLoadManager.Instance == null)
        {
            Debug.LogError("❌ SaveLoadManager is missing in the scene!");
        }
    }

    private void Start()
    {
        var slm = SaveLoadManager.Instance;
        if (slm == null) return;

        // Load global.json first (settings + lastUsedSlot)
        slm.LoadGlobal();

        // Set current save slot to last used
        currentSaveSlot = slm.globalData.lastUsedSlot;

        // Load the save slot
        slm.LoadSlot(currentSaveSlot);

        // Safety: ensure no null data
        if (slm.saveData == null) slm.saveData = new JSONSaveData2();
        if (slm.playerData == null) slm.playerData = new JSONPlayerData2();
        if (slm.questionData == null) slm.questionData = new JSONUsedMathQuestionData2();

        // Link UI modal logic if UIManager exists
        if (UIManager.Instance != null)
            RegisterUIManager(UIManager.Instance);

        Debug.Log($"✅ Loaded Save Slot {currentSaveSlot}");
    }

    // ---------------- UI MODAL ----------------
    public void RegisterUIManager(UIManager ui)
    {
        ui.OnModalToggled -= HandleModalToggled;
        ui.OnModalToggled += HandleModalToggled;
    }

    private void HandleModalToggled(bool isOpen)
    {
        if (isOpen)
            UpdateGameState(GameState.Paused);
        else
            UpdateGameState(GameState.Playing);
    }

    // ---------------- GAME STATE ----------------
    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Playing: Time.timeScale = 1f; break;
            case GameState.Paused: Time.timeScale = 0f; break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    // ---------------- SAVE / LOAD ----------------
    public void SelectSaveSlot(int slot)
    {
        currentSaveSlot = slot;

        var slm = SaveLoadManager.Instance;
        if (slm == null) return;

        // Update global last-used slot
        slm.globalData.lastUsedSlot = slot;
        slm.SaveGlobal();

        // Load selected slot
        slm.LoadSlot(slot);

        // Safety: ensure no null data
        if (slm.saveData == null) slm.saveData = new JSONSaveData2();
        if (slm.playerData == null) slm.playerData = new JSONPlayerData2();
        if (slm.questionData == null) slm.questionData = new JSONUsedMathQuestionData2();

        Debug.Log($"✅ Selected Save Slot {slot}");
    }

    public void SaveGame()
    {
        var slm = SaveLoadManager.Instance;
        if (slm == null) return;

        slm.SaveSlot(currentSaveSlot);
        Debug.Log($"💾 Saved Slot {currentSaveSlot}");
    }

    public void ClearSaveSlot(int slot)
    {
        var slm = SaveLoadManager.Instance;
        if (slm == null) return;

        slm.saveData = new JSONSaveData2();
        slm.playerData = new JSONPlayerData2();
        slm.questionData = new JSONUsedMathQuestionData2();

        slm.SaveSlot(slot);

        Debug.Log($"🗑 Cleared Save Slot {slot}");
    }

    // ---------------- LEVEL PROGRESSION ----------------
    public void CompleteLevel(int chapter, int level)
    {
        var data = SaveLoadManager.Instance.saveData;

        data.chapters[chapter].levels[level].isCompleted = true;

        // Unlock next level
        if (level < data.chapters[chapter].levels.Length - 1)
            data.chapters[chapter].levels[level + 1].isUnlocked = true;
        else if (chapter < data.chapters.Length - 1)
            data.chapters[chapter + 1].levels[0].isUnlocked = true;

        SaveGame();
        Debug.Log($"✅ Completed Chapter {chapter + 1}, Level {level + 1}");
    }

    public void LoadLevel(int chapterIndex, int levelIndex)
    {
        string sceneName = $"Level{chapterIndex + 1}_{levelIndex + 1}";
        StartCoroutine(LoadLevelAsync(sceneName));
    }

    private IEnumerator LoadLevelAsync(string sceneName)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => load.isDone);

        SpawnPlayer();
    }

    // ---------------- PLAYER SPAWN ----------------
    private void SpawnPlayer()
    {
        if (playerPrefab == null) return;

        var spawn = GameObject.FindWithTag("PlayerSpawn");
        Vector3 spawnPos = spawn != null ? spawn.transform.position : Vector3.zero;

        currentPlayerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        currentPlayerInstance.name = "Player";

        OnPlayerSpawned?.Invoke(currentPlayerInstance);

        Debug.Log("✅ Player spawned");
    }

    // ---------------- MATH QUESTION HELPERS ----------------
    public System.Collections.Generic.List<MathQuestion> GetUnusedQuestions(MathTopic topic, QuestionDifficulty difficulty)
    {
        var ids = new System.Collections.Generic.HashSet<int>(
            SaveLoadManager.Instance.questionData.UsedMathQuestionIds
        );

        return MathQuestionLoaderJSON.LoadByTopic(topic, ids);
    }

    public void MarkQuestionAsUsed(MathQuestion question)
    {
        var qData = SaveLoadManager.Instance.questionData;

        if (!qData.UsedMathQuestionIds.Contains(question.id))
        {
            qData.UsedMathQuestionIds.Add(question.id);
            SaveGame();
            Debug.Log($"✅ Marked question {question.id} as used");
        }
    }
}
