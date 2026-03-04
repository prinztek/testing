using UnityEngine;
using UnityEngine.EventSystems;

public class BankElement : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private UnionAndIntersectionElement draggablePrefab;
    [SerializeField] private Canvas canvas;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Spawn clone
        UnionAndIntersectionElement clone =
            Instantiate(draggablePrefab, canvas.transform);

        RectTransform cloneRect = clone.GetComponent<RectTransform>();
        cloneRect.position = transform.position;

        // 🔥 CRITICAL PART
        eventData.pointerDrag = clone.gameObject;

        // Manually trigger clone drag start
        clone.OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Do nothing
    }
}