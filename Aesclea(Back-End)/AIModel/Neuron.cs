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
        private double _dropoutRate = 0.0; // Dropout probability

        public enum ActivationType
        {
            ReLU,
            Sigmoid,
            LeakyReLU
        }

        // Activation functions
        public static double ReLU(double x) => Math.Max(0, x);
        public static double ReLUDerivative(double x) => x > 0 ? 1 : 0;

        public static double LeakyReLU(double x) => x > 0 ? x : 0.01 * x;
        public static double LeakyReLUDerivative(double x) => x > 0 ? 1 : 0.01;

        public static double Sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-x));
        public static double SigmoidDerivative(double x) => x * (1 - x); // Note: x should be sigmoid output

        public Neuron(int numberOfInputs, ActivationType activationType = ActivationType.LeakyReLU, double dropoutRate = 0.0)
        {
            _activationType = activationType;
            _dropoutRate = dropoutRate;
            Weights = new List<double>(numberOfInputs);

            // He initialization for ReLU/LeakyReLU, Xavier/Glorot for Sigmoid
            double weightScale = (_activationType == ActivationType.Sigmoid)
                ? Math.Sqrt(2.0 / numberOfInputs)
                : Math.Sqrt(2.0 / numberOfInputs);

            for (int i = 0; i < numberOfInputs; i++)
            {
                Weights.Add((random.NextDouble() * 2 - 1) * weightScale);
            }
            Bias = (random.NextDouble() * 2 - 1) * 0.1;
        }

        public double FeedForward(List<double> inputs, bool isTraining = true)
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
            else if (_activationType == ActivationType.LeakyReLU)
            {
                Output = LeakyReLU(Input);
            }
            else // ReLU
            {
                Output = ReLU(Input);
            }

            // Apply dropout during training
            if (isTraining && _dropoutRate > 0)
            {
                if (random.NextDouble() < _dropoutRate)
                {
                    Output = 0;
                }
                else
                {
                    // Scale output to maintain same expected value
                    Output /= (1 - _dropoutRate);
                }
            }

            return Output;
        }

        public void UpdateWeights(List<double> inputs, double learningRate, double l2Lambda = 0.0001)
        {
            // Add gradient clipping to prevent exploding gradients
            double clippedError = Math.Max(-1.0, Math.Min(1.0, Error));

            // Calculate derivative based on activation function
            double derivative;
            if (_activationType == ActivationType.Sigmoid)
            {
                derivative = SigmoidDerivative(Output); // Note: Using Output for sigmoid
            }
            else if (_activationType == ActivationType.LeakyReLU)
            {
                derivative = LeakyReLUDerivative(Input);
            }
            else // ReLU
            {
                derivative = ReLUDerivative(Input);
            }

            for (int i = 0; i < Weights.Count; i++)
            {
                // Calculate weight delta with gradient clipping
                double delta = learningRate * clippedError * derivative * inputs[i];

                // Add L2 regularization
                double regularizationTerm = -learningRate * l2Lambda * Weights[i];

                // Check for NaN and prevent it
                if (!double.IsNaN(delta))
                {
                    // ADD delta for gradient ascent (not descent) because Error = expected - actual
                    Weights[i] += delta + regularizationTerm;
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