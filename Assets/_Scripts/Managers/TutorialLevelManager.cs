using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class TutorialLevelManager : MonoBehaviour
{
    public static TutorialLevelManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject grimoireTutorialCanvas; // Canvas that contains the grimoire tutorial UI
    [SerializeField] private CharacterStats playerStats;
    [SerializeField] private ExitPoint exitPoint; // Exit object
    [SerializeField] public bool IsInTutorial = true; // Flag to indicate if the current level is a tutorial level
    private int totalEnemies;
    private int defeatedEnemies;
    [Header("Math Settings")]
    private MathQuestionManager mqm; // Reference to MathQuestionManager to set topic/difficulty
    private LessonListManager lessonListManager; // Reference to LessonListManager to trigger lesson button outline to hint the user that the question is related to this topic
    public MathTopic levelTopic = MathTopic.Permutation_and_Its_Conditions;
    public QuestionDifficulty levelDifficulty = QuestionDifficulty.Easy;

    [Header("Grimoire Hint")]
    [SerializeField] private float grimoireHintTimeThreshold = 60f;
    [SerializeField] private int maxHitsWithoutKill = 6;
    private float grimoireTimer;
    private int hitsSinceLastKill;
    private bool grimoireHintActive;
    public static event Action<bool> OnGrimoireHintStateChanged;

    [Header("Grimoire Hint Cooldown")]
    [SerializeField] private float hintCooldownAfterUse = 20f;
    private bool hintOnCooldown = false;
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
        GameManager.Instance.PlayLevelMusic();
        UIManager.Instance.OnGrimoireOpened += HandleGrimoireOpened;

    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
        UIManager.Instance.OnGrimoireOpened -= HandleGrimoireOpened;

    }
    private void HandleGrimoireOpened()
    {
        if (!IsInTutorial) return;

        if (GrimoireManager.Instance == null)
        {
            Debug.LogError("GrimoireManager not ready");
            return;
        }

        if (GrimoireTutorialController.Instance == null)
        {
            Debug.LogError("TutorialController missing");
            return;
        }

        // grimoireTutorialCanvas.SetActive(true);

        // disable any interaction with the grimoire until tutorial is complete
        GrimoireManager.Instance.canvasGroup.blocksRaycasts = false;

        GrimoireTutorialController.Instance.Initialize(GrimoireManager.Instance);
    }
    private void HandlePlayerSpawned(GameObject playerObj)
    {
        playerStats = playerObj.GetComponent<CharacterStats>();

        if (playerStats != null)
        {
            playerStats.OnDeathFinished -= OnLevelFailed; // avoid duplicate
            playerStats.OnDeathFinished += OnLevelFailed; // subscribe properly
            Debug.Log("LevelManager subscribed to player death event.");
        }

        string sceneName = SceneManager.GetActiveScene().name;

        if (TryParseSceneName(sceneName, out int chapterIndex, out int levelIndex))
        {
            // Only trigger for the first level in each chapter
            if (levelIndex == 0)
            {
                switch (chapterIndex)
                {
                    case 0:
                        if (!GameManager.Instance.badgeManager.IsUnlocked("PERM_START"))
                        {
                            GameManager.Instance.badgeManager.ChapterStart(0); // unlock PERM_START
                        }
                        break;
                    case 1:
                        if (!GameManager.Instance.badgeManager.IsUnlocked("COMB_START"))
                        {
                            GameManager.Instance.badgeManager.ChapterStart(1); // unlock COMB_START
                        }
                        break;
                    case 2:
                        if (!GameManager.Instance.badgeManager.IsUnlocked("PROB_START"))
                        {
                            GameManager.Instance.badgeManager.ChapterStart(2); // unlock PROB_START
                        }
                        break;
                }
            }
        }
    }

    private void Start()
    {

        if (UIManager.Instance != null)
        {
            mqm = UIManager.Instance.GetComponentInChildren<MathQuestionManager>(true);
            lessonListManager = UIManager.Instance.GetComponentInChildren<LessonListManager>(true);

            if (mqm != null)
            {
                // Initialize MathQuestionManager with level settings
                mqm.SetTopic(levelTopic, levelDifficulty);
                // Debug.Log("MathQuestionManager found and initialized.");
            }
            else
            {
                Debug.LogWarning("MathQuestionManager not found under UIManager.");
            }

            if (lessonListManager != null)
            {
                // Pass normalized topic name to LessonListManager so it can highlight the relevant lesson button
                lessonListManager.SetTopic(levelTopic);
                // lessonListManager.RefreshTopicHighlight(levelTopic.ToString());
                // Debug.Log("LessonListManager found and topic name set.");
            }
            else
            {
                Debug.LogWarning("LessonListManager not found under UIManager.");
            }
        }

        // --- Count total enemies in the scene ---
        totalEnemies = FindObjectsByType<EnemyStatsNew>(FindObjectsSortMode.None).Length +
                       FindObjectsByType<EnemyStats>(FindObjectsSortMode.None).Length;
        defeatedEnemies = 0;
        // Debug.Log($"LevelManager initialized. Found {totalEnemies} enemies.");
    }

    // Called when an enemy dies
    public void OnEnemyDefeated()
    {
        defeatedEnemies++;

        ResetGrimoireHintState();

        if (defeatedEnemies >= totalEnemies)
        {
            UnlockExit();
            Debug.Log("Exit unlocked!");
        }
    }

    private void UnlockExit()
    {
        if (exitPoint != null)
        {
            exitPoint.Unlock();
        }
        else
        {
            Debug.LogWarning("ExitPoint missing, cannot unlock exit.");
        }
    }

    public void OnLevelCompleted()
    {
        // Debug.Log("Level completed!");

        if (UIManager.Instance != null)
        {
            StartCoroutine(LevelCompleteSequence());
        }
        else
            Debug.LogWarning("UIManager.Instance not found — cannot show level complete screen.");
    }


    public void OnLevelFailed()
    {
        Debug.Log("Level failed!");

        if (GameManager.Instance != null)
            GameManager.Instance.UpdateGameState(GameManager.GameState.Lose);
        else
            Debug.LogWarning("GameManager.Instance not found during level failure.");

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
            Debug.LogWarning("UIManager.Instance is null in LevelManager Update.");
        }
        // Pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // if (GameManager.Instance != null && GameManager.Instance.GameState == GameState.PLaying)
            //     return; // Do not pause if game is lost

            // Only show pause menu if no other UI panel is active
            if (UIManager.Instance.GetActivePanel() == null)
            {
                UIManager.Instance.ShowPauseMenu(true);
            }
        }

        UpdateGrimoireHint(Time.deltaTime);
    }

    private void UpdateGrimoireHint(float deltaTime)
    {
        // Level already completed — no hint needed
        if (defeatedEnemies >= totalEnemies)
            return;

        grimoireTimer += deltaTime;

        // Check if hint should be shown based on cooldown
        if (hintOnCooldown)
        {
            return;
        }

        if (!grimoireHintActive &&
            (grimoireTimer >= grimoireHintTimeThreshold ||
             hitsSinceLastKill >= maxHitsWithoutKill))
        {
            grimoireHintActive = true;
            OnGrimoireHintStateChanged?.Invoke(true);
        }
    }

    public void RegisterEnemyHit()
    {
        hitsSinceLastKill++;
    }

    public void NotifyGrimoireOpened()
    {
        if (grimoireHintActive)
        {
            Debug.Log("Grimoire hint (button indicator) used, starting cooldown.");
            // Hide the hint immediately
            grimoireHintActive = false;
            OnGrimoireHintStateChanged?.Invoke(false);

            // Start cooldown
            hintOnCooldown = true;
            StartCoroutine(HintCooldownRoutine());

            // Reset timer so hint doesn't immediately reactivate after cooldown
            grimoireTimer = 0f;
            hitsSinceLastKill = 0;
        }
    }

    private IEnumerator HintCooldownRoutine()
    {
        yield return new WaitForSeconds(hintCooldownAfterUse);
        hintOnCooldown = false;
    }

    private void ResetGrimoireHintState()
    {
        grimoireTimer = 0f;
        hitsSinceLastKill = 0;

        if (!grimoireHintActive && !hintOnCooldown &&
            (grimoireTimer >= grimoireHintTimeThreshold ||
             hitsSinceLastKill >= maxHitsWithoutKill))
        {
            grimoireHintActive = true;
            OnGrimoireHintStateChanged?.Invoke(true);
        }

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

    private IEnumerator LevelCompleteSequence()
    {

        yield return GameManager.Instance.uiFade.FastFadeOut();

        GameManager.Instance.LoadLevel(0, 0);
    }

    // PANELS

    public GameObject joystickPanel;
    public GameObject attackPanel;
    public GameObject jumpPanel;
    public GameObject holdJumpPanel;
    public GameObject joystickOneWayPanel;
    public GameObject joystickOneWay2Panel;
    public GameObject interactPanel;
    public GameObject grimoirePanel;

    public void ShowPanel(GameObject panelToShow)
    {
        // Deactivate all panels
        joystickPanel.SetActive(false);
        attackPanel.SetActive(false);
        jumpPanel.SetActive(false);
        holdJumpPanel.SetActive(false);
        joystickOneWayPanel.SetActive(false);
        joystickOneWay2Panel.SetActive(false);
        interactPanel.SetActive(false);
        grimoirePanel.SetActive(false);
        // Activate the selected panel
        panelToShow.SetActive(true);
    }



}

