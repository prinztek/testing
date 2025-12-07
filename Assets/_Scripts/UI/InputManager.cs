using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public InputController playerController;
    public InputController mobileController;
    public GameObject player;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
    }

    void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    void HandlePlayerSpawned(GameObject newPlayer)
    {
        player = newPlayer;
        AssignInput(Application.isMobilePlatform ? mobileController : playerController);
    }


    void Start()
    {
        if (Application.isMobilePlatform)
        {
            AssignInput(mobileController);
        }
        else
        {
            AssignInput(playerController);
        }
    }

    void AssignInput(InputController controller)
    {
        foreach (var comp in player.GetComponents<MonoBehaviour>())
        {
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

            if (comp is CharacterStats charStats)
            {
                charStats.input = controller;
                // Debug.Log("Assigned to CharacterStats");
            }

            if (comp is PlayerDropThrough dropThrough)
            {
                dropThrough.input = controller;
                // Debug.Log("Assigned to CharacterStats");
            }
        }
    }
}

