using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CombinationRuneGameManager : MonoBehaviour
{
    public List<RuneSlot> runeSlots; // Assign in Inspector
    public List<string> correctCombinations = new List<string>();
    // public List<string> userSequences = new List<string>(); // { "ABC", "ACB", "BAC", "BCA", "CAB", "CBA" }
    public List<string> userCombinations = new List<string>(); // { "ABC", "ACB", "BAC", "BCA", "CAB", "CBA" }
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
        if (runesParentPanel == null)
        {
            Debug.LogError("Runes Parent Panel is not assigned!");
            return;
        }
        userCombinations.Clear();
    }

    public void OnSubmit()
    {
        if (isSolved)
            return; // Puzzle already solved, do nothing

        // if (!userSequences.Any())
        if (!userCombinations.Any())
        {
            return;
        }

        // bool isCorrect = CheckSequence();
        bool isCorrect = CheckCombinations();

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

        // Normalize the sequence to ensure order doesn't matter (e.g., ABC, BAC, CBA all become ABC)
        currentSequence = Normalize(currentSequence);

        // add the normalized sequence to the list of user combinations
        userCombinations.Add(currentSequence);

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

        if (userCombinations.Count == 0)
            return;

        // Remove the last added sequence
        int lastIndex = userCombinations.Count - 1;
        userCombinations.RemoveAt(lastIndex);

        // Remove visual entry
        completedPermutationsPanel.RemoveLastPermutation(); // can also be use by combination
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

    // This makes ABC, BAC, CBA all become ABC
    string Normalize(string input)
    {
        Debug.Log($"Normalized '{input}'");
        char[] chars = input.ToCharArray();
        System.Array.Sort(chars);
        Debug.Log($"to '{new string(chars)}'");
        return new string(chars);
    }

    public bool CheckCombinations()
    {
        if (userCombinations.Count != correctCombinations.Count)
            return false;

        List<string> normalizedCorrect = correctCombinations
            .Select(c => Normalize(c))
            .ToList();

        foreach (var correct in normalizedCorrect)
        {
            if (!userCombinations.Contains(correct))
                return false;
        }

        // double-check that all user sequences are valid (the user didn't add any extra invalid sequences)
        foreach (var user in userCombinations)
        {
            if (!normalizedCorrect.Contains(user))
                return false;
        }

        return true;
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
