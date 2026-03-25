using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PermutationRuneGameManager : MonoBehaviour
{
    public List<RuneSlot> runeSlots; // Assign in Inspector
    // Will hold the permutations or a single answer // e.g., {"ABC", "ACB", "BAC", "BCA", "CAB", "CBA"}
    // This will hold either a single sequence or multiple sequences
    public List<string> correctSequences = new List<string>();
    public List<string> userSequences = new List<string>(); // { "ABC", "ACB", "BAC", "BCA", "CAB", "CBA" }
    public bool isDistinct = false; // If true, only distinct sequences are considered correct
    public bool isSolved = false;

    [Tooltip("Assign the parent panel that holds all the rune GameObjects as children")]
    public GameObject runesParentPanel;
    private List<string> runes = new List<string>();

    public Canvas runeGameCanvas; // Reference to the Rune Game Canvas

    [Header("UI")]
    [SerializeField] private CompletedPermutationsPanel completedPermutationsPanel;

    [Header("Explanation Panel")]
    [SerializeField] private GameObject explanationPanel;
    [SerializeField] private Button continueButton;

    public UnityEvent OnPuzzleSolved;

    [Header("Sound Clip References")]
    [SerializeField] private AudioClip correctAnswerSoundClip;
    [SerializeField] private AudioClip wrongAnswerSoundClip;

    void Awake()
    {
        continueButton.onClick.AddListener(OnContinuePressed);
    }
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

        // Debug.Log($"Found {runes.Count} runes from the panel.");

        // Clear previous data
        // correctSequences.Clear();
        userSequences.Clear();

        // Generate correct sequences based on settings
        // correctSequences = isDistinct ? GenerateDistinctPermutations(runes) : GeneratePermutations(runes);

    }

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
            // isSolved = true; // Mark puzzle as solved
            // OnPuzzleSolved?.Invoke();
            SoundFXManager.Instance.playOneShotSoundFXClilp(correctAnswerSoundClip, transform, 0.3f);
            Debug.Log("Correct sequence!");
            ShowExplanation();
        }
        else
        {
            SoundFXManager.Instance.playOneShotSoundFXClilp(wrongAnswerSoundClip, transform, 0.3f);
            Debug.Log("Incorrect sequence.");
        }

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
            // Not enough sequences or too many submitted
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

        // double-check that all user sequences are valid (the user didn't add any extra invalid sequences)
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

    // ---------------- EXPLANATION RELATED ----------------
    public void ShowExplanation()
    {
        StartCoroutine(ShowExplanationSequence());
        Debug.Log("Explanation shown.");
    }

    // ---------------- CONTINUE ----------------
    public void OnContinuePressed()
    {
        isSolved = true;
        OnPuzzleSolved?.Invoke();
    }

    private IEnumerator ShowExplanationSequence()
    {
        yield return GameManager.Instance.uiFade.FastFadeOut();

        explanationPanel.SetActive(true);

        yield return GameManager.Instance.uiFade.FastFadeIn();
    }

}
