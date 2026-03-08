using UnityEngine;

public class RangeEnemyAnimationEventRelay : MonoBehaviour
{
    [SerializeField] private RangeEnemy rangeEnemy;
    public void OnShootArrow()
    {
        rangeEnemy?.OnShootArrow();
        // ExecuteScreenShakeForCrystalBarrage();
    }

}
