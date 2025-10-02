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
    public string answer;

    // 🔹 Convenience property (so existing code using "prompt" still works)
    public string prompt => questionString;
}

[System.Serializable]
public class MathQuestionDatabaseJSON
{
    public MathQuestionJSON[] questions;
}
