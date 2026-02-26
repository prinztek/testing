using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AncientBossHealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider slider; // The slider component for the health bar
    public Image fill; // The fill image to display health
    public TMP_Text healthText;

    [Header("References")]
    public EnemyStats stats; // Reference to the EnemyStats script

    private void Start()
    {
        if (stats == null)
        {
            Debug.LogError("Stats not assigned to HealthBar!");
            return;
        }

        // Set the initial values of the health bar
        SetMaxHealth(stats.maxHealth); // Set max health
        SetHealth(stats.CurrentHealth); // Set the current health

        // Subscribe to health change events
        stats.OnHealthChanged += UpdateHealthBar;
        stats.OnDeath += HandleDeath; // When enemy dies, hide the health bar
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        if (stats != null)
        {
            stats.OnHealthChanged -= UpdateHealthBar;
            stats.OnDeath -= HandleDeath;
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
        this.gameObject.SetActive(false); // Hide the health bar on death
    }
}