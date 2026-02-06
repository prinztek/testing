using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleSlotUI : MonoBehaviour, IDropHandler
{
    public string correctValue; // expected answer
    public PuzzleBlockUI placedBlock;

    public void OnDrop(PointerEventData eventData)
    {
        PuzzleBlockUI block =
            eventData.pointerDrag?.GetComponent<PuzzleBlockUI>();

        if (block == null) return;

        PlaceBlock(block);
    }

    void PlaceBlock(PuzzleBlockUI block)
    {
        if (placedBlock != null)
        {
            placedBlock.CurrentSlot = null;
            placedBlock.transform.SetParent(block.transform.parent);
        }

        placedBlock = block;
        block.CurrentSlot = this;

        block.transform.SetParent(transform);
        block.transform.localPosition = Vector3.zero;
    }

    public bool IsCorrect()
    {
        return placedBlock != null &&
               placedBlock.value == correctValue;
    }

    public void ClearSlot()
    {
        placedBlock = null;
    }
}
