using Anonym.Isometric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StateSystem;

[System.Serializable]
public class MonsterData
{
    public Vector3 position;
    public Status status;
}

public class MonsterInteraction : MonoBehaviour
{
    public Tile tileTarget;
    public float waterAmount;

    public ActionBubble actionBubble;

    [SerializeField]
    IsometricMovement target;

    [Header("Status field")]
    [SerializeField]
    private Status status;
    public float hungerDecay;
    public float tirenessDecay;
    public float emotionDecay;
    private UI_GaugeArea ui_gaugeArea;

    [Header("Delay time on each state")]
    [SerializeField]
    private float waitTimeBeforeAction;
    [SerializeField]
    private float timePerAction;

    [SerializeField, HideInInspector]
    IsometricNavMeshAgent NMAgent = null;

    // state index 0 = move, 1 = action
    public int actionIndex = 0;

    [Header("Checking each state")]
    public bool isArrived = false;
    public bool isThinkAction = false;
    public bool isOnMoveState = false;
    public bool isOnActionState = false;
    public bool isStateMachineRunning = false;

    public float timer;

    private MonsterData data;
    private Animator animator;

    public StateMachine<MonsterInteraction> stateMachine { get; set; }

    private void OnEnable()
    {
        GameManager.SecondEvent += DecreaseStatusOverSecond;
    }

    private void OnDisable()
    {
        GameManager.SecondEvent -= DecreaseStatusOverSecond;
    }

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        LoadMonsterData();
  
        ui_gaugeArea = FindObjectOfType<UI_GaugeArea>();
        if (ui_gaugeArea != null)
        {
            ui_gaugeArea.SetGauge(status.hunger, status.tireness, status.emotion);
        }
        else
        {
            Debug.Log("Please attach UI_GaugeArea in Game Scene");
        }

        Init();
        timer = 0f;

