using UnityEngine;

public class RatArcherEnemyAnimationEventRelay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private ArcherEnemy archerEnemy;

    private void Awake()
    {
        archerEnemy ??= GetComponentInParent<ArcherEnemy>();
    }
    public void OnShootArrow()
    {
        archerEnemy?.OnShootArrow();
        // ExecuteScreenShakeForCrystalBarrage();
    }

    // public void ExecuteScreenShakeForCrystalBarrage()
    // {
    //     ancientBoss?.StartScreenshakeForAttacking();
    // }
}
