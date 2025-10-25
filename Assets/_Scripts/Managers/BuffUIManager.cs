using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuffUIManager : MonoBehaviour
{
    public GameObject buffSlotPrefab;
    public Transform buffPanel;
    public GameObject canvas; // Reference to the canvas or panel holding the buff UI

    public StatusEffectManager statusEffectManager;

    private Dictionary<object, GameObject> buffSlots = new Dictionary<object, GameObject>();

    // This function shows or hides the canvas based on the active buffs
    private void ToggleCanvasVisibility()
    {
        if (buffSlots.Count > 0)  // If there are buffs, show the canvas
        {
            canvas.SetActive(true);
        }
        else  // No buffs, hide the canvas
        {
            canvas.SetActive(false);
        }
    }

    // ===== LEGACY BUFF SUPPORT =====
    public void AddBuffUI(Buff buff)
    {
        if (buffSlots.ContainsKey(buff)) return;

        GameObject slot = Instantiate(buffSlotPrefab, buffPanel);
        buffSlots[buff] = slot;

        UpdateBuffSlot(buff);

        // Ensure the canvas is visible when a buff is added
        ToggleCanvasVisibility();
        statusEffectManager.ShowBuffIcon(buff);
    }

    public void RemoveBuffUI(Buff buff)
    {
        if (buffSlots.TryGetValue(buff, out GameObject slot))
        {
            Destroy(slot);
            buffSlots.Remove(buff);
        }

        // Ensure the canvas is updated when a buff is removed
        ToggleCanvasVisibility();

        statusEffectManager.HideBuffIcon();
    }

    public void UpdateBuffSlot(Buff buff)
    {
        if (!buffSlots.ContainsKey(buff)) return;

        GameObject slot = buffSlots[buff];

        TMP_Text text = slot.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = buff.GetUIDisplay();
        }

        Image icon = slot.transform.Find("Icon")?.GetComponent<Image>();
        if (icon != null)
        {
            icon.sprite = buff.GetIcon();
        }
    }

    public void UpdateAll(List<Buff> activeBuffs)
    {
        foreach (var buff in activeBuffs)
        {
            UpdateBuffSlot(buff);
        }

        // Ensure the canvas is visible or hidden based on active buffs
        ToggleCanvasVisibility();
    }

    // ===== Utility to Clear All UI =====
    public void ClearAll()
    {
        foreach (var kvp in buffSlots)
        {
            Destroy(kvp.Value);
        }

        buffSlots.Clear();

        // Hide canvas if no buffs are left
        ToggleCanvasVisibility();
    }
}
