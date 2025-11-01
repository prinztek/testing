using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MathQuestionManager : MonoBehaviour
{
    [Header("UI References")]
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

    public PlayerInventory playerInventory;

    [Header("Question Settings")]
    public MathTopic topic = MathTopic.Permutation_and_Its_Conditions;
    public QuestionDifficulty difficulty = QuestionDifficulty.Easy;
    public int numberOfQuestions = 3;

    public Action OnQuestionBatchCompleted;

    private List<MathQuestion> questionQueue = new();
    private int currentIndex = 0;
    private MathQuestion currentQuestion;
    private bool answeredCorrectly = false;
    private SaveData saveData; // 🔹 keep track of answered IDs

    [Header("Hints Section")]
    public TMP_Text hintText;
    public Button hintButton;
    public int hintUsedCounter = 0;
    public int maxHints;  // max hints from the question
    public int baseHintCost = 25; // Cost for the first hint
    public int currentHintCost = 10; // Cost for each subsequent hint



    void Start()
    {
        headerButton.onClick.AddListener(TogglePanel);
        submitButton.onClick.AddListener(CheckAnswer);
        hintButton.onClick.AddListener(GenerateNewHint);
        saveData = SaveSystem.Load(); // 🔹 load saved data
        GenerateNewQuestions();

        int currentHintCost = GetCurrentHintCost();
        hintButton.GetComponentInChildren<TMP_Text>().text = $"Hint (-{currentHintCost}g)";

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

        answerInput.text = "";
        if (!isGrimoireActive && answeredCorrectly)
            answeredCorrectly = false;
    }

    public void GenerateNewHint()
    {
        // 🔹 Check if player has enough gold
        int currentHintCost = GetCurrentHintCost();

        if (playerInventory.gold < currentHintCost)
        {
            Debug.Log("❌ Not enough gold for a hint. Current Gold: " + playerInventory.gold + ", Hint Cost: " + currentHintCost);
            return;
        }


        maxHints = currentQuestion.hints.Length;
        if (hintUsedCounter < maxHints)
        {
            hintText.text += $"\n💡 {currentQuestion.hints[hintUsedCounter]}";
            hintUsedCounter++;
            Debug.Log("💡 Hint used. Hints remaining: " + (maxHints - hintUsedCounter));

            // Deduct gold
            playerInventory.DeductGold(currentHintCost);
            // Debug.Log("💰 Deducted " + currentHintCost + " gold for a hint. Remaining Gold: " + playerInventory.gold);

            if (hintUsedCounter >= maxHints)
            {
                hintButton.interactable = false;
                Debug.Log("🔒 All hints used. Hint button disabled.");
            }
            else
            {
                // Update the hint cost for the next hint
                currentHintCost = GetCurrentHintCost();
                hintButton.GetComponentInChildren<TMP_Text>().text = $"Hint (-{currentHintCost}g)";
            }
        }
    }

    int GetCurrentHintCost()
    {
        int costPerHintIncrease = baseHintCost;
        return baseHintCost + (costPerHintIncrease * hintUsedCounter);
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
            // if (characterStats != null)
            // {
            //     // characterStats.AddBuff(new HasteBuff(8f, 2f)); // Example: Haste buff for 8 seconds with 3x speed
            //     UIManager.Instance.CloseActivePanel(); // hide the girmoire panel

            //     System.Random random = new System.Random();
            //     int buffIndex = random.Next(0, 5);

            //     switch (buffIndex)
            //     {
            //         case 0: characterStats.AddBuff(new HasteBuff(8f, 3)); break;
            //         case 1: characterStats.AddBuff(new FireInfuseBuff(8f, 3)); break;
            //         case 2: characterStats.AddBuff(new PowerSurgeBuff(8f, 5)); break;
            //         case 3: characterStats.AddBuff(new ShieldBloomBuff(8f, 20)); break; // 20 hits before the shield breaks
            //         case 4: characterStats.AddBuff(new PrecisionStrikeBuff(8f, 50, 3)); break; // *50 multiplier, 3 guaranteed crits
            //     }
            // }

            // Apply a chosen buff
            if (characterStats != null)
            {
                UIManager.Instance.CloseActivePanel();

                var chosen = BuffChoiceManager.Instance.GetRandomBuffChoices(3);

                BuffChoiceManager.Instance.ShowChoices(chosen, (selectedBuff) =>
                {
                    characterStats.AddBuff(selectedBuff);
                    Debug.Log($"🪄 Player chose buff: {selectedBuff.buffName}");
                });
            }

            currentIndex++;
            LoadCurrentQuestion();
            // Reset hints
            hintButton.interactable = true;
            hintText.text = "Hints:";
            hintUsedCounter = 0;
        }
        else
        {
            Debug.Log("❌ Wrong. Expected: " + currentQuestion.answer);
            answeredCorrectly = false;
        }
    }

    // Helper to get a human-readable version of the topic (replaces underscores with spaces)
    public string GetNormalizedTopicName()
    {
        return topic.ToString().Replace('_', ' ');
    }
}
