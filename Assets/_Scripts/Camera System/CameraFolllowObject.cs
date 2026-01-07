using Unity.Cinemachine;
using UnityEngine;

public class CameraFolllowObject : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera; // Reference to the Cinemachine camera
    private void Awake()
    {
        cinemachineCamera.enabled = false;
    }

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(GameObject player)
    {
        cinemachineCamera.Follow = player.transform;
        cinemachineCamera.enabled = true;

    }

    // private void HandlePlayerSpawned(GameObject player)
    // {
    //     cinemachineCamera.transform.position = new Vector3(
    //         player.transform.position.x,
    //         player.transform.position.y,
    //         cinemachineCamera.transform.position.z
    //     );

    //     cinemachineCamera.Follow = player.transform;
    //     cinemachineCamera.enabled = true;
    // }
}


