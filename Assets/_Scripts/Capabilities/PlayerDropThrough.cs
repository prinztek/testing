using UnityEngine;

public class PlayerDropThrough : MonoBehaviour
{
    [SerializeField] internal InputController input = null;
    public float dropDisableTime = 0.25f;

    private Collider2D col;
    private bool dropping = false;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!dropping && input.RetrieveDropInput())
        {
            dropping = true;
            transform.position += Vector3.down;
            dropping = false;
        }
    }

    private bool IsOnDropPlatform()
    {
        // Check for collision with a platform underneath
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f);
        if (hit.collider != null && hit.collider.CompareTag("dropPlatformTag"))
        {
            return true;
        }
        return false;
    }
}
