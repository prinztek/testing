using UnityEngine;

public class ChargerEnemyAnimationEventRelay : MonoBehaviour
{
    ChargerEnemy chargerEnemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        chargerEnemy = GetComponentInParent<ChargerEnemy>();
    }

    void ApplyForwardForce()
    {
        chargerEnemy.ApplyForwardForce();
    }
}
