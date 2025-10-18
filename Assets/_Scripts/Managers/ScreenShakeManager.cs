using Unity.Cinemachine;
using UnityEngine;
public class ScreenShakeManager : MonoBehaviour
{
    // singleton instance
    // This script manages screen shake effects in the game.
    // It allows other scripts to trigger screen shakes with specified duration and magnitude.
    public static ScreenShakeManager Instance { get; private set; }

    [Header("Screen Shake Settings")]
    [SerializeField] private float globalShakeForce = 3f;

    private void Awake()
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

    // Screenshake with cinemachine
    public void ScreenShake(Vector2 direction, CinemachineImpulseSource impulseSource)
    {
        if (impulseSource == null)
        {
            Debug.LogWarning("Impulse source is null. Cannot trigger screen shake.");
            return;
        }
        else
        {
            impulseSource.GenerateImpulseWithVelocity(-direction * globalShakeForce);
        }


    }

    public void TriggerEarthquakeShake(CinemachineImpulseSource impulseSource)
    {
        if (impulseSource == null)
        {
            Debug.LogWarning("Impulse source is null. Cannot trigger screen shake.");
            return;
        }

        impulseSource.GenerateImpulse();
    }



}
