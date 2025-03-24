using UnityEngine;

public class ChargeState : EnemyBaseState
{
    
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

}
