using UnityEngine;

public class RatArcherEnemyAnimationEventRelay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private RatArcherEnemy ratArcherEnemy;

    private void Awake()
    {
        ratArcherEnemy ??= GetComponentInParent<RatArcherEnemy>();
    }
    public void OnShootArrow()
    {
        ratArcherEnemy?.OnShootArrow();
        // ExecuteScreenShakeForCrystalBarrage();
    }

    // public void ExecuteScreenShakeForCrystalBarrage()
    // {
    //     ancientBoss?.StartScreenshakeForAttacking();
    // }
}
