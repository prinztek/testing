using UnityEngine;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject gameplaySettingsPanel;
    public GameObject soundSettingsPanel;
    public GameObject controlsSettingsPanel; // shows control buttons with label

    [Header("Checkbox for Gameplay Settings")]
    public Slider screenShakeMultiplierSlider;

    [Header("Sliders for Sound Settings")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Load saved values
        screenShakeMultiplierSlider.value = GameManager.Instance.settingsGlobalData.shakeMultiplier;
        masterSlider.value = GameManager.Instance.settingsGlobalData.masterVolume;
        musicSlider.value = GameManager.Instance.settingsGlobalData.musicVolume;
        sfxSlider.value = GameManager.Instance.settingsGlobalData.sfxVolume;

        // Add listeners
        screenShakeMultiplierSlider.onValueChanged.AddListener(OnScreenShakeMultiplierChanged);
        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);

        // Show gameplay panel by default
        gameplaySettingsPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        screenShakeMultiplierSlider.onValueChanged.RemoveListener(OnScreenShakeMultiplierChanged);
        masterSlider.onValueChanged.RemoveListener(OnMasterChanged);
        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
    }

    private void OnScreenShakeMultiplierChanged(float value)
    {
        GameManager.Instance.settingsGlobalData.shakeMultiplier = value;
        ScreenShakeManager.Instance.SetShakeMultiplier(value);
        GameManager.Instance.SaveSettingsGlobal();
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
