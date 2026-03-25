using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    public GameObject newPlayerMainMenuPanel;
    public GameObject enterNamePanel;
    public GameObject loadGameFilePanel;
    public GameObject chapterSelectionPanel;
    public GameObject levelSelectionPanel;
    public GameObject gameSettingsPanel;
    public GameObject badgesPanel;
    public Button continueButton;
    public Button chapterSelectButton;
    void Start()
    {
        if (JSONSaveSystem.GetMostRecentlyUpdatedProfileId() == null)
        {
            // no save files yet
            // hide continue button
            continueButton.gameObject.SetActive(false);
            chapterSelectButton.gameObject.SetActive(false);
            // otherwise direct it to the latest level of the most recent slot
        }
        else
        {
            continueButton.gameObject.SetActive(true);
            chapterSelectButton.gameObject.SetActive(false);
            // remove chapter select button redundant 
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
        levelSelectionPanel.SetActive(false);
        gameSettingsPanel.SetActive(false);
        badgesPanel.SetActive(false);
        // Activate the selected panel
        panelToShow.SetActive(true);
    }

    public void PlayGame()
    {
        // this is for a new player
        // loads Level1_1
        GameManager.Instance.LoadLevel(1, 1);
    }

    public void OnContinueGameClicked()
    {
        GameManager.Instance.ContinueGame();
    }

    public void OnChapterSelectClicked()
    {
        GameManager.Instance.SelectChapter();
        ShowPanel(chapterSelectionPanel);
    }

    public void OnSettingsButtonClicked()
    {
        ShowPanel(gameSettingsPanel);
    }

    public void OnBadgesButtonClicked()
    {
        GameManager.Instance.ViewBadges();
        ShowPanel(badgesPanel);
    }
    public void QuitGame()
    {
        Application.Quit(); // This will quit the application when running in a build
        Debug.Log("Quit Game"); // This log will appear in the console when running in the editor
    }
}
