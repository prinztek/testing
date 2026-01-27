using UnityEngine;

public class HasteBuff : Buff
{
    private float moveSpeedBoost;
    private float attackSpeedBoost;

    public HasteBuff(float duration, float moveBoost = 5f, float attackBoost = 5f)
        : base("Haste", duration)
    {
        this.moveSpeedBoost = moveBoost;
        this.attackSpeedBoost = attackBoost;
    }

    public override void OnApply()
    {
        target.moveSpeedMultiplier *= moveSpeedBoost;
        target.attackSpeedMultiplier *= attackSpeedBoost;
        // Debug.Log($"⚡ Haste applied! Speed x{moveSpeedBoost}, Attack Speed x{attackSpeedBoost}");
    }

    public override void OnExpire()
    {
        target.moveSpeedMultiplier /= moveSpeedBoost;
        target.attackSpeedMultiplier /= attackSpeedBoost;
        // Debug.Log("⚡ Haste expired.");
    }
}
