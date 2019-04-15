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
        foreach (var b in brainCollections)
        {
            b.InitEachBrain();
        }
    }

    public int CalculateAction (int actionIndex, List<double> states)
    {
        return brainCollections[actionIndex].CalculateAction(states);
    }

    public List<double> GetQS (int actionIndex)
    {
        return brainCollections[actionIndex].GetQS();

        //return actionBrain.GetQS();
    }

    public void SetMemory (int actionIndex, List<double> states, float reward)
    {
        brainCollections[actionIndex].SetMemory(states, reward);
        //actionBrain.SetMemory(states, reward);
    }
}
