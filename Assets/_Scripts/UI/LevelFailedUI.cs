using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelFailedUI : MonoBehaviour
{
    public GameObject panel;

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
