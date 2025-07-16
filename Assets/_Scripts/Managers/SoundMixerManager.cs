using System;
using UnityEngine;
using UnityEngine.Audio;
public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer AudioMixer;

    // setMasterVolume
    public void SetMasterVolume(float level)
    {
        // AudioMixer.SetFloat("masterVolume", level);
        AudioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20);
    }

    // setMusicVolume
    public void SetMusicVolume(float level)
    {
        // AudioMixer.SetFloat("musicVolume", level);
        AudioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20);
    }

    // setSFXVolume
    public void SetSFXVolume(float level)
    {
        // AudioMixer.SetFloat("soundFXVolume", level);
        AudioMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20);
    }
}
