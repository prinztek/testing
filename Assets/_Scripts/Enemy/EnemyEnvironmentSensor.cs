using UnityEngine;

public class EnemyEnvironmentSensor : MonoBehaviour
{
    public Transform groundCheck;
    public Transform wallCheck;
    public float groundCheckDistance = 0.3f;
    public float wallCheckDistance = 0.3f;
    public LayerMask groundLayer;

    public bool IsGroundAhead(bool facingRight)
    {
        return Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
    }

    public bool IsFacingWall(bool facingRight)
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(
            wallCheck.position,
            direction,
            wallCheckDistance,
            groundLayer
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
            Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);

        if (wallCheck)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * wallCheckDistance);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.left * wallCheckDistance);
        }
    }
}