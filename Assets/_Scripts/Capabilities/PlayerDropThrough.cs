using System.Collections;
using UnityEngine;

public class PlayerDropThrough : MonoBehaviour
{
    [SerializeField] internal InputController input = null;
    public float dropDisableTime = 0.5f;
    [SerializeField] private float dropCooldown = 0.5f; // 1 second cooldown
    [SerializeField] private Ground ground;
    private Collider2D col;
    private float lastDropTime = -1f;
    private bool dropping = false;
    void Start()
    {
        col = GetComponent<Collider2D>();
    }
    void Update()
    {
        // Block input if the game is paused / UI is clicked
        if (!InputGate.CanAcceptInput) return;
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

        // Drop input and check if on OneWayPlatform
        // using coroutine
        if (!dropping && input.RetrieveDropInput() && ground.OnOneWayPlatform && Time.time - lastDropTime >= dropCooldown)
        {
            StartCoroutine(Drop());
            lastDropTime = Time.time;
        }
    }

    public IEnumerator Drop()
    {
        Debug.Log("Dropping Down");
        dropping = true;

        Collider2D platformCol = ground.OneWayPlatformCollider;
        if (platformCol == null)
        {
            dropping = false;
            yield break;
        }

        Physics2D.IgnoreCollision(platformCol, col, true);
        // Wait for a short time in seconds
        yield return new WaitForSeconds(dropDisableTime);
        Physics2D.IgnoreCollision(platformCol, col, false);
        dropping = false;
    }
}
