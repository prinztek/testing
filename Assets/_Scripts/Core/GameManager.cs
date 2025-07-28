using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    // Ensures only one instance of GameManager exists in the scene
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindFirstObjectByType<GameManager>();
            return instance;
        }
    }

    [Header("Input Controllers")]
    public InputController playerController; // PC input
    public InputController mobileController; // Mobile input

    [Header("Player Object")]
    public GameObject player; // Drag your player GameObject here

    [Header("Onscreen Controls")]
    public GameObject onScreenControlsUI; // Drag your player Onscreen Controls UI here
    void Start()
    {
        // Runtime platform check is more reliable for your use case
        if (Application.isMobilePlatform)
        {
            onScreenControlsUI.SetActive(true); // Activate onscreen controls
            AssignInput(mobileController);
        }
        else
        {
            AssignInput(playerController);
        }
    }


    void AssignInput(InputController controller)
    {
        // Debug.Log("Assigning input controller: " + controller?.name);

        if (player == null)
        {
            // Debug.LogWarning("Player not assigned in GameManager.");
            return;
        }

        foreach (var comp in player.GetComponents<MonoBehaviour>())
        {
            // Debug.Log("Checking component: " + comp.GetType());

            if (comp is Move move)
            {
                move.input = controller;
                // Debug.Log("Assigned to Move");
            }
            if (comp is Jump jump)
            {
                jump.input = controller;
                // Debug.Log("Assigned to Jump");
            }
            if (comp is Attack attack)
            {
                attack.input = controller;
                // Debug.Log("Assigned to Attack");
            }
        }
    }

}
