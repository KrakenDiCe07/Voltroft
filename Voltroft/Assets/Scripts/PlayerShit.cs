using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShit : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float wallJumpPush = 5f;

    [Header("References")]
    public LayerMask groundLayer;
    public LayerMask leftWallLayer;
    public LayerMask rightWallLayer;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private bool isGrounded;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;
    private bool canJump = true;
    private bool isWallClinging = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        HandleMovement();
        CheckGroundAndWalls();
        HandleJumping();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");

        if (!isWallClinging)
        {
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        }
        else
        {
            if ((isTouchingLeftWall && horizontal > 0) || (isTouchingRightWall && horizontal < 0))
            {
                rb.velocity = new Vector2(0, 0); // Stick to the wall
            }
            else
            {
                isWallClinging = false; // Detach if moving away
            }
        }
    }

    private void CheckGroundAndWalls()
    {
        isGrounded = IsTouchingLayer(groundLayer);
        isTouchingLeftWall = IsTouchingLayer(leftWallLayer);
        isTouchingRightWall = IsTouchingLayer(rightWallLayer);

        if (isGrounded)
        {
            canJump = true;
            isWallClinging = false;
        }
        else if (isTouchingLeftWall || isTouchingRightWall)
        {
            if ((isTouchingLeftWall && Input.GetAxis("Horizontal") > 0) ||
                (isTouchingRightWall && Input.GetAxis("Horizontal") < 0))
            {
                isWallClinging = true;
                canJump = true;
                rb.velocity = new Vector2(0, 0); // Stick to the wall
            }
            else
            {
                isWallClinging = false;
            }
        }
        else
        {
            isWallClinging = false;
        }
    }

    private bool IsTouchingLayer(LayerMask layer)
    {
        return playerCollider.IsTouchingLayers(layer);
    }

    private void HandleJumping()
    {
        if (Input.GetButtonDown("Jump") && canJump)
        {
            if (isWallClinging)
            {
                if (isTouchingLeftWall)
                {
                    rb.velocity = new Vector2(wallJumpPush, jumpForce);
                }
                else if (isTouchingRightWall)
                {
                    rb.velocity = new Vector2(-wallJumpPush, jumpForce);
                }
            }
            else if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            canJump = false;
        }
    }
}

/*{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float wallJumpPush = 5f;

    [Header("References")]
    public LayerMask groundLayer;
    public LayerMask leftWallLayer;
    public LayerMask rightWallLayer;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private bool isGrounded;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;
    private bool canJump = true;
    private bool isWallClinging = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        HandleMovement();
        CheckGroundAndWalls();
        HandleJumping();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");

        if (!isWallClinging)
        {
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        }
    }

    private void CheckGroundAndWalls()
    {
        isGrounded = IsTouchingLayer(groundLayer);
        isTouchingLeftWall = IsTouchingLayer(leftWallLayer);
        isTouchingRightWall = IsTouchingLayer(rightWallLayer);

        if (isGrounded)
        {
            canJump = true;
            isWallClinging = false;
        }
        else if (isTouchingLeftWall || isTouchingRightWall)
        {
            isWallClinging = true;
            canJump = true;
            rb.velocity = new Vector2(0, 0); // Stick to the wall
        }
        else
        {
            isWallClinging = false;
        }
    }

    private bool IsTouchingLayer(LayerMask layer)
    {
        return playerCollider.IsTouchingLayers(layer);
    }

    private void HandleJumping()
    {
        if (Input.GetButtonDown("Jump") && canJump)
        {
            if (isWallClinging)
            {
                if (isTouchingLeftWall)
                {
                    rb.velocity = new Vector2(wallJumpPush, jumpForce);
                }
                else if (isTouchingRightWall)
                {
                    rb.velocity = new Vector2(-wallJumpPush, jumpForce);
                }
            }
            else if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            canJump = false;
        }
    }
}
*/