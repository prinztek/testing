using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CircularPermutationRuneGameManager : MonoBehaviour
{

    public List<RuneSlot> runeSlots; // Assign in Inspector
    public List<string> correctSequences = new List<string> { "ABC", "BCA", "CAB", }; // Will hold the permutations or a single answer // e.g., {"ABC", "ACB", "BAC", "BCA", "CAB", "CBA"}
    public List<string> correctSequence2 = new List<string>(); // Will hold the permutations or a single answer // e.g., {"ABC", "ACB", "BAC", "BCA", "CAB", "CBA"}
    // This will hold either a single sequence or multiple sequences
    public List<string> userSequences = new List<string>(); // { "ABC", "ACB", "BAC", "BCA", "CAB", "CBA" }
    public bool isSolved = false;

    [Tooltip("Assign the parent panel that holds all the rune GameObjects as children")]
    public GameObject runesParentPanel;

    private List<string> runes = new List<string>();

    public Canvas runeGameCanvas; // Reference to the Rune Game Canvas

    [Header("UI")]
    [SerializeField] private CompletedPermutationsPanel completedPermutationsPanel;

    [Header("Completion UI")]
    public GameObject explanationPanel;

    public UnityEvent OnPuzzleSolved;
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

        // Clear previous data
        userSequences.Clear();
    }



    // ******************************************************************************
    // Public methods to be called by UI buttons or other scripts
    public void OnSubmit()
    {
        if (isSolved)
            return; // Puzzle already solved, do nothing

        if (!userSequences.Any())
        {
            return;
        }

        bool isCorrect = CheckSequence();
        if (isCorrect)
        {
            Debug.Log("Correct sequence!");
            // // runeGameCanvas.enabled = false; // Hide the rune game canvas
            // UIManager.Instance.CloseActivePanel(); // Hide the rune game canvas through the UIManager
            // stoneWall.Lift(); // Lift the stone wall

            isSolved = true; // Mark puzzle as solved
            OnPuzzleSolved?.Invoke();
        }

        Debug.Log("Incorrect sequence.");
    }
    public void OnAddRuneSet()
    {
        if (isSolved)
        {
            return;
        }

        if (!CanAdd())
        {
            return;
        }

        string currentSequence = GetCurrentSequence();

        // Prevent incomplete sequences
        if (currentSequence.Contains("_"))
            return;

        // Prevent duplicates
        // if (userSequences.Contains(currentSequence))
        //     return;

        userSequences.Add(currentSequence);

        // Add a visual entry
        completedPermutationsPanel.AddPermutation(currentSequence);

        ResetSlots();
    }

    public void OnClearLastRuneSet()
    {
        if (isSolved)
        {
            return;
        }

        if (userSequences.Count == 0)
            return;

        // Remove the last added sequence
        int lastIndex = userSequences.Count - 1;
        string lastSequence = userSequences[lastIndex];
        userSequences.RemoveAt(lastIndex);

        // Remove visual entry
        completedPermutationsPanel.RemoveLastPermutation();
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

    bool CanAdd()
    {
        HashSet<Rune> usedRunes = new HashSet<Rune>();

        foreach (var slot in runeSlots)
        {
            if (slot.placedRune == null)
                return false;

            if (!usedRunes.Add(slot.placedRune))
                return false; // duplicate rune
        }

        return true;
    }


    public void ResetSlots()
    {
        foreach (var slot in runeSlots)
        {
            if (slot.placedRune != null)
            {
                Rune rune = slot.placedRune;

                rune.CurrentSlot = null;
                rune.transform.SetParent(rune.runePoolParent);
                rune.transform.localPosition = Vector3.zero;

                slot.placedRune = null;
            }
        }
    }
    public bool CheckSequence()
    {
        // The player has submitted the correct number of sequences
        if (userSequences.Count != correctSequences.Count)
        {
            // Not enough sequences submitted yet
            return false;
        }

        // Check that every correct sequence is included in the user's submissions
        foreach (var correctSeq in correctSequences)
        {
            if (!userSequences.Contains(correctSeq))
            {
                // The user is missing at least one required sequence
                return false;
            }
        }

        // Step 3: double-check that all user sequences are valid (the user didn't add any extra invalid sequences)
        foreach (var userSeq in userSequences)
        {
            if (!correctSequences.Contains(userSeq))
            {
                // The user submitted an invalid sequence
                return false;
            }
        }

        return true; // The user submitted all sequences correctly
    }

    string NormalizeCircular(string s)
    {
        string best = s;

        for (int i = 1; i < s.Length; i++)
        {
            string rotated = s.Substring(i) + s.Substring(0, i);
            if (string.Compare(rotated, best) < 0)
                best = rotated;
        }

        return best;
    }

}



