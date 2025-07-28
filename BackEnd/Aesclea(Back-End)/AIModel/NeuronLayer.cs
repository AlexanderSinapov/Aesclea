using System;
using System.Collections.Generic;

namespace Aesclea_Back_End_.AIModel
{
    public class NeuronLayer
    {
        public List<Neuron> Neurons { get; private set; }
        public List<double> Outputs { get; private set; } // Store outputs for backpropagation
        public double DropoutRate { get; private set; }
        public NeuronLayer(int neuronCount, int inputsForNeuron, Neuron.ActivationType activationType = Neuron.ActivationType.LeakyReLU, double dropoutRate = 0.0)
        {
            Neurons = new List<Neuron>(neuronCount);
            Outputs = new List<double>(neuronCount);
            DropoutRate = dropoutRate;
            for (int i = 0; i < neuronCount; i++)
            {
                Neurons.Add(new Neuron(inputsForNeuron, activationType, dropoutRate));
            }
        }
        public List<double> FeedForward(List<double> inputs, bool isTraining = true)
        {
            Outputs = new List<double>();
            foreach (var neuron in Neurons)
            {
                Outputs.Add(neuron.FeedForward(inputs, isTraining));
            }
            return Outputs;
        }
        public void UpdateWeights(List<double> inputs, double learningRate, double l2Lambda = 0.0001)
        {
            for (int i = 0; i < Neurons.Count; i++)
            {
                var neuron = Neurons[i];
                // Pass the previous layer's outputs as inputs for updating weights
                neuron.UpdateWeights(inputs, learningRate, l2Lambda);
            }
        }
    }
}
