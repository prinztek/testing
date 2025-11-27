using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MobileInputUIHandler : MonoBehaviour
{
    public CharacterStats characterStats;
    public InteractionTrigger playerInteraction;
    public MobileInputController mobileController;
    public Joystick joystick;
    public Button jumpButton;
    public Button attackButton;
    public Button swapWeaponButton;
    public Sprite meleeSprite;
    public Sprite rangedSprite;

    private Image attackButtonImage;

    // ------------------------------------------------------
    // INITIALIZATION
    // ------------------------------------------------------

    private void Awake()
    {
        if (attackButton != null)
        {
            attackButtonImage = attackButton.GetComponent<Image>();
            if (attackButtonImage == null)
                Debug.LogError("Attack Button is missing an Image component.");
        }
        else
        {
            Debug.LogError("Attack Button reference is missing in Inspector.");
        }
    }

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;

        // Try linking immediately if player already exists
        TryAssignPlayer();
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private IEnumerator Start()
    {
        // Fallback: wait until player exists
        while (characterStats == null)
        {
            TryAssignPlayer();
            yield return null;
        }

        // Ensure UI icon matches player
        UpdateAttackButtonSprite();
    }

    // ------------------------------------------------------
    // PLAYER ASSIGNMENT
    // ------------------------------------------------------

    private void TryAssignPlayer()
    {
        if (characterStats != null) return;
        if (playerInteraction != null) return;

        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            HandlePlayerSpawned(GameManager.Instance.CurrentPlayer);
        }
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        if (playerObj == null)
        {
            Debug.LogWarning("[MobileUI] Spawn event received but playerObj was NULL.");
            return;
        }

        characterStats = playerObj.GetComponent<CharacterStats>();

        if (characterStats == null)
        {
            Debug.LogError("[MobileUI] Player is missing CharacterStats component.");
            return;
        }

        playerInteraction = playerObj.GetComponent<InteractionTrigger>();

        if (playerInteraction == null)
        {
            Debug.LogError("[MobileUI] Player is missing playerInteraction component.");
            return;
        }

        UpdateAttackButtonSprite();
    }

    // ------------------------------------------------------
    // UPDATE LOOP
    // ------------------------------------------------------

    private void Update()
    {
        if (mobileController == null) return;

        float raw = joystick.Horizontal;

        if (Mathf.Abs(raw) < joystick.DeadZone)
            mobileController.mobileMoveInput = 0f;
        else
            mobileController.mobileMoveInput = Mathf.Sign(raw);
    }

    // ------------------------------------------------------
    // UI BUTTON EVENTS
    // ------------------------------------------------------

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
            Debug.LogWarning("[MobileUI] SwapWeapon pressed but characterStats is NULL.");
            TryAssignPlayer();
            return;
        }

        characterStats.TryToggleAttackMode();
        UpdateAttackButtonSprite();
    }

    private void UpdateAttackButtonSprite()
    {
        if (characterStats == null || attackButtonImage == null)
            return;

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
        playerInteraction?.TryInteract();
    }

    public void OnMenuPressed()
    {
        UIManager.Instance.ShowPauseMenu(true);
    }
}
