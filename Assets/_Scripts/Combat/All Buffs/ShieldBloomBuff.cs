using UnityEngine;

public class ShieldBloomBuff : Buff
{
    private int maxHits;
    private int hitsRemaining;

    public ShieldBloomBuff(float duration, int hitCount = 2) : base("Shield Bloom", duration)
    {
        maxHits = hitCount;
        hitsRemaining = hitCount;
    }

    public override void OnApply()
    {
        target.shieldHitsRemaining = hitsRemaining;
        target.EnableShieldVFX();
        Debug.Log($"🛡️ Shield Bloom applied! Absorbs {hitsRemaining} hits.");
    }

    public override void OnExpire()
    {
        target.shieldHitsRemaining = 0;
        target.DisableShieldVFX();
        Debug.Log("🛡️ Shield Bloom expired.");
    }
}
