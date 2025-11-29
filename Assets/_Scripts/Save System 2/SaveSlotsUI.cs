using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SaveSlotsUI : MonoBehaviour
{
    [Header("UI References")]
    public Button slot1Button;
    public Button slot2Button;
    public Button slot3Button;

    public TextMeshProUGUI slot1Text;
    public TextMeshProUGUI slot2Text;
    public TextMeshProUGUI slot3Text;

    private void Start()
    {
        SaveLoadManager.Instance.LoadGlobal();  // load global.json first
        RefreshSlots();

        slot1Button.onClick.AddListener(() => OnSlotSelected(1));
        slot2Button.onClick.AddListener(() => OnSlotSelected(2));
        slot3Button.onClick.AddListener(() => OnSlotSelected(3));
    }

    // -------------------------
    // UI text update
    // -------------------------
    private void RefreshSlots()
    {
        slot1Text.text = File.Exists(GetSlotPath(1)) ? "Slot 1 (Continue)" : "Slot 1 (Empty)";
        slot2Text.text = File.Exists(GetSlotPath(2)) ? "Slot 2 (Continue)" : "Slot 2 (Empty)";
        slot3Text.text = File.Exists(GetSlotPath(3)) ? "Slot 3 (Continue)" : "Slot 3 (Empty)";
    }

    private string GetSlotPath(int slot)
    {
        return Application.persistentDataPath + $"/save_slot_{slot}.json";
    }

    // -------------------------
    // When a slot is clicked
    // -------------------------
    private void OnSlotSelected(int slot)
    {
        // Save global last-used slot
        SaveLoadManager.Instance.globalData.lastUsedSlot = slot;
        SaveLoadManager.Instance.SaveGlobal();

        // If empty → new save
        if (!File.Exists(GetSlotPath(slot)))
        {
            Debug.Log($"Creating new save slot {slot}");
            SaveLoadManager.Instance.saveData = new JSONSaveData2();
            SaveLoadManager.Instance.playerData = new JSONPlayerData2();
            SaveLoadManager.Instance.questionData = new JSONUsedMathQuestionData2();
            SaveLoadManager.Instance.SaveSlot(slot);
        }
        else
        {
            Debug.Log($"Loading existing save slot {slot}");
            SaveLoadManager.Instance.LoadSlot(slot);
        }

        // Continue to next scene (your main menu or level select)
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
