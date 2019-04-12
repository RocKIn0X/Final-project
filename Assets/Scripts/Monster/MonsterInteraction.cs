using Anonym.Isometric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInteraction : MonoBehaviour
{
    public Tile tileTarget;
    public ActionBubble actionBubble;

    [SerializeField]
    IsometricMovement target;
    [SerializeField]
    private Status status;
    [SerializeField]
    private float timePerAction;
    [SerializeField, HideInInspector]
    IsometricNavMeshAgent NMAgent = null;

    private bool isOnMoveState = false;

    private void Start()
    {
        status = new Status();
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isOnMoveState = true;

            Vector3 targetPosition = GetTargetPosition();
            
            NMAgent.MoveToDestination(targetPosition);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Pressed A");
            tileTarget.EatHere();
        }
    }

    public void DisplayBubble (int index)
    {
        // actionBubble.ShowAction(timePerAction);
    }

    public void MonsterAction ()
    {
        int index = GetRandomActionIndex();
        // tileTarget.GetComponent<TileClass>().Action(index, this);
        // DisplayBubble(index);
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

    private Vector3 GetTargetPosition ()
    {
        status.RandomStatus();

        int index = ActionManager.instance.CalculateAction(GetStatusStates());
        Debug.Log("Index: " + index);

        Vector3 targetPosition = this.transform.position;
        string tileName = "";
        if (index == 0)
        {
            tileName = "Work Tile";
            tileTarget = TileManager.Instance.GetWorkTile();
        }
        else if (index == 1)
        {
            tileName = "Food Tile";
            tileTarget = TileManager.Instance.GetFoodTile();
        }
        else if (index == 2)
        {
            tileName = "Rest Tile";
            tileTarget = TileManager.Instance.GetRestTile();
        }
        Debug.Log("Move to " + tileName);

        targetPosition = tileTarget.pos;

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
        return (int)Random.Range(0, 6);
    }

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("Hit something");
        //tileTarget = col.gameObject;
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject == tileTarget)
        {
            tileTarget = null;
        }
    }
}
