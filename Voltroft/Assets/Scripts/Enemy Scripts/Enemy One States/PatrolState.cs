using UnityEngine;

public class PatrolState : EnemyBaseState 
{
    public PatrolState(Bittles bittles, string animationName) : base (bittles, animationName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (bittles.CheckForPlayer())
            bittles.SwitchState(bittles.playerDetectedState);

        if (bittles.CheckForObstacles())
            Rotate();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (bittles.facingRight)
            bittles.rb.linearVelocity = new Vector2(bittles.speed, bittles.rb.linearVelocity.y);
        else
            bittles.rb.linearVelocity = new Vector2(-bittles.speed, bittles.rb.linearVelocity.y);
    }
    void Rotate()
    {
        bittles.transform.Rotate(0, 180, 0);
        bittles.facingRight = !bittles.facingRight;
    }
}
