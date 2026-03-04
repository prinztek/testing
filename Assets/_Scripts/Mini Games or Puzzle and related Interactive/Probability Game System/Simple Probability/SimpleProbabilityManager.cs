using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class SimpleProbabilityManager : MonoBehaviour
{
    [Header("Selector Visuals")]
    [SerializeField] private Image numeratorSelectorUI;
    [SerializeField] private Image denominatorSelectorUI;

    [Header("Outcome Slots")]
    [SerializeField] private OutcomeSlot numeratorSlot;
    [SerializeField] private OutcomeSlot denominatorSlot;

    [Header("Fraction Display")]
    [SerializeField] private TextMeshProUGUI fractionText;

    private enum SelectedInput
    {
        Numerator,
        Denominator
    }

    private SelectedInput currentSelection = SelectedInput.Numerator;

    [Header("Explanation Panel")]
    [SerializeField] private GameObject explanationPanel;
    [SerializeField] private Button continueButton;

    [Header("Events")]
    public UnityEvent OnPuzzleSolved;
    public bool isSolved = false;

    [Header("Sound Clip References")]
    [SerializeField] private AudioClip correctAnswerSoundClip;
    [SerializeField] private AudioClip wrongAnswerSoundClip;

    private void Start()
    {
        SetSelection(SelectedInput.Numerator);
        numeratorSelectorUI.enabled = true;
        denominatorSelectorUI.enabled = false;
    }

    // ============================
    // Selection Switching
    // ============================

    public void SelectNumerator()
    {
        SetSelection(SelectedInput.Numerator);
        numeratorSelectorUI.enabled = true;
        denominatorSelectorUI.enabled = false;
    }

    public void SelectDenominator()
    {
        SetSelection(SelectedInput.Denominator);
        denominatorSelectorUI.enabled = true;
        numeratorSelectorUI.enabled = false;
    }

    private void SetSelection(SelectedInput selection)
    {
        currentSelection = selection;

        numeratorSelectorUI.enabled = (selection == SelectedInput.Numerator);
        denominatorSelectorUI.enabled = (selection == SelectedInput.Denominator);
    }

    // ============================
    // Called by OutcomeItem
    // ============================

    // add the outcome item to numerator or denominator
    public void HandleOutcomeItemClick(OutcomeItem item)
    {
        if (currentSelection == SelectedInput.Numerator)
        {
            item.ToggleNumerator(numeratorSlot);
        }
        else
        {
            item.ToggleDenominator(denominatorSlot);
        }
    }

    public void OnSubmit()
    {
        if (isSolved)
            return;

        // is numeratorSlot holding the correct outcome in the sample space
        // is denominatorSlot holding the correct outcome in the sample space

        bool numeratorCorrect = numeratorSlot.IsCorrect();
        bool denominatorCorrect = denominatorSlot.IsCorrect();

        if (numeratorCorrect && denominatorCorrect)
        {
            if (correctAnswerSoundClip != null)
            {
                SoundFXManager.Instance.playOneShotSoundFXClilp(correctAnswerSoundClip, transform, 0.3f);
            }


            Debug.Log("CORRECT CORRECT CORRECT");
            ShowExplanation();
        }
        else
        {
            Debug.Log("WRONG WRONG WRONG");

            if (wrongAnswerSoundClip != null)
                SoundFXManager.Instance.playOneShotSoundFXClilp(wrongAnswerSoundClip, transform, 0.3f);
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