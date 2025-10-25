using UnityEngine;

public class EnemyEnvironmentSensor : MonoBehaviour
{
    public Transform groundCheck;
    public Transform wallCheck;
    public float groundCheckDistance = 0.3f;
    public float wallCheckDistance = 0.3f;
    public LayerMask groundLayer;

    public LayerMask wallLayer;
    public bool FacingRight { get; private set; }

    private void Start()
    {
        // Start Direction Check
        StartDirectionCheck();
    }

    private void StartDirectionCheck()
    {
        // Set FacingRight based on the Y rotation at start
        FacingRight = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, 0f)) < 90f;
    }
    public bool IsGroundAhead(bool facingRight)
    {
        return Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
    }

    // public bool IsFacingWall(bool facingRight)
    // {
    //     Vector2 direction = facingRight ? Vector2.right : Vector2.left;
    //     return Physics2D.Raycast(
    //         wallCheck.position,
    //         direction,
    //         wallCheckDistance,
    //         groundLayer
    //     );
    // }

    // Check if facing a wall or obstacle (either ground or wall)
    public bool IsFacingWall(bool facingRight)
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;

        // Combine layers: detect both ground and wall
        LayerMask combinedMask = groundLayer | wallLayer;

        return Physics2D.Raycast(
            wallCheck.position,
            direction,
            wallCheckDistance,
            combinedMask
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