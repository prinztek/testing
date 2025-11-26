using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button button;                  // Button for click events
    [SerializeField] private Image backgroundImage;         // Child image acting as background
    [SerializeField] private TextMeshProUGUI label;         // Level number label
    [SerializeField] private GameObject completedMark;      // Completed checkmark
    [SerializeField] private GameObject lockedMark;         // Locked icon

    [Header("Chapter Level Sprites")]
    [SerializeField] private Sprite[] chapterBackgroundSprites; // Background per chapter

    private int chapterIndex;
    private int levelIndex;

    /// <summary>
    /// Setup the button for the given chapter and level
    /// </summary>
    public void Setup(int chapter, int level, LevelData data)
    {
        chapterIndex = chapter;
        levelIndex = level;

        // Set background sprite for this chapter
        if (backgroundImage != null && chapterBackgroundSprites != null && chapter < chapterBackgroundSprites.Length)
        {
            backgroundImage.sprite = chapterBackgroundSprites[chapter];
        }

        // Button click
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);

            // Only interactable if unlocked
            button.interactable = data.isUnlocked;
        }

        // Set label text and color
        if (label != null)
        {
            label.text = (level + 1).ToString();             // Displayed to player is 1-based
            if (chapter == 1)
            {
                label.color = Color.white;
            }
        }

        // Show completed mark only if level is completed
        if (completedMark != null)
            completedMark.SetActive(data.isCompleted);

        // Show locked mark if level is locked
        if (lockedMark != null)
            lockedMark.SetActive(!data.isUnlocked);
    }

    /// <summary>
    /// Called when the button is clicked
    /// </summary>
    private void OnClick()
    {
        if (button.interactable)
        {
            GameManager.Instance.LoadLevel(chapterIndex, levelIndex);
        }
    }
}
