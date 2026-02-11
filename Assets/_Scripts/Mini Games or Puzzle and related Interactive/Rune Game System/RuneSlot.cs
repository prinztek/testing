using UnityEngine;
using UnityEngine.EventSystems;

public class RuneSlot : MonoBehaviour, IDropHandler
{
    public Rune placedRune;
    public bool isLocked = false;
    [SerializeField] private Transform runePoolParent;
    public void OnDrop(PointerEventData eventData)
    {
        Rune rune = eventData.pointerDrag?.GetComponent<Rune>();
        if (rune == null) return;

        PlaceRune(rune);
    }

    private void PlaceRune(Rune rune)
    {
        if (isLocked)
        {
            return; // Slot is locked, do nothing
        }

        // Replace existing rune
        if (placedRune != null)
        {
            placedRune.CurrentSlot = null;
            placedRune.transform.SetParent(runePoolParent);
            placedRune.transform.localPosition = Vector3.zero;
        }

        placedRune = rune;
        rune.CurrentSlot = this;

        rune.transform.SetParent(transform);
        rune.transform.localPosition = Vector3.zero;
    }

    public void ClearSlot()
    {
        placedRune = null;
    }
}
