using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
    private bool movingRight = true;
    private EnemyEnvironmentSensor sensor;

    public override void EnterState(EnemyStateMachine enemy)
    {
        sensor = enemy.GetComponent<EnemyEnvironmentSensor>();
        enemy.animator.CrossFade("enemyrunning", 0.1f, 0, 0f); // Start running animation
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        if (sensor == null) return;

        float moveDir = movingRight ? 1f : -1f;
        enemy.rb.linearVelocity = new Vector2(moveDir * enemy.stats.moveSpeed, enemy.rb.linearVelocity.y);

        bool noGroundAhead = !sensor.IsGroundAhead(movingRight);
        bool isWall = sensor.IsFacingWall(movingRight);

        if (noGroundAhead || isWall)
        {
            Flip(enemy);
        }

        if (enemy.stats.canChase)
        {
            float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);
            float verticalDiff = Mathf.Abs(enemy.transform.position.y - enemy.player.position.y);

            if (distance <= enemy.stats.DetectionRange && verticalDiff <= 1f)
                enemy.TransitionToState(enemy.chaseState);
        }
    }

    private void Flip(EnemyStateMachine enemy)
    {
        movingRight = !movingRight;
        enemy.FlipDirection(movingRight);  // Flip the direction
    }

    public override void ExitState(EnemyStateMachine enemy)
    {
        enemy.rb.linearVelocity = Vector2.zero;
    }
}
