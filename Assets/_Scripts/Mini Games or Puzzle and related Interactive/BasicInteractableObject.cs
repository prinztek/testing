using UnityEngine;

public class BasicInteractableObject : MonoBehaviour
{
    // BasicInteractableObject - an interactable that opens up a canvas with the mini game
    public GameObject uiCanvas; // Reference to your UI canvas - assign in inspector - the window that pops up when interacting
    public GameObject stoneWallObject; // Reference to the stone wall object - or any gate to be lifted - assign in inspector

    public void Interact()
    {
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(true); // Show your interaction canvas
            Debug.Log("Interacting with object.");
        }
    }

    public void CloseCanvas()
    {
        uiCanvas.SetActive(false);
    }
}
