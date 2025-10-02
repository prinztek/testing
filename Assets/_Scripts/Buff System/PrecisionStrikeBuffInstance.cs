using UnityEngine;

public class PrecisionStrikeBuffInstance : BuffInstance
{
    private int remainingCrits;

    public PrecisionStrikeBuffInstance(PrecisionStrikeBuffSO data, CharacterStats target)
        : base(data, target)
    {
        remainingCrits = data.guaranteedCrits;
    }

    public override void OnApply()
    {
        Debug.Log($"🎯 Precision Strike applied! {remainingCrits} guaranteed crits ready.");
        target.guaranteedCrits = remainingCrits;
    }

    public override void OnAttackHit(GameObject enemy)
    {
        if (remainingCrits > 0)
        {
            Debug.Log($"🎯 Crit! Hit {enemy.name} with Precision Strike.");
            remainingCrits--;
            target.guaranteedCrits = remainingCrits;

            if (remainingCrits <= 0)
            {
                remainingTime = 0f; // Expire the buff early
            }
        }
    }

    public override void OnExpire()
    {
        target.guaranteedCrits = 0;
        Debug.Log("🎯 Precision Strike expired.");
    }

    public override string GetUIDisplay()
    {
        return $"🎯 Precision Strike - {remainingCrits} crits";
    }
}
