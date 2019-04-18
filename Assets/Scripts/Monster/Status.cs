using UnityEngine;

[System.Serializable]
public class Status
{
    private float maxHungry;
    private float maxTireness;
    private float maxEmotion;

    public float hunger;
    public float tireness;
    public float emotion;

    public Status()
    {
        Reset();
    }

    private float CalculateEmotion ()
    {
        if (hunger > 50f && tireness > 50f)
        {
            return 1f;
        }
        else
        {
            return -1f;
        }
    }

    public void Reset ()
    {
        maxHungry = 100;
        maxTireness = 100;
        maxEmotion = 100;

        hunger = maxHungry;
        tireness = maxTireness;
        emotion = maxEmotion;
    }

    public void RandomStatus ()
    {
        hunger = Random.Range(0, 100);
        tireness = Random.Range(0, 100);
        emotion = Random.Range(0, 100);
    }

    public void SetStatus(float hg, float tn, float em)
    {
        hunger = hg;
        tireness = tn;
        emotion = em;
    }

    public double GetHungryRatio()
    {
        return (double)hunger / maxHungry;
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
