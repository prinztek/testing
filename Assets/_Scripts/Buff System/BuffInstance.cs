using UnityEngine;

public abstract class BuffInstance
{
    public ScriptableObjectBuff buffData;
    public CharacterStats target;
    public float remainingTime;
    public bool isExpired => remainingTime <= 0f;

    public BuffInstance(ScriptableObjectBuff buffData, CharacterStats target)
    {
        this.buffData = buffData;
        this.target = target;
        this.remainingTime = buffData.duration;
    }

    public virtual void OnApply() { }
    public virtual void OnExpire() { }
    public virtual void OnAttackHit(GameObject enemy) { }

    public virtual void Update(float deltaTime)
    {
        remainingTime -= deltaTime;
        if (isExpired)
        {
            OnExpire();
        }
    }

    public virtual string GetUIDisplay() => $"{buffData.buffName} - {remainingTime:F1}s";
    public virtual Sprite GetIcon() => buffData.GetIcon();
}
