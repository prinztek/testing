using UnityEngine;

[CreateAssetMenu(fileName = "PrecisionStrikeBuffSO", menuName = "Buffs/Precision Strike")]
public class PrecisionStrikeBuffSO : ScriptableObjectBuff
{
    public int guaranteedCrits = 2;

    public override BuffInstance CreateInstance(CharacterStats target)
    {
        return new PrecisionStrikeBuffInstance(this, target);
    }
}
