using UnityEngine;

[System.Serializable]
public class BuffOption
{
    public string name;
    public string description;
    public Sprite icon;

    // A function to create an instance of the actual buff
    public System.Func<Buff> CreateBuff;

    public BuffOption(string name, string description, Sprite icon, System.Func<Buff> createBuff)
    {
        this.name = name;
        this.description = description;
        this.icon = icon;
        this.CreateBuff = createBuff;
    }
}

