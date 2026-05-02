using UnityEngine;
using UnityEngine.UI;

public class TutorialPromptManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject tutorialPromptCanvas;
    public Button playTutorialButton;
    public Button skipButton;

    private bool hasShown = false;

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;

        // If player already exists, trigger immediately
        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            HandlePlayerSpawned(GameManager.Instance.CurrentPlayer);
        }

        // Prevent duplicate listeners
        playTutorialButton.onClick.RemoveAllListeners();
        skipButton.onClick.RemoveAllListeners();

        playTutorialButton.onClick.AddListener(PlayTutorial);
        skipButton.onClick.AddListener(SkipTutorial);
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        if (hasShown) return;

        hasShown = true;

        UIManager.Instance.ShowModal(tutorialPromptCanvas);
        // .SetActive(true);
    }

    public void PlayTutorial()
    {
        // Just hide prompt (stay in tutorial level)
        UIManager.Instance.ClosePanel(tutorialPromptCanvas);
        // tutorialPromptCanvas.SetActive(false);
    }

    public void SkipTutorial()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadLevel(0, 0);
        }
    }
}