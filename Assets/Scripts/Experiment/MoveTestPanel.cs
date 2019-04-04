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
    public TextMeshProUGUI actionText;

    private Brain brain;

    // Start is called before the first frame update
    void Start()
    {
        brain = GetComponent<Brain>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStates (out List<double> states)
    {
        states = new List<double>();

        double hungerRatio = double.Parse(hungerField.text.ToString()) / 100;
        double tirenessRatio = double.Parse(tirenessField.text.ToString()) / 100;
        double emotionRatio = double.Parse(emotionField.text.ToString()) / 100;

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
}
