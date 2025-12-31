using TMPro;
using UnityEngine;

public class PermutationEntry : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform runesContainer; // HorizontalLayoutGroup parent
    [SerializeField] private GameObject displayRunePrefab; // Prefab of DisplayRune

    // Initialize this entry with a sequence string, e.g., "ABC"
    public void Initialize(string sequence)
    {
        // Clear existing runes in case of reuse
        foreach (Transform child in runesContainer)
        {
            Destroy(child.gameObject);
        }

        // Create a DisplayRune for each character
        foreach (char c in sequence)
        {
            GameObject runeGO = Instantiate(displayRunePrefab, runesContainer);
            TMP_Text textComponent = runeGO.GetComponentInChildren<TMP_Text>();
            if (textComponent != null)
                textComponent.text = c.ToString();
        }
    }
}
