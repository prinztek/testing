using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CircularPermutationRuneGameManager : MonoBehaviour
{

    public List<RuneSlot> runeSlots; // Assign in Inspector
    public List<string> correctSequences = new List<string> { "ABC", "ACB" }; // Will hold the permutation/s // e.g., {"ABC", "ACB", "BAC", "BCA", "CAB", "CBA"}
    public List<string> userSequences = new List<string>(); // { "ABC", "ACB" }
    public bool isSolved = false;
    public bool isFlip = false;

    [Tooltip("Assign the parent panel that holds all the rune GameObjects as children")]
    public GameObject runesParentPanel;
    private List<string> runes = new List<string>();
    [Header("UI")]
    [SerializeField] private CompletedPermutationsPanel completedPermutationsPanel;

    [Header("Completion UI")]
    [SerializeField] private GameObject explanationPanel;
    [SerializeField] private Button continueButton;

    public UnityEvent OnPuzzleSolved;

    [Header("Sound Clip References")]
    [SerializeField] private AudioClip correctAnswerSoundClip;
    [SerializeField] private AudioClip wrongAnswerSoundClip;
    private void Awake()
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
            // isSolved = true; // Mark puzzle as solved
            // OnPuzzleSolved?.Invoke();
            SoundFXManager.Instance.playOneShotSoundFXClilp(correctAnswerSoundClip, transform, 0.3f);
            Debug.Log("Correct sequence!");
            ShowExplanation();
        }
        else
        {
            Debug.Log("Incorrect sequence.");
            SoundFXManager.Instance.playOneShotSoundFXClilp(wrongAnswerSoundClip, transform, 0.3f);
        }
    }
    public void OnAddRuneSet()
    {
        if (isSolved)
            return;

        if (!CanAdd())
            return;

        string sequence = GetCurrentSequence();
        Debug.Log("Attempting to add sequence: " + sequence);

        if (sequence.Contains("_"))
            return;

        if (userSequences.Contains(sequence))
            return;

        userSequences.Add(sequence);
        completedPermutationsPanel.AddPermutation(sequence);

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
            if (slot.placedRune != null && slot.isLocked == false)
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
        if (isFlip)
        {
            // For bracelet mode, only one correct arrangement is required
            // Check if user submitted at least one valid sequence (ABC or ACB)
            foreach (var userSeq in userSequences)
            {
                if (userSeq == "ABC" || userSeq == "ACB")
                    return true; // correct
            }
            return false; // none of the submissions are correct
        }


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

    // ---------------- EXPLANATION RELATED ----------------
    public void ShowExplanation()
    {
        StartCoroutine(ShowExplanationSequence());
        Debug.Log("Explanation shown.");
    }

    // ---------------- CONTINUE ----------------
    public void OnContinuePressed()
    {
        Debug.Log("Continue pressed. Puzzle solved.");
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



