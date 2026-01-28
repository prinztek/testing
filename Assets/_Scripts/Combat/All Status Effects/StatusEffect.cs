using UnityEngine;

public abstract class StatusEffect
{
    public string statusName;
    public float duration;
    protected float remainingTime;
    public bool isExpired => remainingTime <= 0f;

    protected EnemyStats target;

    protected StatusEffect(string name, float duration)
    {
        statusName = name;
        this.duration = duration;
        remainingTime = duration;
    }

    public void Assign(EnemyStats enemy)
    {
        target = enemy;
        OnApply();
    }

    public virtual void Update(float deltaTime)
    {
        remainingTime -= deltaTime;
        if (isExpired)
            OnExpire();
    }

    public abstract void OnApply();
    public abstract void OnExpire();

    public virtual void OnTick(float deltaTime) { }
    public virtual void OnTakeHit(int damage) { }

    // public virtual string GetUIDisplay()
    // {
    //     return $"{statusName} - {remainingTime:F1}s";
    // }
}
