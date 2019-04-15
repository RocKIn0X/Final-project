using Anonym.Isometric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateSystem;

public class MonsterInteraction : MonoBehaviour
{
    public Tile tileTarget;
    public ActionBubble actionBubble;

    [SerializeField]
    IsometricMovement target;
    [SerializeField]
    private Status status;
    [SerializeField]
    private float waitTimeBeforeAction;
    [SerializeField]
    private float timePerAction;
    [SerializeField, HideInInspector]
    IsometricNavMeshAgent NMAgent = null;

    public bool isArrived = false;
    public bool isOnMoveState = false;
    public bool isOnActionState = false;

    public float timer;

    public StateMachine<MonsterInteraction> stateMachine { get; set; }

    private void Start()
    {
        status = new Status();
        Init();
        timer = 0f;

        stateMachine = new StateMachine<MonsterInteraction>(this);
        stateMachine.ChangeState(new MoveState(this));
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isOnMoveState = true;
            MoveToTarget();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Pressed A");
            tileTarget.EatHere();
        }
        */


        stateMachine.Update();
    }

    public IEnumerator MoveState ()
    {
        MoveToTarget();
        
        while (!IsArrivedNow())
        {
            yield return null;
        }
    }

    public IEnumerator ActionState ()
    {
        isOnActionState = true;

        // calculate output for action
        while (timer < 3f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public bool IsWaitTime ()
    {
        timer += Time.deltaTime;

        if (isOnMoveState && timer < waitTimeBeforeAction)
        {
            return false;
        }

        if (isOnActionState && timer < timePerAction)
        {
            return false;
        }

        return true;
    }

    public void DisplayBubble (int index)
    {
        actionBubble.ShowAction(0);
        //actionBubble.ShowAction(index);
    }

    public void RemoveBubble()
    {
        actionBubble.Disappear();
    }

    public void MonsterAction ()
    {
        int index = GetRandomActionIndex();
        DisplayBubble(index);

        if (index == 0)
        {
            tileTarget.EatHere(this);
        }
        else if (index == 1)
        {
            tileTarget.HarvestHere(this);
        }
        else if (index == 2)
        {
            tileTarget.PlantHere(this);
        }
        else if (index == 3)
        {
            tileTarget.SleepHere(this);
        }
        else if (index == 4)
        {
            tileTarget.WaterHere(this);
        }
        else
        {
            Debug.Log("Null action");
        }
    }

    public void EndAction ()
    {
        RemoveBubble();

        isOnActionState = false;
        timer = 0f;
    }

    public void SetStatus (int hungry, int tireness)
    {
        status.SetStatus(hungry, tireness);
    }

    // Get index from ml
    /*
    private int GetActionIndex ()
    {
        return;
    }
    */

    private void Init()
    {
        if (target == null)
            SetTarget(GetComponent<IsometricMovement>());

        if (target == null)
            SetTarget(FindObjectOfType<IsometricMovement>());
    }

    public void SetTarget(IsometricMovement newTarget)
    {
        target = newTarget;
        NMAgent = target == null ? null : target.GetComponent<IsometricNavMeshAgent>();
    }

    private void MoveToTarget ()
    {
        Vector3 targetPosition = GetTargetPosition();
        NMAgent.MoveToDestination(targetPosition);
    }

    private Vector3 GetTargetPosition ()
    {
        status.RandomStatus();

        int index = ActionManager.instance.CalculateAction(0, GetStatusStates());
        Debug.Log("Index: " + index);
        tileTarget = TileManager.Instance.GetTile(index);

        Vector3 targetPosition = this.transform.position;

        if (tileTarget != null)
        {
            targetPosition = tileTarget.pos;
        }

        return targetPosition;
    }

    private List<double> GetStatusStates ()
    {
        List<double> states = new List<double>();
        states.Add(status.GetHungryRatio() - 0.5f);
        states.Add(status.GetTirenessRatio() - 0.5f);
        states.Add(status.GetEmotionRatio() - 0.5f);

        Debug.Log(states[0] + ", " + states[1] + ", " + states[2]);

        return states;
    }

    private int GetRandomActionIndex ()
    {
        return (int)Random.Range(0, 5);
    }

    private bool IsArrivedNow ()
    {
        float distance = Vector3.Distance(transform.position, tileTarget.pos);

        if (distance < 0.3f)
        {
            isArrived = true;

            return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider col)
    {
        //Debug.Log("Hit something");
        //tileTarget = col.gameObject;
        //isArrived = true;
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject == tileTarget)
        {
            tileTarget = null;
        }
    }
}
