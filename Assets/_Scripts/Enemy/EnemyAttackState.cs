using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    private bool isAttacking;
    private float lastAttackTime;

    public override void EnterState(EnemyStateMachine enemy)
    {
        isAttacking = false;
        lastAttackTime = Time.time - enemy.stats.attackCooldown; // Allow immediate attack
        enemy.rb.linearVelocity = Vector2.zero;

        // Debug.Log("Entered Attack State");
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        if (enemy.stats == null || enemy.player == null || enemy.stats.IsDead) return;

        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);
        // REMOVED: verticalDiff calculation

        // If player moved out of attack range, go back to chase
        // REMOVED: verticalDiff check
        if (distance > enemy.stats.AttackRange && !isAttacking)
        {
            enemy.TransitionToState(enemy.chaseState);
            return;
        }

        // Cooldown passed and not already attacking
        if (Time.time >= lastAttackTime + enemy.stats.attackCooldown && !isAttacking)
        {
            isAttacking = true;
            lastAttackTime = Time.time;

            FacePlayer(enemy);
            enemy.rb.linearVelocity = Vector2.zero;

            // Play attack animation
            enemy.animator.CrossFade("enemyattack1", 0.05f, 0, 0f);

            // Wait for animation duration then decide next state
            enemy.StartCoroutine(CompleteAttack(enemy, 0.5f)); // match animation length
        }
    }

    private System.Collections.IEnumerator CompleteAttack(EnemyStateMachine enemy, float delay)
    {
        yield return new WaitForSeconds(delay);

        // --- SAFETY CHECKS BEFORE USING ANY TRANSFORM ---
        if (enemy == null)
            yield break;

        if (enemy.stats == null || enemy.stats.IsDead)
            yield break;

        if (enemy.player == null)
            yield break;

        // Now safe to use positions
        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);

        // Decide next state
        if (enemy.stats.canChase && distance <= enemy.stats.DetectionRange)
        {
            enemy.TransitionToState(enemy.chaseState);
        }
        else if (enemy.stats.canPatrol)
        {
            enemy.TransitionToState(enemy.patrolState);
        }
        else
        {
            enemy.TransitionToState(enemy.idleState);
        }

        isAttacking = false;
    }


    public override void ExitState(EnemyStateMachine enemy)
    {
        isAttacking = false;
    }

    private void FacePlayer(EnemyStateMachine enemy)
    {
        if (enemy.player == null) return;

        bool shouldFaceRight = enemy.player.position.x > enemy.transform.position.x;
        enemy.FlipDirection(shouldFaceRight);
    }
}
