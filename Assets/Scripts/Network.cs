using UnityEngine;

public class Network
{
    public float maxEvolveValue;
    public int rowCount;
    public float[][] neuronValues;
    public float[][][] weights;

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
        if (Random.Range(1, 101) <= chanceToMutate)
        {
            int currentRow = Random.Range(0, rowCount - 1);
            int sourceNeuron = Random.Range(0, weights[currentRow].Length);
            int targetNeuron = Random.Range(0, weights[currentRow][sourceNeuron].Length);
            weights[currentRow][sourceNeuron][targetNeuron] += Random.Range(-maxEvolveValue, maxEvolveValue);
            Debug.Log("Evolved");

            Evolve(chanceToMutate);
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
}

