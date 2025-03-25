using UnityEngine;

public class ChargeState : EnemyBaseState
{
    public ChargeState(Bittles bittles, string animationName) : base(bittles, animationName) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // If the enemy detects the player for a jump attack, switch states
        if (bittles.CheckForJumpAttack())
        {
            bittles.SwitchState(bittles.jumpAttackState);
            return;
        }

        // If the enemy no longer detects the player, return to patrol
        if (!bittles.CheckForPlayer())
        {
            bittles.SwitchState(bittles.patrolState);
            return;
        }

        // If the enemy detects an obstacle or ledge, stop charging
        if (bittles.CheckForObstacles())
        {
            bittles.SwitchState(bittles.patrolState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Charge();
    }

    void Charge()
    {
        bittles.rb.linearVelocity = new Vector2(bittles.chargeSpeed * bittles.facingDirection, bittles.rb.linearVelocity.y);
    }
}


/* {
    
    public ChargeState(Bittles bittles, string animationName) : base(bittles, animationName)
    {

    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (Time.time >= bittles.stateTime + bittles.chargeTime)
        {
            if (bittles.CheckForPlayer())
                bittles.SwitchState(bittles.playerDetectedState);
            else
                bittles.SwitchState(bittles.patrolState);
        }
        else
        {
            Charge();
        }
        
    }

    void Charge()
    {
        bittles.rb.linearVelocity = new Vector2(bittles.chargeSpeed * bittles.facingDirection, bittles.rb.linearVelocity.y);
    }

}*/
