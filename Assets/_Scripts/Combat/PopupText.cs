using UnityEngine;
using TMPro;
using DG.Tweening;

public class PopupText : MonoBehaviour
{
    public float moveUpDistance = 0.35f;
    public float moveDuration = 0.15f;
    public float fadeDelay = 0.1f;
    public float fadeDuration = 0.2f;

    TMP_Text text;
    Vector3 startPos;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        startPos = transform.position;
    }

    void OnEnable()
    {
        Play();
    }

    void Play()
    {
        transform.position = startPos;
        transform.localScale = Vector3.one;

        Color c = text.color;
        c.a = 1f;
        text.color = c;

        // Kill any existing tweens (important for pooling)
        transform.DOKill();
        text.DOKill();

        // Move up (quick, subtle)
        transform.DOMoveY(startPos.y + moveUpDistance, moveDuration)
            .SetEase(Ease.OutQuad);

        // Fade fast
        text.DOFade(0f, fadeDuration)
            .SetDelay(fadeDelay)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(gameObject));
    }
}
