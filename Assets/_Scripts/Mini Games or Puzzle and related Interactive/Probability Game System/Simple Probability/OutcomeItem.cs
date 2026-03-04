using System;
using UnityEngine;
using UnityEngine.UI;

public class OutcomeItem : MonoBehaviour
{
    [Header("Selection Visuals")]
    [SerializeField] private Image numeratorSelector;
    [SerializeField] private Image denominatorSelector;

    [Header("Reference")]
    [SerializeField] private SimpleProbabilityManager manager;

    public string value = "";

    private bool selectedForNumerator = false;
    private bool selectedForDenominator = false;

    public void Start()
    {
        numeratorSelector.enabled = false;
        denominatorSelector.enabled = false;
    }

    // Called by Button component
    public void OnClick()
    {
        manager.HandleOutcomeItemClick(this);
    }

    // ============================
    // Numerator Toggle
    // ============================

    public void ToggleNumerator(OutcomeSlot slot)
    {
        selectedForNumerator = !selectedForNumerator;

        // selector visual
        numeratorSelector.enabled = selectedForNumerator;

        // add/remove from slot
        if (selectedForNumerator)
            slot.Add(value);
        else
            slot.Remove(value);
    }

    // ============================
    // Denominator Toggle
    // ============================

    public void ToggleDenominator(OutcomeSlot slot)
    {
        selectedForDenominator = !selectedForDenominator;

        // selector visual
        denominatorSelector.enabled = selectedForDenominator;

        // add/remove from slot
        if (selectedForDenominator)
            slot.Add(value);
        else
            slot.Remove(value);
    }
}