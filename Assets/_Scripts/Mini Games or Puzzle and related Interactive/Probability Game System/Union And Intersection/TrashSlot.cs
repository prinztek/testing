using UnityEngine;
using UnityEngine.EventSystems;

public class TrashSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        UnionAndIntersectionElement element =
            eventData.pointerDrag?.GetComponent<UnionAndIntersectionElement>();

        if (element != null) // only destroy if the element is not from the bank
        {
            Destroy(element.gameObject);
        }
    }
}
