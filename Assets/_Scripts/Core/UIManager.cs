using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main Panels")]
    public GameObject grimoirePanel;     // Book => Inventory/Crafting/Math Question 
    public GameObject buffChoicePanel;     // Book => Inventory/Crafting/Math Question 
    public GameObject pauseMenu;
    public GameObject levelCompletePanel;
    public GameObject levelFailedPanel;

    // Tracks the currently active modal (null if none open)
    private GameObject activePanel;

    // Event to notify GameManager or other systems when a modal opens/closes
    public event Action<bool> OnModalToggled;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        HideAllModals();
    }

    // === BOOK / GRIMOIRE ===
    public void ToggleBook(bool show)
    {
        if (grimoirePanel == null) return;

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
    public void ShowBuffChoicePanel(bool show)
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
