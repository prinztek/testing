using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    private BasicInteractableObject currentInteractable;
    private InfoBoard currentInfoBoard;

    private Switch switchLever;
    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    public void TryInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }

        if (currentInfoBoard != null)
        {
            currentInfoBoard.Interact();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it has the tag and the Interactable component
        if (other.CompareTag("Interactable") && other.TryGetComponent(out BasicInteractableObject interactable))
        {
            currentInteractable = interactable;
            currentInteractable.HighlightObject();
            // Debug.Log("Can interact with " + interactable.name);
        }

        if (other.CompareTag("Interactable") && other.TryGetComponent(out InfoBoard interactable2))
        {
            currentInfoBoard = interactable2;
            currentInfoBoard.HighlightObject();
            // Debug.Log("Can interact with " + interactable2.name);
        }

        if (other.CompareTag("Interactable") && other.TryGetComponent(out Switch lever))
        {
            switchLever = lever;
            switchLever.HighlightObject();
            // Debug.Log("Can interact with " + interactable2.name);
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Clear only if the same interactable
        if (currentInteractable != null)
        {
            currentInteractable.RemoveHighlightObject();
            currentInteractable = null;
            // Debug.Log("Left interaction range");
        }

        if (currentInfoBoard != null)
        {
            currentInfoBoard.RemoveHighlightObject();
            currentInfoBoard = null;
            // Debug.Log("Left interaction range of info board " + interactable2.name);
        }

        if (switchLever != null)
        {
            switchLever.RemoveHighlightObject();
            switchLever = null;
        }
    }
}
