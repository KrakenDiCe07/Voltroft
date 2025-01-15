using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShit : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float wallJumpForce = 8f;
    public float wallJumpLerpTime = 0.2f;
    public float postWallJumpSpeedModifier = 0.5f;
    public float postWallJumpDuration = 0.5f;
    public float wallSlideSpeed = 2f;
    public float wallDetachJumpGracePeriod = 0.2f; // Time window to allow jumping after leaving a wall

    public LayerMask groundLayer;
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    public Transform leftWallCheck;
    public Transform rightWallCheck;
    public Vector2 wallCheckSize = new Vector2(0.1f, 1f);

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canJump = true;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;
    private bool isWallJumping;
    private bool isWallSliding;
    private float wallJumpTimer;
    private float wallDetachTimer;
    private bool recentlyDetachedFromWall;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
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
        float horizontalInput = Input.GetAxis("Horizontal");
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
        if (Input.GetButtonDown("Jump"))
        {
            if (canJump && isGrounded)
            {
                Jump();
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);

        // Draw wall check gizmos
        Gizmos.color = isTouchingLeftWall ? Color.red : Color.blue;
        Gizmos.DrawWireCube(leftWallCheck.position, wallCheckSize);

        Gizmos.color = isTouchingRightWall ? Color.red : Color.blue;
        Gizmos.DrawWireCube(rightWallCheck.position, wallCheckSize);
    }
}


/*{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float wallJumpForce = 8f;
    public float wallJumpLerpTime = 0.2f;
    public float postWallJumpSpeedModifier = 0.5f;
    public float postWallJumpDuration = 0.5f;
    public float wallSlideSpeed = 2f;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    public Transform leftWallCheck;
    public Transform rightWallCheck;
    public Vector2 wallCheckSize = new Vector2(0.1f, 1f);

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canJump = true;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;
    private bool isWallJumping;
    private bool isWallSliding;
    private float wallJumpTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
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
        float horizontalInput = Input.GetAxis("Horizontal");
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }

        // Handle wall sliding
        isWallSliding = (isTouchingLeftWall || isTouchingRightWall) && !isGrounded  && horizontalInput != 0;
        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }

        // Handle jump
        if (Input.GetButtonDown("Jump"))
        {
            if (canJump && isGrounded)
            {
                Jump();
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
            }
        }

        // Reset wall jump modifier
        if (isWallJumping && Time.time > wallJumpTimer + postWallJumpDuration)
        {
            isWallJumping = false;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        canJump = false;
    }

    private void WallJump(Vector2 direction)
    {
        isWallJumping = true;
        isWallSliding = false;
        wallJumpTimer = Time.time;
        rb.velocity = Vector2.zero;
        StartCoroutine(PerformWallJump(direction));
    }

    private System.Collections.IEnumerator PerformWallJump(Vector2 direction)
    {
        float elapsedTime = 0f;
        while (elapsedTime < wallJumpLerpTime)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(direction.x * wallJumpForce, jumpForce), elapsedTime / wallJumpLerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rb.velocity = new Vector2(direction.x * wallJumpForce * postWallJumpSpeedModifier, rb.velocity.y);
    }

    private void OnDrawGizmos()
    {
        // Draw ground check gizmo
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);

        // Draw wall check gizmos
        Gizmos.color = isTouchingLeftWall ? Color.red : Color.blue;
        Gizmos.DrawWireCube(leftWallCheck.position, wallCheckSize);

        Gizmos.color = isTouchingRightWall ? Color.red : Color.blue;
        Gizmos.DrawWireCube(rightWallCheck.position, wallCheckSize);
    }
}
*/