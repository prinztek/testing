using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public class RuneNotationGameManager : MonoBehaviour
{
    public List<RuneSlot> runeSlots; // Assign in Inspector
    public List<string> correctSequences = new List<string>(); // Will hold the permutations or a single answer // e.g., {"ABC", "ACB", "BAC", "BCA", "CAB", "CBA"}
    // This will hold either a single sequence or multiple sequences
    public string userSequence; // { "ABC", "ACB", "BAC", "BCA", "CAB", "CBA" }
    public bool isSolved = false;


    [Tooltip("Assign the parent panel that holds all the rune GameObjects as children")]
    public GameObject runesParentPanel;

    private List<string> runes = new List<string>();

    public Canvas runeGameCanvas; // Reference to the Rune Game Canvas
    void Start()
    {
        runes.Clear();

        if (runesParentPanel == null)
        {
            Debug.LogError("Runes Parent Panel is not assigned!");
            return;
        }

    }

    public void OnSubmit()
    {
        if (isSolved)
        {
            return;
        }

        string currentSequence = GetCurrentSequence();

        if (CheckSequence(currentSequence))
        {
            isSolved = true;
            Debug.Log("Correct Sequence! Puzzle Solved.");
            // Trigger any success events or animations here
        }
        else
        {
            Debug.Log("Incorrect Sequence. Try Again.");
            // Optionally, provide feedback for incorrect attempts
            userSequence = ""; // Reset user sequence
        }
    }

    public string GetCurrentSequence()
    {
        List<string> runes = new List<string>(); // for single notation

        foreach (var slot in runeSlots)
        {
            if (slot.placedRune != null)
            {
                runes.Add(slot.placedRune.runeID);
            }
        }

        return string.Join("P", runes);
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

    public bool CheckSequence(string sequence)
    {
        Debug.Log("Checking sequence: " + sequence);
        // simple check whether userSequence matches correctSequences
        if (sequence != correctSequences[0]) // Assuming correctSequences contains only one sequence
        {
            return false;
        }

        return true;
    }
}
