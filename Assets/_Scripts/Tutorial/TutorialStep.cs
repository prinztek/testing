using UnityEngine;

[System.Serializable]
public class TutorialStep
{
    public RectTransform target;   // Tab UI element
    [TextArea]
    public string message;         // Tooltip text
    public Vector2 offset;         // Position offset (move a bit to the left)
    public Vector2 highlightPosition; // Calculated position for the highlight
}