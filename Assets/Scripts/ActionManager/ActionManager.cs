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
    private float reward;
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

    public GameObject trainingPopup;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var b in brainCollections)
        {
            b.InitEachBrain();
        }

        reward = idleReward;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CallTrainningPopup();
        }
    }

    public int CalculateAction (int actionIndex, List<double> states)
    {
        this.actionIndex = actionIndex;
        this.states = states;

        // initial reward this state equal idle reward
        reward = idleReward;

        return brainCollections[actionIndex].CalculateAction(states);
    }

    public List<double> GetStates()
    {
        return states;
    }

    public List<double> GetQS ()
    {
        return brainCollections[actionIndex].GetQS();

        //return actionBrain.GetQS();
    }

    public void SetActionIndex (int index)
    {
        actionIndex = index;
    }

    public void praise ()
    {
        reward = praiseReward;
        trainingPopup.SetActive(false);
    }

    public void punish ()
    {
        reward = punishReward;
        trainingPopup.SetActive(false);
    }

    public void SetMemory ()
    {
        Debug.Log("Action index: " + actionIndex);
        brainCollections[actionIndex].SetMemory(states, reward);
    }

    public void SetMemory(float reward)
    {
        brainCollections[actionIndex].SetMemory(states, reward);
    }

    public void CallTrainningPopup ()
    {
        GameManager.Instance.PauseGame();
        trainingPopup.SetActive(true);
        if (actionIndex == 0)
        {
            // status
            trainingPopup.GetComponent<TrainningPopup>().ActivatePopup(actionIndex, states, GetQS());
        }
        else
        {
            // crop info
            List<double> cropInfo = new List<double>();
            cropInfo.Add(0);
            cropInfo.Add(0);
            trainingPopup.GetComponent<TrainningPopup>().ActivatePopup(actionIndex, cropInfo, GetQS());
        }
    }
}
