using UnityEngine;
using DG.Tweening;
public class MovableBlock : MonoBehaviour
{
    public enum MoveDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    [Header("Movement Settings")]
    [SerializeField] private MoveDirection direction;
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private Ease easeType = Ease.InOutSine;

    private bool hasMoved = false;

    public void Move()
    {
        if (hasMoved) return;

        Vector3 moveVector = GetDirectionVector();
        Vector3 targetPosition = transform.position + moveVector * moveDistance;

        hasMoved = true;

        transform.DOMove(targetPosition, moveDuration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                Debug.Log($"{gameObject.name} finished moving {direction}");
            });
    }

    private Vector3 GetDirectionVector()
    {
        switch (direction)
        {
            case MoveDirection.Left:
                return Vector3.left;
            case MoveDirection.Right:
                return Vector3.right;
            case MoveDirection.Up:
                return Vector3.up;
            case MoveDirection.Down:
                return Vector3.down;
            default:
                return Vector3.zero;
        }
    }
}
