using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Ground ground;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Attack attack;
    [SerializeField] private Hurt hurt;

    private string currentAnimation;
    private float hurtLockTimer = 0f;
    private float attackLockTimer = 0f;

    private void Update()
    {
        if (attackLockTimer > 0f)
        {
            attackLockTimer -= Time.deltaTime;
            return;
        }

        if (hurtLockTimer > 0f)
        {
            hurtLockTimer -= Time.deltaTime;
            return;
        }

        if (hurt != null && hurt.IsHurt()) return;

        HandleMovementAnimation();
    }

    private void HandleMovementAnimation()
    {
        bool onGround = ground.OnGround;
        float horizontal = Mathf.Abs(rb.linearVelocity.x);
        float vertical = rb.linearVelocity.y;

        if (!onGround)
        {
            ChangeAnimation(vertical > 0.1f ? "jump" : "fall");
        }
        else
        {
            ChangeAnimation(horizontal > 0.1f ? "running" : "idle");
        }
    }

    public void PlayDeadAnimation(float animationLength = 0.33f)
    {
        PlayAndLock("dead", animationLength, ref hurtLockTimer);
    }

    public void PlayHurtAnimation(float animationLength = 0.33f)
    {
        PlayAndLock("hurt", animationLength, ref hurtLockTimer);
    }

    // ✅ Now takes string for weapon type ("fist", "sword", etc.)
    public void PlayAttackAnimation(int phase, string weaponType)
    {
        string animName = weaponType.ToLower() switch
        {
            "sword" => $"sword_attack{phase}",
            "bow" => $"bow_attack{phase}",
            _ => $"attack{phase}" // default is fist
        };

        float length = GetAnimationLength(animName);
        PlayAndLock(animName, length, ref attackLockTimer);
    }

    private void PlayAndLock(string animName, float length, ref float lockTimer)
    {
        animator.CrossFade(animName, 0.05f, 0, 0f);
        currentAnimation = animName;
        lockTimer = length;
    }

    public float GetHurtAnimationLength() => GetAnimationLength("hurt");
    public float GetDeathAnimationLength() => GetAnimationLength("dead");

    public float GetAttackAnimationLength(int phase, string weaponType)
    {
        string animName = weaponType.ToLower() switch
        {
            "sword" => $"sword_attack{phase}",
            "bow" => $"bow_attack{phase}",
            _ => $"attack{phase}"
        };
        return GetAnimationLength(animName);
    }

    private float GetAnimationLength(string name)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
                return clip.length;
        }

        Debug.LogWarning($"Animation '{name}' not found. Fallback to 0.5s.");
        return 0.5f;
    }

    private void ChangeAnimation(string newAnim)
    {
        if (currentAnimation == newAnim) return;
        animator.CrossFade(newAnim, 0.1f);
        currentAnimation = newAnim;
    }
}
