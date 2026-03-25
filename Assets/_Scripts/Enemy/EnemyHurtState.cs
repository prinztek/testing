using UnityEngine;

public class EnemyHurtState : EnemyBaseState
{
    private float hurtDuration = 0.333f; // Duration of the hurt state in seconds
    private float stateStartTime;
    private bool transitionScheduled;

    // Knockback force values
    private float knockbackForce = 0.5f; // how far the enemy is knocked back
    private float verticalForce = 0f; // Vertical force to give a slight upward knockback effect ** ZERO FOR NOW
    private float verticalDamping = 0.1f; // Rate of vertical force decay

    public override void EnterState(EnemyStateMachine enemy)
    {
        stateStartTime = Time.time;
        transitionScheduled = false;

        // Zero out current movement before knockback
        enemy.rb.linearVelocity = Vector2.zero;

        // Apply knockback using last hit direction  (where the enemy was hit from)
        Vector2 knockbackDir = (enemy.lastHitDirection + Vector2.up * 0.2f).normalized;
        // enemy.rb.AddForce(force, ForceMode2D.Impulse);
        enemy.rb.linearVelocity = new Vector2(knockbackDir.x * knockbackForce, verticalForce);

        // Apply vertical damping over time to avoid continuous upward force
        // verticalForce -= verticalDamping * Time.deltaTime;

        // if (verticalForce <= 0)
        // {
        //     verticalForce = 0;
        // }


        enemy.animator.CrossFade("enemyhurt", 0.1f, 0, 0f);

        // Debug.Log("Entered Enemy Hurt State with knockback");
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        if (!transitionScheduled && Time.time >= stateStartTime + hurtDuration)
        {
            transitionScheduled = true;

            if (enemy.LastState != null)
                enemy.TransitionToState(enemy.LastState);
            else
                enemy.TransitionToState(enemy.patrolState); // fallback
        }
    }

    public override void ExitState(EnemyStateMachine enemy)
    {
        Vector2 linearVelocity = enemy.rb.linearVelocity;
        linearVelocity.x = 0f;
        enemy.rb.linearVelocity = linearVelocity;
    }
}
