using UnityEngine;

public class BarrelBomberEnemyAnimationEventRelay : MonoBehaviour
{
    [SerializeField] private BarrelBomberEnemy barrelBomberEnemy;

    void Awake()
    {
        barrelBomberEnemy ??= GetComponentInParent<BarrelBomberEnemy>();
    }
    public void ThrowBomb()
    {
        barrelBomberEnemy?.ThrowBomb();
    }

}
