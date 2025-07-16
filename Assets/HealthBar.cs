using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider slider; // The slider component for the health bar
    public Image fill; // The fill image to display health

    [Header("References")]
    public CharacterStats characterStats; // Reference to the CharacterStats script

    private void Start()
    {
        if (characterStats == null)
        {
            Debug.LogError("CharacterStats not assigned to HealthBar!");
            return;
        }

        // Set the initial values of the health bar from CharacterStats
        SetMaxHealth(characterStats.maxHealth); // Set max health
        SetHealth(characterStats.CurrentHealth); // Set the current health

        // Optionally, subscribe to health changes if you need to update dynamically
        characterStats.OnDeath += HandleDeath; // When character dies, hide the health bar
    }

    private void OnEnable()
    {
        // Subscribe to health changes (Optional: for dynamic updates if required)
        if (characterStats != null)
        {
            characterStats.OnDeath += HandleDeath;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        if (characterStats != null)
        {
            characterStats.OnDeath -= HandleDeath;
        }
    }

    private void Update()
    {
        if (characterStats != null)
        {
            // Continuously update the health bar when health changes
            SetHealth(characterStats.CurrentHealth);
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
        Debug.Log("Character has died, hiding the health bar.");
        gameObject.SetActive(false); // Optionally hide the health bar on death
    }
}
