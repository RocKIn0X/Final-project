using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BrainCollection
{
    public string nameState;

    // ANN
    [Header("[ANN parameters]")]
    public int numInputs;
    public int numOutputs;
    public List<int> numNEachLayer;
    public double alpha;

    // Reinforcement
    [Header("[Reinforcement parameters]")]
    public float discount;

    private MonsterBrain brain;

    public void InitEachBrain()
    {
        brain = new MonsterBrain(numInputs, numOutputs, numNEachLayer, alpha, discount);
    }

    public int CalculateAction(List<double> states)
    {
        return brain.CalculateAction(states);
    }

    public List<double> GetQS()
    {
        return brain.GetQS();
    }

    public void SetMemory(List<double> states, float reward)
    {
        brain.SetMemory(states, reward);
    }
}

public class ActionManager : MonoBehaviour
{
    public List<BrainCollection> brainCollections = new List<BrainCollection>();

    #region Reward
    public float idleReward;
    public float praiseReward;
    public float punishReward;
    #endregion

    //MonsterBrain actionBrain;

    #region Singleton Object
    public static ActionManager instance = null;

    void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        List<int> moveHiddenLayer = new List<int>();
        moveHiddenLayer.Add(4);
        moveHiddenLayer.Add(6);
        moveHiddenLayer.Add(4);
        //moveBrain = new MonsterBrain(3, 3, moveHiddenLayer, 0.2f);
        //moveBrain = new MonsterBrain();

        /*
        List<int> actionHiddenLayer = new List<int>();
        actionHiddenLayer.Add(3);
        actionBrain = new MonsterBrain(2, 3, actionHiddenLayer, 0.2f);
        */

        foreach (var b in brainCollections)
        {
            b.InitEachBrain();
        }
    }

    public int CalculateAction (List<double> states)
    {
        return brainCollections[0].CalculateAction(states);
    }

    public List<double> GetQS ()
    {
        return brainCollections[0].GetQS();

        //return actionBrain.GetQS();
    }

    public void SetMemory (List<double> states, float reward)
    {
        brainCollections[0].SetMemory(states, reward);
        //actionBrain.SetMemory(states, reward);
    }
}
