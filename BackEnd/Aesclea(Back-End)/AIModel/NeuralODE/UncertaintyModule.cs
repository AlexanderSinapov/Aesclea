// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov
// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;

namespace Aesclea_Back_End_.AIModel.NeuralODE
{
    /// <summary>
    /// Uncertainty quantification with Gaussian distribution
    /// u(t) ~ N(μ(t), σ²(t))
    /// dμ/dt = g_μ(h, e)
    /// dσ/dt = g_σ(h, e)
    /// </summary>
    public class UncertaintyModule
    {
        public int Dimension { get; set; }
        
        // Learnable parameters for uncertainty dynamics
        private double[,] W_mu_h;      // Weight matrix for hidden state -> mean
        private double[,] W_mu_e;      // Weight matrix for embedding state -> mean
        private double[] b_mu;         // Bias for mean
        
        private double[,] W_sigma_h;   // Weight matrix for hidden state -> std
        private double[,] W_sigma_e;   // Weight matrix for embedding state -> std
        private double[] b_sigma;      // Bias for std
        
        private Random random = new Random();

        public UncertaintyModule(int dimension, int hiddenDim, int embeddingDim, int? seed = null)
        {
            Dimension = dimension;
            
            if (seed.HasValue)
                random = new Random(seed.Value);
            
            // Initialize parameters
            W_mu_h = InitializeWeights(dimension, hiddenDim);
            W_mu_e = InitializeWeights(dimension, embeddingDim);
            b_mu = new double[dimension];
            
            W_sigma_h = InitializeWeights(dimension, hiddenDim);
            W_sigma_e = InitializeWeights(dimension, embeddingDim);
            b_sigma = new double[dimension];
            
            // Initialize biases for sigma to small positive values
            for (int i = 0; i < dimension; i++)
            {
                b_sigma[i] = 0.1;
            }
        }

        /// <summary>
        /// Compute uncertainty dynamics: dμ/dt and dσ/dt
        /// </summary>
        public (double[] dmu, double[] dsigma) ComputeDynamics(double[] hidden, double[] embedding, 
            double[] currentMu, double[] currentSigma)
        {
            double[] dmu = new double[Dimension];
            double[] dsigma = new double[Dimension];
            
            // dμ/dt = W_h·h + W_e·e + b_mu - λ·μ (decay term)
            for (int i = 0; i < Dimension; i++)
            {
                dmu[i] = b_mu[i];
                
                // Add hidden state contribution
                for (int j = 0; j < hidden.Length; j++)
                {
                    dmu[i] += W_mu_h[i, j] * hidden[j];
                }
                
                // Add embedding state contribution
                for (int j = 0; j < embedding.Length; j++)
                {
                    dmu[i] += W_mu_e[i, j] * embedding[j];
                }
                
                // Decay term to prevent unbounded growth
                dmu[i] -= 0.1 * currentMu[i];
            }
            
            // dσ/dt = softplus(W_h·h + W_e·e + b_sigma) - λ·σ
            for (int i = 0; i < Dimension; i++)
            {
                double logit = b_sigma[i];
                
                for (int j = 0; j < hidden.Length; j++)
                {
                    logit += W_sigma_h[i, j] * hidden[j];
                }
                
                for (int j = 0; j < embedding.Length; j++)
                {
                    logit += W_sigma_e[i, j] * embedding[j];
                }
                
                // Softplus ensures positive variance, decay prevents explosion
                dsigma[i] = Softplus(logit) - 0.1 * currentSigma[i];
            }
            
            return (dmu, dsigma);
        }

        /// <summary>
        /// Sample from uncertainty distribution u(t) ~ N(μ(t), σ²(t))
        /// </summary>
        public double[] Sample(double[] mu, double[] sigma)
        {
            double[] sample = new double[Dimension];
            
            for (int i = 0; i < Dimension; i++)
            {
                double z = SampleGaussian(0, 1);
                sample[i] = mu[i] + sigma[i] * z;
            }
            
            return sample;
        }

