using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject roomLock;
    // Called when another collider enters the trigger area
    void OnTriggerEnter2D(Collider2D col)
    {
        // Check if the object that entered the trigger is the player
        if (col.CompareTag("Player"))
        {
            roomLock.SetActive(true);
            Debug.Log("Player has entered the boss trigger area!");
        }
    }
}
