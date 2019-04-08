using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayMemory
{
    public List<double> states;
    public double reward;

    public ReplayMemory(List<double> states, double r)
    {
        this.states = states;
        reward = r;
    }
}
