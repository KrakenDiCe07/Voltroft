using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseState
{
    protected Bittles bittles;
    protected string animationName;

    public EnemyBaseState(Bittles bittles, string animationName)
    {
        this.bittles = bittles;
        this.animationName = animationName;
    }

    public virtual void Enter()
    {
        Debug.Log("Entered: " + animationName);
    }
    public virtual void Exit() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }
}
