using System.Collections;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Ground ground;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Attack attack;
    [SerializeField] private Move move;
    [SerializeField] private Hurt hurt;
    private string currentAnimation;
    private float hurtLockTimer = 0f;
    private float attackLockTimer = 0f;

    private void Update()
    {
        if (attackLockTimer > 0f)
        {
            attackLockTimer -= Time.deltaTime;

            // When attack lock ends this frame, immediately refresh animation
            if (attackLockTimer <= 0f)
                HandleMovementAnimation();

            return;
        }

        if (hurtLockTimer > 0f)
        {
            hurtLockTimer -= Time.deltaTime;

            // When hurt lock ends this frame, immediately refresh animation
            if (hurtLockTimer <= 0f)
                HandleMovementAnimation();

            return;
        }

        if (hurt != null && hurt.IsHurt()) return;

        HandleMovementAnimation();
    }
    // *********************************************************************************

    private float normalAnimatorSpeed = 1f;

    // Call this to change animation playback speed, e.g. when buff applied
    public void SetAnimationSpeed(float speed)
    {
        animator.speed = speed;
    }

    // Optional: Reset speed to normal
    public void ResetAnimationSpeed()
    {
        animator.speed = normalAnimatorSpeed;
    }
    // *********************************************************************************
    private bool wasOnGround;

    private void HandleMovementAnimation()
    {
        bool onGround = ground.OnGround;
        Vector2 velocity = rb.linearVelocity;

        if (move != null)
        {
            // If player is on a platform, subtract platform velocity
            Vector2 platformVel = move.GetPlatformVelocity();
            velocity.x -= platformVel.x;
        }


        float horizontal = Mathf.Abs(velocity.x);
        float vertical = velocity.y;

        if (!onGround)
        {
            ChangeAnimation(vertical > 0.1f ? "jump" : "fall");
        }
        else
        {
            ChangeAnimation(horizontal > 0.1f ? "running" : "idle");
        }

        // Don't forget to update this!
        wasOnGround = onGround;
    }



    public void PlayDeadAnimation(float animationLength = 0.33f)
    {
        // Force clear all animation locks so death plays immediately
        attackLockTimer = 0f;
        hurtLockTimer = 0f;

        PlayAndLock("dead", animationLength, ref hurtLockTimer);
    }

    public void PlayHurtAnimation(float animationLength = 0.33f)
    {
        PlayAndLock("hurt", animationLength, ref hurtLockTimer);
    }

    // ✅ Now takes string for weapon type ("fist", "sword", etc.)
    public void PlayAttackAnimation(int phase, string weaponType, bool isAirAttack = false)
    {
        string animName = weaponType.ToLower() switch
        {
            "sword" => isAirAttack ? $"air_sword_attack{phase}" : $"sword_attack{phase}",
            "bow" => isAirAttack ? $"air_bow_attack{phase}" : $"bow_attack{phase}", // currently unused - no air bow attack
            _ => $"attack{phase}" // default is fist
        };

        float length = GetAnimationLength(animName);
        PlayAndLock(animName, length, ref attackLockTimer);
    }

    public void PlayAndLock(string animName, float length, ref float lockTimer)
    {
        animator.CrossFade(animName, 0.05f, 0, 0f);
        currentAnimation = animName;
        lockTimer = length;
    }

    public IEnumerator PlayAndLockCoroutine(string animName)
    {
        float length = GetAnimationLength(animName);
        PlayAndLock(animName, length, ref hurtLockTimer);

        // Wait until the hurtLockTimer counts down to 0
        while (hurtLockTimer > 0f)
            yield return null;
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