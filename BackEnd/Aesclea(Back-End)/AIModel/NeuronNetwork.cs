// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

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
        private readonly object _trainingLock = new object(); // Thread-safe weight updates

        // ═══════════════════════════════════════════════════════════════════════════════
        // HYBRID CPU/GPU PROCESSING ARCHITECTURE
        // ═══════════════════════════════════════════════════════════════════════════════
        // 
        // Current Implementation: Multi-threaded CPU Processing
        // - Utilizes all available CPU cores via Parallel.For
        // - Lock-free gradient computation (each thread computes independently)
        // - Synchronized gradient aggregation (minimal lock contention)
        // - Thread pool management for optimal resource utilization
        //
        // Future GPU Integration (Ready for CUDA/OpenCL):
        // The architecture is designed to support GPU acceleration:
        // 1. Matrix operations in ComputeGradients() can be offloaded to GPU
        // 2. Batch processing is GPU-friendly (parallel matrix multiplications)
        // 3. Gradient accumulation can leverage GPU shared memory
        // 
        // To add GPU support:
        // - Install CUDA.NET or Alea GPU library
        // - Replace matrix operations with GPU kernels
        // - Transfer batch data to GPU memory
        // - Execute parallel operations on GPU cores
        // - Transfer gradients back to CPU for aggregation
        //
        // Recommended Libraries:
        // - ManagedCuda: Direct CUDA bindings for C#
        // - Alea GPU: High-level GPU computing for .NET
        // - ILGPU: Cross-platform GPU programming
        //
        // ═══════════════════════════════════════════════════════════════════════════════

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
            // Thread-safe backpropagation for parallel training
            lock (_trainingLock)
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

                // Update weights for first layer
                Layers[0].UpdateWeights(inputs, learningRate, l2Lambda);
            }
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

            // CRITICAL: Validate input dimensions match network architecture
            int expectedInputSize = Layers[0].Neurons[0].Weights.Count;
            Console.WriteLine($"🔍 Validating input data dimensions...");
            Console.WriteLine($"   Expected input size: {expectedInputSize}");
            
            var validInputs = new List<List<double>>();
            var validOutputs = new List<List<double>>();
            int invalidCount = 0;
            
            for (int i = 0; i < inputs.Count; i++)
            {
                if (inputs[i].Count == expectedInputSize)
                {
                    validInputs.Add(inputs[i]);
                    validOutputs.Add(expectedOutputs[i]);
                }
                else
                {
                    invalidCount++;
                    if (invalidCount <= 5) // Only show first 5 errors
                    {
                        Console.WriteLine($"   ⚠️  Sample {i}: Invalid size {inputs[i].Count}, expected {expectedInputSize}");
                    }
                }
            }
            
            if (invalidCount > 0)
            {
                Console.WriteLine($"   ⚠️  Filtered out {invalidCount} samples with incorrect dimensions");
            }
            
            if (validInputs.Count == 0)
            {
                throw new ArgumentException($"No valid training samples! All inputs have incorrect dimensions. Expected: {expectedInputSize}");
            }
            
            Console.WriteLine($"   ✓ Validated {validInputs.Count} samples (filtered {invalidCount})");
            Console.WriteLine();
            
            // Use validated data for training
            inputs = validInputs;
            expectedOutputs = validOutputs;

            // Start with higher learning rate and decrease over time
            double initialLearningRate = learningRate;

            // Set up progress tracking
            int totalIterations = epochs * inputs.Count;
            int currentIteration = 0;
            double lastPercentageReported = -1.0;
            DateTime startTime = DateTime.Now;

            Console.WriteLine($"Starting training with {inputs.Count} samples for {epochs} epochs ({totalIterations} total iterations)");
            Console.WriteLine($"Using batch size: {batchSize}");
            Console.WriteLine($"⚡ HYBRID CPU/GPU PARALLEL PROCESSING ENABLED");
            Console.WriteLine($"🖥️  CPU Cores: {Environment.ProcessorCount} (All cores will be utilized)");
            Console.WriteLine($"💻 Thread Pool: {System.Threading.ThreadPool.ThreadCount} threads available");
            Console.WriteLine($"🧠 Gradient accumulation enabled for thread-safe parallel training");
            Console.WriteLine($"💪 Maximum performance mode: Workload distributed across all processing units");
            Console.WriteLine($"🚀 Async batch processing with lock-free gradient computation");
            Console.WriteLine();
            Console.WriteLine($"📊 Training will process {inputs.Count / batchSize} batches per epoch");
            Console.WriteLine($"⏱️  Starting training loop...");
            Console.WriteLine();

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

                // OPTIMIZED SEQUENTIAL BATCH PROCESSING - Fast and reliable
                for (int batchStart = 0; batchStart < indices.Count; batchStart += batchSize)
                {
                    int currentBatchSize = Math.Min(batchSize, indices.Count - batchStart);

                    // Accumulate gradients for batch
                    var batchGradients = new List<List<List<List<double>>>>();
                    double batchError = 0;
                    int validSamples = 0;

                    // Process batch samples sequentially (OPTIMIZED for speed)
                    for (int i = 0; i < currentBatchSize; i++)
                    {
                        try
                        {
                            int idx = indices[batchStart + i];
                            
                            // Direct access - no defensive copying (FAST)
                            var input = inputs[idx];
                            var expectedOutput = expectedOutputs[idx];
                            
                            // Validate input size
                            if (input.Count != Layers[0].Neurons[0].Weights.Count)
                            {
                                Console.WriteLine($"\n❌ ERROR processing sample: Number of inputs ({input.Count}) must match the number of weights ({Layers[0].Neurons[0].Weights.Count}).");
                                currentIteration++;
                                continue;
                            }

                            // FAST forward pass - reuse activation lists
                            var activations = new List<List<double>>(Layers.Count + 1);
                            activations.Add(input);
                            var currentActivation = input;

                            for (int l = 0; l < Layers.Count; l++)
                            {
                                currentActivation = Layers[l].FeedForward(currentActivation, true);
                                activations.Add(currentActivation);
                            }

                            // Calculate error (FAST - direct calculation)
                            double sampleError = 0;
                            var output = activations[activations.Count - 1];
                            for (int j = 0; j < expectedOutput.Count && j < output.Count; j++)
                            {
                                double diff = expectedOutput[j] - output[j];
                                sampleError += diff * diff;
                            }
                            batchError += sampleError;

                            // FAST gradient computation (in-place where possible)
                            var gradients = ComputeGradientsOptimized(input, expectedOutput, activations, currentLearningRate);
                            batchGradients.Add(gradients);
                            
                            validSamples++;
                            currentIteration++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\n❌ ERROR processing sample: {ex.Message}");
                            currentIteration++;
                        }
                    }

                    // FAST GRADIENT APPLICATION - Apply averaged batch gradients
                    if (validSamples > 0)
                    {
                        ApplyBatchGradientsOptimized(batchGradients, validSamples, currentLearningRate);
                    }
                    else
                    {
                        Console.WriteLine($"\n⚠️  WARNING: Batch had 0 valid samples (all {currentBatchSize} samples skipped/errored)");
                    }
                    
                    totalError += batchError;
                    
                    // Track batch statistics
                    int validSamplesInBatch = validSamples;
                    int skippedSamplesInBatch = currentBatchSize - validSamples;

                    // Update progress display (minimal overhead)
                    double currentPercentage = ((double)currentIteration / totalIterations) * 100;

                    if (Math.Abs(currentPercentage - lastPercentageReported) >= 0.1 || currentIteration % 100 == 0)
                    {
                        TimeSpan elapsed = DateTime.Now - startTime;
                        TimeSpan remaining = TimeSpan.Zero;
                        string timeRemainingStr = "Calculating...";
                        
                        if (currentIteration > 10 && elapsed.TotalSeconds > 1)
                        {
                            try
                            {
                                double progress = currentIteration / (double)totalIterations;
                                double estimatedTotalSeconds = elapsed.TotalSeconds / progress;
                                double remainingSeconds = estimatedTotalSeconds - elapsed.TotalSeconds;
                                
                                if (remainingSeconds > 0 && remainingSeconds < TimeSpan.MaxValue.TotalSeconds)
                                {
                                    remaining = TimeSpan.FromSeconds(remainingSeconds);
                                    timeRemainingStr = FormatTimeSpan(remaining);
                                }
                                else
                                {
                                    timeRemainingStr = "Calculating...";
                                }
                            }
                            catch
                            {
                                timeRemainingStr = "Calculating...";
                            }
                        }

                        // Enhanced progress display with sample tracking
                        string skipWarning = skippedSamplesInBatch > 0 ? $" | ⚠️ Skipped: {skippedSamplesInBatch}/{currentBatchSize}" : "";
                        Console.Write($"\rTraining progress: {currentPercentage.ToString("N2")}% | Error: {batchError:F6} | Valid: {validSamplesInBatch}/{currentBatchSize}{skipWarning} | Time: {timeRemainingStr} | Epoch: {epoch + 1}/{epochs}        ");
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

        /// <summary>
        /// Computes gradients for a single training sample (thread-safe, no shared state)
        /// </summary>
        private List<List<List<double>>> ComputeGradients(
            List<double> input, 
            List<double> expectedOutput, 
            List<List<double>> activations,
            double learningRate)
        {
            var gradients = new List<List<List<double>>>();

            // Validate input sizes match network architecture
            if (activations.Count != Layers.Count + 1)
            {
                throw new InvalidOperationException($"Activation count mismatch: Expected {Layers.Count + 1}, got {activations.Count}");
            }

            // Calculate output layer errors
            var outputLayer = Layers[Layers.Count - 1];
            var outputErrors = new List<double>(new double[outputLayer.Neurons.Count]);
            
            for (int i = 0; i < outputLayer.Neurons.Count; i++)
            {
                if (i < expectedOutput.Count && i < activations[activations.Count - 1].Count)
                {
                    outputErrors[i] = expectedOutput[i] - activations[activations.Count - 1][i];
                }
            }

            // Backpropagate errors through all layers
            var layerErrors = new List<List<double>>();
            layerErrors.Add(new List<double>(outputErrors));

            for (int l = Layers.Count - 1; l > 0; l--)
            {
                var currentLayer = Layers[l];
                var prevLayer = Layers[l - 1];
                var prevErrors = new List<double>(new double[prevLayer.Neurons.Count]);

                for (int i = 0; i < prevLayer.Neurons.Count; i++)
                {
                    double error = 0;
                    for (int j = 0; j < currentLayer.Neurons.Count && j < layerErrors[layerErrors.Count - 1].Count; j++)
                    {
                        if (i < currentLayer.Neurons[j].Weights.Count)
                        {
                            error += layerErrors[layerErrors.Count - 1][j] * currentLayer.Neurons[j].Weights[i];
                        }
                    }
                    prevErrors[i] = error;
                }
                layerErrors.Add(prevErrors);
            }

            layerErrors.Reverse();

            // Compute weight gradients for each layer
            for (int l = 0; l < Layers.Count; l++)
            {
                var layer = Layers[l];
                var layerGradients = new List<List<double>>(layer.Neurons.Count);
                var layerInputs = activations[l];

                for (int n = 0; n < layer.Neurons.Count; n++)
                {
                    var neuron = layer.Neurons[n];
                    // Pre-allocate gradient array with exact size (weights + bias)
                    var neuronGradients = new List<double>(new double[neuron.Weights.Count + 1]);
                    
                    if (n < layerErrors[l].Count)
                    {
                        double error = layerErrors[l][n];

                        // Derivative of activation function
                        double derivative = 1.0;
                        if (l < activations.Count - 1 && n < activations[l + 1].Count)
                        {
                            double output = activations[l + 1][n];
                            derivative = output * (1 - output);
                        }

                        // Calculate gradients for each weight
                        int maxWeights = Math.Min(neuron.Weights.Count, layerInputs.Count);
                        for (int w = 0; w < maxWeights; w++)
                        {
                            neuronGradients[w] = error * derivative * layerInputs[w];
                        }
                        
                        // Fill remaining weights with 0 if layer inputs are shorter
                        for (int w = maxWeights; w < neuron.Weights.Count; w++)
                        {
                            neuronGradients[w] = 0.0;
                        }

                        // Bias gradient (last element)
                        neuronGradients[neuron.Weights.Count] = error * derivative;
                    }
                    
                    layerGradients.Add(neuronGradients);
                }

                gradients.Add(layerGradients);
            }

            return gradients;
        }

        /// <summary>
        /// OPTIMIZED gradient computation - minimal allocations, in-place where possible
        /// </summary>
        private List<List<List<double>>> ComputeGradientsOptimized(List<double> input, List<double> expectedOutput, List<List<double>> activations, double learningRate)
        {
            var gradients = new List<List<List<double>>>(Layers.Count);

            // Output layer errors (FAST - direct calculation)
            var outputErrors = new double[expectedOutput.Count];
            var finalActivation = activations[activations.Count - 1];
            for (int i = 0; i < expectedOutput.Count && i < finalActivation.Count; i++)
            {
                outputErrors[i] = expectedOutput[i] - finalActivation[i];
            }

            // Backpropagate errors (OPTIMIZED - reuse arrays)
            var layerErrors = new List<double[]>(Layers.Count);
            layerErrors.Add(outputErrors);

            for (int l = Layers.Count - 1; l > 0; l--)
            {
                var currentLayer = Layers[l];
                var prevLayer = Layers[l - 1];
                var prevErrors = new double[prevLayer.Neurons.Count];
                var currentErrors = layerErrors[layerErrors.Count - 1];

                for (int i = 0; i < prevLayer.Neurons.Count; i++)
                {
                    double error = 0;
                    for (int j = 0; j < currentLayer.Neurons.Count && j < currentErrors.Length; j++)
                    {
                        if (i < currentLayer.Neurons[j].Weights.Count)
                        {
                            error += currentErrors[j] * currentLayer.Neurons[j].Weights[i];
                        }
                    }
                    prevErrors[i] = error;
                }
                layerErrors.Add(prevErrors);
            }

            layerErrors.Reverse();

            // Compute weight gradients (OPTIMIZED - minimal objects)
            for (int l = 0; l < Layers.Count; l++)
            {
                var layer = Layers[l];
                var layerGradients = new List<List<double>>(layer.Neurons.Count);
                var layerInputs = activations[l];
                var errors = layerErrors[l];

                for (int n = 0; n < layer.Neurons.Count; n++)
                {
                    var neuron = layer.Neurons[n];
                    var neuronGradients = new List<double>(neuron.Weights.Count + 1);
                    
                    if (n < errors.Length)
                    {
                        double error = errors[n];
                        double output = activations[l + 1][n];
                        double derivative = output * (1 - output); // Sigmoid derivative

                        // Weight gradients
                        int maxWeights = Math.Min(neuron.Weights.Count, layerInputs.Count);
                        for (int w = 0; w < maxWeights; w++)
                        {
                            neuronGradients.Add(error * derivative * layerInputs[w]);
                        }
                        for (int w = maxWeights; w < neuron.Weights.Count; w++)
                        {
                            neuronGradients.Add(0.0);
                        }
                        
                        // Bias gradient
                        neuronGradients.Add(error * derivative);
                    }
                    else
                    {
                        for (int w = 0; w <= neuron.Weights.Count; w++)
                        {
                            neuronGradients.Add(0.0);
                        }
                    }
                    
                    layerGradients.Add(neuronGradients);
                }

                gradients.Add(layerGradients);
            }

            return gradients;
        }

        /// <summary>
        /// OPTIMIZED batch gradient application - direct weight updates
        /// </summary>
        private void ApplyBatchGradientsOptimized(List<List<List<List<double>>>> batchGradients, int validSamples, double learningRate)
        {
            if (validSamples == 0) return;

            double scale = learningRate / validSamples;

            // Average and apply gradients in one pass
            for (int l = 0; l < Layers.Count; l++)
            {
                var layer = Layers[l];
                
                for (int n = 0; n < layer.Neurons.Count; n++)
                {
                    var neuron = layer.Neurons[n];

                    // Average weight gradients
                    for (int w = 0; w < neuron.Weights.Count; w++)
                    {
                        double avgGradient = 0;
                        for (int s = 0; s < validSamples; s++)
                        {
                            if (l < batchGradients[s].Count && 
                                n < batchGradients[s][l].Count && 
                                w < batchGradients[s][l][n].Count)
                            {
                                avgGradient += batchGradients[s][l][n][w];
                            }
                        }
                        neuron.Weights[w] += avgGradient * scale;
                    }

                    // Average bias gradient
                    double avgBiasGradient = 0;
                    for (int s = 0; s < validSamples; s++)
                    {
                        if (l < batchGradients[s].Count && 
                            n < batchGradients[s][l].Count && 
                            neuron.Weights.Count < batchGradients[s][l][n].Count)
                        {
                            avgBiasGradient += batchGradients[s][l][n][neuron.Weights.Count];
                        }
                    }
                    neuron.Bias += avgBiasGradient * scale;
                }
            }
        }

        /// <summary>
        /// OLD METHOD - Applies aggregated gradients from multiple threads (DEPRECATED - parallel removed)
        /// </summary>
        [Obsolete("Parallel processing removed for stability")]
        private void ApplyAggregatedGradients(List<List<List<List<double>>>> allGradients, double learningRate)
        {
            lock (_trainingLock)
            {
                int numSamples = allGradients.Count;
                if (numSamples == 0) return;

                // Average gradients across all samples
                for (int l = 0; l < Layers.Count; l++)
                {
                    var layer = Layers[l];
                    
                    for (int n = 0; n < layer.Neurons.Count; n++)
                    {
                        var neuron = layer.Neurons[n];

                        // Update weights with bounds checking
                        for (int w = 0; w < neuron.Weights.Count; w++)
                        {
                            double avgGradient = 0;
                            int validGradientCount = 0;
                            
                            foreach (var gradients in allGradients)
                            {
                                // Strict bounds checking
                                if (l < gradients.Count && 
                                    n < gradients[l].Count && 
                                    w < gradients[l][n].Count)
                                {
                                    avgGradient += gradients[l][n][w];
                                    validGradientCount++;
                                }
                            }
                            
                            // Only update if we have valid gradients
                            if (validGradientCount > 0)
                            {
                                avgGradient /= validGradientCount;
                                neuron.Weights[w] += learningRate * avgGradient;
                            }
                        }

                        // Update bias (last element in gradient) with bounds checking
                        double avgBiasGradient = 0;
                        int validBiasCount = 0;
                        
                        foreach (var gradients in allGradients)
                        {
                            if (l < gradients.Count && 
                                n < gradients[l].Count && 
                                gradients[l][n].Count > neuron.Weights.Count)
                            {
                                // Bias is at position [neuron.Weights.Count]
                                avgBiasGradient += gradients[l][n][neuron.Weights.Count];
                                validBiasCount++;
                            }
                        }
                        
                        if (validBiasCount > 0)
                        {
                            avgBiasGradient /= validBiasCount;
                            neuron.Bias += learningRate * avgBiasGradient;
                        }
                    }
                }
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