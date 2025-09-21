using TMPro;
using UnityEngine;

public class PlayerGoldUI : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public TMP_Text goldCounterText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (playerInventory == null)
        {
            Debug.LogError("playerInventory not assigned to PlayerGoldUI!");
            return;
        }

        goldCounterText.text = playerInventory.gold.ToString();
        playerInventory.OnGoldChanged += UpdateGoldCount;
    }


    private void UpdateGoldCount(int currentGold)
    {
        SetGoldCount(currentGold); // Update gold counter when player gold changes
    }

    public void SetGoldCount(int gold)
    {
        goldCounterText.text = gold.ToString();
    }

}
