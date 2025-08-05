using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterSelectUI : MonoBehaviour
{
    // this is assigned to button onclick with value of 1, 2, 3
    public void LoadChapter(int chapterNumber)
    {
        GameData.CurrentChapter = chapterNumber;
        SceneManager.LoadScene("LevelSelect");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}