        /// <summary>
        /// Compute KL divergence from standard Gaussian: KL(N(μ,σ²) || N(0,1))
        /// KL = 0.5 * Σ(μ² + σ² - log(σ²) - 1)
        /// </summary>
        public double ComputeKLDivergence(double[] mu, double[] sigma)
        {
            double kl = 0.0;
            
            for (int i = 0; i < Dimension; i++)
            {
                double mu_sq = mu[i] * mu[i];
                double sigma_sq = sigma[i] * sigma[i];
                double log_sigma_sq = Math.Log(sigma_sq + 1e-8);
                
                kl += mu_sq + sigma_sq - log_sigma_sq - 1.0;
            }
            
            return 0.5 * kl;
        }

        /// <summary>
        /// Get epistemic uncertainty (model uncertainty)
        /// </summary>
        public double GetEpistemicUncertainty(double[] sigma)
        {
            double sum = 0.0;
            foreach (var s in sigma)
                sum += s * s; // Variance
            return Math.Sqrt(sum / Dimension);
        }

        /// <summary>
        /// Get aleatoric uncertainty (data uncertainty)
        /// Estimated from variance of predictions
        /// </summary>
        public double GetAleatoricUncertainty(double[] predictions, double[] mu)
        {
            double sum = 0.0;
            for (int i = 0; i < predictions.Length; i++)
            {
                double diff = predictions[i] - mu[i];
                sum += diff * diff;
            }
            return Math.Sqrt(sum / predictions.Length);
        }

        /// <summary>
        /// Combine uncertainties for total predictive uncertainty
        /// </summary>
        public double GetTotalUncertainty(double epistemicUncertainty, double aleatoricUncertainty)
        {
            return Math.Sqrt(epistemicUncertainty * epistemicUncertainty + 
                           aleatoricUncertainty * aleatoricUncertainty);
        }

        private double[,] InitializeWeights(int rows, int cols)
        {
            double[,] weights = new double[rows, cols];
            double scale = Math.Sqrt(2.0 / (rows + cols)); // He initialization
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    weights[i, j] = SampleGaussian(0, scale);
                }
            }
            
            return weights;
        }

        private double Softplus(double x)
        {
            return Math.Log(1.0 + Math.Exp(x));
        }

        private double SampleGaussian(double mean, double stddev)
        {
            // Box-Muller transform
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stddev * randStdNormal;
        }

        /// <summary>
        /// Get all trainable parameters for optimization
        /// </summary>
        public double[] GetParameters()
        {
            int totalParams = W_mu_h.Length + W_mu_e.Length + b_mu.Length +
                            W_sigma_h.Length + W_sigma_e.Length + b_sigma.Length;
            
            double[] parameters = new double[totalParams];
            int idx = 0;
            
            // Flatten all weight matrices and biases
            foreach (var w in W_mu_h) parameters[idx++] = w;
            foreach (var w in W_mu_e) parameters[idx++] = w;
            foreach (var b in b_mu) parameters[idx++] = b;
            foreach (var w in W_sigma_h) parameters[idx++] = w;
            foreach (var w in W_sigma_e) parameters[idx++] = w;
            foreach (var b in b_sigma) parameters[idx++] = b;
            
            return parameters;
        }

        /// <summary>
        /// Set all trainable parameters from flat array
        /// </summary>
        public void SetParameters(double[] parameters)
        {
            int idx = 0;
            
            for (int i = 0; i < W_mu_h.GetLength(0); i++)
                for (int j = 0; j < W_mu_h.GetLength(1); j++)
                    W_mu_h[i, j] = parameters[idx++];
                    
            for (int i = 0; i < W_mu_e.GetLength(0); i++)
                for (int j = 0; j < W_mu_e.GetLength(1); j++)
                    W_mu_e[i, j] = parameters[idx++];
                    
            for (int i = 0; i < b_mu.Length; i++)
                b_mu[i] = parameters[idx++];
                
            for (int i = 0; i < W_sigma_h.GetLength(0); i++)
                for (int j = 0; j < W_sigma_h.GetLength(1); j++)
                    W_sigma_h[i, j] = parameters[idx++];
                    
            for (int i = 0; i < W_sigma_e.GetLength(0); i++)
                for (int j = 0; j < W_sigma_e.GetLength(1); j++)
                    W_sigma_e[i, j] = parameters[idx++];
                    
            for (int i = 0; i < b_sigma.Length; i++)
                b_sigma[i] = parameters[idx++];
        }
    }
}
