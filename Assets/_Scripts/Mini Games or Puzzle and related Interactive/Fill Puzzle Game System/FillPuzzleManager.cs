using UnityEngine;
using UnityEngine.UI;

public class FillPuzzleManager : MonoBehaviour
{
    public PuzzleSlotUI[] slots; // manually assign in inspector
    public Button checkAnswerButton;

    void Awake()
    {
        checkAnswerButton.onClick.AddListener(CheckAnswer);
    }

    public void CheckAnswer()
    {
        foreach (var slot in slots)
        {
            if (!slot.IsCorrect())
            {
                Debug.Log("INCORRECT — try again");
                return;
            }
        }

        Debug.Log("CORRECT CORRECT CORRECT");
        // unlock next step / close puzzle / give reward
    }
}
