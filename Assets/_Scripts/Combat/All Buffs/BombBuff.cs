using UnityEngine;

public class BombBuff : Buff
{
    private int maxHits;
    private int hitsRemaining;
    private float bombDelay; // delay before bomb explodes
    private int bombDamage;
    private GameObject bombVfxPrefab;

    public BombBuff(float duration, int maxHits = 3, float bombDelay = 3f, int bombDamage = 5)
        : base("Bomb Buff", duration)
    {
        this.maxHits = maxHits;
        hitsRemaining = maxHits;
        this.bombDelay = bombDelay;
        this.bombDamage = bombDamage;
    }

    public override void OnApply()
    {
        Debug.Log($"💣 BombBuff applied! Max Hits: {maxHits}");
    }

    public override void OnAttackHit(GameObject enemy)
    {
        if (enemy.TryGetComponent(out EnemyStats enemyStats))
        {
            // Apply BombStatus on the enemy
            enemyStats.AddStatus(new BombStatus(bombDamage, bombDelay));
            Debug.Log($"💥 BombStatus applied to {enemy.name}!");
        }

        hitsRemaining--;

        if (hitsRemaining <= 0)
        {
            remainingTime = 0f; // expire the buff
        }
    }

    public override void OnExpire()
    {
        Debug.Log("💣 BombBuff expired.");
    }

    public override string GetUIDisplay()
    {
        return $"💣 Bomb Buff - {hitsRemaining} hits";
    }
}
