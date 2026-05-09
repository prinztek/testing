using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private CharacterStats playerStats;
    [SerializeField] private ExitPoint exitPoint; // Exit object
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
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
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
            // inform the math question manager that all enemies are defeated
            // it should prevent the player from answering more questions and show a message that all enemies are defeated and they can exit now
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
        Debug.Log("Level completed! LEVEL MANAGER");

        string sceneName = SceneManager.GetActiveScene().name;

        if (TryParseSceneName(sceneName, out int chapterIndex, out int levelIndex))
        {
            // if this is the first time completing a level
            if (chapterIndex == 0 && levelIndex == 0)
            {
                GameManager.Instance.badgeManager.FirstLevelComplete();
            }

            if (GameManager.Instance.badgeManager.IsUnlocked("PERM_MASTER") && chapterIndex == 0 && levelIndex == 7)
            {
                GameManager.Instance.badgeManager.ChapterComplete(0);
            }
            else if (GameManager.Instance.badgeManager.IsUnlocked("COMBO_MASTER") && chapterIndex == 1 && levelIndex == 7)
            {
                GameManager.Instance.badgeManager.ChapterComplete(1);
            }
            else if (GameManager.Instance.badgeManager.IsUnlocked("PROB_MASTER") && chapterIndex == 2 && levelIndex == 7)
            {
                GameManager.Instance.badgeManager.ChapterComplete(2);
            }

            // Debug.Log($"Parsed scene name '{sceneName}' as Chapter {chapterIndex + 1}, Level {levelIndex + 1}.");
            if (GameManager.Instance != null)
            {
                // if this is chapter 3 level 8 or 2_7, this is the last level and we should trigger the true ending instead of just a victory screen
                if (chapterIndex == 2 && levelIndex == 7)
                {
                    // GameManager.Instance.TriggerTrueEnding();
                    StartCoroutine(GameCompleteSequence());
                    return;
                }
                // normal level completion
                GameManager.Instance.CompleteLevel(chapterIndex, levelIndex, 0);
                GameManager.Instance.UpdateGameState(GameManager.GameState.Victory);
            }
            else
            {
                Debug.LogWarning("GameManager.Instance not found during level completion.");
            }
        }
        else
        {
            Debug.LogWarning($"LevelManager: Could not parse scene name '{sceneName}'. Expected format 'LevelX_Y'.");
        }

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

        if (UIManager.Instance != null)
        {
            StartCoroutine(LevelFailedSequence());
        }
        else
            Debug.LogWarning("UIManager.Instance not found — cannot show level failed screen.");
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

        UIManager.Instance.ShowLevelComplete(true);

        yield return GameManager.Instance.uiFade.FastFadeIn();
    }
    private IEnumerator LevelFailedSequence()
    {
        yield return GameManager.Instance.uiFade.FastFadeOut();

        UIManager.Instance.ShowLevelFailed(true);

        yield return GameManager.Instance.uiFade.FastFadeIn();
    }

    private IEnumerator GameCompleteSequence()
    {
        yield return GameManager.Instance.uiFade.FastFadeOut();

        GameManager.Instance.TriggerTrueEnding();
    }

}

