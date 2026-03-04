using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject[] roomLocks; // Array to hold references to room lock GameObjects
    // Called when another collider enters the trigger area
    private bool isTriggered = false;
    [SerializeField] GameObject boss;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (isTriggered == true)
        {
            return;
        }

        // Check if the object that entered the trigger is the player
        if (col.CompareTag("Player"))
        {
            isTriggered = true;
            // wake up the boss
            AncientBoss ancientBoss = boss.GetComponent<AncientBoss>();

            if (ancientBoss != null)
            {
                ancientBoss.WakeUp();
            }

            // SkeletonSummonerBoss skeletonSummonerBoss = boss.GetComponent<SkeletonSummonerBoss>();
            // if (skeletonSummonerBoss != null)
            // {
            //     skeletonSummonerBoss.WakeUp();
            // }


            if (roomLocks.Length > 0)
            {
                for (int i = 0; i < roomLocks.Length; i++)
                {
                    roomLocks[i].SetActive(true); // Activate each room lock
                }
            }
            Debug.Log("Player has entered the boss trigger area!");
        }
    }
}
