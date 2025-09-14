// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Newtonsoft.Json;
using Aesclea_Back_End_.DDOs;

namespace Aesclea_Back_End_.AIModel
{
    public class RecurrentNeuralNetwork
    {
        public List<RecurrentLayer> RecurrentLayers { get; private set; }
        public List<NeuronLayer> OutputLayers { get; private set; }
        private int TotalRecurrentLayers;
        private int TotalOutputLayers;

        /// <summary>
        /// Creates a new Recurrent Neural Network with specified architecture
        /// </summary>
        /// <param name="recurrentLayerSizes">Array of neuron counts for recurrent layers</param>
        /// <param name="outputLayerSizes">Array of neuron counts for standard output layers</param>
        /// <param name="inputSize">Size of the input vector</param>
        /// <param name="dropoutRate">Initial dropout rate for regularization</param>
        public RecurrentNeuralNetwork(int[] recurrentLayerSizes, int[] outputLayerSizes, int inputSize, double dropoutRate = 0.0)
        {
            if (recurrentLayerSizes.Length < 1)
                throw new ArgumentException("Network must have at least one recurrent layer");

            if (outputLayerSizes.Length < 1)
                throw new ArgumentException("Network must have at least one output layer");

            TotalRecurrentLayers = recurrentLayerSizes.Length;
            TotalOutputLayers = outputLayerSizes.Length;

            RecurrentLayers = new List<RecurrentLayer>(TotalRecurrentLayers);
            OutputLayers = new List<NeuronLayer>(TotalOutputLayers);

            // First recurrent layer receives the input directly
            RecurrentLayers.Add(new RecurrentLayer(
                recurrentLayerSizes[0],
                inputSize,
                Neuron.ActivationType.LeakyReLU,
                dropoutRate));

            // Additional recurrent layers
            for (int i = 1; i < recurrentLayerSizes.Length; i++)
            {
                RecurrentLayers.Add(new RecurrentLayer(
                    recurrentLayerSizes[i],
                    recurrentLayerSizes[i - 1],
                    Neuron.ActivationType.LeakyReLU,
                    dropoutRate * (1.0 - (double)i / recurrentLayerSizes.Length)));
            }

            // First output layer receives inputs from the last recurrent layer
            OutputLayers.Add(new NeuronLayer(
                outputLayerSizes[0],
                recurrentLayerSizes[recurrentLayerSizes.Length - 1],
                Neuron.ActivationType.LeakyReLU,
                dropoutRate * 0.5));

            // Additional output layers (if any)
            for (int i = 1; i < outputLayerSizes.Length; i++)
            {
                OutputLayers.Add(new NeuronLayer(
                    outputLayerSizes[i],
                    outputLayerSizes[i - 1],
                    i == outputLayerSizes.Length - 1 ? Neuron.ActivationType.Sigmoid : Neuron.ActivationType.LeakyReLU,
                    i == outputLayerSizes.Length - 1 ? 0.0 : dropoutRate * 0.5));
            }
        }

        /// <summary>
        /// Process a sequence of inputs through the network
        /// </summary>
        /// <param name="inputSequence">List of input vectors representing a sequence</param>
        /// <param name="isTraining">Whether we're in training mode (applies dropout)</param>
        /// <returns>Output from the final timestep</returns>
        public List<double> ProcessSequence(List<List<double>> inputSequence, bool isTraining = false)
        {
            ResetState(); // Clear memory state before processing new sequence

            List<double> output = null;

            // Process each timestep in the sequence
            foreach (var input in inputSequence)
            {
                output = FeedForward(input, isTraining);
            }

            return output; // Return final output
        }

        /// <summary>
        /// Reset memory state for all recurrent layers
        /// </summary>
        public void ResetState()
        {
            foreach (var layer in RecurrentLayers)
            {
                layer.ResetState();
            }
        }

