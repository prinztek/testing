using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    public GameObject newPlayerMainMenuPanel;
    public GameObject enterNamePanel;
    public GameObject loadGameFilePanel;
    public GameObject chapterSelectionPanel;
    public GameObject gameSettingsPanel;
    public Button continueButton;


    void Start()
    {
        if (JSONSaveSystem.GetMostRecentlyUpdatedProfileId() == null)
        {
            // no save files yet
            // hide continue button
            continueButton.gameObject.SetActive(false);
            // otherwise direct it to the latest level of the most recent slot
        }
        else
        {
            continueButton.gameObject.SetActive(true);
        }

        // Ensure the New Player Main Menu Panel is active when the game starts
        ShowPanel(newPlayerMainMenuPanel);
    }

    public void ShowPanel(GameObject panelToShow)
    {
        // Deactivate all panels
        newPlayerMainMenuPanel.SetActive(false);
        enterNamePanel.SetActive(false);
        loadGameFilePanel.SetActive(false);
        chapterSelectionPanel.SetActive(false);
        gameSettingsPanel.SetActive(false);
        // Activate the selected panel
        panelToShow.SetActive(true);
    }

    public void PlayGame()
    {
        // this is for a new player
        // loads Level1_1
        GameManager.Instance.LoadLevel(1, 1);
        // SceneManager.LoadScene("Level1_1"); // Update with your game scene name
    }

    public void OnContinueGameClicked()
    {
        GameManager.Instance.ContinueGame();
    }

    public void QuitGame()
    {
        Application.Quit(); // This will quit the application when running in a build
        Debug.Log("Quit Game"); // This log will appear in the console when running in the editor
    }
}
