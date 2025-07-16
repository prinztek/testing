using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour
{
    public Animator animator;
    private EnemyStatsNew stats;
    private Rigidbody2D rb;
    private EnemyEnvironmentSensor sensor;

    private bool movingRight = true;
    private bool isPaused = false;

    public float pauseBeforeFlip = 4f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<EnemyStatsNew>();
        sensor = GetComponent<EnemyEnvironmentSensor>();

        if (rb == null)
            Debug.LogError("Rigidbody2D not found on " + gameObject.name);
        if (stats == null)
            Debug.LogError("EnemyStatsNew not found on " + gameObject.name);
        if (sensor == null)
            Debug.LogError("EnemyEnvironmentSensor not found on " + gameObject.name);
    }


    void FixedUpdate()
    {
        if (isPaused || !stats.canPatrol || sensor == null)
            return;

        // Move the enemy
        rb.linearVelocity = new Vector2((movingRight ? 1 : -1) * stats.moveSpeed, rb.linearVelocity.y);

        // Check environment using sensor
        bool noGround = !sensor.IsGroundAhead(movingRight);
        bool hitWall = sensor.IsFacingWall(movingRight);

        if ((noGround && !hitWall) || (noGround && hitWall))
        {
            // Debug.Log("noGround: " + noGround + ", hitWall: " + hitWall);
            StartCoroutine(HandleFlip());
        }
    }

    private IEnumerator HandleFlip()
    {
        isPaused = true;
        rb.linearVelocity = Vector2.zero;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("enemyidle"))
        {
            animator.CrossFade("enemyidle", 0.1f);
        }

        yield return new WaitForSeconds(pauseBeforeFlip);

        Flip();

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("enemyrunning"))
        {
            animator.CrossFade("enemyrunning", 0.1f);
        }

        isPaused = false;
    }

    void Flip()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}