using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class NewBehaviourScript : MonoBehaviour
{

    // The CHaracter's Movement Intialzied Variables
    [SerializeField] private PlayerMovementStats _stats;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    [Header("Jump Arc Visualization")]
    [SerializeField] private bool _drawJumpArc = true;
    [SerializeField] private int _arcResolution = 30;
    [SerializeField] private float _arcTime = 2f;
    [SerializeField] private Color _arcColor = Color.green;

    [Header("Pickaxe Hook")]
    [SerializeField] private float _hookRange = 1.5f;
    [SerializeField] private float _hookBoost = 12f;
    [SerializeField] private float _ledgeClearHeight = 0.6f;
    [SerializeField] private float _hookCooldown = 0.5f;

    private bool _hookToConsume;
    private float _lastHookTime = float.MinValue;
    private bool _isHooked;
    private float _hookDuration = 0.15f;
    private float _hookEndTime;
    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;


    private float _time;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();


        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    // Calls the input, put animations here for the future
    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();
    }



    private void GatherInput()
    {
        _frameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
            JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            HookDown = Input.GetMouseButtonDown(0),
        };

        if (_stats.SnapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
        }

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }

        if (_frameInput.HookDown) {
            _hookToConsume = true;
        }
           
    }
        private void FixedUpdate()
        {
            CheckCollisions();

            HandleJump();
            HandleDirection();
            HandleGravity();
            HandlePickaxeHook();
            ApplyMovement();
        }

        #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        Vector2 boxCenter = _col.bounds.center;
        Vector2 boxSize = _col.bounds.size;

        bool groundHit = Physics2D.BoxCast(
            boxCenter,
            boxSize,
            0f,
            Vector2.down,
            _stats.GrounderDistance,
            ~_stats.PlayerLayer
        );

        bool ceilingHit = Physics2D.BoxCast(
            boxCenter,
            boxSize,
            0f,
            Vector2.up,
            _stats.GrounderDistance,
            ~_stats.PlayerLayer
        );

        if (ceilingHit)
            _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;

            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;

            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }




    #region  Jumping

    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_grounded || CanUseCoyote) ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = _stats.JumpPower;
        Jumped?.Invoke();
    }

    #endregion

    #region Horizontal

    private void HandleDirection()
    {
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Gravity

    private void HandleGravity()
    {
        if (_isHooked) return;

        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
        }
        else
        {
            var inAirGravity = _stats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Pickaxe Hook

    private void HandlePickaxeHook()
    {
        if (_hookToConsume && _time > _lastHookTime + _hookCooldown)
        {
            TryHook();
            _hookToConsume = false;
        }

        if (_isHooked && (_time >= _hookEndTime || _frameVelocity.y < 0))
        {
            _isHooked = false;
            _endedJumpEarly = false;
        }
    }

    private void TryHook()
    {
        Vector2[] dirs = { Vector2.right, Vector2.left };

        foreach (var dir in dirs)
        {
            Vector2 origin = _col.bounds.center;
            LayerMask hookMask = ~(1 << gameObject.layer);

            RaycastHit2D wallHit = Physics2D.Raycast(origin, dir, _hookRange, hookMask);

            Debug.Log($"Raycast {dir}: {(wallHit.collider != null ? wallHit.collider.name + " on layer " + wallHit.collider.gameObject.layer : "nothing hit")}");

            if (wallHit.collider != null)
            {
                // Check for open space above the hit point (the ledge)
                Vector2 ledgeCheckOrigin = wallHit.point + Vector2.up * _ledgeClearHeight;
                RaycastHit2D ledgeCheck = Physics2D.Raycast(ledgeCheckOrigin, dir, 0.3f, hookMask);

                Debug.Log($"Ledge check: {(ledgeCheck.collider == null ? "CLEAR - should hook!" : "blocked by " + ledgeCheck.collider.name)}");

                if (ledgeCheck.collider == null) // Open space = ledge exists
                {
                    ExecuteHook(dir);
                    Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
                    return;
                }
            }
        }
        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    private void ExecuteHook(Vector2 wallDir)
    {
        _isHooked = true;
        _lastHookTime = _time;
        _hookEndTime = _time + _hookDuration;
        _frameVelocity.y = _hookBoost;
        _frameVelocity.x = -wallDir.x * 3f;
        _endedJumpEarly = false;
    }

    #endregion
    private void ApplyMovement() => _rb.velocity = _frameVelocity;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
    }
#endif

    private void OnDrawGizmos()
    {
        if (!_drawJumpArc || _stats == null)
            return;

        DrawJumpArc(1);
    }

    // The Visualization of the Jump Arc
    private void DrawJumpArc(float direction)
    {
        Gizmos.color = _arcColor;

        Vector2 startPosition;

        if (Application.isPlaying && _col != null)
        {
            startPosition = _col.bounds.center;
        }
        else
        {
            startPosition = transform.position;
        }

        float horizontalSpeed = _stats.MaxSpeed * direction;
        float verticalSpeed = _stats.JumpPower;

        Vector2 velocity = new Vector2(horizontalSpeed, verticalSpeed);

        float timeStep = _arcTime / _arcResolution;

        Vector2 previousPoint = startPosition;

        for (int i = 1; i <= _arcResolution; i++)
        {
            float t = i * timeStep;

            // Use your gravity
            float gravity = _stats.FallAcceleration;

            Vector2 displacement = new Vector2(
                velocity.x * t,
                velocity.y * t - 0.5f * gravity * t * t
            );

            //Starts the Drawing at the ground
            Vector2 point = startPosition + new Vector2(
     horizontalSpeed * t,
     verticalSpeed * t - 0.5f * _stats.FallAcceleration * t * t
 );

            // Use a GROUND layer mask here — not PlayerLayer
            RaycastHit2D hit = Physics2D.Linecast(
                previousPoint,
                point,
                _stats.GroundLayer   // <-- make sure this is your ground layer
            );

            if (hit.collider != null)
            {
                Gizmos.DrawLine(previousPoint, hit.point);
                break;
            }

            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }

}

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public Vector2 Move;
    public bool HookDown;
}

public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;

    public event Action Jumped;
    public Vector2 FrameInput { get; }
  }



#endregion