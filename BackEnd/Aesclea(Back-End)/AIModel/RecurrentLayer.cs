// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

namespace Aesclea_Back_End_.AIModel
{
    public class RecurrentLayer
    {
        public List<RecurrentNeuron> Neurons { get; private set; }
        public List<double> Outputs { get; private set; }
        public double DropoutRate { get; private set; }

        public RecurrentLayer(int neuronCount, int inputsForNeuron, Neuron.ActivationType activationType = Neuron.ActivationType.LeakyReLU, double dropoutRate = 0.0)
        {
            Neurons = new List<RecurrentNeuron>(neuronCount);
            Outputs = new List<double>(neuronCount);
            DropoutRate = dropoutRate;

            for (int i = 0; i < neuronCount; i++)
            {
                Neurons.Add(new RecurrentNeuron(inputsForNeuron, activationType));
            }
        }

        public List<double> FeedForward(List<double> inputs, bool isTraining = true)
        {
            Outputs = new List<double>();
            Random random = new Random();

            foreach (var neuron in Neurons)
            {
                double output = neuron.FeedForwardRecurrent(inputs);

                // Apply dropout during training
                if (isTraining && DropoutRate > 0)
                {
                    if (random.NextDouble() < DropoutRate)
                    {
                        output = 0;
                    }
                    else
                    {
                        // Scale output to maintain same expected value
                        output /= (1 - DropoutRate);
                    }
                }

                Outputs.Add(output);
            }

            return Outputs;
        }

        public void UpdateWeights(List<double> inputs, double learningRate, double l2Lambda = 0.0001)
        {
            foreach (var neuron in Neurons)
            {
                // First update the standard weights
                neuron.UpdateWeights(inputs, learningRate, l2Lambda);

                // Now update recurrent weights
                double derivative;
                if (neuron._activationType == Neuron.ActivationType.Sigmoid)
                {
                    derivative = Neuron.SigmoidDerivative(neuron.Output);
                }
                else if (neuron._activationType == Neuron.ActivationType.LeakyReLU)
                {
                    derivative = Neuron.LeakyReLUDerivative(neuron.Input);
                }
                else
                {
                    derivative = Neuron.ReLUDerivative(neuron.Input);
                }

                // Gradient clipping
                double clippedError = Math.Max(-1.0, Math.Min(1.0, neuron.Error));

                // Update recurrent weights with the previous output and L2 regularization
                for (int i = 0; i < neuron.RecurrentWeights.Count; i++)
                {
                    double delta = learningRate * clippedError * derivative * neuron.PreviousOutput;
                    double regularizationTerm = -learningRate * l2Lambda * neuron.RecurrentWeights[i];

                    if (!double.IsNaN(delta))
                    {
                        neuron.RecurrentWeights[i] += delta + regularizationTerm;
                    }
                }
            }
        }

        public void ResetState()
        {
            foreach (var neuron in Neurons)
            {
                neuron.ResetState();
            }
        }
    }
}
