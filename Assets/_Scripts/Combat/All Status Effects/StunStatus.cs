using UnityEngine;

public class StunStatus : StatusEffect
{
    public StunStatus(float duration) : base("Stun", duration) { }

    public override void OnApply()
    {
        if (target != null)
        {
            target.canMove = false;
            target.canAttack = false;
            Debug.Log($"{target.name} is stunned for {remainingTime} seconds!");
            // Optional: spawn VFX for stun
        }
    }

    public override void OnExpire()
    {
        if (target != null)
        {
            target.canMove = true;
            target.canAttack = true;
            Debug.Log($"{target.name} is no longer stunned.");
            // Optional: remove VFX
        }
    }
}
