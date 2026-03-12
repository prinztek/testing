using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class ProbabilityTwoEventsManager : MonoBehaviour
{
    // ============================
    // SLOTS
    // ============================

    [Header("Numerator Slots")]
    [SerializeField] private OutcomeSlot[] numeratorSlots;

    [Header("Denominator Slots")]
    [SerializeField] private OutcomeSlot[] denominatorSlots;

    // ============================
    // SELECTOR UI
    // ============================

    [Header("Selector UI")]
    [SerializeField] private Image[] numeratorSelectors;
    [SerializeField] private Image[] denominatorSelectors;

    // ============================
    // FRACTIONS
    // ============================

    [Header("Fraction Panels")]
    [SerializeField] private GameObject[] fractions;

    // ============================
    // SAMPLE SPACES
    // ============================

    [Header("Sample Spaces")]
    [SerializeField] private GameObject[] sampleSpaces;

    // ============================
    // OUTCOME ITEMS
    // ============================

    public OutcomeItem[] outcomeItems;

    [SerializeField] private Button resetButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI nextButtonText;

    // ============================
    // EXPLANATION
    // ============================

    [Header("Explanation Panel")]
    [SerializeField] private GameObject explanationPanel;
    [SerializeField] private Button continueButton;

    // ============================
    // EVENTS
    // ============================

    [Header("Events")]
    public UnityEvent OnPuzzleSolved;
    public bool isSolved = false;

    // ============================
    // AUDIO
    // ============================

    [Header("Sound Clip References")]
    [SerializeField] private AudioClip correctAnswerSoundClip;
    [SerializeField] private AudioClip wrongAnswerSoundClip;

    // ============================
    // INTERNAL STATE
    // ============================

    private enum SelectedInput
    {
        Numerator,
        Denominator
    }

    private SelectedInput currentSelection = SelectedInput.Numerator;
    private int currentEventIndex = 0;

    // ============================
    // INITIALIZATION
    // ============================

    private void Awake()
    {
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetAll);

        if (nextButton != null)
            nextButton.onClick.AddListener(SwitchEvent);

        SetEventVisualState(0);

        if (sampleSpaces.Length > 1)
            sampleSpaces[1].SetActive(false);

        UpdateNextButtonText();
    }

    private void Start()
    {
        SetSelection(SelectedInput.Numerator);
    }

    // ============================
    // RESET
    // ============================

    public void ResetAll()
    {
        foreach (var slot in numeratorSlots)
            slot.ResetSlot();

        foreach (var slot in denominatorSlots)
            slot.ResetSlot();

        foreach (var item in outcomeItems)
            item.ResetSelection();
    }

    // ============================
    // SELECTION
    // ============================

    public void SelectNumerator()
    {
        SetSelection(SelectedInput.Numerator);
    }

    public void SelectDenominator()
    {
        SetSelection(SelectedInput.Denominator);
    }

    private void SetSelection(SelectedInput selection)
    {
        currentSelection = selection;

        for (int i = 0; i < numeratorSelectors.Length; i++)
        {
            numeratorSelectors[i].enabled =
                (i == currentEventIndex && selection == SelectedInput.Numerator);

            denominatorSelectors[i].enabled =
                (i == currentEventIndex && selection == SelectedInput.Denominator);
        }
    }

    // ============================
    // EVENT SWITCHING
    // ============================

    public void SwitchEvent()
    {
        currentEventIndex = (currentEventIndex == 0) ? 1 : 0;

        UpdateNextButtonText();

        for (int i = 0; i < fractions.Length; i++)
        {
            CanvasGroup cg = fractions[i].GetComponent<CanvasGroup>();

            if (cg == null) continue;

            if (i == currentEventIndex)
            {
                cg.alpha = 1f;
                cg.interactable = true;
            }
            else
            {
                cg.alpha = 0.4f;
                cg.interactable = false;
            }
        }

        for (int i = 0; i < sampleSpaces.Length; i++)
        {
            sampleSpaces[i].SetActive(i == currentEventIndex);
        }

        SetSelection(SelectedInput.Numerator);
    }

    private void UpdateNextButtonText()
    {
        if (nextButtonText == null) return;

        nextButtonText.text =
            currentEventIndex == 0 ? "To Event B" : "To Event A";
    }

    private void SetEventVisualState(int index)
    {
        for (int i = 0; i < fractions.Length; i++)
        {
            CanvasGroup cg = fractions[i].GetComponent<CanvasGroup>();

            if (cg == null) continue;

            if (i == index)
            {
                cg.alpha = 1f;
                cg.interactable = true;
            }
            else
            {
                cg.alpha = 0.8f;
                cg.interactable = false;
            }
        }
    }

    // ============================
    // OUTCOME ITEM CLICK
    // ============================

    public void HandleOutcomeItemClick(OutcomeItem item)
    {
        OutcomeSlot numerator = numeratorSlots[currentEventIndex];
        OutcomeSlot denominator = denominatorSlots[currentEventIndex];

        if (currentSelection == SelectedInput.Numerator)
            item.ToggleNumerator(numerator);
        else
            item.ToggleDenominator(denominator);
    }

    // ============================
    // SUBMIT
    // ============================

    public void OnSubmit()
    {
        if (isSolved)
            return;

        bool allCorrect = true;

        for (int i = 0; i < numeratorSlots.Length; i++)
        {
            if (!numeratorSlots[i].IsCorrect() ||
                !denominatorSlots[i].IsCorrect())
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            if (correctAnswerSoundClip != null)
            {
                SoundFXManager.Instance.playOneShotSoundFXClilp(
                    correctAnswerSoundClip,
                    transform,
                    0.3f
                );
            }

            Debug.Log("ALL EVENTS CORRECT");
            ShowExplanation();
        }
        else
        {
            Debug.Log("WRONG ANSWER");

            if (wrongAnswerSoundClip != null)
            {
                SoundFXManager.Instance.playOneShotSoundFXClilp(
                    wrongAnswerSoundClip,
                    transform,
                    0.3f
                );
            }
        }
    }

    // ============================
    // EXPLANATION
    // ============================

    public void ShowExplanation()
    {
        StartCoroutine(ShowExplanationSequence());
    }

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