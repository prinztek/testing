using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject gameplaySettingsPanel;     // open on default, game settings like screenshake, etc.
    public GameObject soundSettingsPanel;     // volume sliders for music, sfx, voice
    public GameObject controlsSettingsPanel;     // keybindings and controller layout
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Gameplay Panel on by default
        gameplaySettingsPanel.SetActive(true);
    }

    public void ShowGameplaySettingsPanel()
    {
        gameplaySettingsPanel.SetActive(true);
        soundSettingsPanel.SetActive(false);
        controlsSettingsPanel.SetActive(false);
    }
    public void ShowSoundSettingsPanel()
    {
        gameplaySettingsPanel.SetActive(false);
        soundSettingsPanel.SetActive(true);
        controlsSettingsPanel.SetActive(false);
    }
    public void ShowControlsSettingsPanel()
    {
        gameplaySettingsPanel.SetActive(false);
        soundSettingsPanel.SetActive(false);
        controlsSettingsPanel.SetActive(true);
    }
}
