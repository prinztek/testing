using UnityEngine;

public class CameraFolllowObject : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector2 offsetRight = new Vector2(1f, 0f);
    [SerializeField] private Vector2 offsetLeft = new Vector2(-1f, 0f);
    [SerializeField] private float smoothTime = 0.2f;

    private Vector3 velocity = Vector3.zero;
    private Move playerMoveScript;

    private void Start()
    {
        playerMoveScript = playerTransform.GetComponent<Move>();
    }

    private void LateUpdate()
    {
        Vector2 desiredOffset = playerMoveScript.FacingRight ? offsetRight : offsetLeft;

        // Only offset the X axis; keep Y exactly player's Y position
        Vector3 targetPosition = new Vector3(
            playerTransform.position.x + desiredOffset.x,
            playerTransform.position.y,
            transform.position.z // keep current camera Z (depth)
        );

        // Smoothly move to the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

}

// Note: This script assumes there is a "Move" script attached to the player GameObject
// that has a public boolean property "FacingRight" indicating the player's facing direction.

// UP: IS FIXED LERP
// DOWN: FASTER ON FALL OR SAME AS UP
// using UnityEngine;

// public class CameraFollowObject : MonoBehaviour
// {
//     [SerializeField] private Transform playerTransform;
//     [SerializeField] private Vector2 offsetRight = new Vector2(1f, 0f);
//     [SerializeField] private Vector2 offsetLeft = new Vector2(-1f, 0f);
//     [SerializeField] private float smoothTime = 0.2f;
//     [SerializeField] private float smoothTimeUp = 0.2f;    // slower when going up
//     [SerializeField] private float smoothTimeDown = 0.01f; // snappy when going down

//     private Vector3 velocity = Vector3.zero;
//     private Move playerMoveScript;
//     private Ground playerGroundScript;

//     private float previousPlayerY;

//     private void Start()
//     {
//         playerMoveScript = playerTransform.GetComponent<Move>();
//         playerGroundScript = playerTransform.GetComponent<Ground>();
//         previousPlayerY = playerTransform.position.y;
//     }

//     private void LateUpdate()
//     {
//         Vector2 desiredOffset = playerMoveScript.FacingRight ? offsetRight : offsetLeft;

//         float currentPlayerY = playerTransform.position.y;
//         float verticalVelocity = currentPlayerY - previousPlayerY;

//         float currentSmoothTimeY = verticalVelocity > 0 ? smoothTimeUp : smoothTimeDown;

//         Vector3 targetPosition = new Vector3(
//             playerTransform.position.x + desiredOffset.x,
//             currentPlayerY,
//             transform.position.z
//         );

//         // X uses regular smooth
//         float newX = Mathf.SmoothDamp(transform.position.x, targetPosition.x, ref velocity.x, smoothTime);

//         // Y uses different smooth time based on whether going up or down
//         float newY = Mathf.SmoothDamp(transform.position.y, targetPosition.y, ref velocity.y, currentSmoothTimeY);

//         transform.position = new Vector3(newX, newY, transform.position.z);

//         // Update for next frame
//         previousPlayerY = currentPlayerY;
//     }
// }

