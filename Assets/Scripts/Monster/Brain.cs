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

    float reward = 0.0f;                            //reward to associate with actions
    List<Replay> replayMemory = new List<Replay>(); //memory - list of past actions and rewards
    int mCapacity = 10000;                          //memory capacity

    float discount = 0.99f;                         //how much future states affect rewards
    float exploreRate = 100.0f;                     //chance of picking random action
    float maxExploreRate = 100.0f;					//max chance value
    float minExploreRate = 0.01f;					//min chance value
    float exploreDecay = 0.0001f;                   //chance decay amount for each update

    // Vector3 ballStartPos;                           //record start position of object
    int failCount = 0;                              //count when the ball is dropped
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
        GUI.Label(new Rect(10, 100, 500, 30), "Timer: " + timer, guiStyle);
        GUI.EndGroup();
    }

    // Update is called once per frame
    void Update()
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

    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= 5f)
        {
            Debug.Log("Time passed");
            // status.SetStatus(-5, -5, -5);
            timer = 0;
        }
    }

    public IEnumerator MoveState ()
    {
        Debug.Log("Start Move State!");

        isMoveState = true;

        List<double> states = new List<double>();
        List<double> qs = new List<double>();

        double hungry = status.GetHungryRatio();
        double tireness = status.GetTirenessRatio();
        double emotion = status.GetEmotionRatio();
        double maxQ;

        states.Add(hungry);
        states.Add(tireness);
        states.Add(emotion);

        qs = SoftMax(ann.CalcOutput(states));
        maxQ = qs.Max();
        //ReceiveInput(hungry, tireness, emotion, out maxQ);
        int maxQIndex = qs.ToList().IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

        //if (Random.Range(0, 100) < exploreRate)
        //   maxQIndex = Random.Range(0, 2);

        // Choose action from max Q Index
        ChooseAction(maxQIndex);

        yield return new WaitForSeconds(3f);
        Debug.Log("Arrive");
        reward = GetReward();
        Debug.Log("Reward: " + reward);
        AddMemory(hungry, tireness, emotion, reward);
        TrainANN(maxQ);
        yield return null;
        Debug.Log("Change to Action state");
        isMoveState = false;
        reward = 0;
    }

    void Init ()
    {
        status = new Status();

        // Setting ANN
        ann = new ANN(3, 3, 1, 9, 0.6f);
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
        for (int i = replayMemory.Count - 1; i >= 0; i--)
        {
            List<double> toutputsOld = new List<double>();
            List<double> toutputsNew = new List<double>();
            toutputsOld = SoftMax(ann.CalcOutput(replayMemory[i].states));

            double maxQOld = toutputsOld.Max();
            int action = toutputsOld.ToList().IndexOf(maxQOld);

            double feedback;
            if (i == replayMemory.Count - 1 || replayMemory[i].reward == -1)
                feedback = replayMemory[i].reward;
            else
            {
                toutputsNew = SoftMax(ann.CalcOutput(replayMemory[i + 1].states));
                maxQ = toutputsNew.Max();
                feedback = (replayMemory[i].reward +
                    discount * maxQ);
            }

            toutputsOld[action] = feedback;
            ann.Train(replayMemory[i].states, toutputsOld);
        }
    }

    float GetReward ()
    {
        if (giveReward)
        {
            giveReward = false;

            if (!isPunished)
                return 1f;
            else
                return -1f;
        }

        else
            return 0f;
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
        int hungry = Random.Range(20, 40);
        // decrease tireness status
        int tireness = Random.Range(-15, -5);
        // Nothing in emotion statue
        int emotion = Random.Range(0, 20);

        status.SetStatus(hungry, tireness, emotion);
    }

    void MoveToRestTile ()
    {
        Debug.Log("Choose Rest Tile");

        // decrease hungry status
        int hungry = Random.Range(-5, -10);
        // increase tireness status
        int tireness = Random.Range(20, 40);
        // increase emotion status
        int emotion = Random.Range(10, 30);

        status.SetStatus(hungry, tireness, emotion);
    }

    void MoveToWorkTile ()
    {
        Debug.Log("Choose Work Tile");

        // decrease hungry status
        int hungry = Random.Range(-5, -10);
        // decrease tireness status
        int tireness = Random.Range(-30, -10);
        // decrease emotioness status
        int emotion = Random.Range(-20, -10);

        status.SetStatus(hungry, tireness, emotion);
    }
}
