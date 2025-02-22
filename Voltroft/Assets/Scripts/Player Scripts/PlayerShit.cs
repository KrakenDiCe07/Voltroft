using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShit : MonoBehaviour
{
    public float moveSpeed;
    public float wallSlideSpeed;
    public float wallDetachJumpGracePeriod;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    public Transform leftWallCheck;
    public Transform rightWallCheck;
    public Vector2 wallCheckSize = new Vector2(0.1f, 1f);

    private Rigidbody2D rb;
    private int facingDirection = 1;
    private bool isGrounded;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;
    private bool isWallJumping;
    private bool isWallSliding;
    private bool isJumping;
    private float wallJumpTimer;
    private float wallDetachTimer;
    private bool recentlyDetachedFromWall;
    private Coroutine jumpCoroutine;
    private Coroutine wallJumpCoroutine;

    [Header("Shooting")]
    [SerializeField]private GameObject projectilePrefab;
    [SerializeField]private GameObject chargedProjectilePrefab;
    [SerializeField]private Transform firePoint;
    public KeyCode fireKey = KeyCode.K;
    public bool shotCharged = false;
    [SerializeField]private float chargeTime;
    [SerializeField]private float chargeSpeed;
    private bool isCharging = false;

    [Header("Jump Options")]
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpForce = 10f;
    public float jumpTime = 0.3f;  // How long you can hold the button to reach max height
    public float jumpCutMultiplier = 0.5f;

    public float wallJumpForce = 12f;
    public float wallJumpTime = 0.2f;
    public float postWallJumpSpeedModifier = 0.5f;
    
    [Header("Dash Options")]
    public KeyCode dashKey = KeyCode.J;
    public float dashDuration = 0.4f;
    public float dashVelocity = 8f;
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // Update facing direction
        if (horizontalInput > 0) facingDirection = 1;
        else if (horizontalInput < 0) facingDirection = -1;

        firePoint.localPosition = new Vector3(0.8f * facingDirection, 0, 0);

        // Handle dash
        if (Input.GetKeyDown(dashKey) && canDash && isGrounded)
        {
            isDashing = true;
            canDash = false;
            moveSpeed += dashVelocity;
            dashTime = dashDuration;
        }
        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0 && (isGrounded || isWallSliding))
            {
                isDashing = false;
                moveSpeed -= dashVelocity;
                canDash = true;
            }
        }
        if (Input.GetKeyUp(dashKey) && isDashing)
        {
            isDashing = false;
            moveSpeed -= dashVelocity;
            canDash = true;
        }

        // Check ground and wall states
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        isTouchingLeftWall = Physics2D.OverlapBox(leftWallCheck.position, wallCheckSize, 0f, groundLayer);
        isTouchingRightWall = Physics2D.OverlapBox(rightWallCheck.position, wallCheckSize, 0f, groundLayer);

        // Reset jump ability when grounded
        if (isGrounded)
        {
            isJumping = false;
        }

        // Handle movement
        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }

        // Handle wall sliding (only activates when falling)
        isWallSliding = (isTouchingLeftWall || isTouchingRightWall) && !isGrounded && horizontalInput != 0 && rb.linearVelocity.y <= 0;
        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
            recentlyDetachedFromWall = false;
        }
        else if ((isTouchingLeftWall || isTouchingRightWall) && !isWallSliding)
        {
            recentlyDetachedFromWall = false;
        }

        // Handle jump
        if (Input.GetKeyDown(jumpKey))
        {
            if (isGrounded)
            {
                if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);
                jumpCoroutine = StartCoroutine(JumpRoutine());
            }
            else if (isWallSliding)
            {
                if (wallJumpCoroutine != null) StopCoroutine(wallJumpCoroutine);
                wallJumpCoroutine = StartCoroutine(WallJumpRoutine());
            }
        }

        // Reduce jump height if key is released early
        if (Input.GetKeyUp(jumpKey) && isJumping && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
        if (Input.GetKey(fireKey) && chargeTime < 2)
            {
                isCharging = true;
                if (isCharging == true)
                {
                    chargeTime += Time.deltaTime * chargeSpeed;
                }
            }
        if (Input.GetKeyDown(fireKey))
        {
            Shoot();
            chargeTime = 0;
        }
        else if (Input.GetKeyUp(fireKey) && chargeTime >= 2)
        {
            ShootCharged();
        }
    }

    private IEnumerator JumpRoutine()
    {
        isJumping = true;
        float time = 0f;
        float initialBoost = jumpForce * 0.6f; // Strong push at the start
        float remainingForce = jumpForce - initialBoost; // Smooth force applied over time

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, initialBoost); // Apply initial push

        while (Input.GetKey(jumpKey) && time < jumpTime)
        {
            float jumpProgress = time / jumpTime; // 0 to 1
            float appliedForce = Mathf.Lerp(initialBoost, jumpForce, jumpProgress);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, appliedForce);

            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;
        isWallSliding = false;
        float time = 0;
        Vector2 direction = isTouchingLeftWall ? Vector2.right : Vector2.left;

        while (Input.GetKey(jumpKey) && time < wallJumpTime)
        {
            rb.linearVelocity = new Vector2(
                Mathf.Lerp(rb.linearVelocity.x, direction.x * wallJumpForce, time / wallJumpTime),
                Mathf.Lerp(rb.linearVelocity.y, jumpForce, time / wallJumpTime)
            );
            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        isWallJumping = false;
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        PlayerProjectile projectileScript = projectile.GetComponent<PlayerProjectile>();

        Vector2 shootDirection = facingDirection == 1 ? Vector2.right : Vector2.left;
        float rotationZ = facingDirection == 1 ? -90f : 90f;
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        projectileScript.Initialize(shootDirection);
    }
    private void ShootCharged()
    {
        GameObject chargedProjectile = Instantiate(chargedProjectilePrefab, firePoint.position, Quaternion.identity);
        PlayerProjectile projectileScript = chargedProjectile.GetComponent<PlayerProjectile>();

        Vector2 shootDirection = facingDirection == 1 ? Vector2.right : Vector2.left;
        float rotationZ = facingDirection == 1 ? -90f : 90f;
        chargedProjectile.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        projectileScript.Initialize(shootDirection);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);

        Gizmos.color = isTouchingLeftWall ? Color.red : Color.blue;
        Gizmos.DrawWireCube(leftWallCheck.position, wallCheckSize);

        Gizmos.color = isTouchingRightWall ? Color.red : Color.blue;
        Gizmos.DrawWireCube(rightWallCheck.position, wallCheckSize);
    }
}

