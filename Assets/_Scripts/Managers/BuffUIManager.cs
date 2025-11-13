using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuffUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject buffSlotPrefab;
    [SerializeField] private GameObject buffPanel;
    [SerializeField] private StatusEffectManager statusEffectManager;

    private readonly Dictionary<Buff, GameObject> buffSlots = new();

    void Awake()
    {
        if (buffPanel != null)
            buffPanel.SetActive(false);
    }

    public void AddBuffUI(Buff buff)
    {
        if (buff == null || buffSlots.ContainsKey(buff)) return;

        GameObject slot = Instantiate(buffSlotPrefab, buffPanel.transform);
        buffSlots[buff] = slot;
        buffPanel.SetActive(true);
        UpdateBuffSlot(buff);

        StatusEffectManager.Instance?.ShowBuffIcon(buff); // show fx on top of the character
    }

    public void RemoveBuffUI(Buff buff)
    {
        if (buff == null) return;

        if (buffSlots.TryGetValue(buff, out GameObject slot))
        {
            Destroy(slot);
            buffSlots.Remove(buff);
        }

        if (buffSlots.Count == 0)
            buffPanel.SetActive(false);

        StatusEffectManager.Instance?.HideBuffIcon(); // hide fx on top of the character
    }

    public void UpdateBuffSlot(Buff buff)
    {
        if (!buffSlots.TryGetValue(buff, out GameObject slot)) return;

        TMP_Text text = slot.GetComponentInChildren<TMP_Text>();
        Image icon = slot.transform.Find("Icon")?.GetComponent<Image>();

        if (text != null)
            text.text = buff.GetUIDisplay();

        if (icon != null)
            icon.sprite = buff.GetIcon();
    }

    public void UpdateAll(List<Buff> activeBuffs)
    {
        foreach (var buff in activeBuffs)
            UpdateBuffSlot(buff);
    }

    public void ClearAll()
    {
        foreach (var kvp in buffSlots)
            Destroy(kvp.Value);

        buffSlots.Clear();
    }
}
