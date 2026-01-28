using UnityEngine;

public class BombStatus : StatusEffect
{
    private int explosionDamage;

    public BombStatus(int damage, float delay)
        : base("Bomb", delay)
    {
        explosionDamage = damage;
    }

    public override void OnApply()
    {
        Debug.Log("💣 Bomb attached");
    }

    public override void OnExpire()
    {
        target.TakeDamage(explosionDamage, target.transform.position, true);
        Debug.Log("💥 Bomb exploded");
    }
}
