using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Hurt : MonoBehaviour
{
    // [Header("Knockback Settings")]
    // [SerializeField] private float knockbackForce = 10f;
    // [SerializeField] private float verticalForce = 4f;
    private CharacterStats stats;

    [Header("Hurt Settings")]
    [SerializeField] private float hurtDuration = 0.33f;
    [SerializeField] private AnimationHandler animationHandler;

    [Header("Flicker & Invincibility")]
    [SerializeField] private float flickerDuration = 1.0f;
    [SerializeField] private float flickerSpeed = 0.1f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Attack attack;

    private Rigidbody2D rb;
    private bool isHurt = false;
    private bool isInvincible = false;
    private float hurtTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>();
    }

    private void Update()
    {
        if (isHurt)
        {
            hurtTimer -= Time.deltaTime;
            if (hurtTimer <= 0f)
            {
                isHurt = false;
            }
        }
    }

    public void TriggerHurt(Vector2 attackerPosition)
    {
        if (stats.IsDead()) return;

        if (attack != null)
        {
            attack.CancelAttack();
        }

        if (isInvincible) return;

        isHurt = true;
        isInvincible = true;
        hurtTimer = hurtDuration;

        // Optional knockback (comment out if not needed)
        // Vector2 knockbackDir = (Vector2.left * Mathf.Sign(transform.position.x - attackerPosition.x));
        // knockbackDir.y = 1f;

        // rb.linearVelocity = Vector2.zero;
        // rb.AddForce(knockbackDir.normalized * knockbackForce + Vector2.up * verticalForce, ForceMode2D.Impulse);

        // Trigger hurt animation
        if (animationHandler != null)
        {
            float length = animationHandler.GetHurtAnimationLength();
            animationHandler.PlayHurtAnimation(length);
        }

        // Start flicker & invincibility
        StartCoroutine(FlickerAndInvincibility());
    }

    private IEnumerator FlickerAndInvincibility()
    {
        float elapsed = 0f;

        while (elapsed < flickerDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flickerSpeed);
            elapsed += flickerSpeed;
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
    }

    public bool IsHurt()
    {
        return isHurt;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
}
