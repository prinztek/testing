using UnityEngine;

public class HandleMusic : MonoBehaviour
{
    void OnEnable()
    {
        MusicManager.Instance.StopMusic();
    }
}
