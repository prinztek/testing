using UnityEngine;
using DG.Tweening;

public class StatusEffectManager : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Character or enemy to follow
    public Vector3 offset = new Vector3(0, 2f, 0);

    [Header("Buff Icon Settings")]
    public GameObject buffIconPrefab; // Prefab: parent (empty) + child (SpriteRenderer)
    public float revealDuration = 0.4f;
    public float floatAmplitude = 0.15f;
    public float floatSpeed = 1.5f;

    private GameObject activeIcon;
    private Tween floatTween;

    public void ShowBuffIcon(Buff buff)
    {
        if (target == null || buffIconPrefab == null)
        {
            Debug.LogWarning("StatusEffectManager: Missing target or prefab reference!");
            return;
        }

        // Clean up previous icon
        if (activeIcon != null)
        {
            Destroy(activeIcon);
            floatTween?.Kill();
        }

        // Spawn new buff icon at target position
        activeIcon = Instantiate(buffIconPrefab, target.position + offset, Quaternion.identity);

        // Find the SpriteRenderer inside the prefab (the child “Visual”)
        SpriteRenderer sr = activeIcon.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Sprite iconSprite = buff.GetIcon();
            if (iconSprite != null)
                sr.sprite = iconSprite;
            else
                Debug.LogWarning($"No icon found for buff: {buff.buffName}");
        }

        // Start invisible and small for pop-in animation
        activeIcon.transform.localScale = Vector3.zero;
        Color startColor = sr.color;
        sr.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // Animate reveal (scale + fade)
        Sequence s = DOTween.Sequence();
        s.Append(activeIcon.transform.DOScale(1f, revealDuration).SetEase(Ease.OutBack));
        s.Join(sr.DOFade(1f, revealDuration * 0.8f));

        // Floating animation
        floatTween = activeIcon.transform
            .DOLocalMoveY(activeIcon.transform.localPosition.y + floatAmplitude, floatSpeed)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void HideBuffIcon(float fadeOutDuration = 0.3f)
    {
        if (activeIcon == null) return;

        floatTween?.Kill();

        SpriteRenderer sr = activeIcon.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.DOFade(0f, fadeOutDuration);

        activeIcon.transform.DOScale(0f, fadeOutDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => Destroy(activeIcon));
    }

    void LateUpdate()
    {
        if (target == null || activeIcon == null) return;

        // Keep icon above the target
        activeIcon.transform.position = target.position + offset;

        // Optional: make it face the camera
        if (Camera.main != null)
        {
            activeIcon.transform.forward = Camera.main.transform.forward;
        }
    }
}
