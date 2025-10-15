using UnityEngine;
using DG.Tweening;

public class MovingBlockTrap : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Vector3 _previousPosition;
    private Tween _moveTween;
    private Transform _target;

    private void Start()
    {
        _previousPosition = transform.position;
        _target = pointB;
        MoveToTarget(_target);
    }

    private void MoveToTarget(Transform target)
    {
        float distance = Vector2.Distance(transform.position, target.position);
        float duration = distance / speed;

        _moveTween?.Kill();

        _moveTween = transform.DOMove(target.position, duration)
            // Change to a different easing function for faster middle movement
            .SetEase(Ease.InOutQuad) // Or try Ease.InOutCubic for even more dramatic slowing
            .OnComplete(() =>
            {
                _target = target == pointA ? pointB : pointA;
                MoveToTarget(_target);
            });
    }
}
