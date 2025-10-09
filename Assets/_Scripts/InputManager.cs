using UnityEngine;

public class InputManager : MonoBehaviour
{
    public InputController playerController;
    public InputController mobileController;
    public GameObject onScreenControlsUI;
    public GameObject player;

    void Start()
    {
        if (Application.isMobilePlatform)
        {
            onScreenControlsUI.SetActive(true);
            AssignInput(mobileController);
        }
        else
        {
            onScreenControlsUI.SetActive(false);
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
        }
    }
}

