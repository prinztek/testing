using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemyHealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider slider; // The slider component for the health bar
    public Image fill; // The fill image to display health
    public TMP_Text healthText;

    [Header("References")]
    public Boss2 boss; // Reference to the Boss2 script
    private void Start()
    {
        if (boss == null)
        {
            Debug.LogError("Boss not assigned to HealthBar!");
            return;
        }

        // Set the initial values of the health bar from Boss2
        SetMaxHealth(boss.maxHealth); // Set max health
        SetHealth(boss.CurrentHealth); // Set the current health

        // Subscribe to health change events
        boss.OnHealthChanged += UpdateHealthBar;
        boss.OnDeathStarted += HandleDeath; // When character dies, hide the health bar
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        if (boss != null)
        {
            boss.OnHealthChanged -= UpdateHealthBar;
            boss.OnDeathStarted -= HandleDeath;
        }
    }

    private void UpdateHealthBar(int currentHealth)
    {
        SetHealth(currentHealth); // Update health bar when health changes
    }

    private void UpdateHealthText(int current, int max)
    {
        if (healthText != null)
        {
            healthText.text = $"{current} / {max}";
        }
    }

    // Set the max value for the health bar (called during initialization)
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health; // Set the slider's maximum value
        slider.value = health; // Set the initial slider value to max health
        UpdateHealthText(health, (int)slider.maxValue);
    }

    // Set the current value for the health bar
    public void SetHealth(int health)
    {
        slider.value = health; // Set the slider value to current health
        UpdateHealthText(health, (int)slider.maxValue);
    }

    // Handle death event: Hide the health bar
    private void HandleDeath()
    {
        Debug.Log("Boss has died, hiding the health bar.");
        gameObject.SetActive(false); // Optionally hide the health bar on death
    }
}

