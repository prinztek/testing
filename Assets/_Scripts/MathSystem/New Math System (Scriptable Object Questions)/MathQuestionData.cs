using UnityEngine;

public enum MathQuestionType
{
    Permutation,
    Combination,
    Probability
}

public enum MathQuestionDifficulty
{
    Easy = 1,
    Medium = 2,
    Hard = 3
}

[CreateAssetMenu(fileName = "NewMathQuestion", menuName = "Math/Question", order = 1)]
public class MathQuestionData : ScriptableObject
{
    [Header("Question Content")]
    [TextArea(3, 5)]
    public string question;

    public string simplifiedQuestion;

    public MathQuestionType type;

    public QuestionDifficulty difficulty;

    [Header("Formula")]
    [TextArea(2, 4)]
    public string formula;

    [Header("Hints")]
    public string hint1;
    public string hint2;
    public string hint3;

    [Header("Answer")]
    public string answer;

    [HideInInspector]
    public bool isUsed = false;
}
