using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TexDrawLib;
using System.Collections;

public class MathQuestionManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject grimoirePanel;
    [SerializeField] private TEXDraw expandedQuestionText;
    [SerializeField] private TMP_InputField answerInput;
    [SerializeField] private RectTransform answerInputTransform;
    [SerializeField] private GameObject errorBorder;
    [SerializeField] private float duration = 0.5f;
    private Coroutine flashCoroutine;
    private Coroutine shakeCoroutine;
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private Button hintButton;

    [Header("Gameplay References")]
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private PlayerInventory playerInventory;

    [Header("Math Settings")]
    public MathTopic topic = MathTopic.Permutation_and_Its_Conditions;
    public QuestionDifficulty difficulty = QuestionDifficulty.Easy;

    [Header("Hint Settings")]
    public int baseHintCost = 25;
    private int hintUsedCounter = 0;
    private int maxHints = 0;

    private List<MathQuestionJSON> questionQueue = new();
    private int currentIndex = 0;
    private MathQuestionJSON currentQuestion;

    public Action OnQuestionBatchCompleted;

    [Header("Sound Clip References")]
    [SerializeField] private AudioClip correctAnswerSoundClip;
    [SerializeField] private AudioClip wrongAnswerSoundClip;

    private void Awake()
    {
        // submitButton?.onClick.AddListener(CheckAnswer);
        hintButton?.onClick.AddListener(GenerateNewHint);
    }

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
        if (GameManager.Instance.CurrentPlayer != null)
            HandlePlayerSpawned(GameManager.Instance.CurrentPlayer);
    }

    private void OnDisable() => GameManager.OnPlayerSpawned -= HandlePlayerSpawned;

    private void HandlePlayerSpawned(GameObject player)
    {
        characterStats = player.GetComponent<CharacterStats>();
        playerInventory = player.GetComponent<PlayerInventory>();
    }

    private void Start()
    {
        answerInput.shouldHideMobileInput = true;
        GenerateNewQuestions();
        ResetHintButtonText();
    }

    public void SetTopic(MathTopic newTopic, QuestionDifficulty newDifficulty)
    {
        topic = newTopic;
        difficulty = newDifficulty;

        // Reset internal state
        hintUsedCounter = 0;
        currentIndex = 0;

        // Use GameManager’s used math question IDs to avoid repeats
        GenerateNewQuestions();
    }

    public void GenerateNewQuestions()
    {
        var usedIds = new HashSet<int>(GameManager.Instance.usedMathQuestionData.UsedMathQuestionIds);
        // Try unused questions first
        questionQueue = MathQuestionLoaderJSON.LoadByTopic(topic, usedIds);

        // If none left, allow reuse
        if (questionQueue.Count == 0)
        {
            Debug.Log("No unused questions left — reusing old ones.");
            questionQueue = MathQuestionLoaderJSON.Load(topic);
        }

        currentIndex = 0;
        LoadCurrentQuestion();
    }

    private void LoadCurrentQuestion()
    {
        // Check if we've answered all questions
        if (currentIndex >= questionQueue.Count)
        {
            GenerateNewQuestions(); // 🔁 restart immediately
            return;
        }

        // Load the next question
        currentQuestion = questionQueue[currentIndex];
        expandedQuestionText.text = currentQuestion.prompt;
        answerInput.text = "";
        answerInput.interactable = true;
        hintText.text = "";
        hintUsedCounter = 0;
        ResetHintButtonText();

        // Disable if there are no hints
        if (currentQuestion.hints == null || currentQuestion.hints.Length == 0)
        {
            hintButton.interactable = false;
        }
        else
        {
            hintButton.interactable = true;
        }
    }


    private void ResetHintButtonText()
    {
        int cost = baseHintCost + (baseHintCost * hintUsedCounter);
        if (hintButton != null)
            hintButton.GetComponentInChildren<TMP_Text>().text = $"(-{cost}g)";
    }

    public void GenerateNewHint()
    {
        if (playerInventory == null || currentQuestion == null) return;

        int cost = baseHintCost + (baseHintCost * hintUsedCounter);
        if (playerInventory.Gold < cost) return;

        maxHints = currentQuestion.hints.Length;
        if (hintUsedCounter < maxHints)
        {
            hintText.text += $"\n {currentQuestion.hints[hintUsedCounter]}";
            hintUsedCounter++;
            playerInventory.DeductGold(cost);
            ResetHintButtonText();
            if (hintUsedCounter >= maxHints) hintButton.interactable = false;
        }
    }

    public void CheckAnswer()
    {
        if (currentQuestion == null) return;
        // normalize answers by trimming whitespace and ignoring case

        if (answerInput.text.Trim() == currentQuestion.answer.ToString())
        {
            SoundFXManager.Instance.playOneShotSoundFXClilp(correctAnswerSoundClip, transform, 0.3f);
            Debug.Log($"Player typed: '{answerInput.text}'");
            Debug.Log($"Correct answer: '{currentQuestion.answer}'");

            GameManager.Instance.MarkQuestionAsUsed(currentQuestion);

            // Give player a buff
            if (characterStats != null)
            {
                UIManager.Instance.CloseActivePanel(); // close the grimoire
                // generates the random buffs
                var choices = BuffChoiceManager.Instance.GetRandomBuffChoices(3);
                // pass the choices to Choosing Buff Canvas
                BuffChoiceManager.Instance.ShowChoices(choices, selectedBuff =>
                {
                    characterStats.AddBuff(selectedBuff);
                });
                // displays the ui for choosing the generated random buffs
                UIManager.Instance.ShowBuffChoiceCanvas(true);
            }

            currentIndex++;
            LoadCurrentQuestion();
        }
        else
        {
            SoundFXManager.Instance.playOneShotSoundFXClilp(wrongAnswerSoundClip, transform, 0.2f);
            TriggerError();
            Debug.Log($"Wrong. Expected: {currentQuestion.answer}");
        }
    }

    public string GetNormalizedTopicName()
    {
        return topic.ToString().Replace('_', ' ');
    }


    public void TriggerError()
    {
        // prevent submit button by disabling it temporarily
        submitButton.interactable = false;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        flashCoroutine = StartCoroutine(FlashError());
        shakeCoroutine = StartCoroutine(ShakeInputField());
    }

    IEnumerator FlashError()
    {
        Debug.Log("FlashError START");

        if (errorBorder == null)
        {
            Debug.Log("errorBorder is NULL");
            yield break;
        }

        // errorBorder.gameObject.SetActive(true);

        CanvasGroup cg = errorBorder.GetComponent<CanvasGroup>();
        cg.alpha = 1f;

        Debug.Log("Before Wait");

        yield return new WaitForSecondsRealtime(duration);

        cg.alpha = 0f;
        // errorBorder.gameObject.SetActive(false);

        flashCoroutine = null;
    }

    IEnumerator ShakeInputField(float duration = 0.3f, float magnitude = 5f)
    {
        if (answerInputTransform == null) yield break;

        Vector3 originalPos = answerInputTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            answerInputTransform.anchoredPosition = originalPos + new Vector3(x, y, 0);

            yield return null;
        }

        // Reset position
        answerInputTransform.anchoredPosition = originalPos;
        submitButton.interactable = true;
    }
}