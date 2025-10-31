using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private MathQuestionManager mathQuestionManager;
    [SerializeField] private CharacterStats playerStats;

    private int totalEnemies;
    private int defeatedEnemies;

    public GameObject endpoint; // Exit object

    private void Awake()
    {
        // Singleton-like behavior to prevent duplicates
        if (FindObjectsByType<LevelManager>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Count total enemies in the level
        totalEnemies = FindObjectsByType<EnemyStatsNew>(FindObjectsSortMode.None).Length;
        defeatedEnemies = 0;

        if (playerStats == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerStats = player.GetComponent<CharacterStats>();
        }

        if (playerStats != null)
            playerStats.OnDeathFinished += OnLevelFailed;

    }

    // Call this when an enemy dies
    public void OnEnemyDefeated()
    {
        defeatedEnemies++;
        if (defeatedEnemies >= totalEnemies)
        {
            UnlockExit();
        }
    }

    private void UnlockExit()
    {
        if (endpoint != null)
            endpoint.SetActive(true);
    }

    // Level complete logic
    public void OnLevelCompleted()
    {
        Debug.Log("✅ Level completed!");

        // Notify GameManager (handles progression, saving, etc.)
        string sceneName = SceneManager.GetActiveScene().name; // e.g., "Level1_1"
        string[] parts = sceneName.Replace("Level", "").Split('_');
        int chapterIndex = int.Parse(parts[0]) - 1;
        int levelIndex = int.Parse(parts[1]) - 1;

        GameManager.Instance.CompleteLevel(chapterIndex, levelIndex, 0);

        // Let UIManager handle the modal + pause
        if (UIManager.Instance != null)
            UIManager.Instance.ShowLevelComplete();
    }

    // Level failed logic
    public void OnLevelFailed()
    {
        Debug.Log("❌ Level failed!");
        if (UIManager.Instance != null)
            UIManager.Instance.ShowLevelFailed();
    }

    private void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnDeathFinished -= OnLevelFailed;

        if (mathQuestionManager != null)
            mathQuestionManager.OnQuestionBatchCompleted -= OnLevelCompleted;
    }

    // Optional input handling for book/pause
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // Open/close book
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ToggleBook(!UIManager.Instance.grimoirePanel.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) // Pause menu
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ShowPauseMenu(!UIManager.Instance.pauseMenu.activeSelf);
        }
    }
}
