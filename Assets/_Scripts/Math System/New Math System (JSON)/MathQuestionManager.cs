using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TexDrawLib;

public class MathQuestionManager : MonoBehaviour
{
    [Header("UI References (Assign in Inspector)")]
    [SerializeField] private GameObject grimoirePanel;      // Optional if you want to toggle panel
    [SerializeField] private TEXDraw expandedQuestionText;
    [SerializeField] private TMP_InputField answerInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private Button hintButton;

    [Header("Gameplay References")]
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private PlayerInventory playerInventory;

    [Header("Question Settings")]
    public MathTopic topic = MathTopic.Permutation_and_Its_Conditions;
    public QuestionDifficulty difficulty = QuestionDifficulty.Easy;
    public int numberOfQuestions = 3;

    [Header("Hints Settings")]
    public int baseHintCost = 25;
    private int hintUsedCounter = 0;
    private int maxHints = 0;

    private List<MathQuestion> questionQueue = new();
    private int currentIndex = 0;
    private MathQuestion currentQuestion;
    private bool answeredCorrectly = false;
    private SaveData saveData;

    public Action OnQuestionBatchCompleted;

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
        // 🔹 If player already exists when UI enables, connect immediately
        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            HandlePlayerSpawned(GameManager.Instance.CurrentPlayer);
        }
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(GameObject player)
    {
        characterStats = player.GetComponent<CharacterStats>();
        playerInventory = player.GetComponent<PlayerInventory>();
    }

    private void Awake()
    {
        if (submitButton != null)
            submitButton.onClick.AddListener(CheckAnswer);

        if (hintButton != null)
            hintButton.onClick.AddListener(GenerateNewHint);
    }

    private void Start()
    {
        saveData = SaveSystem.Load(); // Load saved progress
        GenerateNewQuestions();
        ResetHintButtonText();
    }

    private void ResetHintButtonText()
    {
        int currentHintCost = GetCurrentHintCost();
        if (hintButton != null)
            hintButton.GetComponentInChildren<TMP_Text>().text = $"Hint (-{currentHintCost}g)";
    }

    public void ToggleGrimoirePanel()
    {
        if (grimoirePanel != null)
        {
            bool isActive = grimoirePanel.activeSelf;
            grimoirePanel.SetActive(!isActive);

            answerInput.text = "";
            if (!isActive)
                answeredCorrectly = false;
        }
    }

    public void GenerateNewHint()
    {
        if (playerInventory == null || currentQuestion == null) return;

        int currentHintCost = GetCurrentHintCost();
        if (playerInventory.Gold < currentHintCost)
        {
            Debug.Log($"❌ Not enough gold for a hint. Current Gold: {playerInventory.Gold}, Hint Cost: {currentHintCost}");
            return;
        }

        maxHints = currentQuestion.hints.Length;
        if (hintUsedCounter < maxHints)
        {
            hintText.text += $"\n💡 {currentQuestion.hints[hintUsedCounter]}";
            hintUsedCounter++;

            playerInventory.DeductGold(currentHintCost);

            if (hintUsedCounter >= maxHints)
                hintButton.interactable = false;
            else
                ResetHintButtonText();
        }
    }

    private int GetCurrentHintCost()
    {
        return baseHintCost + (baseHintCost * hintUsedCounter);
    }

    public void GenerateNewQuestions()
    {
        questionQueue = MathQuestionLoaderJSON.Load(topic, difficulty, numberOfQuestions, new HashSet<int>(saveData.answeredQuestionIds));
        currentIndex = 0;
        LoadCurrentQuestion();
    }

    private void LoadCurrentQuestion()
    {
        if (currentIndex < questionQueue.Count)
        {
            currentQuestion = questionQueue[currentIndex];
            expandedQuestionText.text = currentQuestion.prompt;
            answerInput.text = "";
        }
        else
        {
            OnQuestionBatchCompleted?.Invoke();
        }
    }

    public void CheckAnswer()
    {
        string input = answerInput.text.Trim();
        if (currentQuestion == null) return;

        if (input == currentQuestion.answer)
        {
            Debug.Log($"✅ Correct! Answer: {currentQuestion.answer}");
            answeredCorrectly = true;

            if (!saveData.answeredQuestionIds.Contains(currentQuestion.id))
            {
                saveData.answeredQuestionIds.Add(currentQuestion.id);
                SaveSystem.Save(saveData);
            }

            if (characterStats != null)
            {
                UIManager.Instance.CloseActivePanel();

                var chosen = BuffChoiceManager.Instance.GetRandomBuffChoices(3);
                BuffChoiceManager.Instance.ShowChoices(chosen, (selectedBuff) =>
                {
                    characterStats.AddBuff(selectedBuff);
                    Debug.Log($"🪄 Player chose buff: {selectedBuff.buffName}");
                });

                UIManager.Instance.ShowBuffChoiceCanvas(true);
            }

            currentIndex++;
            LoadCurrentQuestion();

            hintButton.interactable = true;
            hintText.text = "Hints:";
            hintUsedCounter = 0;
            ResetHintButtonText();
        }
        else
        {
            Debug.Log($"❌ Wrong. Expected: {currentQuestion.answer}");
            answeredCorrectly = false;
        }
    }

    public string GetNormalizedTopicName()
    {
        return topic.ToString().Replace('_', ' ');
    }
}
