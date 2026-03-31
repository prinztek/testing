using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;

    void OnEnable()
    {
        MusicManager.Instance.PlayMusic(musicClip, true);
    }
}
