using UnityEngine;

public class FireInfuseBuff : Buff
{
    private int maxHits = 3;
    private int hitsRemaining;

    public FireInfuseBuff(float duration, int maxHits = 3) : base("Fire Infuse", duration)
    {
        this.maxHits = maxHits;
        hitsRemaining = maxHits;
    }

    public override void OnApply()
    {
        Debug.Log("🔥 Fire Infuse applied: " + maxHits + " fiery hits");
    }

    public override void OnAttackHit(GameObject enemy)
    {
        if (enemy.TryGetComponent(out EnemyDummy dummy))
        {
            // ✅ Apply DoT
            dummy.ApplyDot(damagePerTick: 3, duration: 2f, interval: 0.5f, attackerPosition: Vector2.zero);
        }

        hitsRemaining--;
        // Debug.Log($"🔥 Burning DoT applied to {enemy.name}! Hits left: {hitsRemaining}");

        if (hitsRemaining <= 0)
        {
            remainingTime = 0f; // Expire the buff
        }
    }

    public override void OnExpire()
    {
        Debug.Log("🔥 Fire Infuse expired.");
    }

    public override string GetUIDisplay()
    {
        return $"🔥 Fire Infuse - {hitsRemaining} hits";
    }

}
