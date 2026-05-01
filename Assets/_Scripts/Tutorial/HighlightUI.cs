using UnityEngine;
using UnityEngine.UI;

public class HighlightUI : MonoBehaviour
{
    [SerializeField] private RectTransform highlightBorder;
    [SerializeField] private RectTransform cloneRoot; // parent for clones

    private GameObject currentClone;

    public void Show(RectTransform target, Vector2 offset)
    {
        // --- BORDER (your existing logic) ---
        highlightBorder.gameObject.SetActive(true);
        highlightBorder.position = target.position;

        // --- CLONE ---
        CreateClone(target);
    }

    public void Hide()
    {
        highlightBorder.gameObject.SetActive(false);

        if (currentClone != null)
            Destroy(currentClone);
    }

    void CreateClone(RectTransform target)
    {
        // Destroy previous
        if (currentClone != null)
            Destroy(currentClone);

        // Instantiate copy
        currentClone = Instantiate(target.gameObject, cloneRoot);

        // Reset transform
        RectTransform cloneRect = currentClone.GetComponent<RectTransform>();
        cloneRect.position = target.position;
        cloneRect.rotation = target.rotation;
        // cloneRect.localScale = target.lossyScale;

        // Remove interaction
        RemoveInteraction(currentClone);
    }

    void RemoveInteraction(GameObject obj)
    {
        // Disable Unity Button
        var btn = obj.GetComponent<Button>();
        if (btn != null)
        {
            btn.interactable = false;
            btn.enabled = false;
        }

        // Disable your custom tab script
        var tab = obj.GetComponent<TabGroupButton>();
        if (tab != null)
        {
            tab.enabled = false;
        }

        // Disable raycasts on all graphics
        var images = obj.GetComponentsInChildren<Image>(true);
        foreach (var img in images)
        {
            img.raycastTarget = false;
        }
    }
}
// using UnityEngine;
// using TMPro;

// public class HighlightUI : MonoBehaviour
// {
//     public RectTransform panel;

//     // public void Show(string message, RectTransform target, Vector2 offset)
//     // {
//     //     text.text = message;
//     //     gameObject.SetActive(true);

//     //     // Position relative to target
//     //     Vector3 worldPos = target.transform.position;
//     //     panel.position = worldPos + (Vector3)offset;
//     // }

//     public void Show(RectTransform target, Vector2 offset)
//     {
//         gameObject.SetActive(true);

//         RectTransform canvasRect = panel.root as RectTransform;

//         Vector2 localPoint;

//         // Convert world position → canvas local position
//         RectTransformUtility.ScreenPointToLocalPointInRectangle(
//             canvasRect,
//             RectTransformUtility.WorldToScreenPoint(null, target.position),
//             null,
//             out localPoint
//         );

//         // Apply offset in UI space
//         panel.anchoredPosition = localPoint + offset;
//     }

//     public void Hide()
//     {
//         gameObject.SetActive(false);
//     }
// }