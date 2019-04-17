using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateSystem;

public class MoveState : State<MonsterInteraction>
{
    #region initiate
    private static MoveState instance;

    public MoveState(MonsterInteraction owner) : base(owner)
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }
    #endregion

    public override void EnterState()
    {
        Debug.Log("Enter move state");
        owner.EnterMoveState();
    }

    public override void ExecuteState()
    {
        if (owner.isArrived)
        {
            owner.timer += Time.deltaTime;

            if (owner.IsWaitTime())
            {
                // change to Action state
                owner.stateMachine.ChangeState(new ActionState(owner));
            }
        }
    }

    public override void ExitState()
    {
        owner.ExitMoveState();
    }
}
