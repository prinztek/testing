using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class OutcomeSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI countText;

    [Header("Validation")]
    public List<string> correctValues = new List<string>();
    public List<string> userSelectedValues = new List<string>();

    // ============================
    // Clear All Values
    // ============================
    public void ClearSlot()
    {
        userSelectedValues.Clear();
        UpdateUI();
    }

    // ============================
    // Add Value
    // ============================
    public void Add(string value)
    {
        if (userSelectedValues.Contains(value))
            return; // prevent duplicate

        userSelectedValues.Add(value);
        UpdateUI();
    }

    // ============================
    // Remove Value
    // ============================
    public void Remove(string value)
    {
        if (!userSelectedValues.Contains(value))
            return;

        userSelectedValues.Remove(value);
        UpdateUI();
    }

    // ============================
    // Count
    // ============================
    public int GetCount()
    {
        return userSelectedValues.Count;
    }

    // ============================
    // Reset
    // ============================
    public void ResetSlot()
    {
        userSelectedValues.Clear();
        UpdateUI();
    }

    // ============================
    // Correctness Check
    // ============================
    public bool IsCorrect()
    {
        if (userSelectedValues.Count != correctValues.Count)
            return false;

        foreach (var value in correctValues)
        {
            if (!userSelectedValues.Contains(value))
                return false;
        }

        return true;
    }

    // ============================
    // UI Update
    // ============================
    private void UpdateUI()
    {
        if (countText != null)
            countText.text = userSelectedValues.Count.ToString();
    }
}