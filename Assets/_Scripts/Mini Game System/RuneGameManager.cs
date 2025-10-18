using System.Collections.Generic;
using UnityEngine;

public class RuneGameManager : MonoBehaviour
{
    public List<string> correctSequences = new List<string>(); // Will hold the permutations or a single answer // e.g., {"ABC", "ACB", "BAC", "BCA", "CAB", "CBA"}
    public List<RuneSlot> runeSlots; // Assign in Inspector

    // This will hold either a single sequence or multiple sequences
    public List<string> userSequences = new List<string>(); // { "ABC", "ACB", "BAC", "BCA", "CAB", "CBA" }

    public string singleCorrectSequence = "ABC"; // Default single correct sequence

    public bool useMultipleAnswers = false; // Toggle this to switch between a single or multiple answers

    public bool isDistinct = false; // If true, only distinct sequences are considered correct


    [Tooltip("Assign the parent panel that holds all the rune GameObjects as children")]
    public GameObject runesParentPanel;

    private List<string> runes = new List<string>();

    public Canvas runeGameCanvas; // Reference to the Rune Game Canvas

    public StoneWall stoneWall; // Assign in Inspector


    void Start()
    {
        runes.Clear();

        if (runesParentPanel == null)
        {
            Debug.LogError("Runes Parent Panel is not assigned!");
            return;
        }

        // Get all Rune components from children of the panel
        Rune[] runeComponents = runesParentPanel.GetComponentsInChildren<Rune>();

        foreach (var runeComp in runeComponents)
        {
            if (runeComp != null)
                runes.Add(runeComp.runeID);
        }

        Debug.Log($"Found {runes.Count} runes from the panel.");

        // Clear previous data
        correctSequences.Clear();
        userSequences.Clear();

        // Generate correct sequences based on settings
        if (useMultipleAnswers)
        {
            if (isDistinct)
            {
                correctSequences = GenerateDistinctPermutations(runes);
            }
            else
            {
                correctSequences = GeneratePermutations(runes);
            }
        }
        else
        {
            correctSequences.Add(singleCorrectSequence);
        }

        // Debug print
        foreach (var seq in correctSequences)
            Debug.Log("Correct sequence: " + seq);
    }


    // Generates all permutations of a list of strings
    List<string> GeneratePermutations(List<string> list)
    {
        List<string> permutations = new List<string>();
        Permute(list, 0, permutations);
        return permutations;
    }

    // Helper function to generate permutations recursively
    void Permute(List<string> list, int startIndex, List<string> result)
    {
        if (startIndex == list.Count - 1)
        {
            // Join the current list of runes into a single string and add to result
            result.Add(string.Join("", list.ToArray()));
            return;
        }

        for (int i = startIndex; i < list.Count; i++)
        {
            // Swap the elements to create a new permutation
            Swap(list, startIndex, i);

            // Recursively permute the rest of the list
            Permute(list, startIndex + 1, result);

            // Swap back to undo the previous swap (backtrack)
            Swap(list, startIndex, i);
        }
    }

    // Swaps two elements in a list
    void Swap(List<string> list, int i, int j)
    {
        string temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }

    // Generate distinct permutations for a list that may have duplicates
    List<string> GenerateDistinctPermutations(List<string> items)
    {
        List<string> results = new List<string>();
        items.Sort(); // Sort to handle duplicates properly
        bool[] used = new bool[items.Count];
        List<string> current = new List<string>();

        Backtrack(items, used, current, results);
        return results;
    }

    void Backtrack(List<string> items, bool[] used, List<string> current, List<string> results)
    {
        if (current.Count == items.Count)
        {
            results.Add(string.Join("", current));
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (used[i]) continue;

            // Skip duplicates: if same as previous and previous is not used in this path
            if (i > 0 && items[i] == items[i - 1] && !used[i - 1]) continue;

            used[i] = true;
            current.Add(items[i]);

            Backtrack(items, used, current, results);

            used[i] = false;
            current.RemoveAt(current.Count - 1);
        }
    }

    // ******************************************************************************
    // Public methods to be called by UI buttons or other scripts
    public void OnSubmit()
    {
        bool isCorrect = CheckSequence();
        if (isCorrect)
        {
            Debug.Log("Correct sequence!");
            runeGameCanvas.enabled = false; // Hide the rune game canvas
            stoneWall.Lift(); // Lift the stone wall
            return;
        }

        Debug.Log("Incorrect sequence.");
    }
    public void OnAddRuneSet()
    {
        // Capture the current sequence
        string currentSequence = GetCurrentSequence();
        // Add the currentSequence to userSequences
        userSequences.Add(currentSequence);
        Debug.Log("Added sequence: " + currentSequence);
        ResetSlots();
    }
    public string GetCurrentSequence()
    {
        string sequence = "";
        foreach (var slot in runeSlots)
        {
            if (slot.placedRune != null)
                sequence += slot.placedRune.runeID;
            else
                sequence += "_"; // Empty placeholder
        }
        return sequence;
    }

    public void ResetSlots()
    {
        foreach (var slot in runeSlots)
        {
            if (slot.placedRune != null)
            {
                // Return the rune to its original parent
                slot.placedRune.transform.SetParent(slot.placedRune.OriginalParent);
                slot.placedRune.transform.localPosition = Vector3.zero;
                slot.placedRune = null;
            }
        }
    }

    public bool CheckSequence()
    {
        // Loop through all user sequences
        foreach (var userSeq in userSequences)
        {
            if (!correctSequences.Contains(userSeq))
            {
                return false; // Found an incorrect sequence, user is incorrect
            }
        }
        // If we get here, all user sequences were valid
        return true;
    }

}
