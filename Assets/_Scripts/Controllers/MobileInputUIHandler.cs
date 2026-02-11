using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

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

    [Header("Interact Button Visuals")]
    [SerializeField] private Button interactButton;
    [SerializeField] private GameObject interactGlowImage;
    private Tween interactGlowTween;
    private Tween grimoireGlowTween;


    [Header("Grimoire Button Visuals")]
    [SerializeField] private Button grimoireButton;
    [SerializeField] private GameObject grimoireGlowImage;



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
        InteractionTrigger.OnInteractionAvailabilityChanged += HandleInteractState;
        LevelManager.OnGrimoireHintStateChanged += HandleGrimoireHint;

        // Try linking immediately if player already exists
        TryAssignPlayer();
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
        InteractionTrigger.OnInteractionAvailabilityChanged -= HandleInteractState;
        LevelManager.OnGrimoireHintStateChanged -= HandleGrimoireHint;
    }

    private void HandleInteractState(bool canInteract)
    {
        if (interactButton != null)
            interactButton.interactable = canInteract;

        if (interactGlowImage != null)
            interactGlowImage.SetActive(canInteract);

        if (canInteract)
            StartInteractGlowAnimation();
        else
            StopInteractGlowAnimation();
    }

    private void HandleGrimoireHint(bool show)
    {
        if (grimoireGlowImage != null)
            grimoireGlowImage.SetActive(show);

        if (show)
            StartGrimoireGlowAnimation();
        else
            StopGrimoireGlowAnimation();
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

        // Assign the joystick to MobileInputController at runtime
        if (mobileController != null)
        {
            mobileController.joystick = joystick;
        }
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
        mobileController.uiJumpPressed = true;
        mobileController.uiJumpHeld = true;
    }

    public void OnJumpReleased()
    {
        mobileController.uiJumpHeld = false;
    }

    public void OnAttackPressed()
    {
        mobileController.uiAttackPressed = true;
    }

    public void OnGrimoirePressed()
    {
        mobileController.uiToggleGrimoirePressed = true;

        // Notify LevelManager so it can start the cooldown (hide the highlight and prevent it from showing again for a while)
        LevelManager.Instance?.NotifyGrimoireOpened();
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

    private void StartInteractGlowAnimation()
    {
        if (interactGlowImage == null) return;

        // Prevent stacking tweens
        interactGlowTween?.Kill();

        interactGlowImage.transform.localScale = Vector3.one;

        interactGlowTween = interactGlowImage.transform
            .DOScale(1.15f, 0.4f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void StopInteractGlowAnimation()
    {
        if (interactGlowImage == null) return;

        interactGlowTween?.Kill();
        interactGlowImage.transform.localScale = Vector3.one;
    }

    private void StartGrimoireGlowAnimation()
    {
        if (grimoireGlowImage == null) return;

        // Prevent stacking tweens
        grimoireGlowTween?.Kill();

        grimoireGlowImage.transform.localScale = Vector3.one;

        grimoireGlowTween = grimoireGlowImage.transform
            .DOScale(1.15f, 0.4f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
    private void StopGrimoireGlowAnimation()
    {
        if (grimoireGlowImage == null) return;

        grimoireGlowTween?.Kill();
        grimoireGlowImage.transform.localScale = Vector3.one;
    }
}
