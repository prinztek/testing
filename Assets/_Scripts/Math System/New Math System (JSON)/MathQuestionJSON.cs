using UnityEngine;
// public enum MathTopic { Permutation, Combination, Probability }
// public enum QuestionDifficulty { Easy = 1, Medium = 2, Hard = 3 }

[System.Serializable]
public class MathQuestionJSON
{
    // Defines your data model - A data model is a visual and logical representation that
    // shows how data is organized, structured, and related within a system or domain
    public int id;
    public string questionString;
    public string type; // "Permutation", "Combination", "Probability" = Topic in MathQuestion
    public string difficulty; // "Easy", "Medium", "Hard" = Difficulty in MathQuestion
    public string simplifiedQuestionString;
    public string formulaNeeded;
    public string[] hints;
    public float answer;
    public float[] answers; // used for union and intersection questions
    public string prompt => questionString;

    public MathQuestionJSON(int id, MathTopic topic, QuestionDifficulty difficulty, string prompt, float answer, string[] hints)
    {
        this.id = id;
        this.type = topic.ToString();
        this.difficulty = difficulty.ToString();
        this.questionString = prompt;
        this.answer = answer;
        this.hints = hints;
    }
}

[System.Serializable]
public class MathQuestionDatabaseJSON
{
    public MathQuestionJSON[] questions;


}
