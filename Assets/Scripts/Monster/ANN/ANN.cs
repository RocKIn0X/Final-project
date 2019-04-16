using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ANN{
	private int numInputs;
	private int numOutputs;
	private List<int> numNEachLayer = new List<int>();
    private double alpha;
	List<Layer> layers = new List<Layer>();

    public ANN ()
    {
        if (numNEachLayer.Count > 0)
        {
            for (int i = 0; i < numNEachLayer.Count + 1; i++)
            {
                if (i == 0)
                {
                    layers.Add(new Layer(numNEachLayer[i], numInputs));
                    continue;
                }

                if (i == numNEachLayer.Count)
                {
                    layers.Add(new Layer(numOutputs, numNEachLayer[i - 1]));
                    break;
                }

                layers.Add(new Layer(numNEachLayer[i], numNEachLayer[i - 1]));

            }
        }
        else
        {
            layers.Add(new Layer(numOutputs, numInputs));
        }
    }

	public ANN(int nI, int nO, List<int> hL, double a)
	{
		numInputs = nI;
		numOutputs = nO;
		numNEachLayer = hL;
		alpha = a;

        if (numNEachLayer.Count > 0)
        {
            for (int i = 0; i < numNEachLayer.Count + 1; i++)
            {
                if (i == 0)
                {
                    layers.Add(new Layer(numNEachLayer[i], numInputs));
                    continue;
                }

                if (i == numNEachLayer.Count)
                {
                    layers.Add(new Layer(numOutputs, numNEachLayer[i - 1]));
                    break;
                }

                layers.Add(new Layer(numNEachLayer[i], numNEachLayer[i - 1]));

            }
        }
        else
        {
            layers.Add(new Layer(numOutputs, numInputs));
        }
	}

    public List<double> Train(List<double> inputValues, List<double> desiredOutput)
	{
		List<double> outputValues = new List<double>();
		outputValues = CalcOutput(inputValues, desiredOutput);
		UpdateWeights(outputValues, desiredOutput);
		return outputValues;
	}

	public List<double> CalcOutput(List<double> inputValues, List<double> desiredOutput)
	{
		List<double> inputs = new List<double>();
		List<double> outputValues = new List<double>();
		int currentInput = 0;

		if(inputValues.Count != numInputs)
		{
			Debug.Log("ERROR: Number of Inputs must be " + numInputs);
			return outputValues;
		}

		inputs = new List<double>(inputValues);
		for(int i = 0; i < numNEachLayer.Count + 1; i++)
		{
				if(i > 0)
				{
					inputs = new List<double>(outputValues);
				}
				outputValues.Clear();

				for(int j = 0; j < layers[i].numNeurons; j++)
				{
					double N = 0;
					layers[i].neurons[j].inputs.Clear();

					for(int k = 0; k < layers[i].neurons[j].numInputs; k++)
					{
					    layers[i].neurons[j].inputs.Add(inputs[currentInput]);
						N += layers[i].neurons[j].weights[k] * inputs[currentInput];
						currentInput++;
					}

					N -= layers[i].neurons[j].bias;

					if(i == numNEachLayer.Count)
						layers[i].neurons[j].output = ActivationFunctionO(N);
					else
						layers[i].neurons[j].output = ActivationFunction(N);
					
					outputValues.Add(layers[i].neurons[j].output);
					currentInput = 0;
				}
		}
		return outputValues;
	}

	public List<double> CalcOutput(List<double> inputValues)
	{
		List<double> inputs = new List<double>();
		List<double> outputValues = new List<double>();
		int currentInput = 0;

		if(inputValues.Count != numInputs)
		{
			Debug.Log("ERROR: Number of Inputs must be " + numInputs);
			return outputValues;
		}

		inputs = new List<double>(inputValues);
		for(int i = 0; i < numNEachLayer.Count + 1; i++)
		{
				if(i > 0)
				{
					inputs = new List<double>(outputValues);
				}
				outputValues.Clear();

				for(int j = 0; j < layers[i].numNeurons; j++)
				{
					double N = 0;
					layers[i].neurons[j].inputs.Clear();

					for(int k = 0; k < layers[i].neurons[j].numInputs; k++)
					{
					    layers[i].neurons[j].inputs.Add(inputs[currentInput]);
						N += layers[i].neurons[j].weights[k] * inputs[currentInput];
						currentInput++;
					}

					N -= layers[i].neurons[j].bias;

					if(i == numNEachLayer.Count)
						layers[i].neurons[j].output = ActivationFunctionO(N);
					else
						layers[i].neurons[j].output = ActivationFunction(N);
					
					outputValues.Add(layers[i].neurons[j].output);
					currentInput = 0;
				}
		}

		return outputValues;
	}


	public string PrintWeights()
	{
		string weightStr = "";
		foreach(Layer l in layers)
		{
			foreach(Neuron n in l.neurons)
			{
				foreach(double w in n.weights)
				{
					weightStr += w + ",";
				}
				weightStr += n.bias + ",";
			}
		}
		return weightStr;
	}

    public void SaveWeights()
    {
        Debug.Log("Save...");
        string path = Application.dataPath + "/weights.txt";
        var sr = File.CreateText(path);
        string weightStr = "";
        foreach (Layer l in layers)
        {
            foreach (Neuron n in l.neurons)
            {
                foreach (double w in n.weights)
                {
                    weightStr += w + ",";
                }
                weightStr += n.bias + ",";
            }
        }
        sr.WriteLine(weightStr);
        sr.Close();
        Debug.Log("Save completed at " + path);
    }

    public void LoadWeights()
	{

        Debug.Log("Load...");
        string path = Application.dataPath + "/weights.txt";
        string weightStr = "";
        if (File.Exists(path))
        {
            var sr = File.OpenText(path);
            weightStr = sr.ReadLine();
        }
        else
            return;

        if (weightStr == "") return;
		string[] weightValues = weightStr.Split(',');
		int w = 0;
		foreach(Layer l in layers)
		{
			foreach(Neuron n in l.neurons)
			{
				for(int i = 0; i < n.weights.Count; i++)
				{
					n.weights[i] = System.Convert.ToDouble(weightValues[w]);
					w++;
				}
				n.bias = System.Convert.ToDouble(weightValues[w]);
				w++;
			}
		}
	}
	
	void UpdateWeights(List<double> outputs, List<double> desiredOutput)
	{
		double error;
		for(int i = numNEachLayer.Count; i >= 0; i--)
		{
			for(int j = 0; j < layers[i].numNeurons; j++)
			{
				if(i == numNEachLayer.Count)
				{
					error = desiredOutput[j] - outputs[j];
					layers[i].neurons[j].errorGradient = outputs[j] * (1-outputs[j]) * error;
				}
				else
				{
					layers[i].neurons[j].errorGradient = layers[i].neurons[j].output * (1-layers[i].neurons[j].output);
					double errorGradSum = 0;
					for(int p = 0; p < layers[i+1].numNeurons; p++)
					{
						errorGradSum += layers[i+1].neurons[p].errorGradient * layers[i+1].neurons[p].weights[j];
					}
					layers[i].neurons[j].errorGradient *= errorGradSum;
				}	
				for(int k = 0; k < layers[i].neurons[j].numInputs; k++)
				{
					if(i == numNEachLayer.Count)
					{
						error = desiredOutput[j] - outputs[j];
						layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * error;
					}
					else
					{
						layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * layers[i].neurons[j].errorGradient;
					}
				}
				layers[i].neurons[j].bias += alpha * -1 * layers[i].neurons[j].errorGradient;
			}

		}

	}


	double ActivationFunction(double value)
	{
		return TanH(value);
	}

	double ActivationFunctionO(double value)
	{
		return Sigmoid(value);
	}

	double TanH(double value)
	{
		double k = (double) System.Math.Exp(-2*value);
    	return 2 / (1.0f + k) - 1;
	}

	double ReLu(double value)
	{
		if(value > 0) return value;
		else return 0;
	}

	double Linear(double value)
	{
		return value;
	}

	double LeakyReLu(double value)
	{
		if(value < 0) return 0.01*value;
   		else return value;
	}

	double Sigmoid(double value) 
	{
    	double k = (double) System.Math.Exp(value);
    	return k / (1.0f + k);
	}
}
