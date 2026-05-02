using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer visual;

    [Header("Colors")]
    [SerializeField] private Color lockedColor = Color.red;
    [SerializeField] private Color unlockedColor = Color.green;
    private bool unlocked = false;
    private bool canExit = true;
    private void Awake()
    {
        if (!visual)
            visual = GetComponentInChildren<SpriteRenderer>();

        SetLocked();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!unlocked) return;

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Level Complete!");
            // LevelManager.Instance.OnLevelCompleted();

            // check if we're in a tutorial level and trigger tutorial completion if so
            if (TutorialLevelManager.Instance != null)
            {
                TutorialLevelManager.Instance.OnLevelCompleted();
            }
            else
            {
                LevelManager.Instance.OnLevelCompleted();
            }
        }
    }

    public void Unlock()
    {
        unlocked = true;
        visual.color = unlockedColor;
    }

    private void SetLocked()
    {
        unlocked = false;
        visual.color = lockedColor;
    }
}
