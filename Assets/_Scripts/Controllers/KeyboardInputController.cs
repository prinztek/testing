using UnityEngine;

[CreateAssetMenu(fileName = "KeyboardController", menuName = "InputController/Keyboard")]
public class KeyboardInputController : InputController
{
    private float downBuffer;
    private const float downBufferTime = 0.25f;

    public override void SampleInput()
    {
        // Movement
        Move = Input.GetAxisRaw("Horizontal");

        // Jump
        JumpPressed = Input.GetButtonDown("Jump");
        JumpHeld = Input.GetButton("Jump");

        // Attack
        AttackPressed = Input.GetButtonDown("Fire1");

        // Down input for one-way platforms
        bool pullingDown = Input.GetAxisRaw("Vertical") < -0.5f;

        if (pullingDown)
            downBuffer = downBufferTime;
        else
            downBuffer -= Time.deltaTime;

        // Drop-through logic
        DropPressed = JumpPressed && downBuffer > 0f;

        // UI
        ToggleGrimoirePressed = Input.GetKeyDown(KeyCode.Tab);
    }
}
