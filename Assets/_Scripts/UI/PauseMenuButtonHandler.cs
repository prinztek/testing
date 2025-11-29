using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtonHandler : MonoBehaviour
{
    public void OnResumePressed()
    {
        UIManager.Instance.ShowPauseMenu(false);
    }

    public void OnBackToHomePressed()
    {
        UIManager.Instance.ShowPauseMenu(false);
        Time.timeScale = 1f; // ensure game unfreezes
        SceneManager.LoadScene("MainMenu");
    }
}
