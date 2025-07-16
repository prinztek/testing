using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    public void LoadLevel(string levelSceneName)
    {
        SceneManager.LoadScene(levelSceneName);
    }

    public void BackToChapterSelect()
    {
        SceneManager.LoadScene("ChapterSelect");
    }
}
// This script allows the player to select a level from the level select menu and return to the chapter select menu.
// The LoadLevel method takes the name of the level scene to load, and the BackToChapterSelect method returns the player to the chapter select menu.