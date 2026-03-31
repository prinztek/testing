using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElementsSlot : MonoBehaviour, IDropHandler
{
    // this acts like an event A or event B
    public List<int> correctElements = new List<int>();
    public List<int> userElements = new List<int>(); // record of elements placed by the user
    public List<UnionAndIntersectionElement> placedElements = new List<UnionAndIntersectionElement>();
    [SerializeField] private Transform puzzleBlockPoolParent;

    [SerializeField] private AudioClip onDropSoundClip;

    public void OnDrop(PointerEventData eventData)
    {
        UnionAndIntersectionElement block = eventData.pointerDrag?.GetComponent<UnionAndIntersectionElement>();

        if (block == null) return;

        PlaceElement(block);
    }

    // void PlaceElement(UnionAndIntersectionElement element)
    // {
    //     // you can't place the same element twice if canOnlyBeUsedOnce is true
    //     // if (element.canOnlyBeUsedOnce) return;
    //     // loop through placed elements and see if the user is trying to place the same element again
    //     foreach (var placedElement in placedElements)
    //     {
    //         if (placedElement.value == element.value)
    //         {
    //             // the user is trying to place the same element again, so we ignore this placement
    //             return;
    //         }
    //     }


    //     element.CurrentSlot = this;

    //     element.transform.SetParent(transform);
    //     element.transform.localPosition = Vector3.zero;

    //     placedElements.Add(element);
    // }

    // void PlaceElement(UnionAndIntersectionElement element)
    // {
    //     // Remove from previous slot first
    //     if (element.CurrentSlot != null)
    //     {
    //         element.CurrentSlot.RemoveElement(element);
    //     }

    //     // prevent duplicate values
    //     foreach (var placedElement in placedElements)
    //     {
    //         if (placedElement.value == element.value)
    //         {
    //             return;
    //         }
    //     }

    //     if (placedElements.Contains(element))
    //         return;

    //     element.CurrentSlot = this;

    //     element.transform.SetParent(transform);
    //     element.transform.localPosition = Vector3.zero;

    //     placedElements.Add(element);
    // }

    void PlaceElement(UnionAndIntersectionElement element)
    {
        // If the element already belongs to a slot, remove it first
        if (element.CurrentSlot != null)
        {
            element.CurrentSlot.RemoveElement(element);
        }

        // Prevent duplicate values inside this slot
        foreach (var placedElement in placedElements)
        {
            if (placedElement.value == element.value)
            {
                return;
            }
        }

        element.CurrentSlot = this;

        element.transform.SetParent(transform);
        element.transform.localPosition = Vector3.zero;

        placedElements.Add(element);

        if (onDropSoundClip != null)
        {
            SoundFXManager.Instance.playOneShotSoundFXClilp(onDropSoundClip, transform, 0.2f);
        }
    }

    public void RemoveElement(UnionAndIntersectionElement element)
    {
        element.CurrentSlot = null;

        placedElements.Remove(element);
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

        // double-check that all user sequences are valid (the user didn't add any extra invalid sequences)
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
