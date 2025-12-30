using UnityEngine;

public class LockedGate : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Entered");
            PlayerInventory playerInventory = col.gameObject.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                bool isUnlocked = playerInventory.UnlockGate();
                if (isUnlocked == true)
                {
                    Debug.Log("Gate unlocked");
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("You need a key to unlock the gate");
                }
            }
        }
    }
}
