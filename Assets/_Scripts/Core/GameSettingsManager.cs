using UnityEngine;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject gameplaySettingsPanel;
    public GameObject soundSettingsPanel;
    public GameObject controlsSettingsPanel; // shows control buttons with label

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Load saved values
        masterSlider.value = GameManager.Instance.settingsGlobalData.masterVolume;
        musicSlider.value = GameManager.Instance.settingsGlobalData.musicVolume;
        sfxSlider.value = GameManager.Instance.settingsGlobalData.sfxVolume;

        // Add listeners (THIS is where you call the mixer manager)
        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);

        // Show gameplay panel by default
        gameplaySettingsPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        masterSlider.onValueChanged.RemoveListener(OnMasterChanged);
        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
    }

    private void OnMasterChanged(float value)
    {
        GameManager.Instance.settingsGlobalData.masterVolume = value;
        SoundMixerManager.Instance.SetMasterVolume(value);
        GameManager.Instance.SaveSettingsGlobal();
    }

    private void OnMusicChanged(float value)
    {
        GameManager.Instance.settingsGlobalData.musicVolume = value;
        SoundMixerManager.Instance.SetMusicVolume(value);
        GameManager.Instance.SaveSettingsGlobal();
    }

    private void OnSfxChanged(float value)
    {
        GameManager.Instance.settingsGlobalData.sfxVolume = value;
        SoundMixerManager.Instance.SetSFXVolume(value);
        GameManager.Instance.SaveSettingsGlobal();
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
