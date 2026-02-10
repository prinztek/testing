using System;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    private BasicInteractableObject currentInteractable;
    private InfoBoard currentInfoBoard;
    private Switch switchLever;

    public static event Action<bool> OnInteractionAvailabilityChanged;
    [Header("Interaction Availability")]
    private bool canInteract;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void SetCanInteract(bool value)
    {
        if (canInteract == value) return;

        canInteract = value;
        OnInteractionAvailabilityChanged?.Invoke(canInteract);
    }

    public void TryInteract()
    {
        if (currentInteractable != null)
            currentInteractable.Interact();

        if (currentInfoBoard != null)
            currentInfoBoard.Interact();

        if (switchLever != null)
            switchLever.Toggle();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Interactable")) return;

        if (other.TryGetComponent(out BasicInteractableObject interactable))
        {
            currentInteractable = interactable;
            interactable.HighlightObject();
            SetCanInteract(true);
        }

        if (other.TryGetComponent(out InfoBoard info))
        {
            currentInfoBoard = info;
            info.HighlightObject();
            SetCanInteract(true);
        }

        if (other.TryGetComponent(out Switch lever))
        {
            switchLever = lever;
            lever.HighlightObject();
            SetCanInteract(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Interactable")) return;

        currentInteractable?.RemoveHighlightObject();
        currentInfoBoard?.RemoveHighlightObject();
        switchLever?.RemoveHighlightObject();

        currentInteractable = null;
        currentInfoBoard = null;
        switchLever = null;

        SetCanInteract(false);
    }
}

// Any object that uses BasicInteractableObject can be interacted with
// when the player is nearby and presses the interact key.
// InteractionTrigger checks what the player can interact with,
// highlights those objects, and runs their interaction when used.
// It also lets other systems know when an interaction is available. (UI Onscreen control)
