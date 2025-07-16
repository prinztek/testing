using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    private bool canExit = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canExit) return;

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Level Complete!");
            // Notify LevelManager

            FindFirstObjectByType<LevelManager>()?.OnLevelCompleted();

            canExit = false; // prevent multiple triggers
        }
    }
}
