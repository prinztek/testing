using TMPro;
using UnityEngine;
using DG.Tweening;

public class DamageTextPopup : MonoBehaviour
{
    public TextMeshProUGUI textMesh;

    [Header("Animation Settings")]
    public float floatDistance = 1f;          // How high to float up
    public float floatDuration = 0.5f;        // How fast to float
    public float fadeOutDuration = 0.3f;      // Fade out speed

    private Sequence animationSequence;

    public void SetText(string text, Color color, float size = 72f)
    {
        textMesh.text = text;
        textMesh.color = color;
        textMesh.fontSize = size;

        Animate();
    }

    private void Animate()
    {
        if (animationSequence != null) animationSequence.Kill();

        transform.localScale = Vector3.zero;

        animationSequence = DOTween.Sequence();

        // 1. Stretch tall (very exaggerated, very fast)
        animationSequence.Append(transform.DOScale(new Vector3(0.2f, 3.0f, 1f), 0.05f).SetEase(Ease.OutQuad));  // More extreme stretch

        // 2. Squash wide (even more exaggerated)
        animationSequence.Append(transform.DOScale(new Vector3(2.0f, 0.3f, 1f), 0.05f).SetEase(Ease.OutQuad));  // Bigger squash

        // 3. Bounce back past normal scale (overshoot with extra bounce)
        animationSequence.Append(transform.DOScale(new Vector3(1.25f, 1.25f, 1f), 0.12f).SetEase(Ease.OutElastic));  // Higher bounce

        // 4. Settle to normal scale
        animationSequence.Append(transform.DOScale(Vector3.one, 0.05f).SetEase(Ease.OutQuad));

        // 5. Float upwards (faster float)
        Vector3 floatTarget = transform.position + Vector3.up * floatDistance;
        animationSequence.Join(transform.DOMove(floatTarget, floatDuration * 0.75f).SetEase(Ease.OutQuad));  // Faster float

        // 6. Add a slight pause for impact before fading
        animationSequence.AppendInterval(0.05f);  // Short pause before fade

        // 7. Fade out near the end
        animationSequence.Append(textMesh.DOFade(0f, fadeOutDuration * 1.2f));  // Longer fade for more dramatic exit

        // 8. Destroy after fade-out is complete
        animationSequence.OnComplete(() => Destroy(gameObject));
    }

    private void OnDestroy()
    {
        if (animationSequence != null) animationSequence.Kill();
    }
}
