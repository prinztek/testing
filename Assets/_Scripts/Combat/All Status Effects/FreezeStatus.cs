using UnityEngine;

public class FreezeStatus : StatusEffect
{
    public FreezeStatus(float duration) : base("Freeze", duration) { }

    public override void OnApply()
    {
        // if (target.fsm != null)
        //     target.fsm.enabled = false;
    }

    public override void OnExpire()
    {
        // if (target.fsm != null)
        //     target.fsm.enabled = true;
    }
}
