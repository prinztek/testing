using UnityEngine;

public static class SavePaths
{
    public static string Root => Application.persistentDataPath + "/Saves";

    public static string Global => Root + "/global.json";

    public static string SlotFolder(int slot) => Root + $"/slot{slot}";
    public static string SlotSave(int slot) => SlotFolder(slot) + "/save.json";
    public static string SlotPlayer(int slot) => SlotFolder(slot) + "/player.json";
    public static string SlotUsedMath(int slot) => SlotFolder(slot) + "/used_math.json";
}
