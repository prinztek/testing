using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using TMPro;

public class UnionAndIntersectionManager : MonoBehaviour
{
    public ElementsSlot[] slots; // manually assign in inspector (venn diagram slots)
    public ElementsSlot[] slots2; // manually assign in inspector (eg. union, intersection, complement slots A, B, AUB, AnB, etc.)

    [Header("Explanation Panel")]
    [SerializeField] private GameObject explanationPanel;
    [SerializeField] private Button continueButton;
    [Header("Part 2 Panel")]
    [SerializeField] private GameObject panel1;
    [SerializeField] private GameObject panel2;
    [SerializeField] private Button nextOrPreviousButton;

    [Header("Events")]
    public UnityEvent OnPuzzleSolved;
    public bool isSolved = false;

    [Header("Sound Clip References")]
    [SerializeField] private AudioClip correctAnswerSoundClip;
    [SerializeField] private AudioClip wrongAnswerSoundClip;
    void Awake()
    {
        continueButton.onClick.AddListener(OnContinuePressed);
    }
    void Start()
    {
        if (panel2 != null)
        {
            if (panel1 != null)
            {
                panel2.SetActive(false);
            }
        }
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
                if (wrongAnswerSoundClip != null)
                {
                    SoundFXManager.Instance.playOneShotSoundFXClilp(wrongAnswerSoundClip, transform, 0.3f);
                }
                return;
            }
        }

        foreach (var slot in slots2)
        {
            if (!slot.IsCorrect())
            {
                Debug.Log("INCORRECT — try again");
                if (wrongAnswerSoundClip != null)
                {
                    SoundFXManager.Instance.playOneShotSoundFXClilp(wrongAnswerSoundClip, transform, 0.3f);
                }
                return;
            }
        }

        if (correctAnswerSoundClip != null)
        {
            SoundFXManager.Instance.playOneShotSoundFXClilp(correctAnswerSoundClip, transform, 0.3f);
        }

        Debug.Log("CORRECT CORRECT CORRECT");
        ShowExplanation();
    }

    // display the next panel where we are asking for the union, intersections, or complements
    public void OnNext()
    {

        if (panel1 == null)
        {
            return;
        }

        if (panel1 != null && panel1.activeSelf)
        {
            if (panel1 != null)
            {
                panel1.SetActive(false);
            }

            if (panel2 != null)
                panel2.SetActive(true);
            if (nextOrPreviousButton != null)
                nextOrPreviousButton.GetComponentInChildren<TextMeshProUGUI>().text = "Previous";
        }
        else
        {
            if (panel2 != null && panel2.activeSelf)
                panel2.SetActive(false);
            if (panel1 != null)
            {
                panel1.SetActive(true);
            }
            if (nextOrPreviousButton != null)
                nextOrPreviousButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
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


// also used for mutually and non-mutually exclusive puzzles