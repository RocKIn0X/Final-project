using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class MonsterBrain
{
    public ANN ann;

    float discount = 0.99f;                         //how much future states affect rewards
    float exploreRate = 100.0f;                     //chance of picking random action
    float maxExploreRate = 100.0f;					//max chance value
    float minExploreRate = 0.01f;					//min chance value
    float exploreDecay = 0.01f;                   //chance decay amount for each update

    float reward = 0.0f;                            //reward to associate with actions
    List<ReplayMemory> replayMemory = new List<ReplayMemory>(); //memory - list of past actions and rewards
    int mCapacity = 10000;                          //memory capacity

    int punishedCount = 0;                              //count player punished

    float timer = 0;                                //timer to keep track of balancing
    float maxBalanceTime = 0;                       //record time ball is kept balanced	

    List<double> states = new List<double>();
    List<double> qs = new List<double>();

    public MonsterBrain()
    {
        // Setting ANN
        ann = new ANN();
    }

    public MonsterBrain(int input, int output, List<int> hiddenLayer, double alpha, float discount)
    {
        this.discount = discount;

        // Setting ANN
        ann = new ANN(input, output, hiddenLayer, alpha);
    }

    public void InitBrain ()
    {
        ann = new ANN();
    }

    public int CalculateAction (List<double> states)
    {
        qs = SoftMax(ann.CalcOutput(states));
        double maxQ = qs.Max();
        int maxQIndex = qs.ToList().IndexOf(maxQ);

        /*
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

        if (Random.Range(0, 100) < exploreRate)
        {
            maxQIndex = Random.Range(0, output);
        }
        */

        return maxQIndex;
    }
    
    public void SetMemory (List<double> states, float reward)
    {
        AddMemory(states, reward);
        if (reward < 0)
        {
            TrainANN();
        }
    }

    public List<double> GetQS ()
    {
        return qs;
    }

    private void AddMemory (List<double> states, float reward)
    {
        ReplayMemory lastMemory = new ReplayMemory(states, reward);

        if (replayMemory.Count > mCapacity)
            replayMemory.RemoveAt(0);

        replayMemory.Add(lastMemory);
    }

    private void TrainANN()
    {
        for (int i = replayMemory.Count - 1; i >= 0; i--)
        {
            List<double> toutputsOld = new List<double>();
            List<double> toutputsNew = new List<double>();
            toutputsOld = SoftMax(ann.CalcOutput(replayMemory[i].states));

            double maxQOld = toutputsOld.Max();
            int action = toutputsOld.ToList().IndexOf(maxQOld);

            double feedback;
            if (i == replayMemory.Count - 1)
                feedback = replayMemory[i].reward;
            else
            {
                toutputsNew = SoftMax(ann.CalcOutput(replayMemory[i + 1].states));
                double maxQ = toutputsNew.Max();
                feedback = (replayMemory[i].reward +
                    discount * maxQ);
            }

            toutputsOld[action] = feedback;
            ann.Train(replayMemory[i].states, toutputsOld);
        }
        replayMemory.Clear();
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
}
