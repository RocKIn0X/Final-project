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
    ANN ann;

    bool hitWall = false;                           //check hit top or bottom wall

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

    public GameObject topWall;
    public GameObject bottomWall;

    void Start()
    {
        // Setting ANN
    }

    GUIStyle guiStyle = new GUIStyle();
    void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 600, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 500, 30), "Fails: " + failCount, guiStyle);
        GUI.Label(new Rect(10, 50, 500, 30), "Decay Rate: " + exploreRate, guiStyle);
        GUI.Label(new Rect(10, 75, 500, 30), "Last Best Balance: " + maxBalanceTime, guiStyle);
        GUI.Label(new Rect(10, 100, 500, 30), "This Balance: " + timer, guiStyle);
        GUI.EndGroup();
    }

    // Update is called once per frame
    void Update()
    {
        // Get input for giving reward

        // Get input for punishing it
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;
        List<double> states = new List<double>();
        List<double> qs = new List<double>();

        // Adding parameter into state

        /*
        double td = Vector3.Distance(transform.position, topWall.transform.position);
        double bd = Vector3.Distance(transform.position, bottomWall.transform.position);

        states.Add(td);
        states.Add(bd);
        */

        qs = SoftMax(ann.CalcOutput(states));
        double maxQ = qs.Max();
        int maxQIndex = qs.ToList().IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

        //if (Random.Range(0, 100) < exploreRate)
         //   maxQIndex = Random.Range(0, 2);

        // Choose action from max Q Index
        /*
        if (maxQIndex == 0)
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * moveForce * (float)qs[maxQIndex]);
        else if (maxQIndex == 1)
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * -moveForce * (float)qs[maxQIndex]); // do nothing
        */

        // Give reward
        /*
        if (hitWall)
            reward = -1.0f;
        else
            reward = 0.1f;
        */

        // Set last memory
        /*
        Replay lastMemory = new Replay(td, bd, reward);
        */

        if (replayMemory.Count > mCapacity)
            replayMemory.RemoveAt(0);

        // replayMemory.Add(lastMemory);

        if (hitWall)
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
            
            /*
            replayMemory.Clear();
            failCount++;
            */
        }
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
            hitWall = true;
        }
    }

    void MoveToFoodTile ()
    {
        // decrease tireness status
        // increase hungry status
    }

    void MoveToRestTile ()
    {
        // decrease tireness status
        // increase tireness status
        // increase emotion status
    }

    void MoveToWorkTile ()
    {
        // decrease emotioness status
        // decrease hungry status
    }
}
