using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElementsSlot : MonoBehaviour, IDropHandler
{
    public List<int> correctElements = new List<int>();
    public List<int> userElements = new List<int>(); // record of elements placed by the user
    public List<UnionAndIntersectionElement> placedElements = new List<UnionAndIntersectionElement>(); [SerializeField] private Transform puzzleBlockPoolParent;
    public void OnDrop(PointerEventData eventData)
    {
        UnionAndIntersectionElement block = eventData.pointerDrag?.GetComponent<UnionAndIntersectionElement>();

        if (block == null) return;

        PlaceElement(block);
    }

    void PlaceElement(UnionAndIntersectionElement element)
    {
        element.CurrentSlot = this;

        element.transform.SetParent(transform);
        element.transform.localPosition = Vector3.zero;

        placedElements.Add(element);
    }

    void RetrieveElementsValue()
    {
        userElements.Clear();

        foreach (var element in placedElements)
        {
            userElements.Add(element.value);
        }
    }
    // public bool CheckSequence()
    public bool IsCorrect()
    {
        RetrieveElementsValue();
        foreach (var element in userElements)
        {
            Debug.Log(element);
        }

        // The player has submitted the correct number of sequences
        if (userElements.Count != correctElements.Count)
        {
            // Not enough sequences submitted yet
            return false;
        }

        // Check that every correct sequence is included in the user's submissions
        foreach (var correcorrectElement in correctElements)
        {
            if (!userElements.Contains(correcorrectElement))
            {
                // The user is missing at least one required sequence
                return false;
            }
        }

        // Step 3: double-check that all user sequences are valid (the user didn't add any extra invalid sequences)
        foreach (var userElement in userElements)
        {
            if (!correctElements.Contains(userElement))
            {
                // The user submitted an invalid sequence
                return false;
            }
        }

        return true; // The user submitted all sequences correctly
    }
}
