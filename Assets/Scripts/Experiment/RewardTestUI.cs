using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardTestUI : MonoBehaviour
{
    public Slider rewardSlider;
    public TextMeshProUGUI rewardText;
   
    public float GetRewardValue ()
    {
        return rewardSlider.value;
    }

    public void ResetRewardSlider ()
    {
        rewardSlider.value = 0;
    }

    public void AdjustReward()
    {
        rewardText.text = rewardSlider.value.ToString("F2");
    }
}
