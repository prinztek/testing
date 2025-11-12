using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    private void Awake()
    {
        // Ensure GameManager and UIManager exist
        if (GameManager.Instance == null)
            Debug.LogError("❌ GameManager missing in Bootstrap!");
        if (UIManager.Instance == null)
            Debug.LogError("❌ UIManager missing in Bootstrap!");

        // Load main menu next
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
