using UnityEngine;

public class FactorialSkill : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private CharacterStats playerStats;

    void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
    }

    void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        playerStats = playerObj.GetComponent<CharacterStats>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.CompareTag("Player"))
        {
            UnlockSkill();
            Destroy(gameObject); // Remove the skill object from the scene
        }
    }

    void UnlockSkill()
    {
        playerStats?.UnlockSkill(SkillType.PermutationPulse); // Grant player experience points
    }
}
