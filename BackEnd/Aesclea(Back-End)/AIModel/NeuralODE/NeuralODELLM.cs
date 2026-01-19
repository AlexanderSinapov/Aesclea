// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov
// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Aesclea_Back_End_.AIModel.NeuralODE
{
    /// <summary>
    /// Main Neural ODE Language Model with:
    /// - Continuous-depth processing via Neural ODEs
    /// - Integral attention with multiple kernel options
    /// - Uncertainty quantification
    /// - Adaptive compute control
    /// </summary>
    public class NeuralODELLM
    {
        // Dimensions
        public int HiddenDim { get; set; }
        public int EmbeddingDim { get; set; }
        public int UncertaintyDim { get; set; }
        public int VocabSize { get; set; }
        
        // Core modules
        public IntegralAttention Attention { get; set; }
        public UncertaintyModule UncertaintyModule { get; set; }
        public ComputeController ComputeController { get; set; }
        
        // Learnable parameters for ODE dynamics
        private double[,] W_h;  // Weight matrix for hidden dynamics
        private double[,] W_e;  // Weight matrix for embedding dynamics
        private double[] b_h;   // Bias for hidden
        private double[] b_e;   // Bias for embedding
        
        // Decay parameters
        private double lambda_h = 0.1;
        private double lambda_e = 0.1;
        
        // Output layer parameters
        private double[,] W_output;
        private double[] b_output;
        
        // Embedding layer (simplified - in real LLM this would be much more sophisticated)
        private double[,] embeddingMatrix;
        
        // Training history
        public List<double> TrainingLosses { get; private set; } = new List<double>();
        public List<double> ComputeCosts { get; private set; } = new List<double>();
        
        private Random random = new Random();

        public NeuralODELLM(int vocabSize, int hiddenDim, int embeddingDim, int uncertaintyDim, 
            IAttentionKernel kernel = null, int? seed = null)
        {
            VocabSize = vocabSize;
            HiddenDim = hiddenDim;
            EmbeddingDim = embeddingDim;
            UncertaintyDim = uncertaintyDim;
            
            if (seed.HasValue)
                random = new Random(seed.Value);
            
            // Initialize modules
            Attention = new IntegralAttention(kernel ?? new GaussianKernel(1.0));
            UncertaintyModule = new UncertaintyModule(uncertaintyDim, hiddenDim, embeddingDim, seed);
            ComputeController = new ComputeController();
            
            // Initialize parameters
            InitializeParameters();
        }

        private void InitializeParameters()
        {
            // ODE dynamics parameters (He initialization)
            W_h = InitializeWeights(HiddenDim, HiddenDim);
            W_e = InitializeWeights(EmbeddingDim, HiddenDim);
            b_h = new double[HiddenDim];
            b_e = new double[EmbeddingDim];
            
            // Output layer
            W_output = InitializeWeights(VocabSize, HiddenDim);
            b_output = new double[VocabSize];
            
            // Embedding matrix
            embeddingMatrix = InitializeWeights(VocabSize, EmbeddingDim);
        }

        /// <summary>
        /// Tokenization and embedding: x₀ = E(tokens)
        /// </summary>
        public ContinuousDepthState Embed(int[] tokens)
        {
            // Average token embeddings (simplified - could use more sophisticated methods)
            double[] embedding = new double[EmbeddingDim];
            
            foreach (int token in tokens)
            {
                if (token >= 0 && token < VocabSize)
                {
                    for (int i = 0; i < EmbeddingDim; i++)
                    {
                        embedding[i] += embeddingMatrix[token, i];
                    }
                }
            }
            
            // Average
            for (int i = 0; i < EmbeddingDim; i++)
            {
                embedding[i] /= tokens.Length;
            }
            
            // Initialize state
            var state = new ContinuousDepthState(HiddenDim, EmbeddingDim, UncertaintyDim);
            state.Embedding = embedding;
            
            // Initialize hidden state from embedding
            for (int i = 0; i < Math.Min(HiddenDim, EmbeddingDim); i++)
            {
                state.Hidden[i] = embedding[i];
            }
            
            // Initialize uncertainty with small values
            for (int i = 0; i < UncertaintyDim; i++)
            {
                state.Uncertainty[i] = 0.1;
            }
            
            return state;
        }

        /// <summary>
        /// Core ODE dynamics function: dx/dt = f(x(t), t; θ)
        /// Integrates: attention, uncertainty, and state evolution
        /// </summary>
        public double[] ODEDynamics(double[] stateVec, double t)
        {
            // Unpack state
            var state = ContinuousDepthState.FromVector(stateVec, HiddenDim, EmbeddingDim, UncertaintyDim, t);
            
            // Compute attention (this would integrate over previous states in full implementation)
            double[] attentionOutput = ComputeAttentionForState(state);
            
            // Compute uncertainty dynamics
            var (dmu, dsigma) = UncertaintyModule.ComputeDynamics(
                state.Hidden, state.Embedding, 
                state.Uncertainty, state.Uncertainty); // Simplified: treating uncertainty as both μ and σ
            
            // Compute hidden dynamics: dh/dt = W_h·attention - λ_h·h
            double[] dh = new double[HiddenDim];
            for (int i = 0; i < HiddenDim; i++)
            {
                dh[i] = b_h[i];
                
                // Attention contribution
                for (int j = 0; j < attentionOutput.Length && j < HiddenDim; j++)
                {
                    dh[i] += W_h[i, j] * attentionOutput[j];
                }
                
                // Decay term
                dh[i] -= lambda_h * state.Hidden[i];
            }
            
            // Compute embedding dynamics: de/dt = W_e·attention - λ_e·e
            double[] de = new double[EmbeddingDim];
            for (int i = 0; i < EmbeddingDim; i++)
            {
                de[i] = b_e[i];
                
                for (int j = 0; j < attentionOutput.Length && j < EmbeddingDim; j++)
                {
                    de[i] += W_e[i, j] * attentionOutput[j];
                }
                
                de[i] -= lambda_e * state.Embedding[i];
            }
            
            // Combine derivatives
            double[] derivatives = new double[HiddenDim + EmbeddingDim + UncertaintyDim];
            int idx = 0;
            
            Array.Copy(dh, 0, derivatives, idx, HiddenDim);
            idx += HiddenDim;
            
            Array.Copy(de, 0, derivatives, idx, EmbeddingDim);
            idx += EmbeddingDim;
            
            Array.Copy(dmu, 0, derivatives, idx, UncertaintyDim);
            
            return derivatives;
        }

        /// <summary>
        /// Compute attention for current state (simplified version)
        /// </summary>
        private double[] ComputeAttentionForState(ContinuousDepthState state)
        {
            // In full implementation, this would attend over sequence history
            // Here we use a simplified self-attention
            double[] combined = new double[HiddenDim];
            
            for (int i = 0; i < Math.Min(HiddenDim, state.Hidden.Length); i++)
            {
                combined[i] = state.Hidden[i];
            }
            
            for (int i = 0; i < Math.Min(HiddenDim, state.Embedding.Length); i++)
            {
                combined[i] += 0.5 * state.Embedding[i];
            }
            
            return combined;
        }

        /// <summary>
        /// Forward pass with adaptive compute
        /// </summary>
        public (double[] logits, double computeCost, double uncertainty) Forward(int[] tokens, double T = 1.0, double dt = 0.1)
        {
            // 1. Embedding
            var state = Embed(tokens);
            
            // 2. Solve ODE with adaptive compute
            double t = 0.0;
            double totalComputeCost = 0.0;
            List<double> computeCoefficients = new List<double>();
            
            while (t < T)
            {
                // Compute control coefficient
                double c_t = ComputeController.ComputeCoefficient(state);
                computeCoefficients.Add(c_t);
                totalComputeCost += c_t * dt;
                
                // Modulated ODE step: dx/dt = c(t) * f(x(t), t)
                var stateVec = state.ToVector();
                var dynamics = ODEDynamics(stateVec, t);
                var modulatedDynamics = ComputeController.ModulateDynamics(dynamics, c_t);
                
                // Take ODE step
                var nextStateVec = ODESolver.RK4Step(stateVec, t, Math.Min(dt, T - t), 
                    (x, time) => ComputeController.ModulateDynamics(ODEDynamics(x, time), c_t));
                
                state = ContinuousDepthState.FromVector(nextStateVec, HiddenDim, EmbeddingDim, UncertaintyDim, t + dt);
                t += dt;
            }
            
            // 3. Decode: p(y|tokens) = softmax(W_o·h(T))
            double[] logits = new double[VocabSize];
            for (int i = 0; i < VocabSize; i++)
            {
                logits[i] = b_output[i];
                for (int j = 0; j < HiddenDim; j++)
                {
                    logits[i] += W_output[i, j] * state.Hidden[j];
                }
            }
            
            // Calculate average uncertainty
            double avgUncertainty = state.Uncertainty.Average();
            
            return (logits, totalComputeCost, avgUncertainty);
        }

        /// <summary>
        /// Generate prediction from logits
        /// </summary>
        public int[] Predict(int[] tokens, int numSamples = 1, double temperature = 1.0)
        {
            var (logits, cost, uncertainty) = Forward(tokens);
            
            // Apply temperature
            for (int i = 0; i < logits.Length; i++)
            {
                logits[i] /= temperature;
            }
            
            // Softmax
            double[] probs = Softmax(logits);
            
            // Sample or take argmax
            int[] predictions = new int[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                predictions[i] = temperature > 0.1 ? SampleFromDistribution(probs) : ArgMax(probs);
            }
            
            return predictions;
        }

        /// <summary>
        /// Medical diagnosis specific forward pass with detailed uncertainty
        /// </summary>
        public MedicalPrediction PredictMedical(int[] tokens, double T = 1.0)
        {
            var (logits, cost, uncertainty) = Forward(tokens, T);
            double[] probs = Softmax(logits);
            
            int predictedClass = ArgMax(probs);
            double confidence = probs[predictedClass];
            
            return new MedicalPrediction
            {
                PredictedClass = predictedClass,
                Confidence = confidence,
                Uncertainty = uncertainty,
                ComputeCost = cost,
                ClassProbabilities = probs
            };
        }

        // Helper methods
        private double[,] InitializeWeights(int rows, int cols)
        {
            double[,] weights = new double[rows, cols];
            double scale = Math.Sqrt(2.0 / (rows + cols));
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    weights[i, j] = (random.NextDouble() * 2 - 1) * scale;
                }
            }
            
            return weights;
        }

        private double[] Softmax(double[] logits)
        {
            double max = logits.Max();
            double[] exp = logits.Select(x => Math.Exp(x - max)).ToArray();
            double sum = exp.Sum();
            return exp.Select(x => x / sum).ToArray();
        }

        private int ArgMax(double[] arr)
        {
            int maxIdx = 0;
            double maxVal = arr[0];
            
            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] > maxVal)
                {
                    maxVal = arr[i];
                    maxIdx = i;
                }
            }
            
            return maxIdx;
        }

        private int SampleFromDistribution(double[] probs)
        {
            double r = random.NextDouble();
            double cumsum = 0.0;
            
            for (int i = 0; i < probs.Length; i++)
            {
                cumsum += probs[i];
                if (r <= cumsum)
                    return i;
            }
            
            return probs.Length - 1;
        }
    }

    /// <summary>
    /// Medical prediction result with uncertainty quantification
    /// </summary>
    public class MedicalPrediction
    {
        public int PredictedClass { get; set; }
        public double Confidence { get; set; }
        public double Uncertainty { get; set; }
        public double ComputeCost { get; set; }
        public double[] ClassProbabilities { get; set; }
        
        public string GetInterpretation()
        {
            if (Confidence > 0.9 && Uncertainty < 0.2)
                return "High confidence, low uncertainty - Strong prediction";
            else if (Confidence > 0.7 && Uncertainty < 0.4)
                return "Moderate confidence - Reliable prediction";
            else if (Uncertainty > 0.6)
                return "High uncertainty - Recommend additional tests or expert review";
            else
                return "Low confidence - Prediction may be unreliable";
        }
    }
}
