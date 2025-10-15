using UnityEngine;

public class EnemyDeathState : EnemyBaseState
{
    private float deathDelay = 1f;
    private float stateStartTime;

    public override void EnterState(EnemyStateMachine enemy)
    {
        stateStartTime = Time.time;

        // Stop movement and disable physics interaction
        enemy.rb.linearVelocity = Vector2.zero;
        enemy.rb.bodyType = RigidbodyType2D.Kinematic;

        // Disable attack collider
        if (enemy.stats.attackCollider != null)
            enemy.stats.attackCollider.SetActive(false);

        // Play death animation
        if (enemy.animator != null)
            enemy.animator.Play("enemydead");

        // Notify LevelManager
        UnityEngine.Object.FindFirstObjectByType<LevelManager>()?.OnEnemyDefeated(); // convert these to an even call?

        // Disable main collider to prevent interaction
        Collider2D col = enemy.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Disable hurtbox for enemies with hurt on touch (crawlid like enemies in hollow kngiht)
        Transform hurtbox = enemy.transform.Find("Visual/HurtBox");
        if (hurtbox != null)
        {
            hurtbox.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Hurtbox not found in Visual/Hurtbox path.");
        }

        // Handle item drops
        if (enemy.stats != null)
        {
            foreach (var dropItem in enemy.stats.dropTable)
            {
                TrySpawnDrop(dropItem, enemy.stats.pickupPrefab, enemy.transform.position);
            }
        }

        // Debug.Log("Entered EnemyDeathState.");
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        if (Time.time >= stateStartTime + deathDelay)
        {
            GameObject.Destroy(enemy.gameObject);
        }
    }

    public override void ExitState(EnemyStateMachine enemy)
    {
        // No transition expected from death, but method is required
    }

    private void TrySpawnDrop(DropItem dropItem, GameObject pickupPrefab, Vector3 spawnPosition)
    {
        if (dropItem == null || pickupPrefab == null) return;

        if (Random.value <= dropItem.dropChance)
        {
            GameObject drop = GameObject.Instantiate(pickupPrefab, spawnPosition, Quaternion.identity);

            var pickup = drop.GetComponent<PickupItem>();
            if (pickup != null)
            {
                pickup.itemData = dropItem.itemData;

                // Assign icon to child sprite renderer
                Transform iconChild = drop.transform.Find("PickupItemIcon");
                if (iconChild != null)
                {
                    var iconRenderer = iconChild.GetComponent<SpriteRenderer>();
                    if (iconRenderer != null)
                    {
                        iconRenderer.sprite = dropItem.itemData.icon;
                        Debug.Log($"[Drop] Assigned icon: {iconRenderer.sprite?.name ?? "null"} to {drop.name}");
                    }
                    else
                    {
                        Debug.LogWarning("SpriteRenderer missing on PickupItemIcon.");
                    }
                }
                else
                {
                    Debug.LogWarning("PickupItemIcon child not found on drop prefab.");
                }

            }
            else
            {
                Debug.LogWarning("PickupItem component missing on pickup prefab.");
            }
        }
    }
}
