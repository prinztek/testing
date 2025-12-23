using UnityEngine;
using TMPro;

public class MarioNumberBlock : MonoBehaviour
{
    public int value = 0;
    public int increaseAmount = 1;
    public int decreaseAmount = 1;

    private int minValue = -1;
    private int maxValue = 10;

    [Header("Optional Text Display")]
    public TextMeshPro numberText;

    void Start()
    {
        if (numberText == null)
            numberText = GetComponentInChildren<TextMeshPro>();

        UpdateText();
    }

    // decrease
    public void HitTop()
    {
        if (value > minValue)
        {
            value -= decreaseAmount;
            UpdateText();
        }
        else
        {
            return;
        }
    }

    // increase
    public void HitBottom()
    {
        if (value < maxValue)
        {
            value += increaseAmount;
            UpdateText();
        }
        else
        {
            return;
        }

    }

    void UpdateText()
    {
        if (numberText != null)
            numberText.text = value.ToString();
    }
}
