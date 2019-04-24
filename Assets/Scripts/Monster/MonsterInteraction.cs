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

[System.Serializable]
public enum MonsterCondition
{
    Normal,
    Hungry,
    Tired
}

public class MonsterInteraction : MonoBehaviour
{
    public Tile tileTarget;
    public float waterAmount;

    public ActionBubble actionBubble;
    private MoveMarker moveMarker;

    [SerializeField]
    IsometricMovement target;

    [Header("Status field")]
    public MonsterCondition condition;
    [SerializeField]
    private Status status;
    public float hungerDecay;
    public float tirenessDecay;
    public float emotionDecay;
    public float hungerMinimum = 10f;
    public float tirenessMinimum = 10f;
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
    public bool canTrain = true;
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
        //LoadMonsterData();
        condition = MonsterCondition.Normal;

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
        Debug.Log(NMAgent.vHorizontalMovement);
        animator.SetFloat("Velocity_x", NMAgent.vHorizontalMovement.x);
        animator.SetFloat("Velocity_z", NMAgent.vHorizontalMovement.z);
        if (isStateMachineRunning)
            if (stateMachine != null)
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

        //if (moveMarker == null)
        //    moveMarker = (MoveMarker)FindObjectOfType(typeof(MoveMarker));
        //moveMarker.Disappear();
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

    private bool isNormalCondition ()
    {
        if (status.hunger < hungerMinimum)
        {
            condition = MonsterCondition.Hungry;
            return false;
        }

        if (status.tireness < tirenessMinimum)
        {
            condition = MonsterCondition.Tired;
            return false;
        }

        return true;
    }

    private void MoveToTarget()
    {
        Debug.Log("Move to TARGET");

        if (isNormalCondition())
        {
            Vector3 targetPosition = GetTargetPosition();
            NMAgent.MoveToDestination(targetPosition);
        }
        else
        {
            canTrain = false;

            if (condition == MonsterCondition.Hungry)
            {
                // if tile here equal work tile
                if (tileTarget.typeTile == TypeTile.WorkTile && tileTarget.gameObject.GetComponent<WorkTile>().crop.HasCrop())
                {
                    Debug.Log("Eat");
                    isArrived = true;
                }
                else
                {
                    // set tile target to be food tile
                    tileTarget = TileManager.Instance.GetTile(1);
                    NMAgent.MoveToDestination(tileTarget.pos);
                }
            }
            else if (condition == MonsterCondition.Tired)
            {
                isArrived = true;
            }

        }
    }

    private void DoAction()
    {
        ActionManager.instance.SetActionIndex(actionIndex);

        if (tileTarget.typeTile == TypeTile.FoodTile)
        {
            EatImmedietely();
        }
        else if (tileTarget.typeTile == TypeTile.RestTile)
        {
            RestImmedietely();
        }
        else if (tileTarget.typeTile == TypeTile.WorkTile)
        {
            List<double> info = tileTarget.info;

            int index = ActionManager.instance.CalculateAction(actionIndex, info);
            tileTarget.ActionResult(index, this);
            DisplayBubble(index);
        }

        isThinkAction = true;
    }

    private void EatImmedietely ()
    {
        canTrain = false;
        tileTarget.ActionResult(3, this);
        DisplayBubble(3);
        isThinkAction = true;
    }

    private void RestImmedietely ()
    {
        canTrain = false;
        tileTarget.ActionResult(4, this);
        DisplayBubble(4);
        isThinkAction = true;
    }

    private Vector3 GetTargetPosition()
    {
        int index = ActionManager.instance.CalculateAction(0, GetStatusStates());
        tileTarget = TileManager.Instance.GetTile(index);

        Vector3 targetPosition = this.transform.position;

        if (tileTarget != null)
        {
            targetPosition = tileTarget.pos;
            if (moveMarker == null)
                moveMarker = (MoveMarker)FindObjectOfType(typeof(MoveMarker));
            moveMarker.SetMarker(tileTarget);
        }

        return targetPosition;
    }

    private List<double> GetStatusStates()
    {
        List<double> states = new List<double>();
        states.Add(status.GetHungryRatio() - 0.5f);
        states.Add(status.GetTirenessRatio() - 0.5f);
        states.Add(status.GetEmotionRatio() - 0.5f);

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
                animator.SetBool("Sleep", true);
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
        condition = MonsterCondition.Normal;

        actionIndex = 0;
        canTrain = true;
        isOnMoveState = true;
        StartCoroutine(MoveState());
        ActionManager.instance.SetActionIndex(actionIndex);
    }

    public void ExitMoveState ()
    {
        Debug.Log("Exit move state");
        isArrived = false;
        isOnMoveState = false;
        timer = 0f;

        moveMarker.Disappear();

        // set reward move state
        if (condition == MonsterCondition.Normal)
        {
            ActionManager.instance.SetMemory();
        }
    }

    public void MonsterAction ()
    {
        actionIndex = 1;
        canTrain = true;
        isOnActionState = true;
        ActionManager.instance.SetActionIndex(actionIndex);
        if (condition == MonsterCondition.Normal) DoAction();
        else if (condition == MonsterCondition.Hungry) EatImmedietely();
        else if (condition == MonsterCondition.Tired) RestImmedietely();
    }

    public void ExitAction ()
    {
        RemoveBubble();

        isOnActionState = false;
        isThinkAction = false;
        timer = 0f;
        animator.SetBool("Sleep", false);
        // set reward action state
        if (tileTarget.typeTile == TypeTile.WorkTile)
        {
            if (condition == MonsterCondition.Normal) ActionManager.instance.SetMemory();
        }


        //SaveMonsterData();
    }
    #endregion

    #region statusMethod
    public void SetStatus(float hungry, float tireness, float emotion)
    {
        status.hunger = Mathf.Clamp(status.hunger + hungry, 0, 100f);
        status.tireness = Mathf.Clamp(status.tireness + tireness, 0, 100f);
        status.emotion = Mathf.Clamp(status.emotion + emotion, 0, 100f);
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
