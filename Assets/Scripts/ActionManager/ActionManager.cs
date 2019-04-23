﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        Debug.Log(brain);
    }

    public int CalculateAction(List<double> states)
    {
        Debug.Log(brain);

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
    public bool isInitBrain = false;
    public bool isTrainable = true;
    public List<BrainCollection> brainCollections = new List<BrainCollection>();

    #region Reward
    [Header("Reward")]
    public float idleReward;
    public float praiseReward;
    public float punishReward;
    private float reward;
    #endregion

    #region WaitReward
    private bool isWaitReward = false;
    private float waitReward = 0;
    private TypeTile waitTile;
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
    private CanvasGroup trainingPopupCanvas;

    [Header("Popup prefab")]
    public GameObject trainingPopup;

    // Start is called before the first frame update
    void Start()
    {
        /*
        foreach (var b in brainCollections)
        {
            b.InitEachBrain();
        }
        */
        StartCoroutine(InitBrain());

        reward = idleReward;
        trainingPopupCanvas = trainingPopup.GetComponent<CanvasGroup>();
    }

    IEnumerator InitEachBrain (BrainCollection b)
    {
        b.InitEachBrain();
        yield return null;
    }

    IEnumerator InitBrain ()
    {
        foreach (BrainCollection b in brainCollections)
        {
            yield return StartCoroutine(InitEachBrain(b));
        }

        ActionManager.instance.isInitBrain = true;
        yield return null;
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
        waitReward = 0;
        if (index == 0 && waitReward != 0)
        {
            if (waitTile != TypeTile.WorkTile && waitTile == TileManager.Instance.tileTarget.typeTile)
            {
                Debug.Log("Apply waitReward");
                reward = waitReward;
            }
        }
        isWaitReward = false;
    }

    public void WaitAndPraise()
    {
        waitTile = TileManager.Instance.tileTarget.typeTile;
        if (waitReward >= praiseReward)
            waitReward = waitReward + praiseReward;
        else
            waitReward = waitReward + praiseReward;
        SetTrainingPopup(false);
    }

    public void WaitAndPunish()
    {
        waitTile = TileManager.Instance.tileTarget.typeTile;
        if (waitReward <= punishReward)
            waitReward = waitReward + punishReward;
        else
            waitReward = waitReward + punishReward;
        SetTrainingPopup(false);
    }

    public void praise ()
    {
        if (isWaitReward == true)
        {
            WaitAndPraise();
            return;
        }
        if (reward >= praiseReward)
            reward = reward + praiseReward;
        else
            reward = praiseReward;
        SetTrainingPopup(false);
    }

    public void punish ()
    {
        if (isWaitReward == true)
        {
            WaitAndPunish();
            return;
        }
        if (reward <= punishReward)
            reward = reward + punishReward;
        else
            reward = punishReward;
        SetTrainingPopup(false);
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

    public void CallTrainningPopup()
    {
        if (actionIndex == 0)
        {
            // status
            trainingPopup.GetComponent<TrainningPopup>().ActivatePopup(actionIndex, states, GetQS());
            isTrainable = true;
        }
        else if (actionIndex == 1 && TileManager.Instance.tileTarget.typeTile == TypeTile.WorkTile)
        {
            Debug.Log("Action index: " + ", type tile: " + TypeTile.WorkTile);

            // crop info
            List<double> cropInfo = new List<double>();
            cropInfo.Add(0);
            cropInfo.Add(0);
            //trainingPopup.GetComponent<TrainningPopup>().ActivatePopup(actionIndex, cropInfo, GetQS());
            trainingPopup.GetComponent<TrainningPopup>().ActivatePopup(actionIndex, states, GetQS());
            isTrainable = true;
        }
        else
        {
            isWaitReward = true;
            //trainingPopup.GetComponent<TrainningPopup>().ActivateNoTrainPopup();
            //isTrainable = false;
        }

        SetTrainingPopup(true);
    }

    public void SetTrainingPopup(bool isOn)
    {
        if (isTrainable == false)
        {
            // TODO Play Untrainable SFX
            return ;
        }

        trainingPopupCanvas.alpha = isOn ? 1 : 0;
        trainingPopupCanvas.blocksRaycasts = isOn;
        trainingPopupCanvas.interactable = isOn;

        if (isOn)
        {
            GameManager.Instance.PauseGame();
            trainingPopupCanvas.GetComponent<Animator>().SetTrigger("Active");
        }
        else
            GameManager.Instance.ResumeGame();
    }
}
