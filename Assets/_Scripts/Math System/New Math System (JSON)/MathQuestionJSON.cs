using UnityEngine;
// public enum MathTopic { Permutation, Combination, Probability }
// public enum QuestionDifficulty { Easy = 1, Medium = 2, Hard = 3 }

[System.Serializable]
public class MathQuestionJSON
{
    public int id;
    public string questionString;
    public string type; // "Permutation", "Combination", "Probability" = Topic in MathQuestion
    public string difficulty; // "Easy", "Medium", "Hard" = Difficulty in MathQuestion
    public string simplifiedQuestionString;
    public string formulaNeeded;
    public string[] hints;
    public float answer; // used for single correct answer
    public float[] answers; // used for union and intersection questions {2.3}
    public string prompt => questionString;

    public MathQuestionJSON(int id, MathTopic topic, QuestionDifficulty difficulty, string prompt, float answer, float[] answers, string[] hints)
    {
        this.id = id;
        this.type = topic.ToString();
        this.difficulty = difficulty.ToString();
        this.questionString = prompt;
        this.answer = answer;
        this.answers = answers ?? new float[0];  // default to empty array if null
        this.hints = hints;
    }
}

[System.Serializable]
public class MathQuestionDatabaseJSON
{
    public MathQuestionJSON[] questions;
}
