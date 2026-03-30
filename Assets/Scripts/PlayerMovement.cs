
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{

    // The CHaracter's Movement Intialzied Variables
    [SerializeField] private PlayerMovementStats _stats;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;
    private bool _isOnIce;
    private bool _facingRight = true;

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
    [SerializeField] private float _snapSpeed = 15f;




    private Vector2 _hookSnapTarget;
    private bool _isSnapping;
    private bool _isLedgeBoosting;

    private float _lastHookTime = float.MinValue;
    private bool _isHooked;
    private float _hookDuration = 3f;
    private float _hookEndTime;
    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;


    private float _time;



    void Start()
    {
        Collider2D hit = Physics2D.OverlapCapsule(
            _col.bounds.center,
            _col.bounds.size,
            _col.direction,
            0f,
            _stats.GroundLayer
        );

        if (hit != null)
        {
            transform.position += Vector3.up * 0.5f;
        }
    }



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
            HookDown = Input.GetMouseButton(0),
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

    }
    private void FixedUpdate()
    {
        CheckCollisions();

        HandleDirection();
        HandleGravity();
        HandlePickaxeHook();
        HandleJump();
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

        //Olivier Changed This
        boxSize.y -= 0.05f; // shrink slightly

        RaycastHit2D groundHit = Physics2D.CapsuleCast(
            _col.bounds.center,
            boxSize,
            _col.direction,
            0f,
            Vector2.down,
            _stats.GrounderDistance,
            _stats.GroundLayer
        );

        _isOnIce = false;
        bool isGroundHit = groundHit.collider != null;



        if (groundHit.collider != null)
        {
            if (groundHit.collider.CompareTag("SlipperyIce"))
            {
                _isOnIce = true;
            }
        }



        bool ceilingHit = Physics2D.CapsuleCast(
        _col.bounds.center,
        _col.bounds.size,
        _col.direction,
            0f,
            Vector2.up,
            _stats.GrounderDistance,
            _stats.GroundLayer
        );

        if (ceilingHit)
            _frameVelocity.y = Mathf.Min(_frameVelocity.y, _stats.GroundingForce);

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
        Debug.Log("Grounded: " + groundHit);






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

    //Olivier Changed This
    private void HandleDirection()
    {
        if (_isHooked) return;

        float acceleration = _stats.Acceleration;
        float deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;

        if (_isOnIce)
        {
            acceleration *= _stats.IceAccelerationMultiplier;
            deceleration *= _stats.IceDecelerationMultiplier;

            // harder to turn on ice
            if (Mathf.Sign(_frameInput.Move.x) != Mathf.Sign(_frameVelocity.x) && _frameInput.Move.x != 0)
            {
                acceleration *= _stats.IceTurnControl;
            }
        }

        if (_frameInput.Move.x == 0)
        {
            _frameVelocity.x = Mathf.MoveTowards(
                _frameVelocity.x,
                0,
                deceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            float targetSpeed = _frameInput.Move.x * _stats.MaxSpeed;

            _frameVelocity.x = Mathf.MoveTowards(
                _frameVelocity.x,
                targetSpeed,
                acceleration * Time.fixedDeltaTime
            );
        }

        if (_frameInput.Move.x > 0 && !_facingRight)
        {
            Flip();
        }
        else if (_frameInput.Move.x < 0 && _facingRight)
        {
            Flip();
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
        if (_frameInput.HookDown && !_isHooked && _time > _lastHookTime + _hookCooldown)
        {
            TryHook();
        }

        if (!_frameInput.HookDown)
        {
            _isHooked = false;
        }

        if (_isHooked)
        {
            if (IsAtLedge(out Vector2 ledgeDir) && _frameVelocity.y > 0 && _frameInput.Move.y > 0 && !_isLedgeBoosting)
            {
                _isLedgeBoosting = true;
                _frameVelocity.y = _stats.JumpPower * 0.6f;
                _frameVelocity.x = ledgeDir.x * _stats.MaxSpeed;
            }

            if (_isLedgeBoosting)
            {
                if (!IsNextToWall(out _, out _))
                {
                    _isHooked = false;
                    _isLedgeBoosting = false;
                }
                return;
            }

            if (_isSnapping)
            {
                Vector2 newPos = Vector2.MoveTowards(transform.position, _hookSnapTarget, _snapSpeed * Time.fixedDeltaTime);
                _rb.MovePosition(newPos);


                if (Vector2.Distance(transform.position, _hookSnapTarget) < 0.01f)
                    _isSnapping = false;

                return;
            }

            if (!IsNextToWall(out _, out _))
            {
                _isHooked = false;
                return;
            }
            _frameVelocity.x = 0f;
            _frameVelocity.y = _frameInput.Move.y * (_stats.MaxSpeed * 0.7f);

            if (_frameInput.JumpDown || _jumpToConsume)
            {
                _isHooked = false;
                _frameVelocity.y = _stats.JumpPower * 2;
                _jumpToConsume = true;
                return;
            }

            // Release after max duration
            if (_time >= _hookEndTime)
                _isHooked = false;
            _lastHookTime = _time;

        }
    }

    private bool IsAtLedge(out Vector2 wallDir)
    {
        LayerMask hookMask = ~(1 << gameObject.layer);
        Vector2 bottom = new Vector2(_col.bounds.center.x, _col.bounds.min.y + 0.3f);
        RaycastHit2D bottomRight = Physics2D.Raycast(bottom, Vector2.right, _hookRange, hookMask);
        RaycastHit2D bottomLeft = Physics2D.Raycast(bottom, Vector2.left, _hookRange, hookMask);

        Vector2 top = new Vector2(_col.bounds.center.x, _col.bounds.max.y + 0.05f);
        RaycastHit2D topRight = Physics2D.Raycast(top, Vector2.right, _hookRange, hookMask);
        RaycastHit2D topLeft = Physics2D.Raycast(top, Vector2.left, _hookRange, hookMask);

        if (bottomRight.collider != null && topRight.collider == null)
        {
            wallDir = Vector2.right;
            return true;
        }
        if (bottomLeft.collider != null && topLeft.collider == null)
        {
            wallDir = Vector2.left;
            return true;
        }

        wallDir = default;
        return false;

    }

    private bool IsNextToWall(out RaycastHit2D hit, out Vector2 wallDir)
    {
        LayerMask hookMask = ~(1 << gameObject.layer); //Temp until I fix the layering issue
        Vector2 center = _col.bounds.center;
        Vector2 bottom = new Vector2(center.x, _col.bounds.min.y + 0.3f);
        Vector2 top = new Vector2(center.x, _col.bounds.max.y - 0.1f);

        Vector2[] origins = { bottom, center, top };
        foreach (var origin in origins)
        {
            RaycastHit2D rightHit = Physics2D.Raycast(origin, Vector2.right, _hookRange, hookMask);
            RaycastHit2D leftHit = Physics2D.Raycast(origin, Vector2.left, _hookRange, hookMask);

            if (rightHit.collider != null)
            {
                hit = rightHit; wallDir = Vector2.right; return true;
            }
            if (leftHit.collider != null)
            {
                hit = leftHit; wallDir = Vector2.left; return true;
            }
        }

        hit = default;
        wallDir = default;
        return false;
    }

    private void SnapToWall(RaycastHit2D wallHit, Vector2 dir)
    {

        float capsuleHalfWidth = _col.bounds.extents.x;

        float snappedX = wallHit.point.x - (dir.x * capsuleHalfWidth);

        if (Mathf.Abs(transform.position.x - snappedX) > 0.05f)
        {
            _hookSnapTarget = new Vector2(snappedX, transform.position.y);
            _isSnapping = true;
            _frameVelocity.x = 0f;
            _rb.velocity = Vector2.zero;
        }
        _frameVelocity.x = 0f;
    }
    private void TryHook()
    {

        if (IsNextToWall(out RaycastHit2D wallHit, out Vector2 wallDir))
        {
            SnapToWall(wallHit, wallDir);
            _isHooked = true;
            _lastHookTime = _time;
            _hookEndTime = _time + _hookDuration;
            _frameVelocity = Vector2.zero;

            if (_grounded)
            {
                _frameVelocity.y = _stats.JumpPower * 0.3f;
            }
        }
        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
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

            // Use a GROUND layer mask here � not PlayerLayer
            RaycastHit2D hit = Physics2D.Linecast(
                previousPoint,
                point,
                _stats.GroundLayer
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


    //Player Turn

    private void Flip()
    {
        _facingRight = !_facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
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
