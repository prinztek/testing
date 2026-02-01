public class HasteBuff : Buff
{
    private float moveBoost = 1.1f;
    private float attackBoost = 1.1f;

    public HasteBuff(float duration, float moveBoost = 1.5f, float attackBoost = 1.25f)
        : base("Haste", duration)
    {
        this.moveBoost = moveBoost;
        this.attackBoost = attackBoost;
    }

    public override void OnApply()
    {
        target.moveSpeedMultiplier = moveBoost;
        target.attackSpeedMultiplier = attackBoost;
        target.animationSpeedMultiplier = attackBoost;
    }

    public override void OnExpire()
    {
        // Do nothing reset via character stats
    }
}
