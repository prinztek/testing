using UnityEngine;
using UnityEngine.UI;

public class ChapterSelectUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject chapterSelectPanel;
    [SerializeField] private GameObject levelSelectPanel;

    [Header("Chapter Buttons")]
    [SerializeField] private Button[] chapterButtons;

    [Header("Level Buttons")]
    [SerializeField] private LevelButton[] levelButtons;

    [Header("Chapter Title UI")]
    [SerializeField] private Image chapterTitleImage;      // UI Image for chapter title
    [SerializeField] private Sprite[] chapterTitleSprites; // Sprites for each chapter

    private int currentChapterIndex;

    private void Start()
    {
        ShowChapterButtons();
    }

    /// <summary>
    /// Shows all chapter buttons and sets their interactability based on save data.
    /// </summary>
    private void ShowChapterButtons()
    {
        var saveData = GameManager.Instance.currentData;

        for (int i = 0; i < chapterButtons.Length; i++)
        {
            // Safely check if the chapter exists in save data
            if (i < saveData.chapters.Length)
            {
                chapterButtons[i].interactable = saveData.chapters[i].levels.Length > 0 && saveData.chapters[i].levels[0].isUnlocked;

                int chapterIndex = i; // closure
                chapterButtons[i].onClick.RemoveAllListeners();
                chapterButtons[i].onClick.AddListener(() => OpenChapter(chapterIndex));
            }
            else
            {
                chapterButtons[i].interactable = false;
            }
        }
    }

    /// <summary>
    /// Opens the selected chapter, updates the chapter title, and sets up level buttons.
    /// </summary>
    public void OpenChapter(int chapterIndex)
    {
        currentChapterIndex = chapterIndex;

        // Show level select panel
        chapterSelectPanel.SetActive(false);
        levelSelectPanel.SetActive(true);

        // -------------------------------
        // Update Chapter Title Image
        // -------------------------------
        if (chapterTitleSprites != null &&
            chapterIndex >= 0 &&
            chapterIndex < chapterTitleSprites.Length &&
            chapterTitleImage != null)
        {
            chapterTitleImage.sprite = chapterTitleSprites[chapterIndex];
        }
        else
        {
            Debug.LogWarning($"Chapter sprite missing for index {chapterIndex}");
            chapterTitleImage.sprite = null;
        }

        // -------------------------------
        // Setup Level Buttons
        // -------------------------------
        var chapter = GameManager.Instance.currentData.chapters[currentChapterIndex];

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i < chapter.levels.Length)
            {
                levelButtons[i].Setup(currentChapterIndex, i, chapter.levels[i]);
                levelButtons[i].gameObject.SetActive(true);
            }
            else
            {
                // Hide buttons that don't have a level
                levelButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Returns to the chapter select panel
    /// </summary>
    public void BackToChapters()
    {
        levelSelectPanel.SetActive(false);
        chapterSelectPanel.SetActive(true);

        ShowChapterButtons();
    }
}
