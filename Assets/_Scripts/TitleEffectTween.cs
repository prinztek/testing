using UnityEngine;
using DG.Tweening;

public class TitleEffectTween : MonoBehaviour
{
    private void Start()
    {
        Vector3 startPos = transform.position;
        transform.position += Vector3.up * 10.5f;

        transform.DOMove(startPos, 0.6f)
            .SetEase(Ease.OutCubic)
            .OnComplete(StartIdle);
    }

    void StartIdle()
    {
        transform.DOMoveY(transform.position.y + 0.2f, 2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
