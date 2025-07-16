using UnityEngine;

public class Ground : MonoBehaviour
{
    public bool OnGround { get; private set; }
    public float Friction { get; private set; }

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private void Update()
    {
        OnGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RetrieveFriction(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        RetrieveFriction(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Friction = 0f;
    }

    private void RetrieveFriction(Collision2D collision)
    {
        if (collision.rigidbody == null) return;

        PhysicsMaterial2D mat = collision.rigidbody.sharedMaterial;
        Friction = mat != null ? mat.friction : 0f;
    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}


// using UnityEngine;


// public class Ground : MonoBehaviour
// {
//     public bool OnGround { get; private set; }
//     public float Friction { get; private set; }
//     [SerializeField] private Transform groundCheck;
//     [SerializeField] private LayerMask groundLayer;
//     private Vector2 _normal;
//     private PhysicsMaterial2D _material;

//     private void OnCollisionExit2D(Collision2D collision)
//     {
//         OnGround = false;
//         Friction = 0;
//     }

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         EvaluateCollision(collision);
//         RetrieveFriction(collision);
//     }

//     private void OnCollisionStay2D(Collision2D collision)
//     {
//         EvaluateCollision(collision);
//         RetrieveFriction(collision);
//     }

//     private void EvaluateCollision(Collision2D collision)
//     {
//         for (int i = 0; i < collision.contactCount; i++)
//         {
//             _normal = collision.GetContact(i).normal;
//             OnGround |= _normal.y >= 0.9f;
//         }
//     }

//     private bool IsGrounded()
//     {
//         return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
//     }

//     private void RetrieveFriction(Collision2D collision)
//     {
//         _material = collision.rigidbody.sharedMaterial;

//         Friction = 0;

//         if (_material != null)
//         {
//             Friction = _material.friction;
//         }
//     }

//     public bool GetOnGround()
//     {
//         return OnGround;
//     }

//     public float GetFriction()
//     {
//         return Friction;
//     }
// }
