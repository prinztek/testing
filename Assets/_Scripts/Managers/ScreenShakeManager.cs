using Unity.Cinemachine;
using UnityEngine;
public class ScreenShakeManager : MonoBehaviour
{
    // singleton instance
    // This script manages screen shake effects in the game.
    // It allows other scripts to trigger screen shakes with specified duration and magnitude.
    public static ScreenShakeManager Instance { get; private set; }

    [Header("Screen Shake Settings")]
    [SerializeField] private float baseShakeForce = 3f; // max base force
    private float shakeMultiplier = 1f; // controlled by slider

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
    public void SetShakeMultiplier(float value)
    {
        shakeMultiplier = Mathf.Clamp01(value); // 0–1
        Debug.Log("ScreenShakeManager: shakeMultiplier = " + shakeMultiplier);
    }
    private float CurrentShakeForce => baseShakeForce * shakeMultiplier;

    // Screenshake with cinemachine
    public void ScreenShake(Vector2 direction, CinemachineImpulseSource impulseSource)
    {
        if (CurrentShakeForce <= 0f || impulseSource == null) return;
        impulseSource.GenerateImpulseWithVelocity(-direction * CurrentShakeForce);
    }

    public void ScreenShake(CinemachineImpulseSource impulseSource)
    {
        if (CurrentShakeForce <= 0f || impulseSource == null) return;
        impulseSource.GenerateImpulse();
    }

    public void TriggerEarthquakeShake(CinemachineImpulseSource impulseSource)
    {
        if (CurrentShakeForce <= 0f || impulseSource == null) return;
        impulseSource.GenerateImpulse();
    }
}
