using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteButtonHandler : MonoBehaviour
{
    public void OnRetryPressed()
    {
        UIManager.Instance.ShowLevelComplete(false);
        Time.timeScale = 1f; // ensure game unfreezes
        string sceneName = SceneManager.GetActiveScene().name;
        if (TryParseSceneName(sceneName, out int chapterIndex, out int levelIndex))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadLevel(chapterIndex, levelIndex);
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ LevelManager: Could not parse scene name '{sceneName}'. Expected format 'LevelX_Y'.");
        }
    }

    public void OnBackToHomePressed()
    {
        UIManager.Instance.ShowLevelComplete(false);
        Time.timeScale = 1f; // ensure game unfreezes
        SceneManager.LoadScene("MainMenu");
    }

    public void OnNextLevelPressed()
    {
        UIManager.Instance.ShowLevelComplete(false);
        Time.timeScale = 1f; // ensure game unfreezes
        string sceneName = SceneManager.GetActiveScene().name;
        if (TryParseSceneName(sceneName, out int chapterIndex, out int levelIndex))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadLevel(chapterIndex, levelIndex + 1);
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ LevelManager: Could not parse scene name '{sceneName}'. Expected format 'LevelX_Y'.");
        }
    }

    // Helper to parse "Level1_2" -> (0, 1)
    private bool TryParseSceneName(string sceneName, out int chapterIndex, out int levelIndex)
    {
        chapterIndex = levelIndex = 0;

        if (!sceneName.StartsWith("Level"))
            return false;

        string[] parts = sceneName.Replace("Level", "").Split('_');
        if (parts.Length < 2)
            return false;

        if (int.TryParse(parts[0], out int chapter) && int.TryParse(parts[1], out int level))
        {
            chapterIndex = chapter - 1;
            levelIndex = level - 1;
            return true;
        }

        return false;
    }
}