        /// <summary>
        /// Feed forward a single input through the network
        /// </summary>
        /// <param name="inputs">Input vector</param>
        /// <param name="isTraining">Whether we're in training mode</param>
        /// <returns>Output vector</returns>
        public List<double> FeedForward(List<double> inputs, bool isTraining = false)
        {
            // Process through recurrent layers
            List<double> currentInputs = new List<double>(inputs);

            foreach (var layer in RecurrentLayers)
            {
                currentInputs = layer.FeedForward(currentInputs, isTraining);
            }

            // Process through output layers
            foreach (var layer in OutputLayers)
            {
                currentInputs = layer.FeedForward(currentInputs, isTraining);
            }

            return currentInputs;
        }

        /// <summary>
        /// Train the network on a sequence
        /// </summary>
        /// <param name="inputSequence">List of input vectors</param>
        /// <param name="expectedOutputSequence">List of expected outputs (one per timestep)</param>
        /// <param name="learningRate">Learning rate</param>
        /// <param name="l2Lambda">L2 regularization parameter</param>
        public void TrainSequence(List<List<double>> inputSequence, List<List<double>> expectedOutputSequence, double learningRate, double l2Lambda = 0.0001)
        {
            if (inputSequence.Count != expectedOutputSequence.Count)
                throw new ArgumentException("Input sequence and expected output sequence must have the same length");

            ResetState(); // Start with fresh state

            // Store activations for backpropagation
            List<List<List<double>>> recurrentActivations = new List<List<List<double>>>();
            List<List<List<double>>> outputActivations = new List<List<List<double>>>();
            List<List<double>> inputActivations = new List<List<double>>();

            // Forward pass through the sequence
            for (int t = 0; t < inputSequence.Count; t++)
            {
                List<double> input = inputSequence[t];
                inputActivations.Add(new List<double>(input));

                // Process through recurrent layers and store activations
                List<List<double>> recurrentTimeStepActivations = new List<List<double>>();
                List<double> currentInputs = new List<double>(input);
                recurrentTimeStepActivations.Add(new List<double>(currentInputs));

                foreach (var layer in RecurrentLayers)
                {
                    currentInputs = layer.FeedForward(currentInputs, true);
                    recurrentTimeStepActivations.Add(new List<double>(currentInputs));
                }

                recurrentActivations.Add(recurrentTimeStepActivations);

                // Process through output layers and store activations
                List<List<double>> outputTimeStepActivations = new List<List<double>>();
                outputTimeStepActivations.Add(new List<double>(currentInputs));

                foreach (var layer in OutputLayers)
                {
                    currentInputs = layer.FeedForward(currentInputs, true);
                    outputTimeStepActivations.Add(new List<double>(currentInputs));
                }

                outputActivations.Add(outputTimeStepActivations);
            }

            // Backpropagation Through Time (BPTT)
            // We'll go backwards through the sequence
            for (int t = inputSequence.Count - 1; t >= 0; t--)
            {
                // Calculate output layer errors (starting from the last layer)
                var outputLayer = OutputLayers[OutputLayers.Count - 1];
                for (int i = 0; i < outputLayer.Neurons.Count; i++)
                {
                    if (i < expectedOutputSequence[t].Count)
                    {
                        // Error = expected - actual
                        outputLayer.Neurons[i].Error = expectedOutputSequence[t][i] - outputActivations[t][outputActivations[t].Count - 1][i];
                    }
                }

                // Backpropagate through output layers
                for (int l = OutputLayers.Count - 1; l > 0; l--)
                {
                    var currentLayer = OutputLayers[l];
                    var prevLayer = OutputLayers[l - 1];

                    // Update weights for current layer
                    currentLayer.UpdateWeights(outputActivations[t][l], learningRate, l2Lambda);

                    // Calculate errors for previous layer
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

                // Update first output layer with inputs from last recurrent layer
                OutputLayers[0].UpdateWeights(recurrentActivations[t][recurrentActivations[t].Count - 1], learningRate, l2Lambda);

                // Propagate error to recurrent layers
                // First, set error for last recurrent layer from first output layer
                var lastRecurrentLayer = RecurrentLayers[RecurrentLayers.Count - 1];
                for (int i = 0; i < lastRecurrentLayer.Neurons.Count; i++)
                {
                    var neuron = lastRecurrentLayer.Neurons[i];
                    neuron.Error = 0;

                    var firstOutputLayer = OutputLayers[0];
                    for (int j = 0; j < firstOutputLayer.Neurons.Count; j++)
                    {
                        if (i < firstOutputLayer.Neurons[j].Weights.Count)
                        {
                            neuron.Error += firstOutputLayer.Neurons[j].Error * firstOutputLayer.Neurons[j].Weights[i];
                        }
                    }
                }

                // Backpropagate through recurrent layers
                for (int l = RecurrentLayers.Count - 1; l > 0; l--)
                {
                    var currentLayer = RecurrentLayers[l];
                    var prevLayer = RecurrentLayers[l - 1];

                    // Update weights for current layer
                    currentLayer.UpdateWeights(recurrentActivations[t][l], learningRate, l2Lambda);

                    // Calculate errors for previous layer
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

                // Update first recurrent layer with original inputs
                RecurrentLayers[0].UpdateWeights(inputActivations[t], learningRate, l2Lambda);

                // If we're not at the beginning of the sequence, propagate errors further back in time
                if (t > 0)
                {
                    // TODO: For a full BPTT implementation, we'd need to track errors across time steps
                    // This would involve accumulating errors for each neuron from future timesteps
                    // For simplicity, we're using a truncated version here
                }
            }
        }

        /// <summary>
        /// Train the network on multiple sequences
        /// </summary>
        /// <param name="inputSequences">List of input sequences</param>
        /// <param name="expectedOutputSequences">List of expected output sequences</param>
        /// <param name="epochs">Number of training epochs</param>
        /// <param name="learningRate">Learning rate</param>
        public void Train(List<List<List<double>>> inputSequences, List<List<List<double>>> expectedOutputSequences,
                          int epochs, double learningRate)
        {
            if (inputSequences.Count != expectedOutputSequences.Count)
                throw new ArgumentException("Number of input sequences must match number of expected output sequences");

            // Initial learning rate
            double initialLearningRate = learningRate;

            Console.WriteLine($"Starting training with {inputSequences.Count} sequences for {epochs} epochs");

            // For early stopping
            double bestError = double.MaxValue;
            int patienceCounter = 0;
            int patienceLimit = 5;

            for (int epoch = 0; epoch < epochs; epoch++)
            {
                // Learning rate scheduler with cosine annealing
                double currentLearningRate = initialLearningRate *
                    (0.5 * (1 + Math.Cos(Math.PI * epoch / epochs)));

                // Shuffle the training data
                List<int> indices = Enumerable.Range(0, inputSequences.Count).ToList();
                Shuffle(indices);

                double totalError = 0;
                int sequenceCount = 0;

                // Process each sequence
                for (int i = 0; i < indices.Count; i++)
                {
                    int idx = indices[i];
                    var inputSequence = inputSequences[idx];
                    var expectedOutputSequence = expectedOutputSequences[idx];

                    // Train on this sequence
                    TrainSequence(inputSequence, expectedOutputSequence, currentLearningRate);

                    // Calculate error for reporting
                    double sequenceError = 0;
                    ResetState();

                    for (int t = 0; t < inputSequence.Count; t++)
                    {
                        var output = FeedForward(inputSequence[t], false);

                        // Calculate mean squared error
                        for (int j = 0; j < expectedOutputSequence[t].Count; j++)
                        {
                            if (j < output.Count)
                            {
                                sequenceError += Math.Pow(expectedOutputSequence[t][j] - output[j], 2);
                            }
                        }
                    }

                    sequenceError /= inputSequence.Count * expectedOutputSequence[0].Count;
                    totalError += sequenceError;
                    sequenceCount++;

                    // Display progress for every 10% of sequences
                    if (i % Math.Max(1, indices.Count / 10) == 0)
                    {
                        Console.WriteLine($"Epoch {epoch + 1}/{epochs}: {i * 100 / indices.Count}% complete, Current Error: {sequenceError:F6}");
                    }
                }

                totalError /= sequenceCount;
                Console.WriteLine($"Epoch {epoch + 1}/{epochs}: Average Error = {totalError:F6}, Learning Rate = {currentLearningRate:F6}");

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
                        Console.WriteLine($"Early stopping triggered after {epoch + 1} epochs");
                        break;
                    }
                }

                // Very low error check
                if (totalError < 0.001)
                {
                    Console.WriteLine($"Training converged at epoch {epoch + 1}");
                    break;
                }
            }

            Console.WriteLine("Training complete!");
        }

