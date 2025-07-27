using UnityEngine;
using DG.Tweening;

public class DropEffectTween : MonoBehaviour
{
    private void Start()
    {
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 1.5f, 0);
        transform.DOJump(transform.position + randomOffset, 1f, 1, 0.5f).SetEase(Ease.OutCubic);
    }
}
