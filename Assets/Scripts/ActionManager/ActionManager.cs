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
    [System.Serializable]
    public struct InputDictionary
    {
        public string inputKey;
        public double inputValue;
    };

    [System.Serializable]
    public struct OutputDictionary
    {
        public string outputKey;
        public double outputValue;
    };

    public List<BrainCollection> brainCollections = new List<BrainCollection>();

    public List<InputDictionary> inputBook = new List<InputDictionary>();
    public List<OutputDictionary> outputBook = new List<OutputDictionary>();

    #region Reward
    public float idleReward;
    public float praiseReward;
    public float punishReward;
    #endregion

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

    private int actionIndex;
    private List<double> states = new List<double>();
    private static Dictionary<string, double> InputDict = new Dictionary<string, double>();
    private static Dictionary<string, double> OutputDict = new Dictionary<string, double>();

    private void PopulateDict()
    {
        foreach (InputDictionary value in inputBook)
        {
            InputDict.Add(value.inputKey, value.inputValue);
        }

        foreach (OutputDictionary value in outputBook)
        {
            OutputDict.Add(value.outputKey, value.outputValue);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var b in brainCollections)
        {
            b.InitEachBrain();
        }

        PopulateDict();
    }

    private void SetInputDict ()
    {
        if (actionIndex == 0)
        {
            InputDict["Hunger"] = states[0];
            InputDict["Tireness"] = states[1];
            InputDict["Emotion"] = states[2];
        }
        else if (actionIndex == 1)
        {
            InputDict["GrowthRate"] = states[0];
            InputDict["WaterGauge"] = states[1];
        }
    }

    private void SetOutputDict ()
    {
        List<double> qs = GetQS(actionIndex);

        if (actionIndex == 0)
        {
            OutputDict["WorkTile"] = qs[0];
            OutputDict["FoodTile"] = qs[1];
            OutputDict["RestTile"] = qs[2];
        }
        else if (actionIndex == 1)
        {
            OutputDict["Idle"] = qs[0];
            OutputDict["Harvest"] = qs[1];
            OutputDict["Water"] = qs[2];
        }
    }

    public int CalculateAction (int actionIndex, List<double> states)
    {
        this.actionIndex = actionIndex;
        this.states = states;

        return brainCollections[actionIndex].CalculateAction(states);
    }

    public List<double> GetQS (int actionIndex)
    {
        return brainCollections[actionIndex].GetQS();

        //return actionBrain.GetQS();
    }

    public void SetMemory (float reward)
    {
        brainCollections[actionIndex].SetMemory(states, reward);
        //actionBrain.SetMemory(states, reward);
    }

    public Dictionary<string, double> GetInputDict ()
    {
        SetInputDict();

        return InputDict;
    }

    public Dictionary<string, double> GetOutputDict()
    {
        SetOutputDict();

        return OutputDict;
    }
}
