// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov
// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;
using System.Numerics;

namespace Aesclea_Back_End_.AIModel.NeuralODE
{
    /// <summary>
    /// Kernel functions for integral attention operator
    /// K(t,s) represents the attention between time points t and s
    /// </summary>
    public interface IAttentionKernel
    {
        double Compute(double t, double s);
        string Name { get; }
    }

    /// <summary>
    /// Single-scale Gaussian kernel: K(t,s) = exp(-(t-s)²/2σ²)
    /// </summary>
    public class GaussianKernel : IAttentionKernel
    {
        public double Sigma { get; set; }
        public string Name => $"Gaussian(σ={Sigma:F3})";

        public GaussianKernel(double sigma = 1.0)
        {
            Sigma = sigma;
        }

        public double Compute(double t, double s)
        {
            double diff = t - s;
            return Math.Exp(-(diff * diff) / (2 * Sigma * Sigma));
        }
    }

    /// <summary>
    /// Multi-scale Gaussian mixture kernel: K(t,s) = Σ αₖ·exp(-(t-s)²/2σₖ²)
    /// Captures attention at multiple temporal scales
    /// </summary>
    public class GaussianMixtureKernel : IAttentionKernel
    {
        public double[] Sigmas { get; set; }
        public double[] Alphas { get; set; }
        public string Name => $"GaussianMixture(M={Sigmas.Length})";

        public GaussianMixtureKernel(double[] sigmas, double[] alphas = null)
        {
            if (sigmas.Length == 0)
                throw new ArgumentException("Must have at least one sigma");

            Sigmas = sigmas;
            
            // Default: equal weights that sum to 1
            if (alphas == null)
            {
                Alphas = new double[sigmas.Length];
                for (int i = 0; i < sigmas.Length; i++)
                    Alphas[i] = 1.0 / sigmas.Length;
            }
            else
            {
                if (alphas.Length != sigmas.Length)
                    throw new ArgumentException("Alphas and sigmas must have same length");
                Alphas = alphas;
            }
        }

        public double Compute(double t, double s)
        {
            double result = 0.0;
            double diff = t - s;
            
            for (int k = 0; k < Sigmas.Length; k++)
            {
                double sigma = Sigmas[k];
                result += Alphas[k] * Math.Exp(-(diff * diff) / (2 * sigma * sigma));
            }
            
            return result;
        }
    }

    /// <summary>
    /// Fourier-based kernel using Random Fourier Features
    /// K(t,s) ≈ φ(t)ᵀφ(s) where φ(t) = √(2/D)[cos(ω₁t+b₁), ..., cos(ωₐt+bₐ)]
    /// </summary>
    public class RandomFourierKernel : IAttentionKernel
    {
        public int D { get; set; }               // Number of features
        public double[] Omegas { get; set; }     // Frequencies
        public double[] Biases { get; set; }     // Phase shifts
        public string Name => $"RandomFourier(D={D})";

        private Random random = new Random();

        public RandomFourierKernel(int d, double scale = 1.0, int? seed = null)
        {
            D = d;
            if (seed.HasValue)
                random = new Random(seed.Value);

            // Sample frequencies from Gaussian distribution
            Omegas = new double[D];
            Biases = new double[D];
            
            for (int i = 0; i < D; i++)
            {
                Omegas[i] = SampleGaussian(0, scale);
                Biases[i] = random.NextDouble() * 2 * Math.PI;
            }
        }

        public double Compute(double t, double s)
        {
            double[] phi_t = ComputeFeatures(t);
            double[] phi_s = ComputeFeatures(s);
            
            double result = 0.0;
            for (int i = 0; i < D; i++)
            {
                result += phi_t[i] * phi_s[i];
            }
            
            return result;
        }

        public double[] ComputeFeatures(double t)
        {
            double[] features = new double[D];
            double scale = Math.Sqrt(2.0 / D);
            
            for (int i = 0; i < D; i++)
            {
                features[i] = scale * Math.Cos(Omegas[i] * t + Biases[i]);
            }
            
            return features;
        }

        private double SampleGaussian(double mean, double stddev)
        {
            // Box-Muller transform
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stddev * randStdNormal;
        }
    }

    /// <summary>
    /// Learnable kernel with parametric form
    /// </summary>
    public class LearnableKernel : IAttentionKernel
    {
        public double[] Parameters { get; set; }
        public string Name => "LearnableKernel";

        public LearnableKernel(int numParams)
        {
            Parameters = new double[numParams];
            var random = new Random();
            for (int i = 0; i < numParams; i++)
                Parameters[i] = random.NextDouble() * 0.1;
        }

        public double Compute(double t, double s)
        {
            // Simple polynomial kernel as example
            // K(t,s) = p₀ + p₁(t-s) + p₂(t-s)² + ...
            double diff = t - s;
            double result = 0.0;
            double power = 1.0;
            
            for (int i = 0; i < Parameters.Length; i++)
            {
                result += Parameters[i] * power;
                power *= diff;
            }
            
            return Math.Exp(-result * result); // Make it positive definite
        }
    }
}
