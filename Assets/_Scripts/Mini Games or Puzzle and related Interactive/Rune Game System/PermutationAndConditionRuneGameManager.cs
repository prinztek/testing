using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
public class PermutationAndConditionRuneGameManager : MonoBehaviour
{

    [Header("Rune Setup")]
    public List<RuneSlot> runeSlots;
    [Header("Condition Settings (optional)")]
    [Tooltip("The rune that must appear in the first slot (leave empty if unused)")]
    public string requiredFirstRune = "";
    [Tooltip("The rune that must appear in the last slot (leave empty if unused)")]
    public string requiredLastRune = "";
    [Tooltip("The rune that must appear somewhere in the sequence (leave empty if unused)")]
    public string requiredContainsRune = "";
    [Tooltip("A rune that must appear at a specific slot (leave index -1 if unused)")]
    public string requiredRuneAtIndex = "";
    public int runeIndex = -1;
    private string userSequence = ""; // holds the sequence submitted by the player

    [Header("Explanation Panel")]
    [SerializeField] private GameObject explanationPanel;
    [SerializeField] private Button continueButton;

    [Header("Events")]
    public UnityEvent OnPuzzleSolved;
    public bool isSolved = false;

    [Header("Sound Clip References")]
    [SerializeField] private AudioClip correctAnswerSoundClip;
    [SerializeField] private AudioClip wrongAnswerSoundClip;

    private void Awake()
    {
        continueButton.onClick.AddListener(OnContinuePressed);
    }
    public void OnSubmit()
    {
        if (isSolved)
            return;

        if (!TryGetSequence()) // Verify all slots are filled
            return;

        if (IsValidSequence(userSequence))
        {
            // Play correct sound
            SoundFXManager.Instance.playOneShotSoundFXClilp(correctAnswerSoundClip, transform, 0.3f);
            Debug.Log("Correct sequence!");
            ShowExplanation();
        }
        else
        {
            userSequence = "";
            ResetSlots();
            Debug.Log("Incorrect sequence. Try again.");
            // Play wrong sound
            SoundFXManager.Instance.playOneShotSoundFXClilp(wrongAnswerSoundClip, transform, 0.3f);
        }
    }

    // ---------------- VALIDATION ----------------

    private bool IsValidSequence(string sequence)
    {
        // Check each condition if set
        if (!string.IsNullOrEmpty(requiredFirstRune) && !sequence.StartsWith(requiredFirstRune))
            return false;

        if (!string.IsNullOrEmpty(requiredLastRune) && !sequence.EndsWith(requiredLastRune))
            return false;

        if (!string.IsNullOrEmpty(requiredContainsRune) && !sequence.Contains(requiredContainsRune))
            return false;

        if (!string.IsNullOrEmpty(requiredRuneAtIndex) && runeIndex >= 0) // 
        {
            // if runeIndex is out of bounds or rune at index doesn't match
            if (runeIndex >= sequence.Length || sequence[runeIndex].ToString() != requiredRuneAtIndex)
                return false;
        }

        return true;
    }


    // ---------------- HELPERS ----------------

    bool TryGetSequence()
    {
        userSequence = "";

        foreach (var slot in runeSlots)
        {
            if (slot.placedRune == null)
                return false;
            Debug.Log("Slot empty: " + slot.name);
            userSequence += slot.placedRune.runeID;
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

    // ---------------- EXPLANATION RELATED ----------------
    public void ShowExplanation()
    {
        StartCoroutine(ShowExplanationSequence());
        Debug.Log("Explanation shown.");
    }

    // ---------------- CONTINUE ----------------
    public void OnContinuePressed()
    {
        // if puzzle is already solve don't invoke onPu
        if (isSolved != true)
        {
            isSolved = true;
        }
        OnPuzzleSolved?.Invoke();
    }

    private IEnumerator ShowExplanationSequence()
    {
        yield return GameManager.Instance.uiFade.FastFadeOut();

        explanationPanel.SetActive(true);

        yield return GameManager.Instance.uiFade.FastFadeIn();
    }
}
