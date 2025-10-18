// using UnityEngine;

// public class CameraFolllowObject : MonoBehaviour
// {
//     [SerializeField] private Transform _playerTransform; // The object to follow

//     [SerializeField] private float _flipYRotataionTime = 0.5f;

//     private Move _player;

//     public bool isFacingRight;
//     void Start()
//     {
//         _player = _playerTransform.GetComponent<Move>();
//         isFacingRight = _player.FacingRight;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // make the cameraFollowObject follow the player's position
//         transform.position = _playerTransform.position;
//     }

//     private float DetermineEndRotationY()
//     {
//         isFacingRight = !isFacingRight;

//         if (isFacingRight)
//         {
//             return 180f;
//         }
//         else
//         {
//             return 0f;
//         }
//     }

//     public void CallTurn()
//     {
//         LeanTween.rotateY(gameObject, DetermineEndRotationY(), _flipYRotataionTime).setEase(LeanTweenType.easeInOutSine);
//     }

// }
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
