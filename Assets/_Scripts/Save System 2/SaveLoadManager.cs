using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ======================================
    // 🔵 GLOBAL SETTINGS DATA (global.json)
    // ======================================

    private string GlobalPath => Application.persistentDataPath + "/global.json";

    public JSONGlobalData globalData = new JSONGlobalData();

    public void SaveGlobal()
    {
        string json = JsonConvert.SerializeObject(globalData, Formatting.Indented);
        File.WriteAllText(GlobalPath, json);
    }

    public void LoadGlobal()
    {
        if (!File.Exists(GlobalPath))
        {
            globalData = new JSONGlobalData();
            SaveGlobal();
            return;
        }

        string json = File.ReadAllText(GlobalPath);
        globalData = JsonConvert.DeserializeObject<JSONGlobalData>(json);
    }

    // ======================================
    // 🔵 PER-SLOT DATA (save_slot_X.json)
    // ======================================

    private string SlotPath(int slot) =>
        Application.persistentDataPath + $"/save_slot_{slot}.json";

    public JSONSaveData2 saveData = new JSONSaveData2();
    public JSONPlayerData2 playerData = new JSONPlayerData2();
    public JSONUsedMathQuestionData2 questionData = new JSONUsedMathQuestionData2();

    // Wrapper to combine all 3 into 1 file
    private class SlotWrapper
    {
        public JSONSaveData2 save;
        public JSONPlayerData2 player;
        public JSONUsedMathQuestionData2 questions;
    }

    public void SaveSlot(int slot)
    {
        SlotWrapper wrapper = new SlotWrapper
        {
            save = saveData,
            player = playerData,
            questions = questionData
        };

        string json = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
        File.WriteAllText(SlotPath(slot), json);
    }

    public void LoadSlot(int slot)
    {
        string path = SlotPath(slot);

        if (!File.Exists(path))
        {
            saveData = new JSONSaveData2();
            playerData = new JSONPlayerData2();
            questionData = new JSONUsedMathQuestionData2();
            SaveSlot(slot);
            return;
        }

        string json = File.ReadAllText(path);
        SlotWrapper wrapper = JsonConvert.DeserializeObject<SlotWrapper>(json);

        saveData = wrapper.save;
        playerData = wrapper.player;
        questionData = wrapper.questions;
    }
}
