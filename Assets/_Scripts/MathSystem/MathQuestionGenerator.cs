using UnityEngine;
public static class MathQuestionGenerator
{
    public static MathQuestion Generate(MathTopic topic, QuestionDifficulty difficulty)
    {
        switch (topic)
        {
            case MathTopic.Permutation:
                return GeneratePermutation(difficulty);
                // Add Combination and Probability next
        }
        return null;
    }

    private static MathQuestion GeneratePermutation(QuestionDifficulty difficulty)
    {
        int n = Random.Range(3, 7);
        int r = Random.Range(2, n);
        int answer = Factorial(n) / Factorial(n - r);
        string prompt = $"How many permutations of {n} items taken {r} at a time?";
        return new MathQuestion(MathTopic.Permutation, difficulty, prompt, answer.ToString(), true);
    }

    private static int Factorial(int n)
    {
        int result = 1;
        for (int i = 2; i <= n; i++) result *= i;
        return result;
    }
}
