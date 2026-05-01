using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public RectTransform panel;
    public TextMeshProUGUI text;

    // public void Show(string message, RectTransform target, Vector2 offset)
    // {
    //     text.text = message;
    //     gameObject.SetActive(true);

    //     // Position relative to target
    //     Vector3 worldPos = target.transform.position;
    //     panel.position = worldPos + (Vector3)offset;
    // }

    public void Show(string message, RectTransform target, Vector2 offset)
    {
        text.text = message;
        gameObject.SetActive(true);

        RectTransform canvasRect = panel.root as RectTransform;

        Vector2 localPoint;

        // Convert world position → canvas local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(null, target.position),
            null,
            out localPoint
        );

        // Apply offset in UI space
        panel.anchoredPosition = localPoint + offset;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}