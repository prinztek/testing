using UnityEngine;

public class EnemyDeathState : EnemyBaseState
{
    private float deathDelay = 1f;
    private float stateStartTime;

    public override void EnterState(EnemyStateMachine enemy)
    {
        stateStartTime = Time.time;

        // Stop all movement
        enemy.rb.linearVelocity = Vector2.zero;
        enemy.rb.bodyType = RigidbodyType2D.Kinematic;

        // Disable attack collider if it exists
        if (enemy.stats.attackCollider != null)
            enemy.stats.attackCollider.SetActive(false);

        // Play death animation
        if (enemy.animator != null)
            enemy.animator.Play("enemydead");

        // ✅ Inform LevelManager to count this enemy as defeated
        UnityEngine.Object.FindFirstObjectByType<LevelManager>()?.OnEnemyDefeated();

        // Optionally disable enemy collider
        Collider2D col = enemy.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        Debug.Log("Entered EnemyDeathState.");
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        // Wait and destroy after deathDelay
        if (Time.time >= stateStartTime + deathDelay)
        {
            GameObject.Destroy(enemy.gameObject);
        }
    }

    public override void ExitState(EnemyStateMachine enemy)
    {
        // Normally won't exit from death, but clean up here if needed
    }
}
