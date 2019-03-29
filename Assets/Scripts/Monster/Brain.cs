using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Replay
{
    public List<double> states;
    public double reward;

    public Replay(double hungry, double tireness, double emotion, double r)
    {
        states = new List<double>();
        states.Add(hungry);
        states.Add(tireness);
        states.Add(emotion);
        reward = r;
    }
}


public class Brain : MonoBehaviour
{
    [SerializeField]
    ANN ann;

    bool isPunished = false;                        //punished by player
    bool giveReward = false;
    bool isMoveState = false;
    bool isTrainedDone = false;

    float reward = 0.0f;                            //reward to associate with actions
    List<Replay> replayMemory = new List<Replay>(); //memory - list of past actions and rewards
    int mCapacity = 10000;                          //memory capacity

    float discount = 0.99f;                         //how much future states affect rewards
    float exploreRate = 100.0f;                     //chance of picking random action
    float maxExploreRate = 100.0f;					//max chance value
    float minExploreRate = 0.01f;					//min chance value
    float exploreDecay = 0.01f;                   //chance decay amount for each update

    // Vector3 ballStartPos;                           //record start position of object
    int punishedCount = 0;                              //count player punished
    // float moveForce = 0.5f;                         //max force to apply force each update
                                                    //make sure this is large enough so that the q value
                                                    //multiplied by it is enough to recover balance
                                                    //when the ball gets a good speed up
    float timer = 0;                                //timer to keep track of balancing
    float maxBalanceTime = 0;                       //record time ball is kept balanced	

    [SerializeField]
    private Status status;

    void Start()
    {
        Init();

        while (!PreTrainedDone())
        {

            AutoMoveState();
        }

        status.Reset();
    }

