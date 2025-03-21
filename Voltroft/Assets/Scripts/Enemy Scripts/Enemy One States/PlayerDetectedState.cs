using UnityEngine;

public class PlayerDetectedState : EnemyBaseState
{
    public PlayerDetectedState(Bittles bittles, string animationName) : base (bittles, animationName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();

        bittles.rb.linearVelocity = Vector2.zero;
        bittles.alert.SetActive(true);
    }
    public override void Exit()
    {
        bittles.alert.SetActive(false);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!bittles.CheckForPlayer())
            bittles.SwitchState(bittles.patrolState);
        else
        {
            if(Time.time >= bittles.stateTime + bittles.playerDetectedWaitTime)
                bittles.SwitchState(bittles.jumpAttackState);
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}