using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    public GameObject newPlayerMainMenuPanel;
    public GameObject createNewGamePanel;
    public GameObject returningPlayerMainMenuPanel;
    public GameObject chapterSelectionPanel;
    public GameObject settingsAndControlPanel;
    public GameObject settingsPanel;
    public GameObject controlsPanel;
    public GameObject creditsPanel;

    void Start()
    {
        // Ensure the New Player Main Menu Panel is active when the game starts
        ShowPanel(newPlayerMainMenuPanel);
    }

    public void ShowPanel(GameObject panelToShow)
    {
        // Deactivate all panels
        newPlayerMainMenuPanel.SetActive(false);
        createNewGamePanel.SetActive(false);
        returningPlayerMainMenuPanel.SetActive(false);
        chapterSelectionPanel.SetActive(false);
        // levelSelectionPanel.SetActive(false);
        settingsAndControlPanel.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);

        // Activate the selected panel
        panelToShow.SetActive(true);
    }

    public void BackToMainMenu()
    {
        // Show returning player main menu when going back to the main menu
        ShowPanel(returningPlayerMainMenuPanel);
    }

    public void PlayGame()
    {
        // this is for a new player
        SceneManager.LoadScene("Level1_1"); // Update with your game scene name
    }

    public void ContinueGame()
    {
        // this is for a returning player
        // Load the next level the player has completed
    }

    public void QuitGame()
    {
        Application.Quit(); // This will quit the application when running in a build
        Debug.Log("Quit Game"); // This log will appear in the console when running in the editor
    }
}
