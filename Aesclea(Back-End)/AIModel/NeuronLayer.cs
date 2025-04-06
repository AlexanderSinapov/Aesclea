using System;
using System.Collections.Generic;

namespace Aesclea_Back_End_.AIModel
{
    public class NeuronLayer
    {
        public List<Neuron> Neurons { get; private set; }
        public List<double> Outputs { get; private set; } // Store outputs for backpropagation

        public NeuronLayer(int neuronCount, int inputsForNeuron, Neuron.ActivationType activationType = Neuron.ActivationType.ReLU)
        {
            Neurons = new List<Neuron>(neuronCount);
            Outputs = new List<double>(neuronCount);

            for (int i = 0; i < neuronCount; i++)
            {
                Neurons.Add(new Neuron(inputsForNeuron, activationType));
            }
        }

        public List<double> FeedForward(List<double> inputs)
        {
            Outputs = new List<double>();
            foreach (var neuron in Neurons)
            {
                Outputs.Add(neuron.FeedForward(inputs));
            }
            return Outputs;
        }

        public void UpdateWeights(List<double> inputs, double learningRate)
        {
            for (int i = 0; i < Neurons.Count; i++)
            {
                var neuron = Neurons[i];
                // Pass the previous layer's outputs as inputs for updating weights
                neuron.UpdateWeights(inputs, learningRate);
            }
        }
    }
}