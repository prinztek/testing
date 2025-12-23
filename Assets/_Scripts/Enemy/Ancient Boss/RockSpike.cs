using System.Collections;
using UnityEngine;

public class RockSpike : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RockSpikeHitBox hitbox;

    public void Emerge()
    {
        animator.CrossFade("spikeEmerge", 0.05f);
        // Activate hitbox after a short delay matching emergence animation
        StartCoroutine(ActivateHitboxAfterDelay(0.1f));
    }

    private IEnumerator ActivateHitboxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hitbox.EnableHitbox();
    }

    public void Retract()
    {
        animator.CrossFade("spikeRetract", 0.05f);
        hitbox.DisableHitbox(); // immediately or after delay matching retract
    }

    public void SetFacing(bool facingRight)
    {
        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
