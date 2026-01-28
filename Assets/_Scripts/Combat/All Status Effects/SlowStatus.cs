using UnityEngine;

public class SlowStatus : StatusEffect
{
    private float slowMultiplier;

    public SlowStatus(float duration, float slowMultiplier)
        : base("Slow", duration)
    {
        this.slowMultiplier = slowMultiplier;
    }

    public override void OnApply()
    {
        target.moveSpeedMultiplier *= slowMultiplier;
    }

    public override void OnExpire()
    {
        target.moveSpeedMultiplier /= slowMultiplier;
    }
}
