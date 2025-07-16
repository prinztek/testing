using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    private bool isAttacking;
    private float lastAttackTime;

    public override void EnterState(EnemyStateMachine enemy)
    {
        isAttacking = false;
        lastAttackTime = Time.time - enemy.stats.attackCooldown; // Allow immediate attack on enter
        enemy.rb.linearVelocity = Vector2.zero;

        Debug.Log("Entered Attack State");
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        if (enemy.stats == null || enemy.player == null) return;

        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);
        float verticalDiff = Mathf.Abs(enemy.transform.position.y - enemy.player.position.y);

        if (distance > enemy.stats.AttackRange || verticalDiff > 1f)
        {
            enemy.TransitionToState(enemy.chaseState);
            return;
        }

        if (Time.time >= lastAttackTime + enemy.stats.attackCooldown && !isAttacking)
        {
            isAttacking = true;
            lastAttackTime = Time.time;

            FacePlayer(enemy);
            enemy.rb.linearVelocity = Vector2.zero;

            // ✅ Forcefully restart attack animation every time
            enemy.animator.CrossFade("enemyattack1", 0.05f, 0, 0f);

            // Reset after animation finishes
            enemy.StartCoroutine(ResetAttack(enemy, 0.5f)); // match to animation duration
        }
    }

    private System.Collections.IEnumerator ResetAttack(EnemyStateMachine enemy, float delay)
    {
        yield return new WaitForSeconds(delay);
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
