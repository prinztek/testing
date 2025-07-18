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

        Debug.Log("Entered Attack State");
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        if (enemy.stats == null || enemy.player == null || enemy.stats.IsDead) return;

        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);
        float verticalDiff = Mathf.Abs(enemy.transform.position.y - enemy.player.position.y);

        // If player moved out of attack range, go back to chase
        if ((distance > enemy.stats.AttackRange || verticalDiff > 1f) && !isAttacking)
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
        isAttacking = false;

        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);
        float verticalDiff = Mathf.Abs(enemy.transform.position.y - enemy.player.position.y);

        if (enemy.stats.IsDead || enemy.player == null) yield break;

        if (enemy.stats.canChase && distance <= enemy.stats.DetectionRange && verticalDiff <= 1f)
        {
            // Still within range → chase again
            enemy.TransitionToState(enemy.chaseState);
        }
        else if (enemy.stats.canPatrol)
        {
            enemy.TransitionToState(enemy.patrolState); // Lost the player
        }
        else
        {
            enemy.TransitionToState(enemy.idleState); // Fallback
        }
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
