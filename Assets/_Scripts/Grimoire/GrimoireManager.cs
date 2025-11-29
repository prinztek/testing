using UnityEngine;

public class GrimoireManager : MonoBehaviour
{
    [SerializeField] private GameObject grimoireUI; // The modal canvas

    // Close the grimoire
    public void CloseGrimoire()
    {
        if (grimoireUI == null) return;
        UIManager.Instance.ToggleBook(false);
        Debug.Log("Grimoire closed.");
    }
}
