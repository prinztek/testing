using UnityEngine;
public abstract class Buff
{
    public string buffName;
    public float duration;
    protected float remainingTime;
    public bool isExpired => remainingTime <= 0f;

    protected CharacterStats target;

    public Buff(string name, float duration)
    {
        this.buffName = name;
        this.duration = duration;
        this.remainingTime = duration;
    }

    public void Assign(CharacterStats character)
    {
        target = character;
        OnApply();
    }

    public virtual void Update(float deltaTime)
    {
        remainingTime -= deltaTime;
        if (isExpired) OnExpire();
    }

    public abstract void OnApply();
    public abstract void OnExpire();
    public virtual void OnAttackHit(GameObject enemy) { }

    public virtual string GetUIDisplay()
    {
        return $"{buffName} - {remainingTime:F1}s";
    }
    public virtual Sprite GetIcon()
    {
        // Debug.LogWarning($"No icon set for buff: {buffName}. Using default icon.");
        return Resources.Load<Sprite>("Icons/" + buffName); // Assuming you have power_surge.png under Resources/Icons/
    }
}

