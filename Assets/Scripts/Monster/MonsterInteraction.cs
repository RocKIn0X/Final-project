using Anonym.Isometric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class MonsterInteraction : MonoBehaviour
{
    public GameObject tileTarget;
    public ActionBubble actionBubble;

    [SerializeField]
    IsometricMovement target;
    [SerializeField]
    private Status status;
    [SerializeField]
    private float timePerAction;
    [SerializeField, HideInInspector]
    IsometricNavMeshAgent NMAgent = null;

    private void Start()
    {
        status = new Status();
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Vector3 targetPosition = TileManager.Instance.GetWorkTilePosition();
            NMAgent.MoveToDestination(targetPosition);
            Debug.Log("Move to (" + targetPosition + ")");
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

    private int GetRandomActionIndex ()
    {
        return (int)Random.Range(0, 6);
    }

    private void OnTriggerEnter(Collider col)
    {
        tileTarget = col.gameObject;
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject == tileTarget)
        {
            tileTarget = null;
        }
    }
}