        StartCoroutine(WaitInitBrain());
    }

    private void Update()
    {
        if (NMAgent.vHorizontalMovement.magnitude > 0) animator.SetTrigger("Moving");
        animator.SetFloat("Velocity_x", NMAgent.vHorizontalMovement.x);
        animator.SetFloat("Velocity_z", NMAgent.vHorizontalMovement.z);
        if (isStateMachineRunning)
            stateMachine.Update();
    }

    #region initial
    private void Init()
    {
        if (target == null)
            SetTarget(GetComponent<IsometricMovement>());

        if (target == null)
            SetTarget(FindObjectOfType<IsometricMovement>());
    }

    private void SetTarget(IsometricMovement newTarget)
    {
        target = newTarget;
        NMAgent = target == null ? null : target.GetComponent<IsometricNavMeshAgent>();
    }
    #endregion

    #region StateMachineFunc
    public IEnumerator WaitInitBrain ()
    {
        while (!ActionManager.instance.isInitBrain)
        {
            yield return null;
            stateMachine = new StateMachine<MonsterInteraction>(this);
            stateMachine.ChangeState(new MoveState(this));
            isStateMachineRunning = true;
        }
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

    private bool IsArrivedNow()
    {
        float distance = Vector3.Distance(transform.position, tileTarget.pos);

        if (distance < 0.3f)
        {
            isArrived = true;

            return true;
        }

        return false;
    }

    private void MoveToTarget()
    {
        Vector3 targetPosition = GetTargetPosition();
        NMAgent.MoveToDestination(targetPosition);
    }

    private void DoAction()
    {
        ActionManager.instance.SetActionIndex(actionIndex);

        if (tileTarget.typeTile == TypeTile.FoodTile)
        {
            tileTarget.ActionResult(0, this);
            DisplayBubble(3);
        }
        else if (tileTarget.typeTile == TypeTile.RestTile)
        {
            tileTarget.ActionResult(0, this);
            DisplayBubble(4);
        }
        else if (tileTarget.typeTile == TypeTile.WorkTile)
        {
            List<double> info = tileTarget.info;

            int index = ActionManager.instance.CalculateAction(actionIndex, info);
            //int index = Random.Range(0, 3);
            tileTarget.ActionResult(index, this);
            DisplayBubble(index);
        }

        isThinkAction = true;
    }

    private Vector3 GetTargetPosition()
    {
        //status.RandomStatus();

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

    private List<double> GetStatusStates()
    {
        List<double> states = new List<double>();
        states.Add(status.GetHungryRatio() - 0.5f);
        states.Add(status.GetTirenessRatio() - 0.5f);
        states.Add(status.GetEmotionRatio() - 0.5f);

        Debug.Log(states[0] + ", " + states[1] + ", " + states[2]);

        return states;
    }

    public bool IsWaitTime ()
    {
        timer += Time.deltaTime;

        // On move state
        if (actionIndex == 0 && timer < waitTimeBeforeAction)
        {
            return false;
        }

        // On action state
        if (actionIndex == 1 && timer < timePerAction)
        {
            return false;
        }

        return true;
    }

    public void DisplayBubble (int index)
    {
        string key = "";
        switch (index)
        {
            case (0) :
                key = "Idle";
                break;
            case (1) :
                key = "Harvest";
                break;
            case (2) :
                key = "Water";
                break;
            case (3) :
                key = "Eat";
                break;
            case (4) :
                key = "Sleep";
                break;
            default :
                key = "Invalid Index in MonsterInteraction.cs";
                break;
        }
        actionBubble.ShowAction(key);
    }

    public void RemoveBubble()
    {
        actionBubble.Disappear();
    }

    public void EnterMoveState ()
    {
        actionIndex = 0;
        isOnMoveState = true;
        StartCoroutine(MoveState());
    }

    public void ExitMoveState ()
    {
        isArrived = false;
        isOnMoveState = false;
        timer = 0f;

        // set reward move state
        Debug.Log("Action index: " + actionIndex);
        ActionManager.instance.SetMemory();
    }

    public void MonsterAction ()
    {
        actionIndex = 1;
        isOnActionState = true;
        DoAction();
    }

    public void ExitAction ()
    {
        RemoveBubble();

        isOnActionState = false;
        isThinkAction = false;
        timer = 0f;

        // set reward action state
        if (tileTarget.typeTile == TypeTile.WorkTile)
            ActionManager.instance.SetMemory();

        SaveMonsterData();
    }
    #endregion

    #region statusMethod
    public void SetStatus(float hungry, float tireness, float emotion)
    {
        status.hunger += hungry;
        status.tireness += tireness;
        status.emotion += emotion;
    }

    public void DecreaseStatusOverSecond ()
    {
        float hunger = Mathf.Clamp(status.hunger + hungerDecay, 0f, 100f);
        float tireness = Mathf.Clamp(status.tireness + tirenessDecay, 0f, 100f);

        bool decreaseEmotion = (hunger < 50 && tireness < 50);
        float emotion = decreaseEmotion ? Mathf.Clamp(status.emotion + emotionDecay, 0f, 100f) : Mathf.Clamp(status.emotion - emotionDecay, 0f, 100f);

        status.SetStatus(hunger, tireness, emotion);
        if (ui_gaugeArea != null)
        {
            Debug.Log("status: " + status.hunger + ", " + status.tireness + ", " + status.emotion);
            ui_gaugeArea.SetGauge(status.hunger, status.tireness, status.emotion);
        }
        else
        {
            Debug.Log("Please attach UI_GaugeArea in Game Scene");
        }
    }
    #endregion

    #region SaveLoad
    public void SaveMonsterData ()
    {
        data.position = transform.position;
        data.status = status;
        DataManager.Instance.SaveMonsterData(data);
    }

    public void LoadMonsterData ()
    {
        if (DataManager.Instance.current_monsterData != null)
        {
            data = DataManager.Instance.current_monsterData;
            transform.position = data.position;
            status = data.status;
        }
        else
        {
            status = new Status();
            data = new MonsterData();
        }
    }
    #endregion

    private int GetRandomActionIndex ()
    {
        return (int)Random.Range(0, 5);
    }
}
