using TMPro;
using UnityEngine;

public class PlayerGoldUI : MonoBehaviour
{
    public CharacterStats characterStats;
    public TMP_Text goldCounterText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (characterStats == null)
        {
            Debug.LogError("CharacterStats not assigned to HealthBar!");
            return;
        }

        goldCounterText.text = characterStats.gold.ToString();
        characterStats.OnGoldChanged += UpdateGoldCount;
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
