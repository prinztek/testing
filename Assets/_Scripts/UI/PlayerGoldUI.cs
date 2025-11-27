using TMPro;
using UnityEngine;

public class PlayerGoldUI : MonoBehaviour
{
    [Header("References")]
    public CharacterStats characterStats; // Reference to the CharacterStats script
    public PlayerInventory playerInventory;
    public TMP_Text goldCounterText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        // this is for getting the character stats to subscribe to death event
        characterStats = playerObj.GetComponent<CharacterStats>();
        if (characterStats == null)
        {
            Debug.LogError("CharacterStats not assigned to PlayerGoldUI!");
            return;
        }

        playerInventory = playerObj.GetComponent<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory not assigned to PlayerGoldUI!");
            return;
        }

        goldCounterText.text = playerInventory.Gold.ToString();
        playerInventory.OnGoldChanged += UpdateGoldCount;
        characterStats.OnDeathStarted += HandleDeath; // When character dies, hide the gold UI
    }

    private void UpdateGoldCount(int currentGold)
    {
        SetGoldCount(currentGold); // Update gold counter when player gold changes
    }

    public void SetGoldCount(int gold)
    {
        goldCounterText.text = gold.ToString();
    }

    private void HandleDeath()
    {
        Debug.Log("Character has died, hiding the health bar.");
        gameObject.SetActive(false); // Optionally hide the health bar on death
    }

}
