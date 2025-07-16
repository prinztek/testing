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
    void Start()
    {
        Application.targetFrameRate = 120; // Set target frame rate to 120 FPS
    }
    //     void Start()
    //     {
    // #if UNITY_ANDROID || UNITY_IOS
    //         AssignInput(mobileController);
    // #else
    //             AssignInput(playerController);
    // #endif
    //     }

    //     void AssignInput(InputController controller)
    //     {
    //         if (player == null)
    //         {
    //             Debug.LogWarning("Player not assigned in GameManager.");
    //             return;
    //         }

    //         foreach (var comp in player.GetComponents<MonoBehaviour>())
    //         {
    //             if (comp is Move move) move.input = controller;
    //             if (comp is Jump jump) jump.input = controller;
    //             if (comp is Attack attack) attack.input = controller;
    //         }

    //         Debug.Log("✅ InputController assigned: " + controller.name);
    //     }
}
