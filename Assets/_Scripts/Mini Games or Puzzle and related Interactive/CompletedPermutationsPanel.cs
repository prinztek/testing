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
    public void RemoveLastPermutation()
    {
        int childCount = contentParent.childCount;
        if (childCount > 0)
        {
            Transform lastChild = contentParent.GetChild(childCount - 1);
            Destroy(lastChild.gameObject);
        }
    }
}
