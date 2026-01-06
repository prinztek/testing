using System;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] internal InputController input = null;
    [SerializeField, Range(0f, 100f)] private float _maxSpeed = 4f;
    [SerializeField, Range(0f, 100f)] private float _maxAcceleration = 35f;
    [SerializeField, Range(0f, 100f)] private float _maxAirAcceleration = 20f;
    [SerializeField] private Attack attack;
    public ParticleSystem dustTrailParticles;

    private CharacterStats stats;

    private Vector2 _direction, _desiredVelocity, _velocity;
    private Rigidbody2D _body;
    private Ground _ground;

    // Updated FacingRight using Y-axis rotation
    public bool FacingRight { get; private set; }

    private float _maxSpeedChange, _acceleration;
    private bool _onGround;
    private bool _wasMoving = false;

    private Rigidbody2D _platformRb;
    private Vector2 _lastPlatformPosition;
    private Vector2 _lastPlatformVelocity;

    // [SerializeField] private float platformVelocityThreshold = 1.5f; // tweak as needed
    private Rigidbody2D activePlatformRb;
    [Header("Footstep Sounds")]
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField, Range(0.05f, 1f)] private float stepInterval = 0.3f; // seconds between steps
    private float _stepTimer = 0f;
    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _ground = GetComponent<Ground>();
        stats = GetComponent<CharacterStats>();


    }

    private void Start()
    {
        // Start Direction Check
        StartDirectionCheck();
    }

    private void Update()
    {
        // 1️⃣ Block all input if the game is paused / a modal is open
        if (!InputGate.CanAcceptInput)
            return;

        // 2️⃣ Block input if clicking/touching UI elements (mobile or PC)
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (stats.IsDead())
        {
            StopImmediately();
            return;
        }

        _direction.x = input.RetrieveMoveInput();

        if (attack != null && attack.IsAttacking())
        {
            _desiredVelocity = Vector2.zero;
            return;
        }

        float effectiveMaxSpeed = _maxSpeed * stats.moveSpeedMultiplier;
        _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(effectiveMaxSpeed - _ground.Friction, 0f);

        // Only check footstep conditions if grounded and moving
        bool isMoving = _ground.OnGround && Mathf.Abs(_desiredVelocity.x) > 0.1f;

        // Play first footstep immediately when starting to move
        if (isMoving && !_wasMoving)
        {
            PlayFootstep();
            _stepTimer = 0f; // reset timer
        }

        if (isMoving)
        {
            _stepTimer += Time.deltaTime;

            if (_stepTimer >= stepInterval)
            {
                PlayFootstep();
                _stepTimer = 0f;
            }
        }
        else
        {
            _stepTimer = 0f;
        }

        _wasMoving = isMoving;
    }

    private void FixedUpdate()
    {
        if (stats.IsDead())
        {
            StopImmediately();
            return;
        }

        _onGround = _ground.OnGround;
        _velocity = _body.linearVelocity;

        // Get platform velocity (if on platform)
        Vector2 platformVelocity = Vector2.zero;

        if (activePlatformRb != null)
            platformVelocity = activePlatformRb.linearVelocity;

        // Attack slowdown
        if (attack != null && attack.IsAttacking())
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0f, 20f * Time.deltaTime);
            _body.linearVelocity = _velocity;
            return;
        }

        // Determine acceleration
        _acceleration = _onGround ? _maxAcceleration : _maxAirAcceleration;
        _maxSpeedChange = _acceleration * Time.deltaTime;

        // Add platform velocity to target
        float targetVelocityX = _desiredVelocity.x + platformVelocity.x;

        // Smooth acceleration toward target
        _velocity.x = Mathf.MoveTowards(_velocity.x, targetVelocityX, _maxSpeedChange);

        _body.linearVelocity = _velocity;

        // Flip sprite
        if (_direction.x != 0)
            TurnCheck();
    }


    private void LateUpdate()
    {
        float currentDirX = _direction.x;

        if (currentDirX != 0)
        {
            TurnCheck();
        }
    }

    private void TurnCheck()
    {
        // Do NOT allow flipping of direction while attacking
        if (attack != null && attack.IsAttacking())
            return;

        if (_direction.x > 0f && !FacingRight)
        {
            Turn();
        }
        else if (_direction.x < 0f && FacingRight)
        {
            Turn();
        }

    }

    private void Turn()
    {
        if (FacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            FacingRight = !FacingRight;
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            FacingRight = !FacingRight;
        }
        // Play dust trail effect if grounded and moving
        PlayDustTrail();
    }

    public void SetPlatform(Rigidbody2D platformRb)
    {
        activePlatformRb = platformRb;
    }

    public void ClearPlatform(Rigidbody2D platformRb)
    {
        if (activePlatformRb == platformRb)
            activePlatformRb = null;
    }

    public Vector2 GetPlatformVelocity()
    {
        if (activePlatformRb != null)
            return activePlatformRb.linearVelocity;

        return Vector2.zero;
    }


    public void StopImmediately()
    {
        _direction = Vector2.zero;
        _desiredVelocity = Vector2.zero;
        _velocity = Vector2.zero;

        if (_body != null)
            _body.linearVelocity = Vector2.zero;
    }

    private void StartDirectionCheck()
    {
        // Set FacingRight based on the Y rotation at start
        FacingRight = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, 0f)) < 90f;
    }
    public void PlayDustTrail()
    {
        if (dustTrailParticles != null && !dustTrailParticles.isPlaying)
        {
            if (_onGround && Mathf.Abs(_velocity.x) > 0.1f)
            {
                dustTrailParticles.Play();
            }
        }
    }

    #region Footstep Sound Management
    private void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        int index = UnityEngine.Random.Range(0, footstepClips.Length);
        AudioClip clip = footstepClips[index];

        // Play using your SoundFXManager with slight random pitch
        SoundFXManager.Instance.playSoundFXClilpRandomPitch(clip, transform, 0.03f);
    }
    #endregion
}

