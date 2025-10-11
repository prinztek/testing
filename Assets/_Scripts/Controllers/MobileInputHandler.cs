using UnityEngine;
using UnityEngine.UI;

public class MobileInputUIHandler : MonoBehaviour
{
    public CharacterStats characterStats; // assign in inspector
    public MobileInputController mobileController;
    public Joystick joystick; // use any floating/fixed joystick from asset store or custom
    public Button jumpButton;
    public Button attackButton;
    public Button swapWeaponButton; // ← single weapon swap button
    public Sprite meleeSprite; // assign in inspector
    public Sprite rangedSprite; // assign in inspector
    private Image attackButtonImage;

    void Awake()
    {
        // Ensure attackButton is assigned and has an Image component
        if (attackButton != null)
        {
            attackButtonImage = attackButton.GetComponent<Image>();
            if (attackButtonImage == null)
            {
                Debug.LogError("Attack Button does not have an Image component attached. Please add one.");
            }
        }
        else
        {
            Debug.LogError("Attack Button is not assigned in the Inspector.");
        }
    }

    void Start()
    {
        // You can also add an additional check here in case of any issues in the Inspector
        if (attackButtonImage == null)
        {
            Debug.LogError("Attack Button Image component is still null at Start. Make sure it's assigned.");
        }
    }
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

    public void OnSwapWeaponPressed()
    {
        // Swap attack mode (melee/ranged)
        characterStats.TryToggleAttackMode();

        // Update attack button sprite based on the new attack mode
        UpdateAttackButtonSprite();
    }

    public void onGrimoirePressed()
    {
        // Open the grimoire UI
        // Implement your grimoire opening logic here
    }

    private void UpdateAttackButtonSprite()
    {
        if (characterStats == null || attackButtonImage == null) return;

        switch (characterStats.currentAttackMode)
        {
            case CharacterStats.AttackMode.Melee:
                attackButtonImage.sprite = meleeSprite;
                break;
            case CharacterStats.AttackMode.Ranged:
                attackButtonImage.sprite = rangedSprite;
                break;
        }
    }
}
