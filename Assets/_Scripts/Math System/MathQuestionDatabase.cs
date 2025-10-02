using UnityEngine;
using System.Collections.Generic;

public static class MathQuestionDatabase
{
    public static List<MathQuestion> GetHardcodedQuestions(MathTopic topic, QuestionDifficulty difficulty)
    {
        var list = new List<MathQuestion>();

        if (topic == MathTopic.Permutation && difficulty == QuestionDifficulty.Easy)
        {
            list.Add(new MathQuestion(1, topic, difficulty, "How many ways can you arrange C, B, and A?", "6"));
            list.Add(new MathQuestion(2, topic, difficulty, "How many ways can you arrange 1, 2, and 3?", "6"));
            list.Add(new MathQuestion(3, topic, difficulty, "Arrange 2 out of 3 books", "6"));
        }

        // Add more based on topic and difficulty

        return list;
    }
}
