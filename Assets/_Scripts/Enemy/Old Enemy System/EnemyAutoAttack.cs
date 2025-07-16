using System.Collections;
using UnityEngine;

public class EnemyAutoAttack : MonoBehaviour
{
    public GameObject attackHitbox;
    public float attackRange = 1.5f;
    public float attackInterval = 2f;
    public float attackDuration = 0.3f;

    private Transform player;
    private bool isAttacking;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);

            if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                if (attackHitbox != null)
                {
                    attackHitbox.SetActive(true);
                    yield return new WaitForSeconds(attackDuration);
                    attackHitbox.SetActive(false);
                }
            }
        }
    }
}
