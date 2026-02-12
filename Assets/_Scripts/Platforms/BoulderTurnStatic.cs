using UnityEngine;

public class BoulderTurnStatic : MonoBehaviour
{
    public LayerMask groundLayer;   // Set this in Inspector

    private Rigidbody2D rb;
    private bool hasLanded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasLanded) return;

        // Check if the object we hit is in the ground layer
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            hasLanded = true;

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            rb.bodyType = RigidbodyType2D.Static;
        }
    }
}
