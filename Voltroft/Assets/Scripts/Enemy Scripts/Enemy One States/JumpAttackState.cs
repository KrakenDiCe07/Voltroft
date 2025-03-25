using UnityEngine;
using System.Collections;

public class JumpAttack : EnemyBaseState
{
    private bool hasJumped;
    private bool isWaiting;
    private GameObject player;

    public JumpAttack(Bittles bittles, string animationName) : base(bittles, animationName) { }

    public override void Enter()
    {
        base.Enter();
        hasJumped = false;
        isWaiting = false;
        bittles.rb.linearVelocity = Vector2.zero; // Stop before jumping

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            JumpTowardsPlayer();
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Wait after landing before chasing
        if (hasJumped && !isWaiting && bittles.rb.linearVelocity.y == 0)
        {
            bittles.StartCoroutine(WaitBeforeChasing());
            isWaiting = true;
        }
    }

    private void JumpTowardsPlayer()
    {
        hasJumped = true;

        if (player == null) return;

        Vector2 direction = (player.transform.position - bittles.transform.position).normalized;

        // Apply jump force
        float jumpForceX = direction.x * bittles.jumpAttackSpeed; // Horizontal force
        float jumpForceY = Mathf.Abs(bittles.jumpAttackSpeed * 1.5f); // Upward force

        bittles.rb.linearVelocity = new Vector2(jumpForceX, jumpForceY);
    }

    private IEnumerator WaitBeforeChasing()
    {
        yield return new WaitForSeconds(bittles.jumpAttackTime); // Wait before switching
        bittles.SwitchState(bittles.chargeState);
    }
}