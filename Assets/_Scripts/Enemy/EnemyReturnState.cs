using UnityEngine;
public class EnemyReturnState : EnemyBaseState
{
    public override void EnterState(EnemyStateMachine enemy)
    {
        enemy.animator.CrossFade("enemyrunning", 0.1f);
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        Vector2 direction = enemy.homePosition - (Vector2)enemy.transform.position;

        enemy.FlipDirection(direction.x > 0f);

        enemy.rb.linearVelocity = new Vector2(
            Mathf.Sign(direction.x) * enemy.stats.moveSpeed,
            enemy.rb.linearVelocity.y
        );

        if (Mathf.Abs(direction.x) < 0.1f)
        {
            enemy.TransitionToState(enemy.patrolState);
        }
    }
    public override void ExitState(EnemyStateMachine enemy) { }
}
