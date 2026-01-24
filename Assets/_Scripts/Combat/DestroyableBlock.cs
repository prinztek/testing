using UnityEngine;

public class DestroyableBlock : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public GameObject destroyEffect; // optional particle effect
    public AudioClip destroySound;    // optional sound

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            DestroyBlock();
        }
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
