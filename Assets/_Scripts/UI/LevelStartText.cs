using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LevelStartText : MonoBehaviour
{
    [SerializeField] private CharacterStats playerStats;
    [SerializeField] private PopupText levelStartTextPrefab;
    [SerializeField] private Coroutine levelStartCoroutine;

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
        StartCoroutine(StartLevelCoroutine());
    }

    private IEnumerator StartLevelCoroutine()
    {
        yield return new WaitForSeconds(1f); // Delay to ensure player has spawned
        OnLevelStart();
    }

    public void OnLevelStart()
    {
        PopupText popup = Instantiate(
            levelStartTextPrefab,
            transform.position + new Vector3(0, 1f, 0),
            Quaternion.identity,
            transform
        );

        popup.Setup("DEFEAT ALL ENEMIES");
    }
}
