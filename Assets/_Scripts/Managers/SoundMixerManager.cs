
using System;
using UnityEngine;
using UnityEngine.Audio;
public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private AudioMixer AudioMixer;

    // setMasterVolume
    public void SetMasterVolume(float level)
    {
        // AudioMixer.SetFloat("masterVolume", level);
        AudioMixer.SetFloat("masterVolume", MathF.Log10(level) * 20);
    }

    // setMusicVolume
    public void SetMusicVolume(float level)
    {
        AudioMixer.SetFloat("musicVolume", MathF.Log10(level) * 20);
    }

    // setSFXVolume
    public void SetSFXVolume(float level)
    {
        AudioMixer.SetFloat("soundFXVolume", MathF.Log10(level) * 20);
    }
}
