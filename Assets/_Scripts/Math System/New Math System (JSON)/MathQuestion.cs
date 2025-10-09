public enum MathTopic
{
    Permutation_and_Its_Conditions,
    Factorials,
    Permutation_Formula,
    Distinguishable_Permutation,
    Circular_Permutation,
    Problem_Solving_on_Permutation
}
public enum QuestionDifficulty { Easy = 1, Medium = 2, Hard = 3 }

public class MathQuestion
{
    public int id;  // Unique ID for tracking
    public MathTopic topic;
    public QuestionDifficulty difficulty;
    public string prompt;
    public string answer; // As string to allow fractions or words

    public string[] hints; // Array of hints for the question
    public bool isGenerated; // for random generated questions later

    public MathQuestion(int id, MathTopic topic, QuestionDifficulty difficulty, string prompt, string answer, string[] hints, bool isGenerated = false)
    {
        this.id = id;
        this.topic = topic;
        this.difficulty = difficulty;
        this.prompt = prompt;
        this.answer = answer;
        this.hints = hints;
        this.isGenerated = isGenerated;
    }

}
