using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using System.Collections.Generic;
public class StoneWall : MonoBehaviour
{
    public float rumbleDuration = 0.5f;
    public float rumbleMagnitude = 0.1f;
    public float liftHeight = 5f;
    public float liftDuration = 1f;
    private CinemachineImpulseSource impulseSource;

    void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();

    }
    public void Lift()
    {
        StartCoroutine(RumbleAndLift());
        ScreenShakeManager.Instance.TriggerEarthquakeShake(impulseSource);
    }

    private IEnumerator RumbleAndLift()
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        // Phase 1: Rumble
        while (elapsed < rumbleDuration)
        {
            float x = Random.Range(-1f, 1f) * rumbleMagnitude;
            float z = Random.Range(-1f, 1f) * rumbleMagnitude;
            transform.position = originalPos + new Vector3(x, 0, z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;

        // Phase 2: Lift
        Vector3 targetPos = originalPos + new Vector3(0, liftHeight, 0);
        elapsed = 0f;

        while (elapsed < liftDuration)
        {
            transform.position = Vector3.Lerp(originalPos, targetPos, elapsed / liftDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }
}
