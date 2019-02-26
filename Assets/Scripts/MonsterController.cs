using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anonym.Isometric;

public class MonsterController : MonoBehaviour
{

    public enum MonsterState
    {
        Idle, Move, Action
    };

    [SerializeField] Tile tiledTarget;

    private IsometricNavMeshAgent navAgent;

    void Start()
    {
        navAgent = GetComponent<IsometricNavMeshAgent>();
    }

    void Update()
    {

    }

    void MoveToDestination(Vector3 vDest)
    {
        navAgent.MoveToDestination(vDest);
    }

}

    //[SerializeField] private float actionStateTime;
    //[SerializeField] private float offsetToTarget;
    //private float timer;
    //private List<Vector3> tileList;
    //private RaycastHit2D hitInfo;
    //private Rigidbody2D rb;
    //private Vector2 moveVelocity;
    //private Vector3 targetPosition;
    //[SerializeField]
    //private Transform top;
    //[SerializeField]
    //private Transform bottom;
    //[SerializeField]
    //private Transform left;
    //[SerializeField]
    //private Transform right;

//public float speed;
//public LayerMask tileLayer;
//public MonsterState state;

//void Start()
//{
//    rb = GetComponent<Rigidbody2D>();
//    state = MonsterState.Idle;
//    timer = actionStateTime;

//    tileList = new List<Vector3>();
//    ShootRayForCheckingTile();
//    targetPosition = tileList[Random.Range(0, tileList.Count)];
//}

//void Update()
//{
//    // Vector3 movementDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
//    // transform.position += movementDirection * speed * Time.deltaTime;
//    UpdateBehavior();
//    moveVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
//}

//void FixedUpdate()
//{
//    rb.MovePosition(rb.position + moveVelocity * speed * Time.fixedDeltaTime);
//    // ShootRayForCheckingTile();
//    // MoveToTarget(targetPosition);
//}

//void Idle ()
//{
//    tileList.Clear();
//    ShootRayForCheckingTile();
//    targetPosition = tileList[Random.Range(0, tileList.Count)];
//    state = MonsterState.Move;
//}

//void Action (float time)
//{
//    timer -= time;

//    if (timer <= 0)
//    {
//        Debug.Log("Exit action state");
//        timer = actionStateTime;
//        state = MonsterState.Idle;
//    }
//}

//void ShootRayForCheckingTile ()
//{
//    hitInfo = Physics2D.Raycast(top.position, Vector2.up, offsetToTarget, tileLayer);
//    if (hitInfo.collider != null)
//    {
//        Debug.DrawRay(top.position, Vector2.up, Color.red, 2f);
//        tileList.Add(hitInfo.collider.transform.position);
//        Debug.Log("Add tile top");
//    }
//    hitInfo = Physics2D.Raycast(bottom.position, Vector2.down, offsetToTarget, tileLayer);
//    if (hitInfo.collider != null)
//    {
//        Debug.DrawRay(bottom.position, Vector2.down, Color.red, 2f);
//        tileList.Add(hitInfo.collider.transform.position);
//        Debug.Log("Add tile bottom");
//    }
//    hitInfo = Physics2D.Raycast(left.position, Vector2.left, offsetToTarget, tileLayer);
//    if (hitInfo.collider != null)
//    {
//        Debug.DrawRay(left.position, Vector2.left, Color.red, 2f);
//        tileList.Add(hitInfo.collider.transform.position);
//        Debug.Log("Add tile left");
//    }
//    hitInfo = Physics2D.Raycast(right.position, Vector2.right, offsetToTarget, tileLayer);
//    if (hitInfo.collider != null)
//    {
//        Debug.DrawRay(right.position, Vector2.right, Color.red, 2f);
//        tileList.Add(hitInfo.collider.transform.position);
//        Debug.Log("Add tile uright");
//    }
//}

//void MoveToTarget (Vector3 targetPos)
//{
//    float step = speed * Time.deltaTime;

//    if (Vector3.Distance(transform.position, targetPos) > 0.001f)
//    {
//        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
//    }
//    else
//    {
//        state = MonsterState.Action;
//    }
//}

//void UpdateBehavior ()
//{
//    switch (state)
//    {
//        case MonsterState.Idle:
//            Idle();
//            break;
//        case MonsterState.Move:
//            MoveToTarget(targetPosition);
//            break;
//        case MonsterState.Action:
//            Action(Time.deltaTime);
//            break;
//    }
//}
