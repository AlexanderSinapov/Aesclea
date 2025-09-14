// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using static Aesclea_Back_End_.AIModel.Neuron;
using System;

namespace Aesclea_Back_End_.AIModel
{
    public class RecurrentNeuron : Neuron
    {
        public List<double> RecurrentWeights { get; set; }
        public double PreviousOutput { get; set; } = 0;
        private static Random random = new Random();
        public ActivationType _activationType; // Making this public so RecurrentLayer can access it

        public RecurrentNeuron(int inputSize, ActivationType activationType = ActivationType.LeakyReLU)
            : base(inputSize, activationType)
        {
            _activationType = activationType;

            RecurrentWeights = new List<double>(inputSize);
            for (int i = 0; i < inputSize; i++)
            {
                // Initialize with small values - using He initialization for recurrent weights
                double weightScale = (_activationType == ActivationType.Sigmoid)
                    ? Math.Sqrt(2.0 / inputSize)
                    : Math.Sqrt(2.0 / inputSize);

                RecurrentWeights.Add((random.NextDouble() * 2 - 1) * weightScale * 0.1); // Small recurrent weights
            }
        }

        public double FeedForwardRecurrent(List<double> inputs)
        {
            if (inputs.Count != Weights.Count)
            {
                throw new ArgumentException($"Number of inputs ({inputs.Count}) must match the number of weights ({Weights.Count}).");
            }

            double sum = 0;
            for (int i = 0; i < inputs.Count; i++)
            {
                sum += inputs[i] * Weights[i];
            }

            // Add recurrent connections - weighted by previous output
            for (int i = 0; i < RecurrentWeights.Count; i++)
            {
                sum += PreviousOutput * RecurrentWeights[i];
            }

            sum += Bias;
            Input = sum;

            if (_activationType == ActivationType.Sigmoid)
                Output = Sigmoid(sum);
            else if (_activationType == ActivationType.LeakyReLU)
                Output = LeakyReLU(sum);
            else
                Output = ReLU(sum);

            PreviousOutput = Output;
            return Output;
        }

        public void ResetState()
        {
            PreviousOutput = 0;
        }

        // Method to update recurrent weights specifically
        public void UpdateRecurrentWeights(double learningRate, double l2Lambda = 0.0001)
        {
            double derivative;
            if (_activationType == ActivationType.Sigmoid)
            {
                derivative = SigmoidDerivative(Output);
            }
            else if (_activationType == ActivationType.LeakyReLU)
            {
                derivative = LeakyReLUDerivative(Input);
            }
            else
            {
                derivative = ReLUDerivative(Input);
            }

            // Gradient clipping
            double clippedError = Math.Max(-1.0, Math.Min(1.0, Error));

            for (int i = 0; i < RecurrentWeights.Count; i++)
            {
                double delta = learningRate * clippedError * derivative * PreviousOutput;
                double regularizationTerm = -learningRate * l2Lambda * RecurrentWeights[i];

                if (!double.IsNaN(delta))
                {
                    RecurrentWeights[i] += delta + regularizationTerm;
                }
            }
        }
    }
}
