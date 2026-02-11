using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;
    [SerializeField] private AudioSource soundFXObject;

    // Global SFX volume (controlled by AudioSettingsManager)
    private float globalVolume = 1f;

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

    /// <summary>
    /// Called by AudioSettingsManager when SFX volume changes
    /// </summary>
    public void SetGlobalVolume(float value)
    {
        globalVolume = Mathf.Clamp01(value);
    }

    public void playOneShotSoundFXClilp(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        // audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.volume = volume * globalVolume; // apply global volume
        audioSource.PlayOneShot(clip);
        Destroy(audioSource.gameObject, clip.length);
    }

    public void playSoundFXClilp(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume * globalVolume; // apply global volume
        audioSource.Play();
        Destroy(audioSource.gameObject, clip.length);
    }

    public void playSoundFXClilpRandomPitch(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume * globalVolume; // apply global volume
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
        Destroy(audioSource.gameObject, clip.length);
    }
}
