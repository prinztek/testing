using UnityEngine;
using UnityEngine.UI;

public class MobileInputUIHandler : MonoBehaviour
{
    CharacterStats characterStats; // assign in inspector
    public MobileInputController mobileController;
    public Joystick joystick; // use any floating/fixed joystick from asset store or custom
    public Button jumpButton;
    public Button attackButton;
    public Button swapWeaponButton; // ← single weapon swap button

    void Update()
    {
        if (mobileController == null) return;

        float raw = joystick.Horizontal;

        // Dead zone threshold
        if (Mathf.Abs(raw) < 0.2f)
            mobileController.mobileMoveInput = 0f;
        else
            mobileController.mobileMoveInput = Mathf.Sign(raw); // Snap to -1 or 1
    }

    public void OnJumpPressed()
    {
        mobileController.mobileJumpInput = true;
        mobileController.mobileJumpHoldInput = true;
    }

    public void OnJumpReleased()
    {
        mobileController.mobileJumpHoldInput = false;
    }

    public void OnAttackPressed()
    {
        mobileController.mobileAttackInput = true;
    }

    // public void OnSwapWeaponPressed()
    // {
    //     if (characterStats == null) return;

    //     var current = characterStats.currentWeapon;
    //     CharacterStats.WeaponType nextWeapon;

    //     switch (current)
    //     {
    //         case CharacterStats.WeaponType.Fist:
    //             nextWeapon = CharacterStats.WeaponType.Sword;
    //             break;
    //         case CharacterStats.WeaponType.Sword:
    //             nextWeapon = CharacterStats.WeaponType.Bow;
    //             break;
    //         default:
    //             nextWeapon = CharacterStats.WeaponType.Fist;
    //             break;
    //     }

    //     characterStats.ChangeWeapon(nextWeapon);
    // }
}
