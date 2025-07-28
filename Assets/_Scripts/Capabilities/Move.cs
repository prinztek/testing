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

        // ⛔ Prevent input movement while attacking
        if (attack != null && attack.IsAttacking())
        {
            _desiredVelocity = Vector2.zero;
            return;
        }

        _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(_maxSpeed - _ground.Friction, 0f);
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

        // ✅ Let attack nudge stay, but optionally slow it down
        if (attack != null && attack.IsAttacking())
        {
            // Optional: smooth slide stop
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0f, 20f * Time.deltaTime);
            _body.linearVelocity = _velocity;
            return;
        }

        _acceleration = _onGround ? _maxAcceleration : _maxAirAcceleration;
        _maxSpeedChange = _acceleration * Time.deltaTime;

        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);
        _body.linearVelocity = _velocity;
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
