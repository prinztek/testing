using UnityEngine;
using UnityEngine.EventSystems;

public class UnionAndIntersectionElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int value; // These are the individual items listed inside the curly braces of a set, such as  in the set 
    public ElementsSlot CurrentSlot { get; set; } // Events A, B or Universal Set
    RectTransform rectTransform;
    Canvas canvas;
    CanvasGroup canvasGroup;
    Transform elementPoolParent;
    [SerializeField] private UnionAndIntersectionElement prefabReference;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip onDragSoundClip;
    [SerializeField] private AudioClip onDropSoundClip;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        elementPoolParent = transform.parent; // fixed, permanent
    }

    // public void OnBeginDrag(PointerEventData eventData)
    // {
    //     if (onDragSoundClip != null)
    //     {
    //         SoundFXManager.Instance.playOneShotSoundFXClilp(onDragSoundClip, transform, 0.2f);
    //     }

    //     transform.SetParent(canvas.transform);
    //     canvasGroup.blocksRaycasts = false;
    // }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CurrentSlot = null;

        // If this object came from pool, refill it
        if (transform.parent == elementPoolParent)
        {
            Instantiate(prefabReference, elementPoolParent);
        }

        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;

        if (onDragSoundClip != null)
        {
            SoundFXManager.Instance.playOneShotSoundFXClilp(onDragSoundClip, transform, 0.2f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (onDropSoundClip != null)
        {
            SoundFXManager.Instance.playOneShotSoundFXClilp(onDropSoundClip, transform, 0.2f);
        }

        canvasGroup.blocksRaycasts = true;

        // If no slot took us, destroy since this is a copy from the pool
        if (CurrentSlot == null)
        {
            // transform.SetParent(elementPoolParent);
            // transform.localPosition = Vector3.zero;
            Destroy(gameObject);
        }
    }
}
