using System;

[Serializable]
public class JSONSettingsData
{
    // Master volume (0.0f – 1.0f)
    public float masterVolume = 1.0f;

    // Music volume (0.0f – 1.0f)
    public float musicVolume = 1.0f;

    // Sound effects volume (0.0f – 1.0f)
    public float sfxVolume = 1.0f;

    // UI volume (0.0f – 1.0f)
    public float uiVolume = 1.0f;

    // Screenshake toggle
    public bool screenShakeEnabled = true;

    // Optional: A constructor for default values
    public JSONSettingsData() { }

    public JSONSettingsData(
        float master,
        float music,
        float sfx,
        float ui,
        bool shake)
    {
        masterVolume = master;
        musicVolume = music;
        sfxVolume = sfx;
        uiVolume = ui;
        screenShakeEnabled = shake;
    }
}

// This class tracks Game Settings (SettingsData)
// This is the data model for saving/loading settings in JSON format.
// Master Volume, Music Volume, SFX Volume, UI Volume, Screenshake On or Off