using UnityEngine;

public class BeamHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Hurtbox"))
        {
            CharacterStats playerStats = collision.GetComponentInParent<CharacterStats>();

            playerStats?.TakeDamage(damage, transform.position);
        }
    }
}
