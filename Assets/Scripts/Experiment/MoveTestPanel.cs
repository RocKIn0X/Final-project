using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveTestPanel : MonoBehaviour
{
    public InputField hungerField;
    public InputField tirenessField;
    public InputField emotionField;
    public GameObject rewardUI;
    public TextMeshProUGUI actionText;

    private Brain brain;

    private bool isAutoTest = false;
    private double hungerRatio;
    private double tirenessRatio;
    private double emotionRatio;
    private float reward;

    // Start is called before the first frame update
    void Start()
    {
        reward = 0f;

        brain = GetComponent<Brain>();

        rewardUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
        CalculateAction();
        // activate reward
        rewardUI.SetActive(true);
        yield return new WaitForSeconds(3f);

        // save states and reward to memory
        SaveStatesAndReward();
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
        emotionRatio = (double.Parse(emotionField.text.ToString()) / 100) - 0.5f;

        states.Add(hungerRatio);
        states.Add(tirenessRatio);
        states.Add(emotionRatio);
    }

    public void CalculateAction ()
    {
        List<double> states;

        SetStates(out states);

        int maxQIndex = brain.GetMaxQIndex(states);

        if (maxQIndex == 0) actionText.text = "Working";
        else if (maxQIndex == 1) actionText.text = "Eatting";
        else if (maxQIndex == 2) actionText.text = "Sleeping";
    }

    void SaveStatesAndReward ()
    {
        reward = rewardUI.GetComponent<RewardTestUI>().GetRewardValue();
    }
}
