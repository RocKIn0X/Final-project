using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveTestPanel : MonoBehaviour
{
    public int actionIndex = 0;

    public InputField hungerField;
    public InputField tirenessField;
    public InputField emotionField;
    public GameObject actionBars;
    public GameObject rewardUI;
    public TextMeshProUGUI actionText;

    private bool isAutoTest = false;
    private double hungerRatio;
    private double tirenessRatio;
    private double emotionRatio;
    private float reward;

    // Start is called before the first frame update
    void Start()
    {
        reward = 0f;

        rewardUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveIndex ()
    {
        actionIndex = 0;
        emotionField.gameObject.SetActive(true);
    }

    public void ActionIndex ()
    {
        actionIndex = 1;
        emotionField.gameObject.SetActive(false);
    }

    public void RunAutoTest ()
    {
        if (!isAutoTest)
        {
            StartCoroutine(AutoTest());
            isAutoTest = true;
        }
    }

    public IEnumerator AutoTest ()
    {
        List<double> states;
        SetStates(out states);

        CalculateAction(states);
        // activate reward
        rewardUI.SetActive(true);
        yield return new WaitForSeconds(3f);

        // save states and reward to memory
        SaveStatesToMemory(states);
        // set value in reward slider to zero
        rewardUI.GetComponent<RewardTestUI>().ResetRewardSlider();
        // deactivate reward
        rewardUI.SetActive(false);
        // set reward to ANN
        Debug.Log("Reward: " + reward);
        // end auto test
        isAutoTest = false;
    }

    public void SetStates (out List<double> states)
    {
        states = new List<double>();

        hungerRatio = (double.Parse(hungerField.text.ToString()) / 100) - 0.5f;
        tirenessRatio = (double.Parse(tirenessField.text.ToString()) / 100) - 0.5f;
        emotionRatio = actionIndex == 0 ? (double.Parse(emotionField.text.ToString()) / 100) - 0.5f : 0f;

        if (actionIndex == 0)
        {
            states.Add(hungerRatio);
            states.Add(tirenessRatio);
            states.Add(emotionRatio);
        }
        else if (actionIndex == 1)
        {
            states.Add(hungerRatio);
            states.Add(tirenessRatio);
        }
    }

    public void CalculateAction (List<double> states)
    {
        int maxQIndex = ActionManager.instance.CalculateAction(actionIndex, states);

        List<double> qs = ActionManager.instance.GetQS();
        if (actionIndex == 0)
            actionBars.GetComponent<ActionTestBar>().SetActionBar((float)qs[0], (float)qs[1], (float)qs[2]);
        else
            actionBars.GetComponent<ActionTestBar>().SetActionBar((float)qs[0], (float)qs[1], (float)qs[2]);

        if (actionIndex == 0)
        {
            if (maxQIndex == 0) actionText.text = "Working";
            else if (maxQIndex == 1) actionText.text = "Eatting";
            else if (maxQIndex == 2) actionText.text = "Sleeping";
        }
        else if (actionIndex == 1)
        {
            if (maxQIndex == 0) actionText.text = "Idle";
            else if (maxQIndex == 1) actionText.text = "Harvest";
            else if (maxQIndex == 2) actionText.text = "Water";
        }
    }

    void SaveStatesToMemory (List<double> states)
    {
        reward = rewardUI.GetComponent<RewardTestUI>().GetRewardValue();
        // save the current state and reward to replay memory
        ActionManager.instance.SetMemory(reward);
    }
}
