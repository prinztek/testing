using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TexDrawLib;

public class MathQuestionManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject grimoirePanel;
    [SerializeField] private TEXDraw expandedQuestionText;
    [SerializeField] private TMP_InputField answerInput;
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

    private List<MathQuestion> questionQueue = new();
    private int currentIndex = 0;
    private MathQuestion currentQuestion;

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
        hintButton.interactable = true;
    }


    private void ResetHintButtonText()
    {
        int cost = baseHintCost + (baseHintCost * hintUsedCounter);
        if (hintButton != null)
            hintButton.GetComponentInChildren<TMP_Text>().text = $"(-{cost}g)";
    }

    public void GenerateNewHint()
    {
        Debug.Log("Hint button clicked");

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

        if (answerInput.text.Trim() == currentQuestion.answer)
        {
            SoundFXManager.Instance.playOneShotSoundFXClilp(correctAnswerSoundClip, transform, 0.3f);
            Debug.Log($"Player typed: '{answerInput.text}'");
            Debug.Log($"Correct answer: '{currentQuestion.answer}'");

            // Debug.Log($"✅ Correct! {currentQuestion.answer}");
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
            Debug.Log($"Wrong. Expected: {currentQuestion.answer}");
        }
    }

    public string GetNormalizedTopicName()
    {
        return topic.ToString().Replace('_', ' ');
    }
}
