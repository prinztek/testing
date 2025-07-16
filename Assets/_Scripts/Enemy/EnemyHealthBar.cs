using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider slider; // The slider component for the health bar
    public Image fill; // The fill image to display health

    [Header("References")]
    public EnemyStatsNew enemyStats; // Reference to the EnemyStatsNew script
    private RectTransform rectTransform;
    public EnemyStateMachine enemyStateMachine; // Reference to the EnemyStateMachine script
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

    private void OnEnable()
    {
        // Subscribe to health changes
        if (enemyStats != null)
        {
            enemyStats.OnHealthChanged += SetHealth;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        if (enemyStats != null)
        {
            enemyStats.OnHealthChanged -= SetHealth;
        }
    }

    private void Update()
    {
        if (enemyStats != null)
        {
            // Continuously update the health bar when health changes
            SetHealth(enemyStats.CurrentHealth);
            rectTransform = GetComponent<RectTransform>();
            // Prevent health bar from flipping

            // Update the RectTransform to match the enemy's position
            rectTransform = GetComponent<RectTransform>();

            // Prevent health bar from flipping
            if (enemyStateMachine.isFacingRight)
            {
                // Don't flip the health bar at all, keep it facing the same direction (1f, 1f)
                rectTransform.localScale = new Vector2(1f, 1f);  // Always face right (no flip)
            }
            else
            {
                // Flip the health bar when the enemy is facing left (-1f, 1f)
                rectTransform.localScale = new Vector2(-1f, 1f); // Flip to face left
            }
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
