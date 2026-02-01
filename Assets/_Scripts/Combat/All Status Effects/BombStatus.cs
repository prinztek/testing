using UnityEngine;

public class BombStatus : StatusEffect
{
    private int damage;
    private GameObject vfxPrefab;
    private bool exploded = false;

    public BombStatus(int damage, float delay, GameObject vfxPrefab = null)
        : base("Bomb", delay)
    {
        this.damage = damage;
        this.vfxPrefab = vfxPrefab;
        remainingTime = delay;
    }

    public override void OnApply()
    {
        Debug.Log($"💣 BombStatus applied! Explodes in {remainingTime} seconds.");

        if (vfxPrefab != null && target != null)
        {
            GameObject vfx = GameObject.Instantiate(vfxPrefab, target.transform.position, Quaternion.identity);
            vfx.transform.SetParent(target.transform);
            vfx.transform.localPosition = Vector3.zero;
        }
    }

    public override void Update(float deltaTime)
    {
        remainingTime -= deltaTime;

        if (remainingTime <= 0f && !exploded)
        {
            Explode();
        }
    }

    private void Explode()
    {
        exploded = true;

        if (target != null)
        {
            target.TakeDamage(damage, target.transform.position);
            Debug.Log($"💥 Bomb exploded on {target.name} for {damage} damage!");
        }

        OnExpire();
    }

    public override void OnExpire()
    {
        Debug.Log("BombStatus expired.");
    }
}
