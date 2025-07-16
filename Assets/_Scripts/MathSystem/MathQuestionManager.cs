using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MathQuestionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mathDropdownPanel; // 🔹 Entire panel (header + expand area)
    public GameObject expandedPanel;

    [Header("Header Button")]
    public Button headerButton;
    public TMP_Text headerQuestionText;

    [Header("Expanded Panel Elements")]
    public TMP_Text expandedQuestionText;
    public TMP_InputField answerInput;
    public Button submitButton;

    [Header("Gameplay")]
    public CharacterStats playerStats;

    [Header("Question Settings")]
    public MathTopic topic = MathTopic.Permutation;
    public QuestionDifficulty difficulty = QuestionDifficulty.Easy;
    public int numberOfQuestions = 3;

    public Action OnQuestionBatchCompleted;

    private List<MathQuestion> questionQueue = new();
    private int currentIndex = 0;
    private MathQuestion currentQuestion;
    private bool answeredCorrectly = false;

    void Start()
    {
        expandedPanel.SetActive(false);
        headerButton.onClick.AddListener(TogglePanel);
        submitButton.onClick.AddListener(CheckAnswer);

        GenerateNewQuestions();
    }

    void TogglePanel()
    {
        bool isActive = expandedPanel.activeSelf;
        expandedPanel.SetActive(!isActive);
        answerInput.text = "";

        if (!isActive && answeredCorrectly)
            answeredCorrectly = false;
    }

    public void GenerateNewQuestions()
    {
        questionQueue = MathQuestionLoader.Load(topic, difficulty, numberOfQuestions);
        currentIndex = 0;
        LoadCurrentQuestion();
    }

    private void LoadCurrentQuestion()
    {
        if (currentIndex < questionQueue.Count)
        {
            currentQuestion = questionQueue[currentIndex];

            headerQuestionText.text = currentQuestion.prompt;
            expandedQuestionText.text = currentQuestion.prompt;
            answerInput.text = "";
        }
        else
        {
            Debug.Log("✅ All questions completed!");
            expandedPanel.SetActive(false);

            // 🔹 Hide entire dropdown panel
            if (mathDropdownPanel != null)
                mathDropdownPanel.SetActive(false);

            OnQuestionBatchCompleted?.Invoke();
        }
    }

    void CheckAnswer()
    {
        string input = answerInput.text.Trim();

        if (input == currentQuestion.answer)
        {
            Debug.Log("✅ Correct! Answer: " + currentQuestion.answer);
            answeredCorrectly = true;
            // 🔹 Apply buff to player stats
            // can you randomize the buff i have the HastBuff, FireInfuseBuff, PowerSurgeBuff, ShieldBloomBuff, PrecisionStrikeBuff
            // and i want to apply one of them randomly
            if (playerStats != null)
            {
                System.Random random = new System.Random();
                int buffIndex = random.Next(0, 5); // Random index between 0 and 4

                switch (buffIndex)
                {
                    case 0:
                        playerStats.AddBuff(new HasteBuff(8f, 3));
                        break;
                    case 1:
                        playerStats.AddBuff(new FireInfuseBuff(8f, 3));
                        break;
                    case 2:
                        playerStats.AddBuff(new PowerSurgeBuff(8f, 3));
                        break;
                    case 3:
                        playerStats.AddBuff(new ShieldBloomBuff(8f, 3));
                        break;
                    case 4:
                        playerStats.AddBuff(new PrecisionStrikeBuff(8f, 3));
                        break;
                }
            }

            currentIndex++;
            LoadCurrentQuestion();
            expandedPanel.SetActive(false);
        }
        else
        {
            Debug.Log("❌ Wrong. Expected: " + currentQuestion.answer);
            answeredCorrectly = false;
        }
    }
}
