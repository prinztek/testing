using UnityEngine;

public class SlowStatus : StatusEffect
{
    private float slowMultiplier;
    private float tickInterval;
    private float tickTimer;
    private int damagePerTick;

    public SlowStatus(float duration, float slowMultiplier, float tickInterval, float tickTimer, int damagePerTick = 0)
        : base("Slow", duration)
    {
        this.slowMultiplier = slowMultiplier;
        this.tickInterval = tickInterval;
        this.tickTimer = tickTimer;
        this.damagePerTick = damagePerTick;
    }

    public override void OnApply()
    {
        target.moveSpeedMultiplier *= slowMultiplier;
    }

    public override void OnExpire()
    {
        target.moveSpeedMultiplier /= slowMultiplier;
    }

    public override void OnTick(float deltaTime)
    {
        tickTimer += deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer -= tickInterval;
            target.TakeDamage(damagePerTick, target.transform.position, false, true);
        }
    }
}
