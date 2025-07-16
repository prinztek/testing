using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterSelectUI : MonoBehaviour
{
    public void LoadChapter(int chapterNumber)
    {
        SceneManager.LoadScene($"LevelSelect_Chapter{chapterNumber}");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
