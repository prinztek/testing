using UnityEngine;

public class BasicInteractableObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isInRange = false;
    public GameObject uiCanvas; // Reference to your UI canvas - assign in inspector - the window that pops up when interacting
    public GameObject stoneWallObject; // Reference to the stone wall object - or any gate to be lifted - assign in inspector
    public bool isAnswered = false; // To track if the interaction has been completed
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E)) // interact button - this needs a button for the onscreen controls ui
        {
            if (uiCanvas != null)
            {
                uiCanvas.SetActive(true); // Show your interaction canvas
                Debug.Log("Interacting with object.");
            }

            // stoneWallObject.GetComponent<StoneWall>().Lift();
            // isAnswered = true;
        }
    }

    // Called when the player triggers a collision
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to a "Player" 
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            // Debug.Log("Player entered");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = !isInRange;
            // Debug.Log("Player exited");
        }
    }
}
