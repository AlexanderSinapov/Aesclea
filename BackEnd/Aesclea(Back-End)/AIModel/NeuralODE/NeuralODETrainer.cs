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
    /// Training pipeline for Neural ODE LLM with multiple loss components:
    /// L = L_LM + λ₁·L_uncert + λ₂·C
    /// </summary>
    public class NeuralODETrainer
    {
        public NeuralODELLM Model { get; set; }
        
        // Hyperparameters
        public double LearningRate { get; set; } = 0.001;
        public double Lambda1 { get; set; } = 0.01;  // Uncertainty regularization weight
        public double Lambda2 { get; set; } = 0.001; // Compute cost weight
        public double MaxComputeCost { get; set; } = 1.0;
        
        // Training history
        public List<TrainingMetrics> History { get; private set; } = new List<TrainingMetrics>();
        
        // Adjoint method for memory-efficient gradient computation
        public bool UseAdjointMethod { get; set; } = true;

        public NeuralODETrainer(NeuralODELLM model)
        {
            Model = model;
        }

        /// <summary>
        /// Language modeling loss: L_LM = -Σ log p(y_i | y_{&lt;i})
        /// </summary>
        public double ComputeLanguageModelingLoss(int[] tokens, int[] targets)
        {
            var (logits, _, _) = Model.Forward(tokens);
            
            double loss = 0.0;
            for (int i = 0; i < Math.Min(targets.Length, logits.Length); i++)
            {
                // Cross-entropy loss
                double[] probs = Softmax(logits);
                if (targets[i] >= 0 && targets[i] < probs.Length)
                {
                    loss -= Math.Log(probs[targets[i]] + 1e-10);
                }
            }
            
            return loss / targets.Length;
        }

        /// <summary>
        /// Uncertainty regularization: L_uncert = KL(N(μ,σ²) || N(0,1))
        /// </summary>
        public double ComputeUncertaintyLoss(ContinuousDepthState state)
        {
            // Treat first half of uncertainty as μ, second half as σ
            int halfDim = state.Uncertainty.Length / 2;
            double[] mu = state.Uncertainty.Take(halfDim).ToArray();
            double[] sigma = state.Uncertainty.Skip(halfDim).Take(halfDim).ToArray();
            
            return Model.UncertaintyModule.ComputeKLDivergence(mu, sigma);
        }

        /// <summary>
        /// Compute cost: C = ∫c(t)dt
        /// </summary>
        public double ComputeCostLoss(double computeCost)
        {
            return computeCost;
        }

        /// <summary>
        /// Total loss: L = L_LM + λ₁·L_uncert + λ₂·C
        /// </summary>
        public (double totalLoss, TrainingMetrics metrics) ComputeTotalLoss(int[] tokens, int[] targets)
        {
            var (logits, computeCost, uncertainty) = Model.Forward(tokens);
            
            // Language modeling loss
            double lm_loss = 0.0;
            double[] probs = Softmax(logits);
            for (int i = 0; i < Math.Min(targets.Length, probs.Length); i++)
            {
                if (targets[i] >= 0 && targets[i] < probs.Length)
                {
                    lm_loss -= Math.Log(probs[targets[i]] + 1e-10);
                }
            }
            lm_loss /= targets.Length;
            
            // Uncertainty loss (simplified - using average uncertainty)
            double uncert_loss = uncertainty * uncertainty; // Penalize high uncertainty
            
            // Compute cost
            double cost_loss = computeCost;
            
            // Total loss
            double total = lm_loss + Lambda1 * uncert_loss + Lambda2 * cost_loss;
            
            var metrics = new TrainingMetrics
            {
                TotalLoss = total,
                LanguageModelingLoss = lm_loss,
                UncertaintyLoss = uncert_loss,
                ComputeCost = cost_loss,
                Accuracy = ComputeAccuracy(logits, targets),
                AverageUncertainty = uncertainty
            };
            
            return (total, metrics);
        }

        /// <summary>
        /// Train on a batch of examples
        /// </summary>
        public TrainingMetrics TrainBatch(List<(int[] tokens, int[] targets)> batch)
        {
            double totalLoss = 0.0;
            double totalLM = 0.0;
            double totalUncert = 0.0;
            double totalCost = 0.0;
            double totalAccuracy = 0.0;
            double totalUncertainty = 0.0;
            
            foreach (var (tokens, targets) in batch)
            {
                var (loss, metrics) = ComputeTotalLoss(tokens, targets);
                
                totalLoss += metrics.TotalLoss;
                totalLM += metrics.LanguageModelingLoss;
                totalUncert += metrics.UncertaintyLoss;
                totalCost += metrics.ComputeCost;
                totalAccuracy += metrics.Accuracy;
                totalUncertainty += metrics.AverageUncertainty;
                
                // Gradient descent (simplified - in practice would use Adam or similar)
                // This is placeholder for actual backpropagation through the adjoint method
                UpdateParametersSimplified(tokens, targets, loss);
            }
            
            int batchSize = batch.Count;
            var avgMetrics = new TrainingMetrics
            {
                TotalLoss = totalLoss / batchSize,
                LanguageModelingLoss = totalLM / batchSize,
                UncertaintyLoss = totalUncert / batchSize,
                ComputeCost = totalCost / batchSize,
                Accuracy = totalAccuracy / batchSize,
                AverageUncertainty = totalUncertainty / batchSize
            };
            
            History.Add(avgMetrics);
            return avgMetrics;
        }

        /// <summary>
        /// Adjoint gradient computation: dL/dθ = ∫ a(t)ᵀ (∂f/∂θ) dt
        /// This is the memory-efficient way to compute gradients through ODEs
        /// </summary>
        public double[] ComputeAdjointGradients(int[] tokens, int[] targets)
        {
            // This would implement the full adjoint method from the Neural ODE paper
            // For now, we use numerical gradients as a placeholder
            
            var (baseLoss, _) = ComputeTotalLoss(tokens, targets);
            
            // Get all parameters (this is simplified - would need proper parameter management)
            int numParams = 100; // Placeholder
            double[] gradients = new double[numParams];
            double epsilon = 1e-5;
            
            // Numerical gradient approximation (placeholder for adjoint method)
            for (int i = 0; i < Math.Min(10, numParams); i++)
            {
                // Perturb parameter
                // double originalParam = ...
                // Compute perturbed loss
                // gradients[i] = (perturbedLoss - baseLoss) / epsilon;
                gradients[i] = 0.0; // Placeholder
            }
            
            return gradients;
        }

        /// <summary>
        /// Simplified parameter update (placeholder for full optimizer)
        /// </summary>
        private void UpdateParametersSimplified(int[] tokens, int[] targets, double loss)
        {
            // In a full implementation, this would:
            // 1. Compute gradients using adjoint method
            // 2. Apply Adam/SGD optimizer
            // 3. Update all model parameters
            
            // For now, this is a placeholder showing the structure
            // Real implementation would compute gradients and update:
            // - Model.W_h, Model.W_e, Model.b_h, Model.b_e
            // - Model.W_output, Model.b_output
            // - Model.UncertaintyModule parameters
            // - Model.Attention kernel parameters (if learnable)
        }

        /// <summary>
        /// Train for multiple epochs
        /// </summary>
        public void Train(List<(int[] tokens, int[] targets)> dataset, int epochs, int batchSize)
        {
            Console.WriteLine($"Starting training for {epochs} epochs...");
            
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                // Shuffle dataset
                var shuffled = dataset.OrderBy(x => Guid.NewGuid()).ToList();
                
                // Create batches
                var batches = new List<List<(int[] tokens, int[] targets)>>();
                for (int i = 0; i < shuffled.Count; i += batchSize)
                {
                    batches.Add(shuffled.Skip(i).Take(batchSize).ToList());
                }
                
                // Train on batches
                double epochLoss = 0.0;
                double epochAccuracy = 0.0;
                
                for (int b = 0; b < batches.Count; b++)
                {
                    var metrics = TrainBatch(batches[b]);
                    epochLoss += metrics.TotalLoss;
                    epochAccuracy += metrics.Accuracy;
                    
                    if (b % 10 == 0)
                    {
                        Console.WriteLine($"Epoch {epoch + 1}/{epochs}, Batch {b + 1}/{batches.Count}: " +
                                        $"Loss={metrics.TotalLoss:F4}, Acc={metrics.Accuracy:F4}, " +
                                        $"Uncert={metrics.AverageUncertainty:F4}, Cost={metrics.ComputeCost:F4}");
                    }
                }
                
                epochLoss /= batches.Count;
                epochAccuracy /= batches.Count;
                
                Console.WriteLine($"Epoch {epoch + 1} completed: Loss={epochLoss:F4}, Accuracy={epochAccuracy:F4}");
                
                // Check compute cost constraint
                double avgCost = History.TakeLast(batches.Count).Average(m => m.ComputeCost);
                if (avgCost > MaxComputeCost)
                {
                    Console.WriteLine($"Warning: Average compute cost {avgCost:F4} exceeds max {MaxComputeCost:F4}");
                }
            }
            
            Console.WriteLine("Training completed!");
        }

        /// <summary>
        /// Validate model on test set
        /// </summary>
        public TrainingMetrics Validate(List<(int[] tokens, int[] targets)> testSet)
        {
            double totalLoss = 0.0;
            double totalLM = 0.0;
            double totalUncert = 0.0;
            double totalCost = 0.0;
            double totalAccuracy = 0.0;
            double totalUncertainty = 0.0;
            
            foreach (var (tokens, targets) in testSet)
            {
                var (loss, metrics) = ComputeTotalLoss(tokens, targets);
                
                totalLoss += metrics.TotalLoss;
                totalLM += metrics.LanguageModelingLoss;
                totalUncert += metrics.UncertaintyLoss;
                totalCost += metrics.ComputeCost;
                totalAccuracy += metrics.Accuracy;
                totalUncertainty += metrics.AverageUncertainty;
            }
            
            return new TrainingMetrics
            {
                TotalLoss = totalLoss / testSet.Count,
                LanguageModelingLoss = totalLM / testSet.Count,
                UncertaintyLoss = totalUncert / testSet.Count,
                ComputeCost = totalCost / testSet.Count,
                Accuracy = totalAccuracy / testSet.Count,
                AverageUncertainty = totalUncertainty / testSet.Count
            };
        }

        private double[] Softmax(double[] logits)
        {
            double max = logits.Max();
            double[] exp = logits.Select(x => Math.Exp(x - max)).ToArray();
            double sum = exp.Sum();
            return exp.Select(x => x / sum).ToArray();
        }

        private double ComputeAccuracy(double[] logits, int[] targets)
        {
            int predicted = logits.ToList().IndexOf(logits.Max());
            int correct = 0;
            
            foreach (var target in targets)
            {
                if (predicted == target)
                    correct++;
            }
            
            return (double)correct / targets.Length;
        }
    }

    /// <summary>
    /// Training metrics tracked during training
    /// </summary>
    public class TrainingMetrics
    {
        public double TotalLoss { get; set; }
        public double LanguageModelingLoss { get; set; }
        public double UncertaintyLoss { get; set; }
        public double ComputeCost { get; set; }
        public double Accuracy { get; set; }
        public double AverageUncertainty { get; set; }
        
        public override string ToString()
        {
            return $"Loss={TotalLoss:F4} (LM={LanguageModelingLoss:F4}, " +
                   $"Uncert={UncertaintyLoss:F4}, Cost={ComputeCost:F4}), " +
                   $"Acc={Accuracy:F4}, AvgUncert={AverageUncertainty:F4}";
        }
    }
}
