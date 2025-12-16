using UnityEngine;

public class PlayerDropThrough : MonoBehaviour
{
    [SerializeField] internal InputController input = null;
    public float dropDisableTime = 0.25f;
    [SerializeField] private float dropCooldown = 1f; // 1 second cooldown
    [SerializeField] private Ground ground;
    private Collider2D col;
    private Rigidbody2D rb;

    private float lastDropTime = -1f;
    private bool dropping = false;
    void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // Block input if the game is paused / UI is clicked
        if (!InputGate.CanAcceptInput) return;
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

        // Drop input and check if on OneWayPlatform
        if (!dropping && input.RetrieveDropInput() && ground.OnOneWayPlatform)
        {
            if (Time.time - lastDropTime >= dropCooldown)
            {
                dropping = true;
                rb.linearVelocity = Vector2.zero;
                // Move slightly down to avoid immediate collision
                transform.position += Vector3.down;
                dropping = false; lastDropTime = Time.time;
            }
        }
    }
}