        /// <summary>
        /// Get the network data for saving
        /// </summary>
        public RecurrentNeuralData GetNetworkData()
        {
            var recurrentLayerWeights = new List<List<List<double>>>();
            var recurrentLayerBiases = new List<List<double>>();
            var recurrentWeights = new List<List<List<double>>>();

            foreach (var layer in RecurrentLayers)
            {
                var layerWeights = new List<List<double>>();
                var layerBiases = new List<double>();
                var layerRecurrentWeights = new List<List<double>>();

                foreach (var neuron in layer.Neurons)
                {
                    layerWeights.Add(neuron.Weights);
                    layerBiases.Add(neuron.Bias);
                    layerRecurrentWeights.Add(neuron.RecurrentWeights);
                }

                recurrentLayerWeights.Add(layerWeights);
                recurrentLayerBiases.Add(layerBiases);
                recurrentWeights.Add(layerRecurrentWeights);
            }

            // Get standard output layer data
            var outputLayerWeights = new List<List<List<double>>>();
            var outputLayerBiases = new List<List<double>>();

            foreach (var layer in OutputLayers)
            {
                var layerWeights = new List<List<double>>();
                var layerBiases = new List<double>();

                foreach (var neuron in layer.Neurons)
                {
                    layerWeights.Add(neuron.Weights);
                    layerBiases.Add(neuron.Bias);
                }

                outputLayerWeights.Add(layerWeights);
                outputLayerBiases.Add(layerBiases);
            }

            return new RecurrentNeuralData(
                recurrentLayerWeights,
                recurrentLayerBiases,
                recurrentWeights,
                outputLayerWeights,
                outputLayerBiases
            );
        }

