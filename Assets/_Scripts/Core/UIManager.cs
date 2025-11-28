using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Global UI Prefabs")]
    public GameObject grimoirePrefab;
    public GameObject buffChoicePrefab;
    public GameObject pauseMenuPrefab;
    public GameObject levelCompletePrefab;
    public GameObject levelFailedPrefab;
    public GameObject onScreenControlsPrefab;

    public GameObject playerHUD; // Reference to the player's HUD

    // Instantiated global panels
    private GameObject grimoirePanel;
    private GameObject buffChoicePanel;
    private GameObject pauseMenu;
    private GameObject levelCompletePanel;
    private GameObject levelFailedPanel;
    private GameObject onScreenControlsInstance;
    private GameObject playerHUDInstance;

    // Scene-specific panels
    private List<GameObject> scenePanels = new List<GameObject>();

    // Currently active modal
    private GameObject activePanel;

    // Event for GameManager to pause/unpause
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

        // Instantiate global UI panels
        if (grimoirePrefab != null)
            grimoirePanel = Instantiate(grimoirePrefab, transform);
        if (buffChoicePrefab != null)
            buffChoicePanel = Instantiate(buffChoicePrefab, transform);
        if (pauseMenuPrefab != null)
            pauseMenu = Instantiate(pauseMenuPrefab, transform);
        if (levelCompletePrefab != null)
            levelCompletePanel = Instantiate(levelCompletePrefab, transform);
        if (levelFailedPrefab != null)
            levelFailedPanel = Instantiate(levelFailedPrefab, transform);
        if (playerHUD != null)
            playerHUDInstance = Instantiate(playerHUD, transform);
        if (onScreenControlsPrefab != null && Application.isMobilePlatform)
        {
            onScreenControlsInstance = Instantiate(onScreenControlsPrefab, transform);
            onScreenControlsInstance.SetActive(true);
        }
        HideAllModals();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.OnPlayerSpawned += HandlePlayerSpawned; // ✅
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned; // ✅
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Disable Player HUD for non-gameplay scenes
        bool isGameplayScene = scene.name.StartsWith("Level");
        Debug.Log("Is Gameplay Scene: " + isGameplayScene);
        if (playerHUDInstance != null)
            playerHUDInstance.SetActive(isGameplayScene);
        // Hide all scene panels when a new scene loads
        foreach (var panel in scenePanels)
            panel.SetActive(false);
    }

    private void HandlePlayerSpawned(GameObject player)
    {
        if (playerHUDInstance != null)
        {
            playerHUDInstance.SetActive(true);
            Debug.Log("✅ Player HUD activated after player spawn.");
        }
    }

    // ===========================
    // GLOBAL UI METHODS
    // ===========================
    // Toggle Grimoire Panel
    public void ToggleBook(bool show)
    {
        if (grimoirePanel == null) return;
        if (show) ShowModal(grimoirePanel);
        else ClosePanel(grimoirePanel);
    }

    // name should be ShowBuffChoice
    public void ShowBuffChoiceCanvas(bool show)
    {
        if (buffChoicePrefab == null) return;
        if (show) ShowModal(buffChoicePanel);
        else ClosePanel(buffChoicePanel);
    }

    public void ShowPauseMenu(bool show)
    {
        if (pauseMenu == null) return;
        if (show) ShowModal(pauseMenu);
        else ClosePanel(pauseMenu);
    }

    public void ShowLevelComplete(bool show)
    {
        if (levelCompletePanel == null) return;
        if (show) ShowModal(levelCompletePanel);
        else ClosePanel(levelCompletePanel);
    }

    public void ShowLevelFailed(bool show)
    {
        if (levelFailedPanel == null) return;
        if (show) ShowModal(levelFailedPanel);
        else ClosePanel(levelFailedPanel);
    }

    // ===========================
    // SCENE-SPECIFIC UI
    // ===========================
    public void RegisterScenePanel(GameObject panel)
    {
        if (!scenePanels.Contains(panel))
            scenePanels.Add(panel);
    }

    public void ShowScenePanel(GameObject panel)
    {
        foreach (var p in scenePanels)
            p.SetActive(false);

        panel.SetActive(true);
        activePanel = panel;
        OnModalToggled?.Invoke(true);
    }

    // ===========================
    // MODAL HANDLING
    // ===========================
    public void ShowModal(GameObject panel)
    {
        if (panel == null) return;

        HideAllModals();
        panel.SetActive(true);
        activePanel = panel;
        OnModalToggled?.Invoke(true); // pause game
    }

    public void ClosePanel(GameObject panel)
    {
        if (panel == null) return;

        panel.SetActive(false);
        if (panel == activePanel)
            activePanel = null;

        OnModalToggled?.Invoke(false); // resume game
    }

    public void HideAllModals()
    {
        // Hide global panels
        if (grimoirePanel != null) grimoirePanel.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (levelFailedPanel != null) levelFailedPanel.SetActive(false);
        if (buffChoicePanel != null) buffChoicePanel.SetActive(false);
        // Hide scene panels
        foreach (var panel in scenePanels)
            panel.SetActive(false);

        activePanel = null;
        OnModalToggled?.Invoke(false);
    }

    public void CloseActivePanel()
    {
        if (activePanel != null)
        {
            Debug.Log("Closing:" + activePanel.name);
            activePanel.SetActive(false);
            activePanel = null;
            OnModalToggled?.Invoke(false);
        }
    }

    public GameObject GetActivePanel() => activePanel;
}
