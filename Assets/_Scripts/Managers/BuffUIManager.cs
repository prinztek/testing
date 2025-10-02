using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuffUIManager : MonoBehaviour
{
    public GameObject buffSlotPrefab;
    public Transform buffPanel;

    private Dictionary<object, GameObject> buffSlots = new Dictionary<object, GameObject>();

    // ===== LEGACY BUFF SUPPORT =====
    public void AddBuffUI(Buff buff)
    {
        if (buffSlots.ContainsKey(buff)) return;

        GameObject slot = Instantiate(buffSlotPrefab, buffPanel);
        buffSlots[buff] = slot;

        UpdateBuffSlot(buff);
    }

    public void RemoveBuffUI(Buff buff)
    {
        if (buffSlots.TryGetValue(buff, out GameObject slot))
        {
            Destroy(slot);
            buffSlots.Remove(buff);
        }
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
    }

    // ===== NEW SCRIPTABLE BUFF SUPPORT =====
    public void AddBuffUI(BuffInstance buffInstance)
    {
        if (buffSlots.ContainsKey(buffInstance)) return;

        GameObject slot = Instantiate(buffSlotPrefab, buffPanel);
        buffSlots[buffInstance] = slot;

        UpdateBuffSlot(buffInstance);
    }

    public void RemoveBuffUI(BuffInstance buffInstance)
    {
        if (buffSlots.TryGetValue(buffInstance, out GameObject slot))
        {
            Destroy(slot);
            buffSlots.Remove(buffInstance);
        }
    }

    public void UpdateBuffSlot(BuffInstance buffInstance)
    {
        if (!buffSlots.ContainsKey(buffInstance)) return;

        GameObject slot = buffSlots[buffInstance];

        TMP_Text text = slot.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = buffInstance.GetUIDisplay();
        }

        Image icon = slot.transform.Find("Icon")?.GetComponent<Image>();
        if (icon != null)
        {
            icon.sprite = buffInstance.GetIcon();
        }
    }

    public void UpdateAll(List<BuffInstance> activeBuffInstances)
    {
        foreach (var buffInstance in activeBuffInstances)
        {
            UpdateBuffSlot(buffInstance);
        }
    }

    // ===== Utility to Clear All UI =====
    public void ClearAll()
    {
        foreach (var kvp in buffSlots)
        {
            Destroy(kvp.Value);
        }

        buffSlots.Clear();
    }
}