        /// <summary>
        /// Load network data
        /// </summary>
        public void SetNetworkData(RecurrentNeuralData data)
        {
            // Validate data format
            if (data.RecurrentLayerWeights.Count != RecurrentLayers.Count ||
                data.RecurrentLayerBiases.Count != RecurrentLayers.Count ||
                data.RecurrentWeights.Count != RecurrentLayers.Count)
            {
                throw new Exception("Recurrent layer data doesn't match current network architecture");
            }

            if (data.OutputLayerWeights.Count != OutputLayers.Count ||
                data.OutputLayerBiases.Count != OutputLayers.Count)
            {
                throw new Exception("Output layer data doesn't match current network architecture");
            }

            try
            {
                // Load recurrent layer data
                for (int layerIndex = 0; layerIndex < RecurrentLayers.Count; layerIndex++)
                {
                    var layer = RecurrentLayers[layerIndex];
                    if (data.RecurrentLayerWeights[layerIndex].Count != layer.Neurons.Count)
                    {
                        throw new Exception($"Recurrent Layer {layerIndex}: Weight count mismatch");
                    }

                    for (int neuronIndex = 0; neuronIndex < layer.Neurons.Count; neuronIndex++)
                    {
                        var neuron = layer.Neurons[neuronIndex];

                        if (data.RecurrentLayerWeights[layerIndex][neuronIndex].Count != neuron.Weights.Count)
                        {
                            throw new Exception($"Neuron {neuronIndex} in recurrent layer {layerIndex}: Weight dimension mismatch");
                        }

                        neuron.Weights = data.RecurrentLayerWeights[layerIndex][neuronIndex];
                        neuron.Bias = data.RecurrentLayerBiases[layerIndex][neuronIndex];
                        neuron.RecurrentWeights = data.RecurrentWeights[layerIndex][neuronIndex];
                    }
                }

                // Load output layer data
                for (int layerIndex = 0; layerIndex < OutputLayers.Count; layerIndex++)
                {
                    var layer = OutputLayers[layerIndex];
                    if (data.OutputLayerWeights[layerIndex].Count != layer.Neurons.Count)
                    {
                        throw new Exception($"Output Layer {layerIndex}: Weight count mismatch");
                    }

                    for (int neuronIndex = 0; neuronIndex < layer.Neurons.Count; neuronIndex++)
                    {
                        var neuron = layer.Neurons[neuronIndex];

                        if (data.OutputLayerWeights[layerIndex][neuronIndex].Count != neuron.Weights.Count)
                        {
                            throw new Exception($"Neuron {neuronIndex} in output layer {layerIndex}: Weight dimension mismatch");
                        }

                        neuron.Weights = data.OutputLayerWeights[layerIndex][neuronIndex];
                        neuron.Bias = data.OutputLayerBiases[layerIndex][neuronIndex];
                    }
                }

                Console.WriteLine("Network data successfully loaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting network data: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Save network to a file
        /// </summary>
        public void SaveNetwork(string filePath)
        {
            try
            {
                var networkData = GetNetworkData();
                var json = JsonConvert.SerializeObject(networkData, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Console.WriteLine($"Network saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving network: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Load network from a file
        /// </summary>
        public void LoadNetwork(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The network file at {filePath} does not exist.");

            try
            {
                var json = File.ReadAllText(filePath);
                var networkData = JsonConvert.DeserializeObject<RecurrentNeuralData>(json);
                SetNetworkData(networkData);
                Console.WriteLine($"Network loaded from {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading network: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Helper method to shuffle training data
        /// </summary>
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

        /// <summary>
        /// Calculate metrics for sequence prediction
        /// </summary>
        public Dictionary<string, double> CalculateSequenceMetrics(
            List<List<List<double>>> testInputSequences,
            List<List<List<double>>> expectedOutputSequences,
            double threshold = 0.5)
        {
            if (testInputSequences.Count != expectedOutputSequences.Count)
                throw new ArgumentException("Test inputs and expected outputs must have the same count");

            int totalSamples = 0;
            int correctPredictions = 0;
            int truePositives = 0;
            int trueNegatives = 0;
            int falsePositives = 0;
            int falseNegatives = 0;
            double totalError = 0;

            for (int i = 0; i < testInputSequences.Count; i++)
            {
                var inputSequence = testInputSequences[i];
                var expectedSequence = expectedOutputSequences[i];

                ResetState();

                // Process each timestep in the sequence
                for (int t = 0; t < inputSequence.Count; t++)
                {
                    var output = FeedForward(inputSequence[t], false);
                    var expected = expectedSequence[t];

                    // Calculate metrics for classification
                    for (int j = 0; j < expected.Count; j++)
                    {
                        if (j < output.Count)
                        {
                            bool predictedClass = output[j] >= threshold;
                            bool actualClass = expected[j] >= threshold;

                            // Update confusion matrix
                            if (actualClass && predictedClass) truePositives++;
                            if (!actualClass && !predictedClass) trueNegatives++;
                            if (!actualClass && predictedClass) falsePositives++;
                            if (actualClass && !predictedClass) falseNegatives++;

                            if (predictedClass == actualClass)
                                correctPredictions++;

                            // Calculate error
                            totalError += Math.Pow(expected[j] - output[j], 2);

                            totalSamples++;
                        }
                    }
                }
            }

            // Calculate final metrics
            double accuracy = totalSamples > 0 ? (double)correctPredictions / totalSamples : 0;
            double precision = (truePositives + falsePositives) > 0 ? (double)truePositives / (truePositives + falsePositives) : 0;
            double recall = (truePositives + falseNegatives) > 0 ? (double)truePositives / (truePositives + falseNegatives) : 0;
            double f1Score = (precision + recall) > 0 ? 2 * precision * recall / (precision + recall) : 0;
            double specificity = (trueNegatives + falsePositives) > 0 ? (double)trueNegatives / (trueNegatives + falsePositives) : 0;
            double mse = totalSamples > 0 ? totalError / totalSamples : 0;

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
    }
}
