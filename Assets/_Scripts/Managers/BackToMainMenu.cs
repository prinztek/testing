using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
