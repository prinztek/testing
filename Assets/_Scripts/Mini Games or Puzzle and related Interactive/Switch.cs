using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private RisingPlatform[] targets; // Platforms or other targets
    private bool isOn;
    private bool playerInRange;
    [SerializeField] private GameObject visualContext;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Toggle();
        }
    }

    void Toggle()
    {
        isOn = !isOn;

        foreach (RisingPlatform target in targets)
        {
            target.SetRaised(isOn);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            visualContext.SetActive(true);
        }


    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            visualContext.SetActive(false);
        }
    }
}
