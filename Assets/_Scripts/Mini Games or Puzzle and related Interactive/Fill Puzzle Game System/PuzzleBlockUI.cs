using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleBlockUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string value; // "5040", "2", "9!", "5!" // ANY VALUE representing the block, e.g. "5040", "2", "3!"

    public PuzzleSlotUI CurrentSlot { get; set; }

    RectTransform rectTransform;
    Canvas canvas;
    CanvasGroup canvasGroup;
    Transform poolParent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        poolParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CurrentSlot != null)
        {
            CurrentSlot.ClearSlot();
            CurrentSlot = null;
        }

        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (CurrentSlot == null)
        {
            transform.SetParent(poolParent);
            transform.localPosition = Vector3.zero;
        }
    }
}
