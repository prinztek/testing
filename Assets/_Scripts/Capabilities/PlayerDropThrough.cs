using UnityEngine;

public class PlayerDropThrough : MonoBehaviour
{
    [SerializeField] internal InputController input = null;
    public float dropDisableTime = 0.25f;
    [SerializeField] private Ground ground;

    private bool dropping = false;

    void Update()
    {
        // Block input if the game is paused / UI is clicked
        if (!InputGate.CanAcceptInput) return;
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

        // Drop input and check if on OneWayPlatform
        if (!dropping && input.RetrieveDropInput() && ground.OnOneWayPlatform)
        {
            dropping = true;

            // Move slightly down to avoid immediate collision
            transform.position += Vector3.down;

            // Optionally: temporarily disable collision with the platform here

            dropping = false;
        }
    }
}
