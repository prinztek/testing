using UnityEngine;

public class ColdInfuseBuff : Buff
{
    private int maxHits = 3;
    private int hitsRemaining;

    public ColdInfuseBuff(float duration, int maxHits = 3) : base("Cold Infuse", duration)
    {
        this.maxHits = maxHits;
        hitsRemaining = maxHits;
    }

    public override void OnApply()
    {
        Debug.Log("Cold Infuse applied: " + maxHits + " cold hits");
    }

    public override void OnAttackHit(GameObject enemy)
    {
        if (enemy.TryGetComponent(out EnemyStats enemyStats))
        {
            enemyStats.AddStatus(new SlowStatus(5, 0.2f));
        }

        --hitsRemaining;

        if (hitsRemaining <= 0)
        {
            remainingTime = 0f;
        }
    }

    public override void OnExpire()
    {
        Debug.Log("Cold Infuse expired.");
    }

    public override string GetUIDisplay()
    {
        return $"Cold Infuse - {hitsRemaining} hits";
    }
}
