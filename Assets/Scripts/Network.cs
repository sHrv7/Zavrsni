using Newtonsoft.Json;
using System.IO;
using UnityEngine;


public class Network
{
    public float maxEvolveValue;
    public int rowCount;
    public float[][] neuronValues;
    public float[][][] weights;

    private const string saveFileName = "SavedNetworks/network_weights.json";



    // Copy constructor
    public Network(Network other)
    {
        maxEvolveValue = other.maxEvolveValue;
        rowCount = other.rowCount;

        // Initialize neurons
        neuronValues = new float[rowCount][];
        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            neuronValues[rowIndex] = new float[other.neuronValues[rowIndex].Length];
        }

        // Clone weights
        weights = new float[rowCount - 1][][];
        for (int currentRow = 0; currentRow < rowCount - 1; currentRow++)
        {
            weights[currentRow] = new float[other.weights[currentRow].Length][];
            for (int sourceNeuron = 0; sourceNeuron < other.weights[currentRow].Length; sourceNeuron++)
            {
                weights[currentRow][sourceNeuron] = (float[])other.weights[currentRow][sourceNeuron].Clone();
            }
        }
    }

    // Standard constructor
    public Network(int[] neuronsPerRow, float maxEvolveValue)
    {
        this.maxEvolveValue = maxEvolveValue;
        rowCount = neuronsPerRow.Length;

        // Initialize neurons
        neuronValues = new float[rowCount][];
        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            neuronValues[rowIndex] = new float[neuronsPerRow[rowIndex]];
        }

        // Initialize weights
        weights = new float[rowCount - 1][][];
        for (int currentRow = 0; currentRow < rowCount - 1; currentRow++)
        {
            weights[currentRow] = new float[neuronsPerRow[currentRow]][];
            for (int sourceNeuron = 0; sourceNeuron < neuronsPerRow[currentRow]; sourceNeuron++)
            {
                weights[currentRow][sourceNeuron] = new float[neuronsPerRow[currentRow + 1]];
                for (int targetNeuron = 0; targetNeuron < neuronsPerRow[currentRow + 1]; targetNeuron++)
                {
                    weights[currentRow][sourceNeuron][targetNeuron] = Random.Range(-maxEvolveValue, maxEvolveValue);
                }
            }
        }
    }

    public void Step()
    {
        for (int currentRow = 1; currentRow < rowCount; currentRow++)
        {
            for (int targetNeuron = 0; targetNeuron < neuronValues[currentRow].Length; targetNeuron++)
            {
                neuronValues[currentRow][targetNeuron] = 0; // Reset the value
                for (int sourceNeuron = 0; sourceNeuron < neuronValues[currentRow - 1].Length; sourceNeuron++)
                {
                    neuronValues[currentRow][targetNeuron] += neuronValues[currentRow - 1][sourceNeuron] * weights[currentRow - 1][sourceNeuron][targetNeuron];
                }

                // Cap the neuron value between -1 and 1
                if (neuronValues[currentRow][targetNeuron] > 1f)
                {
                    neuronValues[currentRow][targetNeuron] = 1f;
                }
                else if (neuronValues[currentRow][targetNeuron] < -1f)
                {
                    neuronValues[currentRow][targetNeuron] = -1f;
                }
            }
        }
    }

    public void Evolve(float chanceToMutate)
    {
        for (int currentRow = 0; currentRow < weights.Length; currentRow++)
        {
            for (int sourceNeuron = 0; sourceNeuron < weights[currentRow].Length; sourceNeuron++)
            {
                for (int targetNeuron = 0; targetNeuron < weights[currentRow][sourceNeuron].Length; targetNeuron++)
                {
                    if (Random.Range(0f, 100f) < chanceToMutate)
                    {
                        weights[currentRow][sourceNeuron][targetNeuron] += Random.Range(-maxEvolveValue, maxEvolveValue) / 4;
                        //Debug.Log("Evolved");
                    }
                }
            }
        }
    }


    public void SetInput(int index, float value)
    {
        neuronValues[0][index] = value;
    }
    public float GetOutput(int index)
    {
        return neuronValues[rowCount - 1][index];
    }

    public void SaveWeights()
    {
        WeightsData data = new WeightsData { weights = this.weights };

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        string path = Path.Combine(Application.dataPath, saveFileName);
        File.WriteAllText(path, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh(); // Refresh editor
#endif

        Debug.Log("Weights saved to " + path);
    }

    public void LoadWeights()
    {
        string path = Path.Combine(Application.dataPath, saveFileName);
        if (!File.Exists(path))
        {
            Debug.LogError("Save file not found at " + path);
            return;
        }

        string json = File.ReadAllText(path);
        WeightsData data = JsonConvert.DeserializeObject<WeightsData>(json);

        if (data != null && data.weights != null)
        {
            this.weights = data.weights;
        }
        else
        {
            Debug.LogError("Failed to deserialize weights.");
        }
    }

}


[System.Serializable]
public class WeightsData
{
    public float[][][] weights;
}
