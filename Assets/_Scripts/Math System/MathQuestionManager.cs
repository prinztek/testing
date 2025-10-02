using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MathQuestionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject questionPanel;
    public GameObject Grimoire;

    [Header("Header Button")]
    public Button headerButton;
    public TMP_Text headerQuestionText;

    [Header("Expanded Panel Elements")]
    public TMP_Text expandedQuestionText;
    public TMP_InputField answerInput;
    public Button submitButton;

    [Header("Gameplay")]

    // 🔹 Reference to CharacterStats for applying buffs
    public CharacterStats characterStats;

    [Header("Question Settings")]
    public MathTopic topic = MathTopic.Permutation;
    public QuestionDifficulty difficulty = QuestionDifficulty.Easy;
    public int numberOfQuestions = 3;

    public Action OnQuestionBatchCompleted;

    private List<MathQuestion> questionQueue = new();
    private int currentIndex = 0;
    private MathQuestion currentQuestion;
    private bool answeredCorrectly = false;
    private SaveData saveData; // 🔹 keep track of answered IDs

    [Header("Buff Related")]

    // 🔹 Reference to the PrecisionStrikeBuff ScriptableObject
    public PrecisionStrikeBuffSO precisionStrikeBuff;


    void Start()
    {
        headerButton.onClick.AddListener(TogglePanel);
        submitButton.onClick.AddListener(CheckAnswer);
        saveData = SaveSystem.Load(); // 🔹 load saved data
        GenerateNewQuestions();
    }

    void Update()
    {
        // 🔹 Example: Press R to reset progress
        if (Input.GetKeyDown(KeyCode.R))
        {
            SaveSystem.ResetProgress();
            saveData = new SaveData(); // reset in memory too
            Debug.Log("🔄 Progress reset, you can replay questions now.");
        }
    }

    void TogglePanel()
    {
        bool isGrimoireActive = Grimoire.activeSelf;
        Grimoire.SetActive(!isGrimoireActive);

        if (!isGrimoireActive && questionPanel != null)
            questionPanel.SetActive(false);
        if (isGrimoireActive && questionPanel != null)
            questionPanel.SetActive(true);

        answerInput.text = "";
        if (!isGrimoireActive && answeredCorrectly)
            answeredCorrectly = false;
    }

    public void GenerateNewQuestions()
    {
        // 🔹 Pass in saveData.answeredQuestionIds so used ones are skipped
        questionQueue = MathQuestionLoaderJSON.Load(topic, difficulty, numberOfQuestions, new HashSet<int>(saveData.answeredQuestionIds));
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

            // 🔹 Mark this question as used
            if (!saveData.answeredQuestionIds.Contains(currentQuestion.id))
            {
                saveData.answeredQuestionIds.Add(currentQuestion.id);
                SaveSystem.Save(saveData); // persist immediately
            }

            // 🔹 Apply a random buff
            if (characterStats != null)
            {
                // 🔹 Apply a random buff PrecisionStrikeBuff for testing
                // characterStats.ApplyScriptableBuff(precisionStrikeBuff);

                characterStats.AddBuff(new FireInfuseBuff(8f, 3));


                // System.Random random = new System.Random();
                // int buffIndex = random.Next(0, 5);

                // switch (buffIndex)
                // {
                //     case 0: characterStats.AddBuff(new HasteBuff(8f, 3)); break;
                //     case 1: characterStats.AddBuff(new FireInfuseBuff(8f, 3)); break;
                //     case 2: characterStats.AddBuff(new PowerSurgeBuff(8f, 5)); break;
                //     case 3: characterStats.AddBuff(new ShieldBloomBuff(8f, 20)); break; // 20 hits before the shield breaks
                //     case 4: characterStats.AddBuff(new PrecisionStrikeBuff(8f, 50, 3)); break; // *50 multiplier, 3 guaranteed crits
                // }
            }

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
