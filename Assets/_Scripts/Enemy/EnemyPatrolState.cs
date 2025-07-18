using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
    private EnemyEnvironmentSensor sensor;

    public override void EnterState(EnemyStateMachine enemy)
    {
        sensor = enemy.sensor;
        enemy.animator.CrossFade("enemyrunning", 0.1f, 0, 0f);
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        if (sensor == null || enemy.player == null) return;

        float moveDir = enemy.movingRight ? 1f : -1f;
        enemy.rb.linearVelocity = new Vector2(moveDir * enemy.stats.moveSpeed, enemy.rb.linearVelocity.y);

        bool noGroundAhead = !sensor.IsGroundAhead(enemy.movingRight);
        bool isWall = sensor.IsFacingWall(enemy.movingRight);

        if (noGroundAhead || isWall)
        {
            // Just stop and idle — direction will flip after the pause
            enemy.TransitionToState(enemy.idleState);
            return;
        }

        if (enemy.stats.canChase)
        {
            float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);
            float verticalDiff = Mathf.Abs(enemy.transform.position.y - enemy.player.position.y);

            if (distance <= enemy.stats.DetectionRange && verticalDiff <= 1f)
            {
                enemy.TransitionToState(enemy.chaseState);
            }
        }
    }

    public override void ExitState(EnemyStateMachine enemy)
    {
        enemy.rb.linearVelocity = Vector2.zero;
    }
}
