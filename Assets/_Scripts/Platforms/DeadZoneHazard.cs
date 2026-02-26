using UnityEngine;

public class DeadZoneHazard : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    [Header("Player")]
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        CharacterStats playerStats = other.GetComponentInParent<CharacterStats>();
        EnemyStatsNew enemyStats = other.GetComponentInParent<EnemyStatsNew>();

        playerStats?.TakeDamage(10, transform.position);
        enemyStats?.TakeDamage(10, transform.position);
        Respawn(other.gameObject);
    }

    private void Respawn(GameObject player)
    {
        // Reset velocity so player doesn't keep falling
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Teleport to safe position
        player.transform.position = respawnPoint.position;
    }
}
