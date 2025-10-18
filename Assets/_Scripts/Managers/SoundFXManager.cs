using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;
    [SerializeField] private AudioSource soundFXObject;

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
    public void playOneShotSoundFXClilp(AudioClip clip, Transform spawnTransform, float volume)
    {
        // spawn game object
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        // random pitch between 0.95 and 1.05 (adjust as needed)
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        // assign volume
        audioSource.volume = volume;

        // play sound with PlayOneShot (no delay, no need to assign clip)
        audioSource.PlayOneShot(clip);

        // destroy game object after clip length
        Destroy(audioSource.gameObject, clip.length);
    }

    public void playSoundFXClilp(AudioClip clip, Transform spawnTransform, float volume)
    {
        // spawn game object
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        // assign clip
        audioSource.clip = clip;
        // assign volume
        audioSource.volume = volume;
        // play sound
        audioSource.Play();
        // get length of clip
        float clipLength = clip.length;
        // destroy game object after clip length
        Destroy(audioSource.gameObject, clipLength);
    }

    // play sound effect with different pitch
    public void playSoundFXClilpRandomPitch(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        // randomize pitch
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
        Destroy(audioSource.gameObject, clip.length);
    }
}
