using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class MonsterInteraction : MonoBehaviour
{
    public GameObject tileTarget;
    public ActionBubble actionBubble;

    [SerializeField]
    private Status status;
    [SerializeField]
    private float timePerAction;

    private void Start()
    {
        status = new Status();
    }

    public void DisplayBubble (int index)
    {
        // actionBubble.ShowAction(timePerAction);
    }

    public void MonsterAction (int index)
    {
        // tileTarget.GetComponent<TileClass>().Action(index, this);
        // DisplayBubble(index);
    }

    public void SetStatus (int hungry, int tireness, int emotion)
    {
        status.SetStatus(hungry, tireness, emotion);
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
