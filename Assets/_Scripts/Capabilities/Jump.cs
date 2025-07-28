using UnityEngine;


// [RequireComponent(typeof(Controller))]
public class Jump : MonoBehaviour
{
    [SerializeField] internal InputController input = null;
    [SerializeField, Range(0f, 10f)] private float _jumpHeight = 3f;
    [SerializeField, Range(0, 5)] private int _maxAirJumps = 0;
    [SerializeField, Range(0f, 5f)] private float _downwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 5f)] private float _upwardMovementMultiplier = 1.7f;
    [SerializeField, Range(0f, 0.3f)] private float _coyoteTime = 0.2f;
    [SerializeField, Range(0f, 0.3f)] private float _jumpBufferTime = 0.2f;
    [SerializeField] private Attack attack; // Assign this via Inspector
    private CharacterStats stats;

    public ParticleSystem jumpParticles;

    // private Controller _controller;
    private Rigidbody2D _body;
    private Ground _ground;
    private Vector2 _velocity;

    private int _jumpPhase;
    private float _defaultGravityScale, _jumpSpeed, _coyoteCounter, _jumpBufferCounter;

    private bool _desiredJump, _onGround, _isJumping;


    // Start is called before the first frame update
    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _ground = GetComponent<Ground>();
        stats = GetComponent<CharacterStats>();

        // _controller = GetComponent<Controller>();

        _defaultGravityScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (stats.IsDead()) return;
        _desiredJump |= input.RetrieveJumpInput();
        // _desiredJump |= _controller.input.RetrieveJumpInput();
    }

    private void FixedUpdate()
    {
        _onGround = _ground.OnGround;
        _velocity = _body.linearVelocity;

        if (_onGround && Mathf.Abs(_body.linearVelocity.y) < 0.01f)
        {
            _jumpPhase = 0;
            _coyoteCounter = _coyoteTime; // reset timer while on ground
            _isJumping = false;
        }
        else
        {
            _coyoteCounter -= Time.deltaTime; // count down when not grounded
        }

        if (_desiredJump)
        {
            _desiredJump = false;
            _jumpBufferCounter = _jumpBufferTime;
        }
        else if (!_desiredJump && _jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        if (_jumpBufferCounter > 0)
        {
            JumpAction();
        }

        if (input.RetrieveJumpHoldInput() && _body.linearVelocity.y > 0)
        {
            _body.gravityScale = _upwardMovementMultiplier;
        }
        else if (!input.RetrieveJumpHoldInput() || _body.linearVelocity.y < 0)
        {
            _body.gravityScale = _downwardMovementMultiplier;
        }
        else
        {
            _body.gravityScale = _defaultGravityScale;
        }

        _body.linearVelocity = _velocity;
    }
    private void JumpAction()
    {
        if (attack != null && attack.IsAttacking()) return; // Prevent jump during attack

        bool isGroundJump = _onGround || (_coyoteCounter > 0f && _jumpPhase < _maxAirJumps);

        if (_coyoteCounter > 0f || (_jumpPhase < _maxAirJumps && _isJumping))
        {

            if (_isJumping)
            {
                _jumpPhase += 1;
            }

            _jumpBufferCounter = 0;
            _coyoteCounter = 0;
            _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight);
            _isJumping = true;

            if (_velocity.y > 0f)
                _jumpSpeed = Mathf.Max(_jumpSpeed - _velocity.y, 0f);
            else if (_velocity.y < 0f)
                _jumpSpeed += Mathf.Abs(_body.linearVelocity.y);

            _velocity.y += _jumpSpeed;

            jumpParticles?.Play();
        }
    }
}

