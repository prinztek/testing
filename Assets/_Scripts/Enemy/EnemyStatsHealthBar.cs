using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatsHealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider slider; // The slider component for the health bar
    public Image fill;    // The fill image for optional visuals

    [Header("Enemy Reference")]
    public EnemyStats enemyStats; // Reference to your existing EnemyStats script

    [Header("Positioning")]
    public Transform target;        // The enemy's transform to follow
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset above enemy

    private void Start()
    {
        if (enemyStats == null)
        {
            Debug.LogError("EnemyStats not assigned to HealthBar!");
            return;
        }

        // Initialize health bar
        SetMaxHealth(enemyStats.maxHealth);
        SetHealth(enemyStats.CurrentHealth);

        // Subscribe to events
        enemyStats.OnHealthChanged += SetHealth;
        enemyStats.OnDeath += HandleDeath;
    }

    private void OnEnable()
    {
        if (enemyStats != null)
        {
            enemyStats.OnHealthChanged += SetHealth;
            enemyStats.OnDeath += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (enemyStats != null)
        {
            enemyStats.OnHealthChanged -= SetHealth;
            enemyStats.OnDeath -= HandleDeath;
        }
    }

    private void Update()
    {
        if (enemyStats != null)
        {
            // Optional: keep updating in case events fail
            SetHealth(enemyStats.CurrentHealth);
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.identity; // keep upright in 2D
        }
    }

    private void HandleDeath()
    {
        Destroy(gameObject); // optional: add fade-out here
    }

    public void SetMaxHealth(int health)
    {
        if (slider != null)
        {
            slider.maxValue = health;
            slider.value = health;
        }
    }

    public void SetHealth(int health)
    {
        if (slider != null)
            slider.value = health;
    }
}
