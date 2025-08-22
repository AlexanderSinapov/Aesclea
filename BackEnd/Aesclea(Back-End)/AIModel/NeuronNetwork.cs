using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Aesclea_Back_End_.DDOs;

namespace Aesclea_Back_End_.AIModel
{
    public class NeuronNetwork
    {
        public List<NeuronLayer> Layers { get; private set; }
        private int TotalNumberOfLayers;

        public NeuronNetwork(int[] neuronsPerLayer, double dropoutRate = 0.0)
        {
            if (neuronsPerLayer.Length < 2)
                throw new ArgumentException("Network must have at least input and output layers");

            TotalNumberOfLayers = neuronsPerLayer.Length - 1; // Subtract 1 because we'll handle input separately
            Layers = new List<NeuronLayer>(TotalNumberOfLayers);

            // Hidden layers (using LeakyReLU)
            for (int i = 1; i < neuronsPerLayer.Length - 1; i++)
            {
                Layers.Add(new NeuronLayer(neuronsPerLayer[i], neuronsPerLayer[i - 1],
                    Neuron.ActivationType.LeakyReLU,
                    // Apply dropout to hidden layers only, with decreasing rate for deeper layers
                    dropoutRate * (1.0 - (double)i / neuronsPerLayer.Length)));
            }

            // Output layer (using Sigmoid for bounded output between 0 and 1)
            Layers.Add(new NeuronLayer(neuronsPerLayer[neuronsPerLayer.Length - 1],
                      neuronsPerLayer[neuronsPerLayer.Length - 2],
                      Neuron.ActivationType.Sigmoid,
                      0.0)); // No dropout in output layer
        }

        public List<double> FeedForward(List<double> inputs, bool isTraining = false)
        {
            List<double> currentInputs = new List<double>(inputs);

            // Process each layer
            foreach (var layer in Layers)
            {
                currentInputs = layer.FeedForward(currentInputs, isTraining);
            }

            return currentInputs;
        }

        public void Backpropagate(List<double> inputs, List<double> expectedOutput, double learningRate, double l2Lambda = 0.0001)
        {
            // Store activations (outputs) from each layer
            List<List<double>> activations = new List<List<double>>();
            activations.Add(new List<double>(inputs));

            // Forward pass to collect all activations
            List<double> currentActivation = new List<double>(inputs);

            for (int i = 0; i < Layers.Count; i++)
            {
                currentActivation = Layers[i].FeedForward(currentActivation, true);
                activations.Add(new List<double>(currentActivation));
                //Console.WriteLine($"Layer {i + 1} activation: {string.Join(", ", currentActivation)}");
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
                currentLayer.UpdateWeights(activations[l], learningRate, l2Lambda);

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
            Layers[0].UpdateWeights(inputs, learningRate, l2Lambda);
        }

        public void Train(List<List<double>> inputs, List<List<double>> expectedOutputs, int epochs, double learningRate)
        {
            TrainWithBatches(inputs, expectedOutputs, epochs, learningRate);
        }

        public void TrainWithBatches(List<List<double>> inputs, List<List<double>> expectedOutputs, int epochs, double learningRate, int batchSize = 16)
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
            double lastPercentageReported = -1.0;
            DateTime startTime = DateTime.Now;

            Console.WriteLine($"Starting training with {inputs.Count} samples for {epochs} epochs ({totalIterations} total iterations)");
            Console.WriteLine($"Using batch size: {batchSize}");

            // For early stopping
            double bestError = double.MaxValue;
            int patienceCounter = 0;
            int patienceLimit = 10; // Stop after 10 epochs without improvement
            List<double> trainingErrors = new List<double>();

            Console.Out.Flush();

            for (int epoch = 0; epoch < epochs; epoch++)
            {
                // Learning rate scheduler - cosine annealing
                double currentLearningRate = initialLearningRate *
                    (0.5 * (1 + Math.Cos(Math.PI * epoch / epochs)));

                double totalError = 0;

                // Shuffle training data for better generalization
                List<int> indices = Enumerable.Range(0, inputs.Count).ToList();
                Shuffle(indices);

                // Process in batches
                for (int batchStart = 0; batchStart < indices.Count; batchStart += batchSize)
                {
                    int currentBatchSize = Math.Min(batchSize, indices.Count - batchStart);

                    // Process each sample in the batch
                    double batchError = 0;
                    for (int i = 0; i < currentBatchSize; i++)
                    {
                        int idx = indices[batchStart + i];
                        var input = inputs[idx];
                        var expectedOutput = expectedOutputs[idx];

                        // Forward pass
                        var output = FeedForward(input, true);

                        // Calculate mean squared error
                        double sampleError = 0;
                        for (int j = 0; j < expectedOutput.Count; j++)
                        {
                            if (j < output.Count)
                            {
                                sampleError += Math.Pow(expectedOutput[j] - output[j], 2);
                            }
                        }
                        batchError += sampleError;

                        // Backpropagation
                        Backpropagate(input, expectedOutput, currentLearningRate);

                        // Update progress
                        currentIteration++;
                    }

                    // Average error for this batch
                    batchError /= currentBatchSize;
                    totalError += batchError * currentBatchSize;

                    // Update progress display
                    double currentPercentage = ((double)currentIteration / totalIterations) * 100;

                    if (currentPercentage != lastPercentageReported)
                    {
                        TimeSpan elapsed = DateTime.Now - startTime;
                        TimeSpan estimated = TimeSpan.FromTicks((long)(elapsed.Ticks / (currentIteration / (double)totalIterations)));
                        TimeSpan remaining = estimated - elapsed;

                        Console.Write($"\rTraining progress: {currentPercentage.ToString("N2")}% | Error: {batchError:F6} | Time remaining: {FormatTimeSpan(remaining)} | Epoch: {epoch}/{epochs} | Iterations: {currentIteration}/{totalIterations}        ");
                        Console.Out.Flush();
                        lastPercentageReported = currentPercentage;
                    }
                }

                totalError /= inputs.Count;
                trainingErrors.Add(totalError);

                // Print detailed progress after each epoch
                Console.WriteLine($"\nEpoch {epoch + 1}/{epochs}: Error = {totalError:F6}, Learning Rate = {currentLearningRate:F6}");

                // Early stopping check
                if (totalError < bestError)
                {
                    bestError = totalError;
                    patienceCounter = 0;
                }
                else
                {
                    patienceCounter++;
                    if (patienceCounter >= patienceLimit)
                    {
                        Console.WriteLine($"\nEarly stopping triggered after {epoch + 1} epochs with no improvement for {patienceLimit} epochs");
                        break;
                    }
                }

                // Very low error check
                if (totalError < 0.001)
                {
                    Console.WriteLine($"\nTraining converged at epoch {epoch + 1} with error {totalError:F6}");
                    break;
                }
            }

            Console.WriteLine("\nTraining complete!");

            // Show error evolution if requested
            Console.WriteLine("Would you like to see the error evolution? (Y/N)");
            if (Console.ReadLine().Trim().ToUpper() == "Y")
            {
                Console.WriteLine("Error evolution across epochs:");
                for (int i = 0; i < trainingErrors.Count; i++)
                {
                    Console.WriteLine($"Epoch {i + 1}: {trainingErrors[i]:F6}");
                }
            }
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

        [Obsolete("SaveWeights is deprecated, please use GetNeuralNetworkData instead.")]

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

        public void SetNeuralNetworkData(NeuralData data)
        {
            List<List<List<double>>> allWeights = data.Weights;
            List<List<double>> allBiases = data.Biases;

            if (allWeights.Count != Layers.Count || allBiases.Count != Layers.Count)
            {
                throw new Exception("Weight file structure doesn't match current network architecture");
            }

            try
            {
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
                Console.WriteLine($"Error setting weights: {ex.Message}");
                throw;
            }


        }

        public NeuralData GetNeuralNetworkData()
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
            return new NeuralData(allWeights, allBiases);
        }

