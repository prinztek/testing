using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MathQuestionManager : MonoBehaviour
{
    private int pauseRequestCount = 0;

    [Header("UI References")]
    public GameObject questionPanel; // 🔹 Entire panel (header + expand area)
    public GameObject Grimoire;

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
        headerButton.onClick.AddListener(TogglePanel);
        submitButton.onClick.AddListener(CheckAnswer);
        GenerateNewQuestions();
    }

    void TogglePanel()
    {
        bool isGrimoireActive = Grimoire.activeSelf;
        Grimoire.SetActive(!isGrimoireActive);

        // Hide the question panel if the Grimoire is active
        if (!isGrimoireActive && questionPanel != null)
        {
            questionPanel.SetActive(false); // Hide the question panel if the Grimoire is shown
        }

        // If Grimoire is now inactive, make sure to show the question panel again
        if (isGrimoireActive && questionPanel != null)
        {
            questionPanel.SetActive(true); // Show the question panel if the Grimoire is hidden
        }

        // Reset the input field and answered state
        answerInput.text = "";
        if (!isGrimoireActive && answeredCorrectly)
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

            // 🔹 Hide entire dropdown panel
            if (questionPanel != null)
                questionPanel.SetActive(false);

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
            // playerStats.AddBuff(new FireInfuseBuff(8f, 3));
            currentIndex++;
            LoadCurrentQuestion();
            Grimoire.SetActive(false);
        }
        else
        {
            Debug.Log("❌ Wrong. Expected: " + currentQuestion.answer);
            answeredCorrectly = false;
        }
    }


}
