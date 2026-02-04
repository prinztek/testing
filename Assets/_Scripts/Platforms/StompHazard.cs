using UnityEngine;
using DG.Tweening;

public class StompHazard : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float stompDistance = 2.5f;
    [SerializeField] private float stompDuration = 0.18f;
    [SerializeField] private float returnDuration = 0.5f;

    [Header("Timing")]
    [SerializeField] private float waitAtTop = 1.2f;
    [SerializeField] private float waitAtBottom = 0.4f;

    [Header("References")]
    [SerializeField] private Collider2D hazardCollider;

    private Sequence stompSequence;
    private Vector3 startPosition;
    private bool isActive;

    private void Awake()
    {
        startPosition = transform.position;
    }

    // =========================
    // PUBLIC API
    // =========================

    public void Activate()
    {
        if (isActive) return;

        if (startPosition == Vector3.zero)
        {
            startPosition = transform.position;
        }

        isActive = true;
        CreateSequence();
    }

    public void Deactivate()
    {
        if (!isActive) return;

        isActive = false;

        stompSequence?.Kill();

        transform.DOMove(startPosition, 0.25f)
                 .SetEase(Ease.OutQuad);
    }

    // =========================
    // INTERNAL
    // =========================

    private void CreateSequence()
    {
        Vector3 bottomPosition = startPosition + Vector3.down * stompDistance;

        stompSequence = DOTween.Sequence();

        stompSequence
            .AppendInterval(waitAtTop)
            .Append(transform.DOMove(bottomPosition, stompDuration)
                .SetEase(Ease.InQuad))
            .AppendInterval(waitAtBottom)
            .Append(transform.DOMove(startPosition, returnDuration)
                .SetEase(Ease.OutQuad))
            .SetLoops(-1);
    }

    private void OnDestroy()
    {
        stompSequence?.Kill();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * stompDistance);
    }
#endif
}
