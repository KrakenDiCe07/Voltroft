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

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    public Transform leftWallCheck;
    public Transform rightWallCheck;
    public float wallCheckDistance = 0.1f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canJump = true;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;
    private bool isWallJumping;
    private float wallJumpTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Check ground and wall states
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isTouchingLeftWall = Physics2D.Raycast(leftWallCheck.position, Vector2.left, wallCheckDistance, groundLayer);
        isTouchingRightWall = Physics2D.Raycast(rightWallCheck.position, Vector2.right, wallCheckDistance, groundLayer);

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
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius);

        // Draw wall check gizmos
        Gizmos.color = isTouchingLeftWall ? Color.red : Color.blue;
        Gizmos.DrawLine(leftWallCheck.position, leftWallCheck.position + Vector3.left * wallCheckDistance);

        Gizmos.color = isTouchingRightWall ? Color.red : Color.blue;
        Gizmos.DrawLine(rightWallCheck.position, rightWallCheck.position + Vector3.right * wallCheckDistance);
    }
}

/*{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float wallJumpForce = 8f;
    public float wallJumpLerpTime = 0.2f;
    public float postWallJumpSpeedModifier = 0.5f;
    public float postWallJumpDuration = 0.5f;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    public Transform leftWallCheck;
    public Transform rightWallCheck;
    public float wallCheckDistance = 0.1f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canJump = true;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;
    private bool isWallJumping;
    private float wallJumpTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Check ground and wall states
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isTouchingLeftWall = Physics2D.Raycast(leftWallCheck.position, Vector2.left, wallCheckDistance, groundLayer);
        isTouchingRightWall = Physics2D.Raycast(rightWallCheck.position, Vector2.right, wallCheckDistance, groundLayer);

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
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius);

        // Draw wall check gizmos
        Gizmos.color = isTouchingLeftWall ? Color.red : Color.blue;
        Gizmos.DrawLine(leftWallCheck.position, leftWallCheck.position + Vector3.left * wallCheckDistance);

        Gizmos.color = isTouchingRightWall ? Color.red : Color.blue;
        Gizmos.DrawLine(rightWallCheck.position, rightWallCheck.position + Vector3.right * wallCheckDistance);
    }
}
*/