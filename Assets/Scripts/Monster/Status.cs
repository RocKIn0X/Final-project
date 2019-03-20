using UnityEngine;

[System.Serializable]
public class Status
{
    private int maxHungry;
    private int maxTireness;
    private int maxEmotion;

    [SerializeField]
    private int hungry;
    [SerializeField]
    private int tireness;
    [SerializeField]
    private int emotion;

    public Status()
    {
        maxHungry = 100;
        maxTireness = 100;
        maxEmotion = 100;

        hungry = maxHungry;
        tireness = maxTireness;
        emotion = maxEmotion;
    }

    public void SetStatus(int hg, int tn, int em)
    {
        hungry += hg;
        tireness += tn;
        emotion += em;

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
