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
    public GameObject playerHUD;
    public GameObject textDamageScreenPrefab;

    // Instantiated global panels
    private GameObject grimoirePanel;
    private GameObject buffChoicePanel;
    private GameObject pauseMenu;
    private GameObject levelCompletePanel;
    private GameObject levelFailedPanel;
    private GameObject onScreenControlsInstance;
    private GameObject playerHUDInstance;
    private GameObject textDamageInstance;

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

        if (playerHUD != null)
        {
            playerHUDInstance = Instantiate(playerHUD, transform);
            playerHUDInstance.SetActive(false); // hide until player spawns
        }

        if (textDamageScreenPrefab != null)
        {
            textDamageInstance = Instantiate(textDamageScreenPrefab, transform);
            textDamageInstance.SetActive(false); // hide until player spawns
        }

        if (onScreenControlsPrefab != null && Application.isMobilePlatform)
        {
            onScreenControlsInstance = Instantiate(onScreenControlsPrefab, transform);
            onScreenControlsInstance.SetActive(false); // hide until player spawns and only show in gameplay scenes
        }

        if (levelCompletePrefab != null)
            levelCompletePanel = Instantiate(levelCompletePrefab, transform);

        if (levelFailedPrefab != null)
            levelFailedPanel = Instantiate(levelFailedPrefab, transform);

        HideAllModals();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isGameplayScene = scene.name.StartsWith("Level");

        // Show HUD only in gameplay scenes
        if (playerHUDInstance != null)
            playerHUDInstance.SetActive(isGameplayScene);

        // Show Canvas only in gameplay scenes (damage text when hit)
        if (textDamageInstance != null)
            textDamageInstance.SetActive(isGameplayScene);

        // Show mobile controls only in gameplay scenes
        if (onScreenControlsInstance != null)
            onScreenControlsInstance.SetActive(isGameplayScene);

        // Hide scene-specific panels
        foreach (var panel in scenePanels)
            panel.SetActive(false);
    }

    private void HandlePlayerSpawned(GameObject player)
    {
        if (playerHUDInstance != null)
        {
            playerHUDInstance.SetActive(true);

            // Assign player references to all HUD components
            var healthUI = playerHUDInstance.GetComponent<HealthBar>();
            var goldUI = playerHUDInstance.GetComponent<PlayerGoldUI>();
            var buffManager = playerHUDInstance.GetComponentInChildren<BuffUIManager>();

            if (healthUI != null)
                healthUI.SetPlayer(player);

            if (goldUI != null)
                goldUI.SetPlayer(player);

            if (buffManager != null)
            {
                buffManager.ClearAll();
                if (buffManager.buffPanel != null)
                    buffManager.buffPanel.SetActive(false);
            }
        }

        textDamageInstance.SetActive(true);

        if (onScreenControlsInstance != null)
            onScreenControlsInstance.SetActive(Application.isMobilePlatform);
    }

    // ===========================
    // GLOBAL UI METHODS
    // ===========================
    public void ToggleBook(bool show)
    {
        if (grimoirePanel == null) return;
        if (show) ShowModal(grimoirePanel);
        else ClosePanel(grimoirePanel);
    }

    public void ShowBuffChoiceCanvas(bool show)
    {
        if (buffChoicePanel == null) return;
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
    // // ===========================
    // public void RegisterScenePanel(GameObject panel)
    // {
    //     if (!scenePanels.Contains(panel))
    //         scenePanels.Add(panel);
    // }

    // public void ShowScenePanel(GameObject panel)
    // {
    //     foreach (var p in scenePanels)
    //         p.SetActive(false);

    //     panel.SetActive(true);
    //     activePanel = panel;
    //     OnModalToggled?.Invoke(true);
    // }

    // ===========================
    // MODAL HANDLING
    // ===========================
    public void ShowModal(GameObject panel)
    {
        if (panel == null) return;

        HideAllModals();
        panel.SetActive(true);
        activePanel = panel;
        OnModalToggled?.Invoke(true);
    }

    public void ClosePanel(GameObject panel)
    {
        if (panel == null) return;

        panel.SetActive(false);
        if (panel == activePanel)
            activePanel = null;

        OnModalToggled?.Invoke(false);
    }

    public void HideAllModals()
    {
        if (grimoirePanel != null) grimoirePanel.SetActive(false);
        if (buffChoicePanel != null) buffChoicePanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (levelFailedPanel != null) levelFailedPanel.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);

        foreach (var panel in scenePanels)
            panel.SetActive(false);

        activePanel = null;
        OnModalToggled?.Invoke(false);
    }

    public void CloseActivePanel()
    {
        if (activePanel != null)
        {
            activePanel.SetActive(false);
            activePanel = null;
            OnModalToggled?.Invoke(false);
        }
    }

    public GameObject GetActivePanel() => activePanel;
}