    GUIStyle guiStyle = new GUIStyle();
    void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 600, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 500, 30), "Hungry: " + status.GetHungryRatio(), guiStyle);
        GUI.Label(new Rect(10, 50, 500, 30), "Tireness: " + status.GetTirenessRatio(), guiStyle);
        GUI.Label(new Rect(10, 75, 500, 30), "Emotion: " + status.GetEmotionRatio(), guiStyle);
        GUI.Label(new Rect(10, 100, 500, 30), "Explore rate: " + exploreRate, guiStyle);
        GUI.EndGroup();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTrainedDone)
        {
            if (Input.GetKeyDown("l"))
            {
                ann.LoadWeights();
            }

            if (Input.GetKeyDown("s"))
            {
                ann.SaveWeights();
            }

            if (Input.GetKeyDown("space") && !isMoveState)
            {
                StartCoroutine(MoveState());
            }

            if (isMoveState)
            {
                if (Input.GetKeyDown("q") && !giveReward)
                    Punished();

                else if (Input.GetKeyDown("e") && !giveReward)
                    Praised();
            }
        }
    }

    void FixedUpdate()
    {
        /*
        timer += Time.deltaTime;

        if (timer >= 5f)
        {
            Debug.Log("Time passed");
            // status.SetStatus(-5, -5, -5);
            timer = 0;
        }
        */
    }

    public IEnumerator MoveState ()
    {
        Debug.Log("Start Move State!");

        isMoveState = true;
        reward = 0.0f;

        List<double> states = new List<double>();
        List<double> qs = new List<double>();

        double hungry = status.GetHungryRatio() - 0.5f;
        double tireness = status.GetTirenessRatio() - 0.5f;
        double emotion = status.GetEmotionRatio() - 0.5f;
        double maxQ;

        states.Add(hungry);
        states.Add(tireness);
        states.Add(emotion);

        Debug.Log("State: " + states[0] + ", " + states[1] + ", " + states[2]);
        qs = SoftMax(ann.CalcOutput(states));
        Debug.Log("QS: " + qs[0] + ", " + qs[1] + ", " + qs[2]);
        maxQ = qs.Max();

        int maxQIndex = qs.ToList().IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

        if (Random.Range(0, 100) < exploreRate)
           maxQIndex = Random.Range(0, 2);

        // Choose action from max Q Index
        ChooseAction(maxQIndex);

        yield return new WaitForSeconds(3f);
        Debug.Log("Arrive");
        reward = GetReward();
        // reward = GetAutoReward(maxQIndex);
        Debug.Log("Reward: " + reward);
        AddMemory(hungry, tireness, emotion, reward);
        if (reward < 0)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            TrainANN(maxQ);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Debug.Log("Total training time: " + elapsedMs);
        }
        yield return new WaitForSeconds(3f);
        Debug.Log("Change to Action state");
        isMoveState = false;
        giveReward = false;
    }

    public void AutoMoveState ()
    {
        isMoveState = true;
        reward = 0.0f;

        List<double> states = new List<double>();
        List<double> qs = new List<double>();

        // Random status
        status.RandomStatus();

        double hungry = status.GetHungryRatio();
        double tireness = status.GetTirenessRatio();
        double emotion = status.GetEmotionRatio();
        double maxQ;

        states.Add(hungry - 0.5f);
        states.Add(tireness - 0.5f);
        states.Add(emotion - 0.5f);

        qs = SoftMax(ann.CalcOutput(states));
        maxQ = qs.Max();

        int maxQIndex = qs.ToList().IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

        if (Random.Range(0, 100) < exploreRate)
        {
            maxQIndex = Random.Range(0, 3);
        }

        // Choose action from max Q Index
        ChooseAction(maxQIndex);

        reward = GetPreTrainReward(maxQIndex);
        Debug.Log("State: " + hungry + ", " + tireness + ", " + emotion + " | Action: " + maxQIndex + " | Reward: " + reward);

        AddMemory(hungry, tireness, emotion, reward);
        if (reward < 0)
        {
            TrainANN(maxQ);
        }

        isMoveState = false;
        giveReward = false;
    }

    void Init ()
    {
        status = new Status();

        // Setting ANN
        List<int> hiddenLayer = new List<int>();
        hiddenLayer.Add(4);
        hiddenLayer.Add(6);
        hiddenLayer.Add(4);
        ann = new ANN(3, 3, hiddenLayer, 0.2f);
    }

    void ReceiveInput (double hungry, double tireness, double emotion, out double maxQ)
    {
        List<double> states = new List<double>();
        List<double> qs = new List<double>();

        states.Add(hungry);
        states.Add(tireness);
        states.Add(emotion);

        qs = SoftMax(ann.CalcOutput(states));
        maxQ = qs.Max();
    }

    void ChooseAction (int index)
    {
        if (index == 0)
            MoveToWorkTile();

        else if (index == 1)
            MoveToFoodTile();

        else if (index == 2)
            MoveToRestTile();
    }


    void TrainANN(double maxQ)
    {
        Debug.Log("memory count: " + replayMemory.Count);
        for (int i = replayMemory.Count - 1; i >= 0; i--)
        {
            Debug.Log("replay " + i + " state: " + replayMemory[i].states[0] + ", " + replayMemory[i].states[1] + ", " + replayMemory[i].states[2]);

            List<double> toutputsOld = new List<double>();
            List<double> toutputsNew = new List<double>();
            toutputsOld = SoftMax(ann.CalcOutput(replayMemory[i].states));
            Debug.Log("toutputsOld: " + toutputsOld[0] + ", " + toutputsOld[1] + ", " + toutputsOld[2]);

            double maxQOld = toutputsOld.Max();
            int action = toutputsOld.ToList().IndexOf(maxQOld);

            double feedback;
            // bug
            if (i == replayMemory.Count - 1)
                feedback = replayMemory[i].reward;
            else
            {
                toutputsNew = SoftMax(ann.CalcOutput(replayMemory[i + 1].states));
                maxQ = toutputsNew.Max();
                feedback = (replayMemory[i].reward +
                    discount * maxQ);
            }

            toutputsOld[action] = feedback;
            Debug.Log("update toutputsOld: " + toutputsOld[0] + ", " + toutputsOld[1] + ", " + toutputsOld[2]);
            // Bug!!!
            ann.Train(replayMemory[i].states, toutputsOld);
        }
        replayMemory.Clear();
    }

    bool PreTrainedDone ()
    {
        if (exploreRate < 5f)
        {
            Debug.Log("Pre Training DONE!");

            isTrainedDone = true;

            return true;
        }

        return false;
    }

    float GetPreTrainReward (int actionIndex)
    {
        if (status.GetHungryRatio() > 0.7f && status.GetTirenessRatio() > 0.7f)
        {
            if (actionIndex == 0) return 0.1f;
        }

        if (actionIndex == 0)
        {
            if (status.GetHungryRatio() >= 0.4f && status.GetTirenessRatio() >= 0.4f)
            {
                return 0.1f;
            }
            else
            {
                return -0.5f;
            }
        }
        else if (actionIndex == 1)
        {
            if (status.GetHungryRatio() < 0.4f && status.GetTirenessRatio() > status.GetHungryRatio())
            {
                return 0.1f;
            }
            else
            {
                return -0.5f;
            }
        }
        else if (actionIndex == 2)
        {
            if (status.GetTirenessRatio() < 0.4f && status.GetTirenessRatio() <= status.GetHungryRatio())
            {
                return 0.1f;
            }
            else
            {
                return -0.5f;
            }
        }

        Debug.Log("ACTION [" + actionIndex + "] OUT OF TESTCASE: " + status.GetHungryRatio() + ", " + status.GetTirenessRatio());
        return 0.1f;
    }

    float GetReward ()
    {
        if (giveReward)
        {
            giveReward = false;

            if (!isPunished)
                return 0.4f;
            else
                return -0.4f;
        }

        else
            return 0.1f;
    }

    // test player want to work if every statuses are more than half. 
    float GetAutoReward (int actionIndex)
    {
        if (status.GetHungryRatio() > 0.5f && status.GetTirenessRatio() > 0.5f && status.GetEmotionRatio() > 0.5f)
        {
            if (actionIndex == 0) return 0.4f;
        }
        else if (status.GetHungryRatio() < status.GetTirenessRatio())
        {
            if (actionIndex == 1) return 0.4f;
        }
        else if (status.GetTirenessRatio() < status.GetHungryRatio())
        {
            if (actionIndex == 2) return 0.4f;
        }

        if (status.GetHungryRatio() < 0.2f || status.GetTirenessRatio() < 0.2f || status.GetEmotionRatio() < 0.2f)
        {
            if (actionIndex == 0) return -0.4f;
        }

        return 0.1f;
    }

    void Praised ()
    {
        Debug.Log("Praise!");

        giveReward = true;
        isPunished = false;
    }

    void Punished ()
    {
        Debug.Log("Punish!");

        giveReward = true;
        isPunished = true;
    }

    void AddMemory (double hungry, double tireness, double emotion, double reward)
    {
        Replay lastMemory = new Replay(hungry, tireness, emotion, reward);

        if (replayMemory.Count > mCapacity)
            replayMemory.RemoveAt(0);

        replayMemory.Add(lastMemory);
    }

    List<double> SoftMax(List<double> values)
    {
        double max = values.Max();

        float scale = 0.0f;
        for (int i = 0; i < values.Count; ++i)
            scale += Mathf.Exp((float)(values[i] - max));

        List<double> result = new List<double>();
        for (int i = 0; i < values.Count; ++i)
            result.Add(Mathf.Exp((float)(values[i] - max)) / scale);

        return result;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "wall")
        {
            isPunished = true;
        }
    }

    void MoveToFoodTile ()
    {
        Debug.Log("Choose Food Tile");

        // increase hungry status
        int hungry = 30;
        // decrease tireness status
        int tireness = 0;

        status.SetStatus(hungry, tireness);
    }

    void MoveToRestTile ()
    {
        Debug.Log("Choose Rest Tile");

        // decrease hungry status
        int hungry = -5;
        // increase tireness status
        int tireness = 30;

        status.SetStatus(hungry, tireness);
    }

    void MoveToWorkTile ()
    {
        Debug.Log("Choose Work Tile");

        // decrease hungry status
        int hungry = -20;
        // decrease tireness status
        int tireness = -30;

        status.SetStatus(hungry, tireness);
    }
}
