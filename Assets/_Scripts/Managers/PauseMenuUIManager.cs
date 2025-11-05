using UnityEngine;

public class PauseMenuUIManager : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject defaultPanel;     // resume button, back to home button, settings button
    public GameObject settingsPanel;     // holds game, sound, and control panels
    public GameObject gameplaySettingsPanel;     // open on default, game settings like difficulty, subtitles, etc.
    public GameObject soundSettingsPanel;     // volume sliders for music, sfx, voice
    public GameObject controlsSettingsPanel;     // keybindings and controller layout


    // this script is only for managing the pause menu UI specifically
    // holds references to all pause menu related panels and switches between them
    // for the settings panel it holds three buttons to switch between subpanels
    private void Start()
    {
        ShowDefaultPanel();
    }
    public void ShowDefaultPanel()
    {
        defaultPanel.SetActive(true);
        settingsPanel.SetActive(false);
        gameplaySettingsPanel.SetActive(false);
        soundSettingsPanel.SetActive(false);
        controlsSettingsPanel.SetActive(false);
    }

    public void ShowSettingsPanel()
    {
        defaultPanel.SetActive(false);
        settingsPanel.SetActive(true);
        ShowGameplaySettingsPanel();
    }

    public void ShowGameplaySettingsPanel()
    {
        settingsPanel.SetActive(true);
        gameplaySettingsPanel.SetActive(true);
        soundSettingsPanel.SetActive(false);
        controlsSettingsPanel.SetActive(false);
    }
    public void ShowSoundSettingsPanel()
    {
        settingsPanel.SetActive(true);
        gameplaySettingsPanel.SetActive(false);
        soundSettingsPanel.SetActive(true);
        controlsSettingsPanel.SetActive(false);
    }
    public void ShowControlsSettingsPanel()
    {
        settingsPanel.SetActive(true);
        gameplaySettingsPanel.SetActive(false);
        soundSettingsPanel.SetActive(false);
        controlsSettingsPanel.SetActive(true);
    }

}
