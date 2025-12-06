using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    private CharacterStats characterStats;
    public Slider slider;
    public Image fill;
    public TMP_Text healthText;

    private void OnEnable()
    {
        // Subscribe to player spawn event
        GameManager.OnPlayerSpawned += SetPlayer;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= SetPlayer;
        UnsubscribeFromCharacter();
    }

    public void SetPlayer(GameObject playerObj)
    {
        UnsubscribeFromCharacter(); // unsubscribe from old player

        characterStats = playerObj.GetComponent<CharacterStats>();
        if (characterStats == null)
        {
            Debug.LogError("CharacterStats not found on player!");
            return;
        }

        // Set initial health
        SetMaxHealth(characterStats.maxHealth);
        SetHealth(characterStats.CurrentHealth);

        // Subscribe to events
        characterStats.OnHealthChanged += UpdateHealthBar;
        characterStats.OnDeathStarted += HandleDeath;
    }

    private void UnsubscribeFromCharacter()
    {
        if (characterStats == null) return;

        characterStats.OnHealthChanged -= UpdateHealthBar;
        characterStats.OnDeathStarted -= HandleDeath;
        characterStats = null;
    }

    private void UpdateHealthBar(int currentHealth)
    {
        SetHealth(currentHealth);
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        UpdateHealthText(health, health);
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        UpdateHealthText(health, (int)slider.maxValue);
    }

    private void UpdateHealthText(int current, int max)
    {
        if (healthText != null)
            healthText.text = $"{current} / {max}";
    }

    private void HandleDeath()
    {
        // Optionally hide health bar
        // gameObject.SetActive(false);
    }
}
