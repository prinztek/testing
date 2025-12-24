// using UnityEngine;

// public abstract class InputController : ScriptableObject
// {
//     public abstract float RetrieveMoveInput();
//     public abstract bool RetrieveJumpInput();
//     public abstract bool RetrieveJumpHoldInput();
//     public abstract bool RetrieveAttackInput();

//     // public abstract bool RetrieveRangedAttackInput();
//     // public abstract bool RetrieveAttackHoldInput();
//     public abstract bool RetrieveDropInput();
//     public abstract bool RetrieveToggleGrimoireInput();
// }

using UnityEngine;

public abstract class InputController : ScriptableObject
{
    // Cached per-frame input (read-only for gameplay)
    public float Move { get; protected set; }
    public bool JumpPressed { get; protected set; }
    public bool JumpHeld { get; protected set; }
    public bool AttackPressed { get; protected set; }
    public bool DropPressed { get; protected set; }
    public bool ToggleGrimoirePressed { get; protected set; }

    // Called once per frame
    public abstract void SampleInput();

    // Called once per frame AFTER gameplay logic
    public virtual void ResetFrameInput()
    {
        JumpPressed = false;
        AttackPressed = false;
        DropPressed = false;
        ToggleGrimoirePressed = false;
    }

    public bool RetrieveJumpInput() => JumpPressed;
    public bool RetrieveJumpHoldInput() => JumpHeld;
    public bool RetrieveAttackInput() => AttackPressed;
    public bool RetrieveDropInput() => DropPressed;
    public float RetrieveMoveInput() => Move;
    public bool RetrieveToggleGrimoireInput() => ToggleGrimoirePressed;
}

