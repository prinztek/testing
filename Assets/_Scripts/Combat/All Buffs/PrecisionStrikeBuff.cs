using UnityEngine;

public class PrecisionStrikeBuff : Buff
{
    private int guaranteedCrits;
    private float multiplier;
    private int remainingCrits;

    public PrecisionStrikeBuff(float duration, float multiplier = 10, int crits = 2) : base("Precision Strike", duration)
    {
        this.multiplier = multiplier;
        guaranteedCrits = crits;
        remainingCrits = crits;
    }

    public override void OnApply()
    {
        // Debug.Log($"🎯 Precision Strike applied! {guaranteedCrits} guaranteed crits ready.");
        target.tempDamageMultiplier *= multiplier;
        target.guaranteedCrits = remainingCrits;
    }

    public override void OnAttackHit(GameObject enemy)
    {
        if (remainingCrits > 0)
        {
            // Debug.Log($"🎯 Crit! Hit {enemy.name} with Precision Strike.");
            remainingCrits--;
            target.guaranteedCrits = remainingCrits;

            if (remainingCrits <= 0)
            {
                remainingTime = 0f;
            }
        }
    }

    public override void OnExpire()
    {
        target.guaranteedCrits = 0;
        // Debug.Log("🎯 Precision Strike expired.");
    }
}
