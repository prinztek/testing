using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private MathQuestionManager mathQuestionManager;
    private int totalEnemies;
    private int defeatedEnemies;

    public GameObject levelCompleteUIPanel; // Assign in inspector
    public GameObject levelFailedUIPanel;   // Assign in inspector
    public GameObject grimoirePanel;   // Assign in inspector
    public GameObject endpoint;             // Exit object
    [SerializeField] private GameObject pauseMenuUI; // Pause menu UI
    [SerializeField] private CharacterStats playerStats;
    public GameObject MathUICanvas; // Assign this in the Inspector

    private int pauseRequestCount = 0;

    private void Awake()
    {
        if (FindObjectsByType<LevelManager>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {


        totalEnemies = FindObjectsByType<EnemyStatsNew>(FindObjectsSortMode.None).Length;
        defeatedEnemies = 0;
        // Debug.Log($"Total enemies in level: {totalEnemies}");
        if (playerStats == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerStats = player.GetComponent<CharacterStats>();
        }

        if (playerStats != null)
        {
            playerStats.OnDeathFinished += OnLevelFailed;
        }
    }

    public void OnEnemyDefeated()
    {
        defeatedEnemies++;
        if (defeatedEnemies >= totalEnemies)
        {
            // Debug.Log("✅ All enemies defeated!");
            UnlockExit();
        }
    }

    public void OnLevelCompleted()
    {
        Debug.Log("✅ Level completed!");
        if (levelCompleteUIPanel != null)
        {
            levelCompleteUIPanel.SetActive(true);
            PauseGame();
        }
    }

    public void OnLevelFailed()
    {
        Debug.Log("❌ Level failed!");
        if (levelFailedUIPanel != null)
        {
            levelFailedUIPanel.SetActive(true);
            PauseGame();
        }
    }

    // unlock the exit when all enemies are defeated
    // this could be a door, portal, or any other object that signifies level completion
    private void UnlockExit()
    {
        if (endpoint != null)
            endpoint.SetActive(true);
    }

    private void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnDeathFinished -= OnLevelFailed;

        if (mathQuestionManager != null)
            mathQuestionManager.OnQuestionBatchCompleted -= OnLevelCompleted;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenOrCloseGrimoire();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleGrimoire();
        }
    }

    public void OpenOrCloseGrimoire()
    {
        bool isOpen = grimoirePanel.activeSelf;
        if (!isOpen) // Only toggle pause menu if Grimoire is not open
        {
            TogglePauseMenu();
        }
        else if (grimoirePanel.activeSelf) // If Grimoire is open, close it
        {
            ToggleGrimoire();
        }
    }
    private void ToggleGrimoire()
    {
        if (grimoirePanel != null)
        {
            bool isOpen = !grimoirePanel.activeSelf;
            grimoirePanel.SetActive(isOpen);

            if (isOpen)
                PauseGame();
            else
                ResumeGame();
        }
    }


    /// <summary>
    /// Toggles the sound menu UI and pauses/resumes the game.
    /// If the sound menu is active, it pauses the game; otherwise, it resumes.
    /// </summary>
    private void TogglePauseMenu()
    {
        bool isActive = !pauseMenuUI.activeSelf;
        pauseMenuUI.SetActive(isActive);

        if (isActive)
            PauseGame();
        else
            ResumeGame();
    }

    /// <summary>
    ///  For Pause Menu
    /// </summary>
    // General pause/resume control
    private void PauseGame()
    {
        pauseRequestCount++;
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        pauseRequestCount = Mathf.Max(0, pauseRequestCount - 1);
        if (pauseRequestCount == 0)
            Time.timeScale = 1f;
    }
}
