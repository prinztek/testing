using Unity.Cinemachine;
using UnityEngine;

public class CameraFolllowObject : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera; // Reference to the Cinemachine camera

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
    }



}


