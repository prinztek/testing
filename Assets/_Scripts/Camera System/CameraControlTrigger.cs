using UnityEngine;
using Unity.Cinemachine;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorObjects customInspectorObjects;
    private Collider2D _coll;
    private void Start()
    {
        _coll = GetComponent<Collider2D>();
    }
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (!collision.CompareTag("Player")) return;

    //     if (customInspectorObjects.SwapCameras &&
    //         customInspectorObjects.cameraOnLeft != null &&
    //         customInspectorObjects.cameraOnRight != null)
    //     {
    //         // Swap back
    //         customInspectorObjects.cameraOnLeft.Priority = 10;
    //         customInspectorObjects.cameraOnRight.Priority = 0;

    //         Debug.Log($"🎥 Swapped back to {customInspectorObjects.cameraOnLeft.name}");
    //     }
    // }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (customInspectorObjects == null) return;

        // Determine exit direction (optional but useful if you have left/right triggers)
        Vector2 exitDirection = (collision.transform.position - _coll.bounds.center).normalized;

        if (customInspectorObjects.SwapCameras && customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null)
        {

            // swap cameras 
            CameraManager.Instance.SwapCamera(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, exitDirection);
            Debug.Log($"🎥 Swapped from {customInspectorObjects.cameraOnLeft.name} → {customInspectorObjects.cameraOnRight.name} (dir: {exitDirection})");
        }
    }
}

[System.Serializable]
public class CustomInspectorObjects
{
    [Header("Camera Swap Settings")]
    public bool SwapCameras = false;
    public CinemachineCamera cameraOnLeft;
    public CinemachineCamera cameraOnRight;
}

