using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private MathQuestionManager mathQuestionManager;
    [SerializeField] private CharacterStats playerStats;
    [SerializeField] private GameObject exitPoint; // Exit object

    private int totalEnemies;
    private int defeatedEnemies;

    private void Awake()
    {
        // Scene-local singleton (resets each level load)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        playerStats = playerObj.GetComponent<CharacterStats>();
    }

    private void Start()
    {
        // --- Count total enemies in the scene ---
        totalEnemies = FindObjectsByType<EnemyStatsNew>(FindObjectsSortMode.None).Length;
        defeatedEnemies = 0;
        Debug.Log($"🧠 LevelManager initialized. Found {totalEnemies} enemies.");

        // --- Subscribe to events ---
        if (playerStats != null)
        {
            playerStats.OnDeathFinished -= OnLevelFailed; // prevent duplicate subscriptions
            playerStats.OnDeathFinished += OnLevelFailed;
        }

        // --- Hide endpoint until level cleared ---
        if (exitPoint != null)
            exitPoint.SetActive(false);
        else
            Debug.LogWarning("⚠️ LevelManager: Exit point not assigned in Inspector.");
    }

    // Called when an enemy dies
    public void OnEnemyDefeated()
    {
        defeatedEnemies++;
        if (defeatedEnemies >= totalEnemies)
            UnlockExit();
    }

    private void UnlockExit()
    {
        if (exitPoint != null)
        {
            exitPoint.SetActive(true);
            Debug.Log("🚪 Exit unlocked!");
        }
        else
        {
            Debug.LogWarning("⚠️ ExitPoint missing, cannot unlock exit.");
        }
    }

    public void OnLevelCompleted()
    {
        Debug.Log("✅ Level completed!");

        string sceneName = SceneManager.GetActiveScene().name;
        if (TryParseSceneName(sceneName, out int chapterIndex, out int levelIndex))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteLevel(chapterIndex, levelIndex, 0);
                GameManager.Instance.updateGameState(GameManager.GameState.Victory);
            }
            else
            {
                Debug.LogWarning("⚠️ GameManager.Instance not found during level completion.");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ LevelManager: Could not parse scene name '{sceneName}'. Expected format 'LevelX_Y'.");
        }

        if (UIManager.Instance != null)
            UIManager.Instance.ShowLevelComplete();
        else
            Debug.LogWarning("⚠️ UIManager.Instance not found — cannot show level complete screen.");
    }

    public void OnLevelFailed()
    {
        Debug.Log("❌ Level failed!");

        if (GameManager.Instance != null)
            GameManager.Instance.updateGameState(GameManager.GameState.Lose);
        else
            Debug.LogWarning("⚠️ GameManager.Instance not found during level failure.");

        if (UIManager.Instance != null)
            UIManager.Instance.ShowLevelFailed();
        else
            Debug.LogWarning("⚠️ UIManager.Instance not found — cannot show level failed screen.");
    }

    private void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnDeathFinished -= OnLevelFailed;
    }

    private void Update()
    {
        if (UIManager.Instance == null)
        {
            Debug.LogWarning("⚠️ UIManager.Instance is null in LevelManager Update.");
        }
        else
        {
            return;
        }
        // Pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
            UIManager.Instance.ShowPauseMenu(!UIManager.Instance.pauseMenu.activeSelf);
    }

    // Helper to parse "Level1_2" -> (0, 1)
    private bool TryParseSceneName(string sceneName, out int chapterIndex, out int levelIndex)
    {
        chapterIndex = levelIndex = 0;

        if (!sceneName.StartsWith("Level"))
            return false;

        string[] parts = sceneName.Replace("Level", "").Split('_');
        if (parts.Length < 2)
            return false;

        if (int.TryParse(parts[0], out int chapter) && int.TryParse(parts[1], out int level))
        {
            chapterIndex = chapter - 1;
            levelIndex = level - 1;
            return true;
        }

        return false;
    }
}

