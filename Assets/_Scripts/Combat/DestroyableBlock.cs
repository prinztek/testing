using Unity.Cinemachine;
using UnityEngine;

public class DestroyableBlock : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public GameObject destroyEffect; // optional particle effect
    public AudioClip destroySound;   // optional sound

    [SerializeField] CinemachineImpulseSource impulseSource;

    [Header("Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;

    private Vector3 originalPos;
    private bool isShaking = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        originalPos = transform.localPosition;
    }

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        currentHealth -= damage;

        if (!isShaking)
            StartCoroutine(Shake());

        if (currentHealth <= 0)
        {
            DestroyBlock();
        }

        // Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;
        // ScreenShakeManager.Instance.ScreenShake(direction, impulseSource);
    }

    private System.Collections.IEnumerator Shake()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            yield return null;
        }

        transform.localPosition = originalPos;
        isShaking = false;
    }

    void DestroyBlock()
    {
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        if (destroySound != null)
        {
            AudioSource.PlayClipAtPoint(destroySound, transform.position);
        }

        Destroy(gameObject);
    }
}
