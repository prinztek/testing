using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    // ✅ Now returns List<MathQuestion> and accepts usedIds
    public static List<MathQuestion> Load(MathTopic topic, QuestionDifficulty difficulty, int count, HashSet<int> usedIds = null)
    {
        EnsureDatabaseLoaded();

        var filtered = database.questions
            .Where(q => q.type.Equals(topic.ToString(), System.StringComparison.OrdinalIgnoreCase))
            .Where(q => q.difficulty.Equals(difficulty.ToString(), System.StringComparison.OrdinalIgnoreCase))
            .Where(q => usedIds == null || !usedIds.Contains(q.id)) // 🔹 Skip answered ones
            .ToList();

        var rng = new System.Random();
        filtered = filtered.OrderBy(q => rng.Next()).ToList();

        return filtered.Take(count)
            .Select(q => new MathQuestion(q.id, topic, difficulty, q.questionString, q.answer, q.hints))
            .ToList();
    }
}
