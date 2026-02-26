using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;

public class FactorialTotemGameManager : MonoBehaviour
{
    [Header("Factorial")]
    public int factorialNumber;

    [Header("UI")]
    public Totem totemPrefab;
    public RectTransform totemParent;
    public Button submitButton;

    private List<Totem> totems = new();
    public UnityEvent OnPuzzleSolved;
    public bool isSolved = false;

    [Header("Completion UI")]
    [SerializeField] private GameObject explanationPanel;
    [SerializeField] private Button continueButton;

    [Header("Sound Clip References")]
    [SerializeField] private AudioClip correctAnswerSoundClip;
    [SerializeField] private AudioClip wrongAnswerSoundClip;

    void Awake()
    {
        continueButton.onClick.AddListener(OnContinuePressed);
    }
    void Start()
    {
        SpawnTotems();
        submitButton.onClick.AddListener(CheckAnswer);
    }

    void SpawnTotems()
    {
        // edge case if 0!
        if (factorialNumber == 0)
        {
            factorialNumber = 1;
            Totem totem = Instantiate(totemPrefab, totemParent);

            RectTransform rect = totem.GetComponent<RectTransform>();

            totem.Initialize(factorialNumber);
            totems.Add(totem);
        }

        if (factorialNumber > 1)
        {
            for (int i = 0; i < factorialNumber; i++)
            {
                Totem totem = Instantiate(totemPrefab, totemParent);

                RectTransform rect = totem.GetComponent<RectTransform>();

                totem.Initialize(factorialNumber);
                totems.Add(totem);
            }
        }

    }

    void CheckAnswer()
    {
        if (factorialNumber == 0)
        {
            int expected = 1;
            if (totems[0].currentValue != expected)
            {
                Debug.Log("Incorrect factorial input");
                SoundFXManager.Instance.playOneShotSoundFXClilp(wrongAnswerSoundClip, transform, 0.3f);
                return;
            }
            else
            {
                Debug.Log("Correct! Factorial understood.");
                isSolved = true;
                SoundFXManager.Instance.playOneShotSoundFXClilp(correctAnswerSoundClip, transform, 0.3f);
                OnPuzzleSolved?.Invoke();
                return;
            }
        }

        // if factorialNumber is n, expected sequence is n, n-1, n-2, ..., 2, 1
        for (int i = 0; i < totems.Count; i++)
        {
            int expected = factorialNumber - i;

            if (totems[i].currentValue != expected)
            {
                Debug.Log("Incorrect factorial input");
                SoundFXManager.Instance.playOneShotSoundFXClilp(wrongAnswerSoundClip, transform, 0.3f);
                return;
            }
        }

        Debug.Log("Correct! Factorial understood.");
        // success logic here
        SoundFXManager.Instance.playOneShotSoundFXClilp(correctAnswerSoundClip, transform, 0.3f);
        ShowExplanation();
        // OnPuzzleSolved?.Invoke();
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
