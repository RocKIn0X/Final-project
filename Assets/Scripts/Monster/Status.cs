using UnityEngine;

[System.Serializable]
public class Status
{
    private float maxHungry;
    private float maxTireness;
    private float maxEmotion;

    [SerializeField]
    private float hungry;
    [SerializeField]
    private float tireness;
    [SerializeField]
    private float emotion;

    public Status()
    {
        maxHungry = 100;
        maxTireness = 100;
        maxEmotion = 100;

        hungry = maxHungry;
        tireness = maxTireness;
        emotion = maxEmotion;
    }

    private float CalculateEmotion ()
    {
        if (hungry > 50f && tireness > 50f)
        {
            return 5f;
        }
        else
        {
            return -5f;
        }
    }

    public void SetStatus(float hg, float tn)
    {
        hungry += hg;
        tireness += tn;
        emotion += CalculateEmotion();

        hungry = Mathf.Clamp(hungry, 0, maxHungry);
        tireness = Mathf.Clamp(tireness, 0, maxTireness);
        emotion = Mathf.Clamp(emotion, 0, maxEmotion);
    }

    public double GetHungryRatio()
    {
        return (double)hungry / maxHungry;
    }

    public double GetEmotionRatio()
    {
        return (double)emotion / maxEmotion;
    }

    public double GetTirenessRatio()
    {
        return (double)tireness / maxTireness;
    }
}
