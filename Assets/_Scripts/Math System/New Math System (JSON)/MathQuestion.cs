public enum MathTopic
{
    Permutation_and_Its_Conditions,
    Factorials,
    Permutation_Formula,
    Distinguishable_Permutation,
    Circular_Permutation,
    Problem_Solving_on_Permutation,
    Illustrates_the_Combination_of_an_Objects,
    Permutation_vs_Combination,
    Combination_Notation,
    Evaluating_Combination_Notation,
    Problem_Solving_on_Permutation_and_Combination,
    Union_and_Intersection_of_Events,
    Simple_Probability,
    Probability_of_Two_Events,
    Mutually_Exclusive_Events,
    Probability_Using_Permutations_and_Combinations,
    Probability_of_Independent_and_Dependent_Events,
    Conditional_Probability
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
