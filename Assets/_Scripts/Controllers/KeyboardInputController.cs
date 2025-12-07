using UnityEngine;

[CreateAssetMenu(fileName = "PlayerController", menuName = "InputController/PlayerController")]
public class KeyboardInputController : InputController
{
    private float downBuffer = 0f;
    private const float downBufferTime = 0.2f;
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

    public override bool RetrieveDropInput()
    {
        bool down = Input.GetAxisRaw("Vertical") < -0.5f;

        // Refresh DOWN buffer if down is held
        if (down)
            downBuffer = downBufferTime;
        else
            downBuffer -= Time.deltaTime;

        bool jump = Input.GetButtonDown("Jump");

        // Drop if jump pressed while DOWN buffer active
        return jump && downBuffer > 0f;
    }


    public override bool RetrieveToggleGrimoireInput()
    {
        return Input.GetKeyDown(KeyCode.Tab);
    }
}
