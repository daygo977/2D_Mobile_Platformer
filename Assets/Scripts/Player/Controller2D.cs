using UnityEngine;
using System.Collections;

public class Controller2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float tapJumpVelocity = 7f;
    [SerializeField] private float maxChargeJumpVelocity = 13f;
    [SerializeField] private float maxChargeTime = 0.7f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Air Dash")]
    [SerializeField] private float dashSpeed = 9f;
    [SerializeField] private float dashDuration = 0.15f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.2f;

    private float uiMoveInput;
    private float moveInput;
    private bool isGrounded;
    private bool wasGrounded;
    private float lastGroundTime = -99f;
    private bool isCharging;
    private float chargeTimer;
    private bool canDash = true;
    private bool isDashing;

    public bool IsGrounded => isGrounded;
    public bool IsCharging => isCharging;
    public bool IsDashing => isDashing;
    public float ChargePercent => Mathf.Clamp01(chargeTimer / maxChargeTime);

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        float keyboardInput = Input.GetAxisRaw("Horizontal");
        moveInput = Mathf.Abs(keyboardInput) > 0.01f ? keyboardInput : uiMoveInput;
#else
        moveInput = uiMoveInput;
#endif

        UpdateGroundedState();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        Vector2 velocity = rb.linearVelocity;

        //Ground only horizontal control
        //If in the air, player will keep exisiting momentum, no directional air movement
        if (isGrounded)
        {
            velocity.x = moveInput * moveSpeed;
        }

        rb.linearVelocity = velocity;
    }

    public void SetMoveInput(float value)
    {
        uiMoveInput = Mathf.Clamp(value, -1f, 1f);
    }

    public void TapJump()
    {
        if (!CanJump())
        {
            return;
        }

        PerformJump(tapJumpVelocity);
    }

    public bool BeginCharge()
    {
        if (!CanJump())
        {
            return false;
        }

        isCharging = true;
        chargeTimer = 0f;
        return true;
    }

    public void TickCharge(float deltaTime)
    {
        if (!isCharging)
        {
            return;
        }

        if (!CanJump())
        {
            CancelCharge();
            return;
        }

        chargeTimer = Mathf.Min(chargeTimer + deltaTime, maxChargeTime);
    }

    public void ReleaseCharge()
    {
        if (!isCharging)
        {
            return;
        }

        float t = Mathf.Clamp01(chargeTimer / maxChargeTime);
        float jumpVelocity = Mathf.Lerp(tapJumpVelocity, maxChargeJumpVelocity, t);

        isCharging = false;
        chargeTimer = 0f;
        PerformJump(jumpVelocity);
    }

    public void CancelCharge()
    {
        isCharging = false;
        chargeTimer = 0f;
    }

    public bool TryDash(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f || !canDash || isDashing || isGrounded)
        {
            return false;
        }

        StartCoroutine(DashRoutine(direction.normalized));
        return true;
    }

    private void PerformJump(float jumpVelocity)
    {
        CancelCharge();
        Vector2 velocity = rb.linearVelocity;
        velocity.y = jumpVelocity;
        rb.linearVelocity = velocity;

        isGrounded = false;
        lastGroundTime = -99f;
    }

    private bool CanJump()
    {
        return !isDashing && Time.time <= lastGroundTime + coyoteTime;
    }

    private IEnumerator DashRoutine(Vector2 direction)
    {
        canDash = false;
        isDashing = true;
        CancelCharge();

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = direction * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    private void UpdateGroundedState()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            lastGroundTime = Time.time;

            if (!wasGrounded)
            {
                canDash = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
