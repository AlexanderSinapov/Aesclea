using System;
using System.Collections.Generic;

namespace Aesclea_Back_End_.AIModel
{
    public class Neuron
    {
        public List<double> Weights { get; set; }
        public double Bias { get; set; }
        public double Output { get; set; }
        public double Input { get; set; }
        public double Error { get; set; }
        private static Random random = new Random();
        private readonly ActivationType _activationType;

        public enum ActivationType
        {
            ReLU,
            Sigmoid
        }

        // Activation functions
        public static double ReLU(double x) => Math.Max(0, x);
        public static double ReLUDerivative(double x) => x > 0 ? 1 : 0.01; // Small leak

        public static double Sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-x));
        public static double SigmoidDerivative(double x) => x * (1 - x); // Note: x should be sigmoid output

        public Neuron(int numberOfInputs, ActivationType activationType = ActivationType.ReLU)
        {
            _activationType = activationType;
            Weights = new List<double>(numberOfInputs);

            // Xavier/Glorot initialization for better convergence
            double weightScale = Math.Sqrt(2.0 / numberOfInputs);

            for (int i = 0; i < numberOfInputs; i++)
            {
                Weights.Add((random.NextDouble() * 2 - 1) * weightScale);
            }
            Bias = (random.NextDouble() * 2 - 1) * 0.1;
        }

        public double FeedForward(List<double> inputs)
        {
            if (inputs.Count != Weights.Count)
            {
                throw new ArgumentException($"Number of inputs ({inputs.Count}) must match the number of weights ({Weights.Count}).");
            }

            Input = 0;
            for (int i = 0; i < inputs.Count; i++)
            {
                Input += inputs[i] * Weights[i];
            }

            Input += Bias;

            // Apply the appropriate activation function
            if (_activationType == ActivationType.Sigmoid)
            {
                Output = Sigmoid(Input);
            }
            else // ReLU
            {
                Output = ReLU(Input);
            }

            return Output;
        }

        public void UpdateWeights(List<double> inputs, double learningRate)
        {
            // Add gradient clipping to prevent exploding gradients
            double clippedError = Math.Max(-1.0, Math.Min(1.0, Error));

            // Calculate derivative based on activation function
            double derivative;
            if (_activationType == ActivationType.Sigmoid)
            {
                derivative = SigmoidDerivative(Output); // Note: Using Output for sigmoid
            }
            else // ReLU
            {
                derivative = ReLUDerivative(Input);
            }

            for (int i = 0; i < Weights.Count; i++)
            {
                // Calculate weight delta with gradient clipping
                double delta = learningRate * clippedError * derivative * inputs[i];

                // Check for NaN and prevent it
                if (!double.IsNaN(delta))
                {
                    // ADD delta for gradient ascent (not descent) because Error = expected - actual
                    Weights[i] += delta;
                }
            }

            // Update bias with gradient clipping
            double biasDelta = learningRate * clippedError * derivative;
            if (!double.IsNaN(biasDelta))
            {
                // ADD delta for gradient ascent
                Bias += biasDelta;
            }
        }

        public void LoadWeights(List<double> weights)
        {
            Weights = weights;
        }
    }
}