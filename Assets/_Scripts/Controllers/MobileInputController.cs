using UnityEngine;

[CreateAssetMenu(fileName = "MobileController", menuName = "InputController/MobileController")]
public class MobileInputController : InputController
{
    [HideInInspector] public bool uiJumpPressed;
    [HideInInspector] public bool uiJumpHeld;
    [HideInInspector] public bool uiAttackPressed;
    [HideInInspector] public bool uiToggleGrimoirePressed;
    [HideInInspector] public float mobileMoveInput;
    public Joystick joystick;
    public float downThreshold = -0.1f;
    private float downBuffer = 0f;
    private const float downBufferTime = 0.3f;

    // [HideInInspector] public bool mobileJumpInput = false;
    // [HideInInspector] public bool mobileJumpHoldInput = false;
    // [HideInInspector] public bool mobileAttackInput = false;
    // [HideInInspector] public bool mobileToggleGrimoireInput = false;
    // [HideInInspector] public bool mobileDropInput = false;
    // public Joystick joystick;
    // public float downThreshold = -0.1f;
    // private float downBuffer = 0f;
    // private const float downBufferTime = 0.3f;

    // public override float RetrieveMoveInput()
    // {
    //     return mobileMoveInput;
    // }

    // public override bool RetrieveJumpInput()
    // {
    //     bool jump = mobileJumpInput;
    //     mobileJumpInput = false; // reset after read
    //     return jump;
    // }

    // public override bool RetrieveJumpHoldInput()
    // {
    //     return mobileJumpHoldInput;
    // }

    // public override bool RetrieveAttackInput()
    // {
    //     bool attack = mobileAttackInput;
    //     mobileAttackInput = false; // reset after read
    //     return attack;
    // }

    // public override bool RetrieveDropInput()
    // {
    //     // Check if joystick is pulled down
    //     bool pullingDown = joystick != null && joystick.Vertical < downThreshold;

    //     // Update DOWN buffer
    //     if (pullingDown)
    //         downBuffer = downBufferTime;
    //     else
    //         downBuffer -= Time.deltaTime;

    //     // Jump button pressed?
    //     // bool jumpPressed = mobileJumpInput;
    //     // mobileJumpInput = false;

    //     // Drop occurs when jump pressed during DOWN buffer window
    //     return mobileJumpInput && downBuffer > 0f;
    // }

    // public override bool RetrieveToggleGrimoireInput()
    // {
    //     bool toggle = mobileToggleGrimoireInput;
    //     mobileToggleGrimoireInput = false; // reset after being read
    //     return toggle;
    // }

    public override void SampleInput()
    {
        // Movement
        Move = mobileMoveInput;

        // Jump
        JumpPressed = uiJumpPressed;
        JumpHeld = uiJumpHeld;

        // Attack
        AttackPressed = uiAttackPressed;

        // Drop-through buffer
        bool pullingDown = joystick != null && joystick.Vertical < downThreshold;

        if (pullingDown)
            downBuffer = downBufferTime;
        else
            downBuffer -= Time.deltaTime;

        DropPressed = JumpPressed && downBuffer > 0f;

        // UI
        ToggleGrimoirePressed = uiToggleGrimoirePressed;
    }

    public override void ResetFrameInput()
    {
        base.ResetFrameInput();

        uiJumpPressed = false;
        uiAttackPressed = false;
        uiToggleGrimoirePressed = false;
    }
}
