using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FillPuzzleManager : MonoBehaviour
{
    public PuzzleSlotUI[] slots; // manually assign in inspector
    public Button checkAnswerButton;

    [Header("Explanation Panel")]
    [SerializeField] private GameObject explanationPanel;
    [SerializeField] private Button continueButton;

    [Header("Events")]
    public UnityEvent OnPuzzleSolved;
    public bool isSolved = false;

    [Header("Sound Clip References")]
    [SerializeField] private AudioClip correctAnswerSoundClip;
    [SerializeField] private AudioClip wrongAnswerSoundClip;

    void Awake()
    {
        checkAnswerButton.onClick.AddListener(OnSubmit);
        continueButton.onClick.AddListener(OnContinuePressed);
    }

    public void OnSubmit()
    {
        if (isSolved)
            return;

        foreach (var slot in slots)
        {
            if (!slot.IsCorrect())
            {
                Debug.Log("INCORRECT — try again");
                SoundFXManager.Instance.playOneShotSoundFXClilp(wrongAnswerSoundClip, transform, 0.3f);
                return;
            }
        }

        SoundFXManager.Instance.playOneShotSoundFXClilp(correctAnswerSoundClip, transform, 0.3f);
        Debug.Log("CORRECT CORRECT CORRECT");
        ShowExplanation();
        // unlock next step / close puzzle / give reward
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
