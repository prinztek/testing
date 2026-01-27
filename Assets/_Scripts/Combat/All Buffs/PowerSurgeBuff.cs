using UnityEngine;

public class PowerSurgeBuff : Buff
{
    private float multiplier;

    public PowerSurgeBuff(float duration, float multiplier = 2f) : base("Power Surge", duration)
    {
        this.multiplier = multiplier;
    }

    public override void OnApply()
    {
        target.tempDamageMultiplier *= multiplier;
        Debug.Log($"💥 Power Surge applied! Damage multiplied by {multiplier}");
    }

    public override void OnExpire()
    {
        target.tempDamageMultiplier /= multiplier;
        Debug.Log("💥 Power Surge expired.");
    }


}