/*{
    public float moveSpeed;
    public float wallSlideSpeed;
    public float wallDetachJumpGracePeriod; // Time window to allow jumping after leaving a wall

    public LayerMask groundLayer;
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    public Transform leftWallCheck;
    public Transform rightWallCheck;
    public Vector2 wallCheckSize = new Vector2(0.1f, 1f);

    private Rigidbody2D rb;
    [SerializeField]
    private int facingDirection = 1;
    private bool isGrounded;    
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;
    private bool isWallJumping;
    private bool isWallSliding;
    private float wallJumpTimer;
    private float wallDetachTimer;
    private bool recentlyDetachedFromWall;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public KeyCode fireKey = KeyCode.K;

    [Header("   JUMP OPTIONS")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField]private bool canJump = true;
    [SerializeField]private bool isJumping = false;
    public float jumpForce;
    public float wallJumpForce;
    public float wallJumpLerpTime = 0.1f;
    public float postWallJumpSpeedModifier;
    public float postWallJumpDuration;

    [Header("   DASH OPTIONS")]
    [SerializeField] private KeyCode dashKey = KeyCode.J; 
    [SerializeField] private float dashDuration = 0.4f;
    [SerializeField] private float dashVelocity = 8f;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool canDash = true;
    private float dashTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // Update facing direction based on movement
        if (horizontalInput > 0)
        {
            facingDirection = 1;
        }
        else if (horizontalInput < 0)
        {
            facingDirection = -1;
        }

        firePoint.localPosition = new Vector3(0.8f * facingDirection, 0, 0);

        if (Input.GetKeyDown(dashKey) && canDash == true && isGrounded)
        {
            isDashing = true;
            canDash = false;
            moveSpeed += dashVelocity;
            dashTime = dashDuration;
        }
        if (isDashing == true)
        {  
            dashTime -= Time.deltaTime;

            if (dashTime <= 0 && (isGrounded || isWallSliding))
            {
                isDashing = false;
                moveSpeed -= dashVelocity;
                canDash = true;
            }
        }
        if (Input.GetKeyUp(dashKey) && isDashing == true)
        {
            isDashing = false;
            moveSpeed -= dashVelocity;
            canDash = true;
        }

        // Check ground and wall states
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        isTouchingLeftWall = Physics2D.OverlapBox(leftWallCheck.position, wallCheckSize, 0f, groundLayer);
        isTouchingRightWall = Physics2D.OverlapBox(rightWallCheck.position, wallCheckSize, 0f, groundLayer);

        // Reset jump ability when grounded
        if (isGrounded && !isWallJumping)
        {
            canJump = true;
        }

        // Handle movement

        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
 
        // Handle wall sliding
        isWallSliding = (isTouchingLeftWall || isTouchingRightWall) && !isGrounded && horizontalInput != 0;
        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
            recentlyDetachedFromWall = false; // Reset the detach flag when sliding
        }
        else if ((isTouchingLeftWall || isTouchingRightWall) && !isWallSliding)
        {
            recentlyDetachedFromWall = false;
        }

        // Handle wall detach timer
        if (!isTouchingLeftWall && !isTouchingRightWall && !isGrounded && !recentlyDetachedFromWall)
        {
            wallDetachTimer = Time.time;
            recentlyDetachedFromWall = true;
        }

        // Handle jump
        if (Input.GetKeyDown(jumpKey))
        {
            if (canJump && isGrounded)
            {
                Jump();
                StartCoroutine(DelayFirstWallGrapple(0.2f));
            }
            else if (!isGrounded)
            {
                if (isTouchingLeftWall)
                {
                    WallJump(Vector2.right);
                }
                else if (isTouchingRightWall)
                {
                    WallJump(Vector2.left);
                }
                else if (recentlyDetachedFromWall && Time.time - wallDetachTimer <= wallDetachJumpGracePeriod)
                {
                    Jump();
                    recentlyDetachedFromWall = false; // Consume the grace period jump
                }
            }
        }

        // Reset wall jump modifier
        if (isWallJumping && Time.time > wallJumpTimer + postWallJumpDuration)
        {
            isWallJumping = false;
        }
        if (Input.GetKeyDown(fireKey))
        {
            Shoot();
        }
    }
    public int GetFacingDirection()
    {
        return facingDirection;
    }
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        canJump = false;
    }

    private void WallJump(Vector2 direction)
    {
        isWallJumping = true;
        isWallSliding = false;
        wallJumpTimer = Time.time;
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(PerformWallJump(direction));
    }

    private System.Collections.IEnumerator PerformWallJump(Vector2 direction)
    {
        float elapsedTime = 0f;
        while (elapsedTime < wallJumpLerpTime)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, new Vector2(direction.x * wallJumpForce, jumpForce), elapsedTime / wallJumpLerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rb.linearVelocity = new Vector2(direction.x * wallJumpForce * postWallJumpSpeedModifier, rb.linearVelocity.y);
    }

    private void OnDrawGizmos()
    {
        // Draw ground check gizmo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);

        Gizmos.color = isGrounded ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);

        // Draw wall check gizmos
        Gizmos.color = isTouchingLeftWall ? Color.red : Color.blue;
        Gizmos.DrawWireCube(leftWallCheck.position, wallCheckSize);

        Gizmos.color = isTouchingRightWall ? Color.red : Color.blue;
        Gizmos.DrawWireCube(rightWallCheck.position, wallCheckSize);
    }
    IEnumerator DelayFirstWallGrapple(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        PlayerProjectile projectileScript = projectile.GetComponent<PlayerProjectile>();

        // Get the direction based on player facing
        Vector2 shootDirection = facingDirection == 1 ? Vector2.right : Vector2.left;
    
        // Set projectile rotation
        float rotationZ = facingDirection == 1 ? -90f : 90f;
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        projectileScript.Initialize(shootDirection);
    }
} */