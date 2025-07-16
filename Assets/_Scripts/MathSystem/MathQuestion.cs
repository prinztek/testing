using UnityEngine;
public enum MathTopic { Permutation, Combination, Probability }
public enum QuestionDifficulty { Easy = 1, Medium = 2, Hard = 3 }

public class MathQuestion
{
    public MathTopic topic;
    public QuestionDifficulty difficulty;
    public string prompt;
    public string answer; // As string to allow fractions or words
    public bool isGenerated;

    public MathQuestion(MathTopic topic, QuestionDifficulty difficulty, string prompt, string answer, bool isGenerated = false)
    {
        this.topic = topic;
        this.difficulty = difficulty;
        this.prompt = prompt;
        this.answer = answer;
        this.isGenerated = isGenerated;
    }
}
