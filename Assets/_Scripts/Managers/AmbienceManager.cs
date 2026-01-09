using UnityEngine;

public class AmbienceManager : MonoBehaviour
{
    public static AmbienceManager Instance;

    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        // If already playing this clip, skip
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetVolume(float level)
    {
        musicSource.volume = Mathf.Clamp01(level);
    }

    public void FadeOut(float duration = 1f)
    {
        StartCoroutine(FadeVolume(0f, duration));
    }

    public void FadeIn(float target = 1f, float duration = 1f)
    {
        StartCoroutine(FadeVolume(target, duration));
    }

    private System.Collections.IEnumerator FadeVolume(float targetVolume, float duration)
    {
        float startVolume = musicSource.volume;
        float time = 0f;

        while (time < duration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        musicSource.volume = targetVolume;
    }
}
