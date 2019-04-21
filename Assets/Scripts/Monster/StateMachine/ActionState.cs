using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateSystem;

public class ActionState : State<MonsterInteraction>
{
    #region initiate
    private static ActionState instance;

    public ActionState(MonsterInteraction owner) : base(owner)
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
        Debug.Log("Enter action state");
        owner.isOnActionState = true;

        // calculate action from action manager
        if (!owner.isThinkAction)
            owner.MonsterAction();
    }

    public override void ExecuteState()
    {
        owner.timer += Time.deltaTime;

        if (owner.IsWaitTime())
        {
            // change to Move state
            owner.stateMachine.ChangeState(new MoveState(owner));
        }
    }

    public override void ExitState()
    {
        owner.ExitAction();
    }
}
