using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    [SerializeField]
    MonsterBrain moveBrain;

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
        List<int> moveHiddenLayer = new List<int>();
        moveHiddenLayer.Add(4);
        moveHiddenLayer.Add(6);
        moveHiddenLayer.Add(4);
        moveBrain = new MonsterBrain(3, 3, moveHiddenLayer, 0.2f);

        /*
        List<int> actionHiddenLayer = new List<int>();
        actionHiddenLayer.Add(3);
        actionBrain = new MonsterBrain(2, 3, actionHiddenLayer, 0.2f);
        */
    }

    public int CalculateAction (List<double> states)
    {
        return moveBrain.CalculateAction(states);
    }

    public List<double> GetQS ()
    {
        return moveBrain.GetQS();

        //return actionBrain.GetQS();
    }

    public void SetMemory (List<double> states, float reward)
    {
        moveBrain.SetMemory(states, reward);
        //actionBrain.SetMemory(states, reward);
    }
}
