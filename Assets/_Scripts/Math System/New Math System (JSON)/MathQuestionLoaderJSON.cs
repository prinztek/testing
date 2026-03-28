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
        }
        else
        {
            database = new MathQuestionDatabaseJSON { questions = new MathQuestionJSON[0] };
        }
    }

    // returns List<MathQuestion> including usedIds
    public static List<MathQuestionJSON> Load(MathTopic topic, QuestionDifficulty difficulty, HashSet<int> usedIds = null)
    {
        EnsureDatabaseLoaded();

        var filtered = database.questions
            .Where(q =>
            {
                if (!Enum.TryParse(q.type, true, out MathTopic qTopic)) return false;
                if (!Enum.TryParse(q.difficulty, true, out QuestionDifficulty qDiff)) return false;

                return qTopic == topic &&
                       qDiff == difficulty &&
                       (usedIds == null || !usedIds.Contains(q.id));
            })
            .ToList();

        var rng = new System.Random();
        filtered = filtered.OrderBy(q => rng.Next()).ToList();

        // Print before returning
        foreach (var item in filtered)
        {
            Debug.Log($"ID:{item.id}, Topic:{item.type}, Difficulty:{item.difficulty}, Question:{item.questionString}, Answer:{item.answer}");
        }

        return filtered
            .Select(q => new MathQuestionJSON(q.id, topic, difficulty, q.questionString, q.answer, q.answers, q.hints))
            .ToList();
    }

    // returns List<MathQuestion> regardless of usedIds
    public static List<MathQuestionJSON> Load(MathTopic topic)
    {
        EnsureDatabaseLoaded();

        var filtered = database.questions
            .Where(q =>
            {
                if (!Enum.TryParse(q.type, true, out MathTopic qTopic))
                    return false;

                return qTopic == topic;
            })
            .ToList();

        // Debug log
        foreach (var item in filtered)
        {
            Debug.Log($"ID:{item.id}, Topic:{item.type}, Difficulty:{item.difficulty}, Question:{item.questionString}, Answer:{item.answer}");
        }

        return filtered
            .Select(q =>
            {
                // Parse safely
                Enum.TryParse(q.type, true, out MathTopic parsedTopic);

                QuestionDifficulty parsedDiff;
                if (!Enum.TryParse(q.difficulty, true, out parsedDiff))
                    parsedDiff = QuestionDifficulty.Easy; // fallback

                return new MathQuestionJSON(
                    q.id,
                    parsedTopic,
                    parsedDiff,
                    q.questionString,
                    q.answer, // single answer
                    q.answers, // multiple answers
                    q.hints
                );
            })
            .OrderBy(q => q.difficulty) // sort by difficulty
            .ToList();
    }

    public static List<MathQuestionJSON> LoadByTopic(MathTopic topic, HashSet<int> usedIds = null)
    {
        EnsureDatabaseLoaded();

        var filtered = database.questions
            .Where(q =>
            {
                // Parse topic
                if (!Enum.TryParse(q.type, true, out MathTopic qTopic))
                    return false;

                return qTopic == topic &&
                       (usedIds == null || !usedIds.Contains(q.id));
            })
            .ToList();
        // Convert to MathQuestion objects and send
        return filtered
            .Select(q =>
            {
                // Parse BOTH topic and difficulty safely
                Enum.TryParse(q.type, true, out MathTopic parsedTopic);

                QuestionDifficulty parsedDiff;
                if (!Enum.TryParse(q.difficulty, true, out parsedDiff))
                    parsedDiff = QuestionDifficulty.Easy; // fallback

                return new MathQuestionJSON(
                    q.id,
                    parsedTopic,
                    parsedDiff,
                    q.questionString,
                    q.answer, // single answer
                    q.answers, // multiple answers
                    q.hints
                );
            })
            .OrderBy(q => q.difficulty) // sort by difficulty
            .ToList();
    }
}
