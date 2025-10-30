using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    private BasicInteractableObject currentInteractable;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it has the tag and the Interactable component
        if (other.CompareTag("Interactable") && other.TryGetComponent(out BasicInteractableObject interactable))
        {
            currentInteractable = interactable;
            Debug.Log("Can interact with " + interactable.name);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Clear only if the same interactable
        if (other.CompareTag("Interactable") && other.GetComponent<BasicInteractableObject>() == currentInteractable)
        {
            currentInteractable = null;
            Debug.Log("Left interaction range");
        }
    }
}
