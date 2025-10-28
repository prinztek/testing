using UnityEngine;
using UnityEngine.UI;

public class ChapterSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject chapterSelectPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private Button[] chapterButtons;
    [SerializeField] private LevelButton[] levelButtons;

    private int currentChapterIndex;

    private void Start()
    {
        ShowChapterButtons();
    }

    private void ShowChapterButtons()
    {
        var saveData = GameManager.Instance.currentData;

        for (int i = 0; i < chapterButtons.Length; i++)
        {
            chapterButtons[i].interactable = saveData.chapters[i].levels[0].isUnlocked;

            int chapterIndex = i;
            chapterButtons[i].onClick.RemoveAllListeners();
            chapterButtons[i].onClick.AddListener(() => OpenChapter(chapterIndex));
        }
    }

    public void OpenChapter(int chapterIndex)
    {
        currentChapterIndex = chapterIndex;
        chapterSelectPanel.SetActive(false);
        levelSelectPanel.SetActive(true);

        // Set up level buttons with real save data
        var chapter = GameManager.Instance.currentData.chapters[currentChapterIndex];
        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].Setup(currentChapterIndex, i, chapter.levels[i]);
        }
    }

    public void BackToChapters()
    {
        levelSelectPanel.SetActive(false);
        chapterSelectPanel.SetActive(true);

        // Refresh chapter buttons in case progress changed
        ShowChapterButtons();
    }
}
