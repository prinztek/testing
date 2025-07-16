using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public override void EnterState(EnemyStateMachine enemy)
    {
        enemy.animator.CrossFade("enemyrunning", 0.1f);
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        if (enemy.stats.IsDead || enemy.player == null) return;

        FacePlayer(enemy);

        Vector2 direction = (enemy.player.position - enemy.transform.position).normalized;
        enemy.rb.linearVelocity = new Vector2(direction.x * enemy.stats.moveSpeed, enemy.rb.linearVelocity.y);

        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);
        float verticalDifference = Mathf.Abs(enemy.transform.position.y - enemy.player.position.y);

        // Check if within attack range
        if (enemy.stats.canAttack && distance <= enemy.stats.AttackRange && verticalDifference <= 1f)
        {
            enemy.TransitionToState(enemy.attackState);
        }
        else if (distance > enemy.stats.DetectionRange * 1.5f)
        {
            if (enemy.stats.canPatrol)
                enemy.TransitionToState(enemy.patrolState);
        }
    }

    private void FacePlayer(EnemyStateMachine enemy)
    {
        if (enemy.player == null) return;

        bool shouldFaceRight = enemy.player.position.x > enemy.transform.position.x;
        enemy.FlipDirection(shouldFaceRight);  // Flip the direction to face the player
    }

    public override void ExitState(EnemyStateMachine enemy) { }
}
