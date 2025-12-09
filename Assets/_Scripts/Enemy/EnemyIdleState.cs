using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private float idleTimer;
    private float idleDuration;

    public override void EnterState(EnemyStateMachine enemy)
    {
        enemy.animator.CrossFade("enemyidle", 0.1f, 0, 0f);
        enemy.rb.linearVelocity = Vector2.zero;

        idleDuration = Random.Range(1f, 2f);
        idleTimer = 0f;
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        if (enemy.stats.IsDead || enemy.player == null) return;

        // --- PLAYER DETECTION BEFORE IDLE TIMER ---
        if (enemy.CanSeePlayer() && enemy.stats.canAttack && enemy.stats.canChase)
        {
            float dist = Vector2.Distance(enemy.transform.position, enemy.player.position);

            if (dist <= enemy.stats.attackRange)
            {
                enemy.TransitionToState(enemy.attackState);
                return;
            }
            else if (dist <= enemy.stats.detectionRange)
            {
                enemy.TransitionToState(enemy.chaseState);
                return;
            }
        }

        // --- IDLE TIMER LOGIC ---
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDuration)
        {
            // Flip direction *after* idling, then patrol
            enemy.movingRight = !enemy.movingRight;
            enemy.FlipDirection(enemy.movingRight);
            enemy.TransitionToState(enemy.patrolState);
        }
    }

    public override void ExitState(EnemyStateMachine enemy)
    {
        idleTimer = 0f;
    }
}
