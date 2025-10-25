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

    private Rigidbody2D _platformRb;
    private Vector2 _lastPlatformPosition;
    private Vector2 _lastPlatformVelocity;

    [SerializeField] private float platformVelocityThreshold = 1.5f; // tweak as needed


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

        Vector2 platformVelocity = Vector2.zero;

        if (_ground.CurrentPlatform != null)
        {
            platformVelocity = (Vector2)_ground.CurrentPlatform.Velocity;
            platformVelocity.y = 0f;
        }

        if (attack != null && attack.IsAttacking())
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0f, 20f * Time.deltaTime);
            _body.linearVelocity = _velocity;
            return;
        }

        if (platformVelocity != _lastPlatformVelocity)
        {
            Vector2 deltaVelocity = platformVelocity - _lastPlatformVelocity;
            if (Mathf.Abs(deltaVelocity.x) > platformVelocityThreshold)
            {
                _velocity += deltaVelocity;
                _body.linearVelocity = _velocity;
            }
            _lastPlatformVelocity = platformVelocity;
        }

        _acceleration = _onGround ? _maxAcceleration : _maxAirAcceleration;
        _maxSpeedChange = _acceleration * Time.deltaTime;

        float targetVelocityX = _desiredVelocity.x + platformVelocity.x;

        _velocity.x = Mathf.MoveTowards(_velocity.x, targetVelocityX, _maxSpeedChange);
        _body.linearVelocity = _velocity;

        float currentDirX = _direction.x;

        if (currentDirX != 0)
        {
            TurnCheck();
        }
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

    private void HandlePlatformMovement()
    {
        if (_onGround && _ground.CurrentPlatform != null)
        {
            var platformRb = _ground.CurrentPlatform.GetComponent<Rigidbody2D>();

            if (platformRb != null)
            {
                if (platformRb != _platformRb)
                {
                    _platformRb = platformRb;
                    _lastPlatformPosition = _platformRb.position;
                }

                Vector2 platformMovement = _platformRb.position - _lastPlatformPosition;
                Vector2 platformVelocity = platformMovement / Time.fixedDeltaTime;

                _velocity.x += platformVelocity.x;

                _lastPlatformPosition = _platformRb.position;
            }
        }
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
}
