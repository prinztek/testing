using UnityEngine;
using UnityEngine.EventSystems;

public class Rune : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string runeID; // e.g., "A", "B", "C"
    public RuneSlot CurrentSlot { get; set; }
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup; // for raycast blocking
    public Transform runePoolParent;
    public bool isFixed = false; // to indicate if the rune is fixed in place

    public Vector3 dragScale = new Vector3(1.4f, 1.4f, 1.4f); // scale when dragging
    private Vector3 originalScale;

    // drag and drop sound clips
    public AudioClip selectSound;
    private AudioClip dropSound;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        runePoolParent = transform.parent; // fixed, permanent
        originalScale = rectTransform.localScale; // save original size
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isFixed)
        {
            return;
        }

        // detach from previous slot
        if (CurrentSlot != null)
        {
            CurrentSlot.ClearSlot();
            CurrentSlot = null;
        }

        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;

        // Increase size
        rectTransform.localScale = dragScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Reset size
        rectTransform.localScale = originalScale;

        // If no slot took us, return to pool
        if (CurrentSlot == null)
        {
            transform.SetParent(runePoolParent);
            transform.localPosition = Vector3.zero;
        }
    }
}
