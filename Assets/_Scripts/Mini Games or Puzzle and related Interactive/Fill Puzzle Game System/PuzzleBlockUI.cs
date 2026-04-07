using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleBlockUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string value; // "5040", "2", "9!", "5!" // ANY VALUE representing the block, e.g. "5040", "2", "3!"
    public PuzzleSlotUI CurrentSlot { get; set; }
    RectTransform rectTransform;
    Canvas canvas;
    CanvasGroup canvasGroup;
    Transform puzzleBlockPoolParent;

    public Vector3 dragScale = new Vector3(1.4f, 1.4f, 1.4f); // scale when dragging
    private Vector3 originalScale;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip onDragSoundClip;
    [SerializeField] private AudioClip onDropSoundClip;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        puzzleBlockPoolParent = transform.parent; // fixed, permanent
        originalScale = rectTransform.localScale; // save original size
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

        // Increase size
        rectTransform.localScale = dragScale;

        // Play drag sound
        if (onDragSoundClip != null)
        {
            SoundFXManager.Instance.playOneShotSoundFXClilp(onDragSoundClip, transform, 0.3f);
        }
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
            transform.SetParent(puzzleBlockPoolParent);
            transform.localPosition = Vector3.zero;
        }

        // Play drop sound
        if (onDropSoundClip != null)
        {
            SoundFXManager.Instance.playOneShotSoundFXClilp(onDropSoundClip, transform, 0.3f);
        }
    }
}
