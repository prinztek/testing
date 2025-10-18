using UnityEngine;
using UnityEngine.EventSystems;

public class RuneSlot : MonoBehaviour, IDropHandler
{
    public Rune placedRune;

    public void OnDrop(PointerEventData eventData)
    {
        Rune draggedRune = eventData.pointerDrag.GetComponent<Rune>();

        if (draggedRune != null)
        {
            if (placedRune != null)
            {
                // Return the previously placed rune to the dragged rune's original parent
                placedRune.transform.SetParent(draggedRune.OriginalParent);
                placedRune.transform.localPosition = Vector3.zero;
            }

            draggedRune.transform.SetParent(transform);
            draggedRune.transform.localPosition = Vector3.zero;
            placedRune = draggedRune;
        }
    }

}
