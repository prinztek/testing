using TMPro;
using UnityEngine;

public class InputFieldGrabber : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("The value we got from the input field")]
    [SerializeField] private string inputText;

    [Header("Showing the reaction to the player")]
    [SerializeField] private GameObject reactionGroup;
    [SerializeField] private TMP_Text reactionTextBox;

    public void GrabFromInputField(string input)
    {
        inputText = input;
    }

    private void DisplayReactionToInput()
    {
        reactionTextBox.text = "Welcome to the team, " + inputText + "!";
        reactionGroup.SetActive(true);
    }
}