        [Obsolete("LoadWeights is deprecated, please use SetNeuralNetworkData instead.")]
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
                var output = FeedForward(testInputs[i], false); // No dropout during evaluation
                bool predictedClass = output[0] >= threshold;
                bool actualClass = expectedOutputs[i][0] >= threshold;

                if (predictedClass == actualClass)
                {
                    correctCount++;
                }
            }

            return (double)correctCount / testInputs.Count;
        }

        // Calculate more detailed metrics
        public Dictionary<string, double> CalculateMetrics(List<List<double>> testInputs, List<List<double>> expectedOutputs, double threshold = 0.5)
        {
            if (testInputs.Count != expectedOutputs.Count)
                throw new ArgumentException("Test inputs and expected outputs must have the same count.");

            int truePositives = 0;
            int trueNegatives = 0;
            int falsePositives = 0;
            int falseNegatives = 0;
            double totalError = 0;

            for (int i = 0; i < testInputs.Count; i++)
            {
                var output = FeedForward(testInputs[i], false); // No dropout during evaluation
                bool predictedClass = output[0] >= threshold;
                bool actualClass = expectedOutputs[i][0] >= threshold;

                // Update confusion matrix
                if (actualClass && predictedClass) truePositives++;
                if (!actualClass && !predictedClass) trueNegatives++;
                if (!actualClass && predictedClass) falsePositives++;
                if (actualClass && !predictedClass) falseNegatives++;

                // Calculate mean squared error
                totalError += Math.Pow(expectedOutputs[i][0] - output[0], 2);
            }

            // Calculate metrics
            double accuracy = (double)(truePositives + trueNegatives) / testInputs.Count;
            double precision = truePositives == 0 ? 0 : (double)truePositives / (truePositives + falsePositives);
            double recall = truePositives == 0 ? 0 : (double)truePositives / (truePositives + falseNegatives);
            double f1Score = precision + recall == 0 ? 0 : 2 * precision * recall / (precision + recall);
            double specificity = trueNegatives == 0 ? 0 : (double)trueNegatives / (trueNegatives + falsePositives);
            double mse = totalError / testInputs.Count;

            return new Dictionary<string, double>
            {
                { "Accuracy", accuracy },
                { "Precision", precision },
                { "Recall", recall },
                { "F1Score", f1Score },
                { "Specificity", specificity },
                { "MSE", mse },
                { "TruePositives", truePositives },
                { "TrueNegatives", trueNegatives },
                { "FalsePositives", falsePositives },
                { "FalseNegatives", falseNegatives }
            };
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