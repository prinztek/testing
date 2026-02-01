using UnityEngine;

public class BurnStatus : StatusEffect
{
    private float tickInterval;
    private float tickTimer;
    private int damagePerTick;

    public BurnStatus(int damage, float duration, float interval)
        : base("Burn", duration)
    {
        damagePerTick = damage;
        tickInterval = interval;
    }

    public override void OnApply()
    {
        Debug.Log("Burn applied");
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

    public override void OnExpire()
    {
        Debug.Log("Burn expired");
    }
}
