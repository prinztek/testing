using UnityEngine;

[CreateAssetMenu(fileName = "PlayerController", menuName = "InputController/PlayerController")]
public class KeyboardInputController : InputController
{
    public override float RetrieveMoveInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }
    public override bool RetrieveJumpInput()
    {
        return Input.GetButtonDown("Jump");
    }

    public override bool RetrieveJumpHoldInput()
    {
        return Input.GetButton("Jump");
    }

    public override bool RetrieveAttackInput()
    {
        return Input.GetButtonDown("Fire1");
    }

    // public override bool RetrieveAttackHoldInput()
    // {
    //     return Input.GetButton("Fire1");
    // }
}
