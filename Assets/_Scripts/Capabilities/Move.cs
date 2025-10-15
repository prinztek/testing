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
    public bool FacingRight => transform.localScale.x > 0f;
    private float _maxSpeedChange, _acceleration;
    private float _lastDirectionX = 0f;
    private bool _onGround;

    private Rigidbody2D _platformRb;  // Track the platform's Rigidbody2D
    private Vector2 _lastPlatformPosition;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _ground = GetComponent<Ground>();
        stats = GetComponent<CharacterStats>();
    }

    private void Update()
    {
        if (stats.IsDead())
        {
            StopImmediately();
            return;
        }

        _direction.x = input.RetrieveMoveInput();

        // Prevent input movement while attacking
        if (attack != null && attack.IsAttacking())
        {
            _desiredVelocity = Vector2.zero;
            return;
        }

        float effectiveMaxSpeed = _maxSpeed * stats.moveSpeedMultiplier;
        _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(effectiveMaxSpeed - _ground.Friction, 0f);
    }

    private Vector2 _lastPlatformVelocity;
    [SerializeField] private float platformVelocityThreshold = 1.5f; // tweak as needed
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
            platformVelocity = (Vector2)_ground.CurrentPlatform.Velocity; // access platform velocity            
            // Only horizontal movement affects player
            platformVelocity.y = 0f;
            Debug.Log("On platform, using velocity: " + platformVelocity);
        }


        // Attack sliding logic
        if (attack != null && attack.IsAttacking())
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0f, 20f * Time.deltaTime);
            _body.linearVelocity = _velocity;
            return;
        }

        // is there a significant change in platform velocity?
        if (platformVelocity != _lastPlatformVelocity)
        {
            Vector2 deltaVelocity = platformVelocity - _lastPlatformVelocity;
            if (Mathf.Abs(deltaVelocity.x) > platformVelocityThreshold)
            {
                // Apply the change in platform velocity to the player
                _velocity += deltaVelocity;
                _body.linearVelocity = _velocity;
                Debug.Log("Platform velocity changed significantly, applying delta: " + deltaVelocity);
            }
            _lastPlatformVelocity = platformVelocity;
        }

        _acceleration = _onGround ? _maxAcceleration : _maxAirAcceleration; // sets the acceleration based on whether the player is on the ground or in the air
        _maxSpeedChange = _acceleration * Time.deltaTime; // calculates the maximum change in speed allowed this frame

        // Compute the player’s intended velocity relative to platform motion
        float targetVelocityX = _desiredVelocity.x + platformVelocity.x;

        // Smoothly adjust the player’s velocity towards the target velocity
        _velocity.x = Mathf.MoveTowards(_velocity.x, targetVelocityX, _maxSpeedChange);
        _body.linearVelocity = _velocity;

    }

    // This method tracks the platform's movement and calculates the relative velocity
    private void HandlePlatformMovement()
    {
        if (_onGround && _ground.CurrentPlatform != null)
        {
            // Get the platform's Rigidbody2D (moving platform)
            var platformRb = _ground.CurrentPlatform.GetComponent<Rigidbody2D>();

            if (platformRb != null)
            {
                // If this is the first time touching this platform, initialize the position
                if (platformRb != _platformRb)
                {
                    _platformRb = platformRb;
                    _lastPlatformPosition = _platformRb.position;
                }

                // Calculate the movement of the platform since the last frame
                Vector2 platformMovement = _platformRb.position - _lastPlatformPosition;
                Vector2 platformVelocity = platformMovement / Time.fixedDeltaTime;

                // Adjust the player's velocity based on the platform's velocity
                _velocity.x += platformVelocity.x;

                // Update the last platform position
                _lastPlatformPosition = _platformRb.position;
            }
        }
    }

    private void LateUpdate()
    {
        float currentDirX = _direction.x;

        if (currentDirX != 0)
        {
            // Call flip function to handle sprite flip and particle effect
            Flip(currentDirX);
        }
    }

    private void Flip(float currentDirX)
    {
        // Flip player sprite
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(currentDirX) * Mathf.Abs(scale.x);
        transform.localScale = scale;

        // Detect direction flip with movement
        if (Mathf.Sign(currentDirX) != Mathf.Sign(_lastDirectionX) && _lastDirectionX != 0 && Mathf.Abs(_velocity.x) > 0.1f && _onGround)
        {
            // Play dust trail particle effect
            if (dustTrailParticles != null && !dustTrailParticles.isPlaying)
            {
                dustTrailParticles.Play(); // Direction changed with movement! Dust played
            }
        }

        // Update last direction
        _lastDirectionX = currentDirX;
    }

    public void StopImmediately()
    {
        _direction = Vector2.zero;
        _desiredVelocity = Vector2.zero;
        _velocity = Vector2.zero;

        if (_body != null)
            _body.linearVelocity = Vector2.zero;
    }
}
