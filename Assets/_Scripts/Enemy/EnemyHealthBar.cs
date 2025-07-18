using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider slider; // The slider component for the health bar
    public Image fill; // The fill image to display health

    [Header("References")]
    public EnemyStatsNew enemyStats; // Reference to the EnemyStatsNew script

    // We don't need RectTransform or EnemyStateMachine directly here for flipping
    // because the health bar's visual orientation should be independent.

    private void Start()
    {
        if (enemyStats == null)
        {
            Debug.LogError("EnemyStatsNew not assigned to HealthBar!");
            return;
        }

        // Set the initial values of the health bar from EnemyStatsNew
        SetMaxHealth(enemyStats.maxHealth); // Set max health
        SetHealth(enemyStats.CurrentHealth); // Set the current health immediately on start

        // Subscribe to the health change event using Action
        enemyStats.OnHealthChanged += SetHealth; // Using delegate for health change

    }

    // OnEnable and OnDisable for subscription management are good practice
    private void OnEnable()
    {
        if (enemyStats != null)
        {
            enemyStats.OnHealthChanged += SetHealth;
        }

        // SetMaxHealth(enemyStats.maxHealth);
        // SetHealth(enemyStats.CurrentHealth);
        // enemyStats.OnHealthChanged += SetHealth;
    }

    private void Update()
    {
        if (enemyStats != null)
        {
            // Continuously update the health bar when health changes
            SetHealth(enemyStats.CurrentHealth);
        }
    }
    private void OnDisable()
    {
        if (enemyStats != null)
        {
            enemyStats.OnHealthChanged -= SetHealth;
        }
    }

    // Set the max value for the health bar (called during initialization)
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health; // Set the slider's maximum value
        slider.value = health; // Set the initial slider value to max health
    }

    // Set the current value for the health bar
    public void SetHealth(int health)
    {
        slider.value = health; // Set the slider value to current health
    }

    // Optional: Handle the death event
    private void HandleDeath()
    {
        Debug.Log("Enemy has died, hiding the health bar.");
        gameObject.SetActive(false); // Optionally hide the health bar on death
    }
}