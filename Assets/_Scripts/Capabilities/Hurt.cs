using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Hurt : MonoBehaviour
{
    // Knockback force values
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float verticalForce = 2f;
    // private float verticalDamping = 0.1f; // Rate of vertical force decay
    [HideInInspector] public Vector2 lastHitDirection;

    private CharacterStats stats;

    [Header("Hurt Settings")]
    [SerializeField] private float hurtDuration = 0.33f;
    [SerializeField] private AnimationHandler animationHandler;

    [Header("Flicker & Invincibility")]
    [SerializeField] private float flickerDuration = 1.0f;
    [SerializeField] private float flickerSpeed = 0.1f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Attack attack;
    private Material originalMaterial;
    [SerializeField] private Material onDamageVfxMat;
    [SerializeField] private float flashDuration = 0.15f;
    private Coroutine onFlickerVfxCoroutine;

    private Rigidbody2D rb;
    private bool isHurt = false;
    private bool isInvincible = false;
    private float hurtTimer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
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
        if (stats.IsDead() || isInvincible) return;

        if (attack != null)
            attack.CancelAttack();

        isHurt = true;
        isInvincible = true;
        hurtTimer = hurtDuration;

        // 🧱 Reset movement before applying knockback
        rb.linearVelocity = Vector2.zero;

        // Determine direction from attacker to this object
        lastHitDirection = ((Vector2)transform.position - attackerPosition).normalized;

        // Add a small upward component
        Vector2 knockbackDir = (lastHitDirection + Vector2.up * 0.2f).normalized;

        // Use constant verticalForce for each knockback
        float appliedVerticalForce = verticalForce; // Do not change the field!
        Vector2 force = new Vector2(knockbackDir.x * knockbackForce, appliedVerticalForce);

        rb.AddForce(force, ForceMode2D.Impulse);

        // Play hurt animation
        if (animationHandler != null)
        {
            float length = animationHandler.GetHurtAnimationLength();
            animationHandler.PlayHurtAnimation(length);
        }

        // Start flicker and invincibility coroutine
        PlayOnFlickerVfx();
    }

    public void PlayOnFlickerVfx()
    {
        if (onFlickerVfxCoroutine != null)
        {
            StopCoroutine(onFlickerVfxCoroutine);
        }
        onFlickerVfxCoroutine = StartCoroutine(FlickerAndInvincibility());
    }
    // private IEnumerator FlickerAndInvincibility()
    // {
    //     float elapsed = 0f;

    //     while (elapsed < flickerDuration)
    //     {
    //         spriteRenderer.material = onDamageVfxMat;
    //         yield return new WaitForSeconds(flashDuration);
    //         elapsed += flashDuration;
    //     }

    //     spriteRenderer.material = originalMaterial;
    //     isInvincible = false;
    // }

    private IEnumerator FlickerAndInvincibility()
    {
        isInvincible = true;

        float elapsed = 0f;
        bool useWhite = true;

        while (elapsed < flickerDuration)
        {
            spriteRenderer.material = useWhite ? onDamageVfxMat : originalMaterial;
            useWhite = !useWhite;

            yield return new WaitForSeconds(0.08f);
            elapsed += 0.08f;
        }

        // make sure we end NORMAL
        spriteRenderer.material = originalMaterial;
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
