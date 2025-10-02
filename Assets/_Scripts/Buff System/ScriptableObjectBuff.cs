using UnityEngine;
public abstract class ScriptableObjectBuff : ScriptableObject
{
    public string buffName;
    public float duration;

    public virtual Sprite GetIcon()
    {
        return Resources.Load<Sprite>("Icons/" + buffName);
    }

    // Create a runtime instance of this buff for tracking state
    public abstract BuffInstance CreateInstance(CharacterStats target);
}
