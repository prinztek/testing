using System.Collections.Generic;

public static class MathQuestionLoader
{
    public static List<MathQuestion> Load(MathTopic topic, QuestionDifficulty difficulty, int total = 3)
    {
        List<MathQuestion> questions = new();
        questions.AddRange(MathQuestionDatabase.GetHardcodedQuestions(topic, difficulty));

        // while (questions.Count < total)
        // {
        //     questions.Add(MathQuestionGenerator.Generate(topic, difficulty));
        // }

        return questions;
    }
}
