using UnityEngine;

[CreateAssetMenu(fileName = "MobileController", menuName = "InputController/MobileController")]
public class MobileInputController : InputController
{
    [HideInInspector] public float mobileMoveInput = 0f;
    [HideInInspector] public bool mobileJumpInput = false;
    [HideInInspector] public bool mobileJumpHoldInput = false;
    [HideInInspector] public bool mobileAttackInput = false;

    public override float RetrieveMoveInput()
    {
        return mobileMoveInput;
    }

    public override bool RetrieveJumpInput()
    {
        bool jump = mobileJumpInput;
        mobileJumpInput = false; // reset after read
        return jump;
    }

    public override bool RetrieveJumpHoldInput()
    {
        return mobileJumpHoldInput;
    }

    public override bool RetrieveAttackInput()
    {
        bool attack = mobileAttackInput;
        mobileAttackInput = false; // reset after read
        return attack;
    }
}
