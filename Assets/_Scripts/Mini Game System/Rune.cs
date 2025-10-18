using UnityEngine;
using UnityEngine.EventSystems;

public class Rune : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string runeID; // e.g., "A", "B", "C"

    private Transform originalParent;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Transform OriginalParent { get; private set; }


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OriginalParent = transform.parent; // Store original parent when drag starts
        transform.SetParent(canvas.transform); // Move to top layer
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        // You can add logic here to snap back if not dropped in a slot
        if (transform.parent == canvas.transform)
        {
            // If not dropped in a slot, return to original position
            transform.SetParent(OriginalParent);
            transform.localPosition = Vector3.zero;
        }
    }


}
