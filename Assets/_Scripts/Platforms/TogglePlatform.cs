using UnityEngine;
using DG.Tweening;
// A platform that can be activated to become solid
public class TogglePlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D platformCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Visual Feedback")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float inactiveAlpha = 0.25f;

    [Header("Start State")]
    [SerializeField] private bool startActive = false;

    private bool isActivated;

    private void Awake()
    {
        if (platformCollider == null)
            platformCollider = GetComponent<Collider2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        isActivated = startActive;
        ApplyState(immediate: true);
    }

    // =========================
    // PUBLIC API
    // =========================

    public void Activate()
    {
        if (isActivated) return;

        isActivated = true;
        ApplyState(true);
    }

    public bool IsActivated() => isActivated;

    // =========================
    // INTERNAL
    // =========================

    private void ApplyState(bool immediate = false)
    {
        platformCollider.enabled = isActivated;

        float targetAlpha = isActivated ? 1f : inactiveAlpha;

        if (immediate)
        {
            Color c = spriteRenderer.color;
            c.a = targetAlpha;
            spriteRenderer.color = c;
        }
        else
        {
            spriteRenderer
                .DOFade(targetAlpha, fadeDuration)
                .SetEase(Ease.OutQuad);
        }
    }
}
