using UnityEngine;

public class CompletedPermutationsPanel : MonoBehaviour
{
    [SerializeField] private Transform contentParent; // ScrollView > Content
    [SerializeField] private PermutationEntry permutationEntryPrefab;

    public void AddPermutation(string sequence)
    {
        // Create new row
        PermutationEntry entry = Instantiate(permutationEntryPrefab, contentParent);
        entry.Initialize(sequence);
    }

    public void Clear()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
    }
}
