using UnityEngine;
using UnityEngine.EventSystems;

public class TrashSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        UnionAndIntersectionElement element = eventData.pointerDrag?.GetComponent<UnionAndIntersectionElement>();

        if (element == null) return;

        ElementsSlot slot = element.CurrentSlot.GetComponent<ElementsSlot>();
        Debug.Log(slot.name);

        if (slot != null)
        {
            slot.RemoveElement(element);
        }

        if (!element.isFromPool)
        {
            Destroy(element.gameObject);
        }
    }
}
