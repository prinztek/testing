using UnityEngine;

public class GrimoireManager : MonoBehaviour
{
    public static GrimoireManager Instance { get; private set; }
    [SerializeField] private GameObject grimoireUI; // The modal canvas
    public RectTransform closeTab;
    public RectTransform questionTab;
    public RectTransform inventoryTab;
    public RectTransform craftingTab;
    public RectTransform calculatorTab;
    public RectTransform modulesTab;
    [SerializeField] public CanvasGroup canvasGroup;
    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Close the grimoire
    public void CloseGrimoire()
    {
        if (grimoireUI == null) return;

        UIManager.Instance.ToggleBook(false);
        Debug.Log("Grimoire closed.");
    }
}