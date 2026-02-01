using UnityEngine;

public class BashStatus : StatusEffect
{
    public BashStatus(float duration) : base("Bash", duration) { }

    public override void OnApply()
    {
        if (target != null)
        {
            // target.rb.velocity = Vector2.zero;
            target.canMove = false;
            Debug.Log($"{target.name} is bashed!");
            // Optional: knockback animation or VFX
        }
    }

    public override void OnExpire()
    {
        if (target != null)
        {
            target.canMove = true;
            Debug.Log($"{target.name} recovers from bash.");
        }
    }
}
