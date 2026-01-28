using UnityEngine;

public class PermutationSkill : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private CharacterStats playerStats;
    [SerializeField] private PopupText factorialEnginePopupTextPrefab;

    [Header("Bobbing Animation")]
    [SerializeField] private float bobHeight = 0.25f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float rotationSpeed = 30f;

    private Vector3 startPosition;
    void Awake()
    {
        startPosition = transform.position;
    }
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

        // if the player already learned the skill        
        if (playerStats != null && playerStats.HasSkill(SkillType.PermutationEngine))
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        AnimateBobbing();
    }

    private void AnimateBobbing()
    {
        float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPosition + new Vector3(0f, yOffset, 0f);

        // Optional subtle rotation
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.CompareTag("Player"))
        {
            if (playerStats != null && playerStats.HasSkill(SkillType.PermutationEngine))
                return;

            UnlockSkill();
            Destroy(gameObject); // Remove the skill object from the scene
        }
    }

    void UnlockSkill()
    {
        playerStats?.UnlockSkill(SkillType.PermutationEngine); // Grant player experience points
        OnPermutationEngineAcquired();
    }

    public void OnPermutationEngineAcquired()
    {
        PopupText popup = Instantiate(
            factorialEnginePopupTextPrefab,
            transform.position + new Vector3(0, 1f, 0),
            Quaternion.identity
        );

        popup.Setup("Permutation Engine Acquired");
    }
}
