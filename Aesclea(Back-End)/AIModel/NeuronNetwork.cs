using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Aesclea_Back_End_.AIModel
{
    public class NeuronNetwork
    {
        public List<NeuronLayer> Layers { get; private set; }
        private int TotalNumberOfLayers;

        public NeuronNetwork(int[] neuronsPerLayer)
        {
            if (neuronsPerLayer.Length < 2)
                throw new ArgumentException("Network must have at least input and output layers");

            TotalNumberOfLayers = neuronsPerLayer.Length - 1; // Subtract 1 because we'll handle input separately
            Layers = new List<NeuronLayer>(TotalNumberOfLayers);

            // Hidden layers (using ReLU)
            for (int i = 1; i < neuronsPerLayer.Length - 1; i++)
            {
                Layers.Add(new NeuronLayer(neuronsPerLayer[i], neuronsPerLayer[i - 1], Neuron.ActivationType.ReLU));
            }

            // Output layer (using Sigmoid for bounded output between 0 and 1)
            Layers.Add(new NeuronLayer(neuronsPerLayer[neuronsPerLayer.Length - 1],
                      neuronsPerLayer[neuronsPerLayer.Length - 2],
                      Neuron.ActivationType.Sigmoid));
        }

        public List<double> FeedForward(List<double> inputs)
        {
            List<double> currentInputs = new List<double>(inputs);

            // Process each layer
            foreach (var layer in Layers)
            {
                currentInputs = layer.FeedForward(currentInputs);
            }

            return currentInputs;
        }

        public void Backpropagate(List<double> inputs, List<double> expectedOutput, double learningRate)
        {
            // Store activations (outputs) from each layer
            List<List<double>> activations = new List<List<double>>();
            activations.Add(new List<double>(inputs));

            // Forward pass to collect all activations
            List<double> currentActivation = new List<double>(inputs);

            for (int i = 0; i < Layers.Count; i++)
            {
                currentActivation = Layers[i].FeedForward(currentActivation);
                activations.Add(new List<double>(currentActivation));
            }

            // Calculate output layer errors
            var outputLayer = Layers[Layers.Count - 1];
            for (int i = 0; i < outputLayer.Neurons.Count; i++)
            {
                if (i < expectedOutput.Count)
                {
                    // Error = expected - actual
                    outputLayer.Neurons[i].Error = expectedOutput[i] - activations[activations.Count - 1][i];
                }
            }

            // Backpropagate error through the network
            for (int l = Layers.Count - 1; l > 0; l--)
            {
                var currentLayer = Layers[l];
                var prevLayer = Layers[l - 1];

                // Update weights for current layer
                currentLayer.UpdateWeights(activations[l], learningRate);

                // Calculate errors for previous layer (adjust error propagation weights)
                for (int i = 0; i < prevLayer.Neurons.Count; i++)
                {
                    var neuron = prevLayer.Neurons[i];
                    neuron.Error = 0;

                    for (int j = 0; j < currentLayer.Neurons.Count; j++)
                    {
                        if (i < currentLayer.Neurons[j].Weights.Count)
                        {
                            neuron.Error += currentLayer.Neurons[j].Error * currentLayer.Neurons[j].Weights[i];
                        }
                    }
                }
            }

            // Update weights for first layer
            Layers[0].UpdateWeights(inputs, learningRate);
        }

        public void Train(List<List<double>> inputs, List<List<double>> expectedOutputs, int epochs, double learningRate)
        {
            if (inputs.Count != expectedOutputs.Count)
                throw new ArgumentException("Number of input samples must match number of expected output samples");

            // Check if we have any data
            if (inputs.Count == 0)
                throw new ArgumentException("No training data provided");

            // Start with higher learning rate and decrease over time
            double initialLearningRate = learningRate;

            // Set up progress tracking
            int totalIterations = epochs * inputs.Count;
            int currentIteration = 0;
            int lastPercentageReported = -1;
            DateTime startTime = DateTime.Now;

            Console.WriteLine($"Starting training with {inputs.Count} samples for {epochs} epochs ({totalIterations} total iterations)");

            for (int epoch = 0; epoch < epochs; epoch++)
            {
                // Decrease learning rate over time (learning rate decay)
                double currentLearningRate = initialLearningRate / (1 + 0.0001 * epoch);
                double totalError = 0;

                // Shuffle training data for better generalization
                List<int> indices = Enumerable.Range(0, inputs.Count).ToList();
                Shuffle(indices);

                foreach (int i in indices)
                {
                    var input = inputs[i];
                    var expectedOutput = expectedOutputs[i];

                    // Forward pass
                    var output = FeedForward(input);

                    // Calculate mean squared error
                    for (int j = 0; j < expectedOutput.Count; j++)
                    {
                        if (j < output.Count)
                        {
                            totalError += Math.Pow(expectedOutput[j] - output[j], 2);
                        }
                    }

                    // Backpropagation
                    Backpropagate(input, expectedOutput, currentLearningRate);

                    // Update progress
                    currentIteration++;

                    // Simplify progress reporting to ensure updates occur
                    int currentPercentage = (int)((double)currentIteration / totalIterations * 100);

                    // Update progress display more frequently (every 1% or when it changes)
                    if (currentPercentage != lastPercentageReported)
                    {
                        TimeSpan elapsed = DateTime.Now - startTime;
                        TimeSpan estimated = TimeSpan.FromTicks((long)(elapsed.Ticks / (currentIteration / (double)totalIterations)));
                        TimeSpan remaining = estimated - elapsed;

                        // Ensure console output is flushed immediately
                        Console.Write($"\rTraining progress: {currentPercentage}% | Error: {totalError / (i + 1):F6} | Time remaining: {FormatTimeSpan(remaining)}        ");
                        Console.Out.Flush();
                        lastPercentageReported = currentPercentage;
                    }
                }

                totalError /= inputs.Count;

                // Print detailed progress after each epoch
                Console.WriteLine($"\nEpoch {epoch + 1}/{epochs}: Error = {totalError:F6}, Learning Rate = {currentLearningRate:F6}");

                // Early stopping if error is very low
                if (totalError < 0.001)
                {
                    Console.WriteLine($"\nTraining converged at epoch {epoch + 1} with error {totalError:F6}");
                    break;
                }
            }

            Console.WriteLine("\nTraining complete!");
        }

        // Helper method to format time remaining
        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalHours >= 1)
            {
                return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m";
            }
            else if (timeSpan.TotalMinutes >= 1)
            {
                return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
            }
            else
            {
                return $"{timeSpan.Seconds}s";
            }
        }

        public void SaveWeights(string filePath)
        {
            try
            {
                var allWeights = new List<List<List<double>>>();
                var allBiases = new List<List<double>>();

                foreach (var layer in Layers)
                {
                    var layerWeights = new List<List<double>>();
                    var layerBiases = new List<double>();

                    foreach (var neuron in layer.Neurons)
                    {
                        layerWeights.Add(neuron.Weights);
                        layerBiases.Add(neuron.Bias);
                    }

                    allWeights.Add(layerWeights);
                    allBiases.Add(layerBiases);
                }

                var modelData = new
                {
                    Weights = allWeights,
                    Biases = allBiases
                };

                var json = JsonConvert.SerializeObject(modelData, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving weights: {ex.Message}");
                throw;
            }
        }

        public void LoadWeights(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The weight file at {filePath} does not exist.");

            try
            {
                var json = File.ReadAllText(filePath);
                dynamic modelData = JsonConvert.DeserializeObject(json);

                var allWeights = JsonConvert.DeserializeObject<List<List<List<double>>>>(modelData.Weights.ToString());
                var allBiases = JsonConvert.DeserializeObject<List<List<double>>>(modelData.Biases.ToString());

                if (allWeights.Count != Layers.Count || allBiases.Count != Layers.Count)
                {
                    throw new Exception("Weight file structure doesn't match current network architecture");
                }

                for (int layerIndex = 0; layerIndex < Layers.Count; layerIndex++)
                {
                    var layer = Layers[layerIndex];
                    if (allWeights[layerIndex].Count != layer.Neurons.Count)
                    {
                        throw new Exception($"Layer {layerIndex}: Weight count mismatch");
                    }

                    for (int neuronIndex = 0; neuronIndex < layer.Neurons.Count; neuronIndex++)
                    {
                        if (allWeights[layerIndex][neuronIndex].Count != layer.Neurons[neuronIndex].Weights.Count)
                        {
                            throw new Exception($"Neuron {neuronIndex} in layer {layerIndex}: Weight dimension mismatch");
                        }

                        layer.Neurons[neuronIndex].Weights = allWeights[layerIndex][neuronIndex];
                        layer.Neurons[neuronIndex].Bias = allBiases[layerIndex][neuronIndex];
                    }
                }

                Console.WriteLine("Weights successfully loaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading weights: {ex.Message}");
                throw;
            }
        }

        // Helper method to calculate model accuracy
        public double CalculateAccuracy(List<List<double>> testInputs, List<List<double>> expectedOutputs, double threshold = 0.5)
        {
            if (testInputs.Count != expectedOutputs.Count)
                throw new ArgumentException("Test inputs and expected outputs must have the same count.");

            int correctCount = 0;

            for (int i = 0; i < testInputs.Count; i++)
            {
                var output = FeedForward(testInputs[i]);
                bool predictedClass = output[0] >= threshold;
                bool actualClass = expectedOutputs[i][0] >= threshold;

                if (predictedClass == actualClass)
                {
                    correctCount++;
                }
            }

            return (double)correctCount / testInputs.Count;
        }

        // Helper method to shuffle training data
        private void Shuffle(List<int> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}