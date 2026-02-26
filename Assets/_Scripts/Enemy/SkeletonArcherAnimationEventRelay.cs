using UnityEngine;

public class SkeletonArcherAnimationEventRelay : MonoBehaviour
{
    [SerializeField] private RangeEnemy rangeEnemy;

    private void Awake()
    {
        rangeEnemy ??= GetComponentInParent<RangeEnemy>();
    }
    public void OnShootArrow()
    {
        rangeEnemy?.OnShootArrow();
        // ExecuteScreenShakeForCrystalBarrage();
    }
}
