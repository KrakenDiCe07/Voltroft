using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShit : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Player's horizontal movement speed
    public float reducedMoveSpeed = 2.5f; // Reduced speed after a wall jump
    public float jumpForce = 10f; // Force applied when the player jumps
    public float wallJumpForce = 10f; // Force applied when the player wall jumps
    public float wallStickDuration = 0.2f; // Delay before the player can re-stick to the wall
    public float speedRecoveryTime = 0.5f; // Time to recover original speed after a wall jump

    [Header("Layer Masks")]
    public LayerMask groundLayer; // Layer representing the ground
    public LayerMask wallLayer; // Layer representing the walls

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Collider2D playerCollider; // Reference to the player's collider

    private bool isGrounded; // Checks if the player is on the ground
    private bool isTouchingWall; // Checks if the player is touching a wall
    private bool isWallClinging; // Checks if the player is clinging to a wall
    private bool canWallJump; // Determines if the player can wall jump

    private float horizontalInput; // Stores horizontal input from the player
    private bool isSpeedReduced; // Tracks if the player's speed is reduced

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to the player
        playerCollider = GetComponent<Collider2D>(); // Get the Collider2D component attached to the player
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // Get horizontal input (A/D or Left/Right arrow keys)
        HandleMovement(); // Handle player's horizontal movement
        CheckSurroundings(); // Check if the player is on the ground or touching a wall

        if (Input.GetButtonDown("Jump")) // Check if the jump button (default: Space) is pressed
        {
            HandleJumping(); // Handle jumping behavior
        }
    }

    private void HandleMovement()
    {
        float currentSpeed = isSpeedReduced ? reducedMoveSpeed : moveSpeed; // Use reduced speed if applicable

        if (isWallClinging)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0); // Stop vertical movement while clinging to the wall
        }
        else
        {
            rb.velocity = new Vector2(horizontalInput * currentSpeed, rb.velocity.y); // Apply horizontal movement
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = playerCollider.IsTouchingLayers(groundLayer); // Check if the player is touching the ground layer
        isTouchingWall = playerCollider.IsTouchingLayers(wallLayer); // Check if the player is touching the wall layer

        if (isGrounded)
        {
            isWallClinging = false; // Disable wall clinging when on the ground
            canWallJump = true; // Reset wall jump ability
        }
        else if (isTouchingWall && !isGrounded && horizontalInput != 0)
        {
            isWallClinging = true; // Enable wall clinging when moving toward a wall
        }
        else
        {
            isWallClinging = false; // Disable wall clinging otherwise
        }
    }

    private void HandleJumping()
    {
        if (isGrounded)
        {
            Jump(Vector2.up * jumpForce); // Perform a normal upward jump
        }
        else if (isWallClinging && canWallJump)
        {
            // Determine the direction of the wall jump based on wall side and input direction
            Vector2 wallJumpDirection = isTouchingWall && horizontalInput > 0 ? new Vector2(-1, 1) : new Vector2(1, 1);
            Jump(wallJumpDirection.normalized * wallJumpForce); // Perform a wall jump
            StartCoroutine(ResetWallCling()); // Prevent immediate re-clinging
            StartCoroutine(ReduceSpeedTemporarily()); // Temporarily reduce speed after a wall jump
        }
    }

    private void Jump(Vector2 force)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); // Reset vertical velocity before applying the jump
        rb.AddForce(force, ForceMode2D.Impulse); // Apply the jump force
    }

    private IEnumerator ResetWallCling()
    {
        canWallJump = false; // Temporarily disable wall jumping
        isWallClinging = false; // Temporarily disable wall clinging
        yield return new WaitForSeconds(wallStickDuration); // Wait for a short duration
        canWallJump = true; // Re-enable wall jumping
    }

    private IEnumerator ReduceSpeedTemporarily()
    {
        isSpeedReduced = true; // Reduce the player's speed
        yield return new WaitForSeconds(speedRecoveryTime); // Wait for the recovery time
        isSpeedReduced = false; // Restore the player's original speed
    }
}
