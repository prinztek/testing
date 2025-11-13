using UnityEngine;
using UnityEngine.UI;

public class MobileInputUIHandler : MonoBehaviour
{
    public CharacterStats characterStats; // assign in inspector
    public InteractionTrigger playerInteraction;
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
    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;

        // 🔹 If player already exists when UI enables, connect immediately
        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            HandlePlayerSpawned(GameManager.Instance.CurrentPlayer);
        }
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        if (playerObj == null)
        {
            Debug.LogWarning("[InventoryUI] Player spawn event received, but player is null!");
            return;
        }

        characterStats = playerObj.GetComponent<CharacterStats>();

        if (characterStats == null)
        {
            Debug.LogWarning("[InventoryUI] Player components missing.");
            return;
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
        if (Mathf.Abs(raw) < joystick.DeadZone)
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


    public void OnGrimoirePressed()
    {
        mobileController.mobileToggleGrimoireInput = true;
    }


    public void OnSwapWeaponPressed()
    {
        if (characterStats == null)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
                characterStats = GameManager.Instance.CurrentPlayer.GetComponent<CharacterStats>();

            if (characterStats == null)
            {
                Debug.LogWarning("SwapWeaponPressed called but CharacterStats is null. Player not ready yet.");
                return;
            }
        }

        characterStats.TryToggleAttackMode();
        UpdateAttackButtonSprite();
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

    public void OnInteractHandPressed()
    {
        playerInteraction.TryInteract();
    }

    public void OnMenuPressed()
    {
        UIManager.Instance.ShowPauseMenu(true);
    }
}
