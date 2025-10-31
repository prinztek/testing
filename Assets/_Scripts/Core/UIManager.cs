using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main Panels")]
    public GameObject grimoirePanel;        // inventory/shop/quest book
    public GameObject pauseMenu;
    public GameObject levelCompletePanel;
    public GameObject levelFailedPanel;

    // Event to notify GameManager when a modal opens/closes
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

    // === Book UI ===
    public void ToggleBook(bool show)
    {
        if (grimoirePanel == null) return;
        grimoirePanel.SetActive(show);
        OnModalToggled?.Invoke(show);
    }

    // === Pause Menu ===
    public void ShowPauseMenu(bool show)
    {
        if (pauseMenu == null) return;
        pauseMenu.SetActive(show);
        OnModalToggled?.Invoke(show);
    }

    // === Level Complete / Failed ===
    public void ShowLevelComplete()
    {
        HideAllModals();
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            OnModalToggled?.Invoke(true);
        }
    }

    public void ShowLevelFailed()
    {
        HideAllModals();
        if (levelFailedPanel != null)
        {
            levelFailedPanel.SetActive(true);
            OnModalToggled?.Invoke(true);
        }
    }

    // === Utility (for level specific puzzle canvas) ===
    public void HideAllModals()
    {
        if (grimoirePanel != null) grimoirePanel.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (levelFailedPanel != null) levelFailedPanel.SetActive(false);
        OnModalToggled?.Invoke(false);
    }

    public void ClosePanel(GameObject panel)
    {
        if (panel == null) return;

        panel.SetActive(false);
        OnModalToggled?.Invoke(false); // notify that a modal closed
    }
}
