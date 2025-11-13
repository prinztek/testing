using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main Panels")]

    public GameObject grimoirePanel;     // Book => Math Question/Inventory/Crafting/Calculator/Lessons
    public GameObject pauseMenu;
    public GameObject buffChoicePanel;
    public GameObject levelCompletePanel;
    public GameObject levelFailedPanel;

    // Tracks the currently active modal (null if none open)
    private GameObject activePanel;

    // Event to notify GameManager or other systems when a modal opens/closes 
    // for the GameManager to pause/unpause the game accordingly
    public event Action<bool> OnModalToggled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        HideAllModals();

        // Subscribe if GameManager already exists
        if (GameManager.Instance != null)
            RegisterToGameManager();
    }

    private void OnEnable()
    {
        // In case GameManager was created after UIManager
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.Instance != null)
            RegisterToGameManager();
    }

    private void RegisterToGameManager()
    {
        // Tell GameManager we're here
        GameManager.Instance.RegisterUIManager(this);
    }

    private void Start()
    {
        HideAllModals();
    }

    // === BOOK / GRIMOIRE ===
    public void ToggleBook(bool show)
    {
        Debug.Log("Toggling Grimoire: " + show);
        if (grimoirePanel == null)
        {
            Debug.LogWarning("⚠️ Grimoire Panel is not assigned in UIManager.");
            return;
        }

        if (show)
        {
            ShowModal(grimoirePanel);
        }
        else
        {
            ClosePanel(grimoirePanel);
        }
    }

    // === BUFF CHOICE / SELECTION OF BUFF TO CHOOSE FROM ===
    public void ShowBuffChoiceCanvas(bool show)
    {
        if (buffChoicePanel == null) return;

        if (show)
        {
            ShowModal(buffChoicePanel);
        }
        else
        {
            ClosePanel(buffChoicePanel);
        }
    }

    // === PAUSE MENU ===
    public void ShowPauseMenu(bool show)
    {
        if (pauseMenu == null) return;

        if (show)
        {
            ShowModal(pauseMenu);
        }
        else
        {
            ClosePanel(pauseMenu);
        }
    }

    // === LEVEL COMPLETE / FAILED ===
    public void ShowLevelComplete()
    {
        HideAllModals();
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            activePanel = levelCompletePanel; // ✅ Track active
            OnModalToggled?.Invoke(true);
        }
    }

    public void ShowLevelFailed()
    {
        HideAllModals();
        if (levelFailedPanel != null)
        {
            levelFailedPanel.SetActive(true);
            activePanel = levelFailedPanel; // ✅ Track active
            OnModalToggled?.Invoke(true);
        }
    }

    // === GENERIC MODAL HANDLING ===
    public void ShowModal(GameObject panelToShow)
    {
        HideAllModals(); // Hide any open panels first

        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
            activePanel = panelToShow; // ✅ Set as active
            OnModalToggled?.Invoke(true);
        }
    }

    public void ClosePanel(GameObject panel)
    {
        if (panel == null) return;

        panel.SetActive(false);

        if (panel == activePanel)
            activePanel = null; // ✅ Clear if it was the active one

        OnModalToggled?.Invoke(false);
    }

    public void HideAllModals()
    {
        if (grimoirePanel != null) grimoirePanel.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (levelFailedPanel != null) levelFailedPanel.SetActive(false);

        activePanel = null; // ✅ No active modal
        OnModalToggled?.Invoke(false);
    }

    // === NEW FEATURE ===
    /// <summary>
    /// Closes whichever panel is currently active (if any).
    /// Useful for other managers like MathQuestionManager or SkillUnlockManager.
    /// </summary>
    public void CloseActivePanel()
    {
        if (activePanel != null)
        {
            activePanel.SetActive(false);
            OnModalToggled?.Invoke(false);
            activePanel = null;
        }
    }

    // Optional getter if you ever need to check which panel is currently open
    public GameObject GetActivePanel()
    {
        return activePanel;
    }
}
