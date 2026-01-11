using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.Mathematics;
using System;

public static class MathQuestionLoaderJSON
{
    private static MathQuestionDatabaseJSON database;

    private static void EnsureDatabaseLoaded()
    {
        if (database != null) return;

        TextAsset jsonFile = Resources.Load<TextAsset>("MathQuestions");
        if (jsonFile != null)
        {
            database = JsonUtility.FromJson<MathQuestionDatabaseJSON>(jsonFile.text);
            // Debug.Log("📘 Loaded " + database.questions.Length + " math questions.");
        }
        else
        {
            // Debug.LogError("❌ MathQuestions.json not found in Resources!");
            database = new MathQuestionDatabaseJSON { questions = new MathQuestionJSON[0] };
        }
    }

    // returns List<MathQuestion> and accepts usedIds
    public static List<MathQuestion> Load(MathTopic topic, QuestionDifficulty difficulty, HashSet<int> usedIds = null)
    {
        EnsureDatabaseLoaded();

        var filtered = database.questions
            .Where(q => q.type.Equals(topic.ToString(), System.StringComparison.OrdinalIgnoreCase))
            .Where(q => q.difficulty.Equals(difficulty.ToString(), System.StringComparison.OrdinalIgnoreCase))
            .Where(q => usedIds == null || !usedIds.Contains(q.id)) // 🔹 Skip answered ones
            .ToList();

        var rng = new System.Random();
        filtered = filtered.OrderBy(q => rng.Next()).ToList();
        // Print before returning
        // foreach (var item in filtered)
        // {
        //     Debug.Log($"ID:{item.id}, Topic:{item.type}, Difficulty:{item.difficulty}, Question:{item.questionString}, Answer:{item.answer}");
        // }

        return filtered
            .Select(q => new MathQuestion(q.id, topic, difficulty, q.questionString, q.answer, q.hints))
            .ToList();
    }

    // returns List<MathQuestion> regardless of usedIds
    public static List<MathQuestion> Load(MathTopic topic)
    {
        EnsureDatabaseLoaded();

        var filtered = database.questions
            .Where(q => q.type.Equals(topic.ToString(), System.StringComparison.OrdinalIgnoreCase))
            .ToList();
        var rng = new System.Random();
        filtered = filtered.OrderBy(q => rng.Next()).ToList();


        return filtered
            .Select(q => new MathQuestion(q.id, topic, Enum.Parse<QuestionDifficulty>(q.difficulty, true), q.questionString, q.answer, q.hints))
            .ToList();
    }

    public static List<MathQuestion> LoadByTopic(MathTopic topic, HashSet<int> usedIds = null)
    {
        EnsureDatabaseLoaded();

        var filtered = database.questions
            .Where(q => q.type.Equals(topic.ToString(), System.StringComparison.OrdinalIgnoreCase))
            .Where(q => usedIds == null || !usedIds.Contains(q.id))
            .ToList();

        // Print before returning
        // foreach (var item in filtered)
        // {
        //     Debug.Log($"ID:{item.id}, Topic:{item.type}, Difficulty:{item.difficulty}, Question:{item.questionString}, Answer:{item.answer}");
        // }

        // Convert to MathQuestion objects
        return filtered
            .Select(q => new MathQuestion(
                q.id,
                topic,
                Enum.TryParse(q.difficulty, out QuestionDifficulty diff) ? diff : QuestionDifficulty.Easy,
                q.questionString,
                q.answer,
                q.hints
            ))
            .ToList();
    }



}
