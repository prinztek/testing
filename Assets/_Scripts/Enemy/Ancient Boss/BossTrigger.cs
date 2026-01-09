using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject[] roomLocks; // Array to hold references to room lock GameObjects
    // Called when another collider enters the trigger area

    [SerializeField] AncientBoss ancientBoss;
    void OnTriggerEnter2D(Collider2D col)
    {
        // Check if the object that entered the trigger is the player
        if (col.CompareTag("Player"))
        {
            // wake up the boss
            ancientBoss.WakeUp();

            for (int i = 0; i < roomLocks.Length; i++)
            {
                roomLocks[i].SetActive(true); // Activate each room lock
            }
            Debug.Log("Player has entered the boss trigger area!");
        }
    }
}
