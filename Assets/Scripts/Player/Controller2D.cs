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

    //Auto grab rigidbody2d when the script is added first
    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //Make sure rigidbody2d reference exist before the game starts
    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        //Use keyboard in editor or PC, else use mobile UI input
#if UNITY_EDITOR || UNITY_STANDALONE
        float keyboardInput = Input.GetAxisRaw("Horizontal");
        moveInput = Mathf.Abs(keyboardInput) > 0.01f ? keyboardInput : uiMoveInput;
#else
        moveInput = uiMoveInput;
#endif
        //Check every frame whether player is standing on the ground 
        UpdateGroundedState();
    }

    private void FixedUpdate()
    {
        //Do not change movement while dashing
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

    //Called by left/right UI buttons
    public void SetMoveInput(float value)
    {
        uiMoveInput = Mathf.Clamp(value, -1f, 1f);
    }

    //Small normal jump from a tap
    public void TapJump()
    {
        if (!CanJump())
        {
            return;
        }
        Debug.Log("Tap jump");
        PerformJump(tapJumpVelocity);
    }

    //Start charging a stronger jump
    public bool BeginCharge()
    {
        if (!CanJump())
        {
            return false;
        }

        isCharging = true;
        chargeTimer = 0f;
        Debug.Log("Charge started");
        return true;
    }

    //Increase charge over time while the player holds input
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

    //Release charge and jump based on how long it was held
    public void ReleaseCharge()
    {
        if (!isCharging)
        {
            return;
        }

        float t = Mathf.Clamp01(chargeTimer / maxChargeTime);
        float jumpVelocity = Mathf.Lerp(tapJumpVelocity, maxChargeJumpVelocity, t);

        Debug.Log("Charge released. Power: " + t);

        isCharging = false;
        chargeTimer = 0f;
        PerformJump(jumpVelocity);
    }

    //Stop charge without jumping
    public void CancelCharge()
    {
        isCharging = false;
        chargeTimer = 0f;
    }

    //Air dash only: one dash per airtime, resets on landing
    public bool TryDash(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f || !canDash || isDashing || isGrounded)
        {
            Debug.Log("Dash failed");
            return false;
        }

        Debug.Log("Dash succeeded: " + direction.normalized);
        StartCoroutine(DashRoutine(direction.normalized));
        return true;
    }

    //Handles jump velocity
    private void PerformJump(float jumpVelocity)
    {
        CancelCharge();
        Vector2 velocity = rb.linearVelocity;
        velocity.y = jumpVelocity;
        rb.linearVelocity = velocity;

        //Force player into an airborne state after jumping
        isGrounded = false;
        lastGroundTime = -99f;
    }

    //Allows jump slightly after leaving the ground (coyote time)
    private bool CanJump()
    {
        return !isDashing && Time.time <= lastGroundTime + coyoteTime;
    }

    //Quickly disable gravity and pushes pplayer in dash direction
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

    //Check if playter is touching ground
    private void UpdateGroundedState()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            lastGroundTime = Time.time;

            //Reset dash when player lands
            if (!wasGrounded)
            {
                canDash = true;
                Debug.Log("Player landed. Dash reset.");
            }
        }
    }

    //Shows ground check area
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
