// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov
// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;

namespace Aesclea_Back_End_.AIModel.NeuralODE
{
    /// <summary>
    /// Adaptive compute controller that modulates processing intensity
    /// c(t) = σ(α·u(t) + β·r) where u = uncertainty, r = resource constraint
    /// </summary>
    public class ComputeController
    {
        public double Alpha { get; set; } = 1.0;    // Uncertainty weight
        public double Beta { get; set; } = 0.1;     // Resource constraint weight
        
        private double resourceConstraint = 0.0;     // r in the formula
        private Random random = new Random();

        public ComputeController(double alpha = 1.0, double beta = 0.1)
        {
            Alpha = alpha;
            Beta = beta;
        }

        /// <summary>
        /// Compute the control coefficient c(t) = σ(α·u(t) + β·r)
        /// </summary>
        public double ComputeCoefficient(double uncertaintyMean, double time)
        {
            // Add time-dependent resource constraint (could be learned)
            resourceConstraint = -0.5 * time; // Decrease compute as we progress
            
            double logit = Alpha * uncertaintyMean + Beta * resourceConstraint;
            return Sigmoid(logit);
        }

        /// <summary>
        /// Compute coefficient for entire state
        /// </summary>
        public double ComputeCoefficient(ContinuousDepthState state)
        {
            double uncertaintyMean = Mean(state.Uncertainty);
            return ComputeCoefficient(uncertaintyMean, state.Time);
        }

        /// <summary>
        /// Modulate ODE dynamics: dx/dt = c(t) * f(x(t), t)
        /// </summary>
        public double[] ModulateDynamics(double[] dynamics, double controlCoef)
        {
            double[] modulated = new double[dynamics.Length];
            for (int i = 0; i < dynamics.Length; i++)
            {
                modulated[i] = controlCoef * dynamics[i];
            }
            return modulated;
        }

        /// <summary>
        /// Compute total cost integral C = ∫c(t)dt over trajectory
        /// </summary>
        public double ComputeTotalCost(double[] controlCoefficients, double dt)
        {
            double cost = 0.0;
            foreach (var c in controlCoefficients)
            {
                cost += c * dt;
            }
            return cost;
        }

        /// <summary>
        /// Check if inference constraint is satisfied: E[C] ≤ C_max
        /// </summary>
        public bool SatisfiesConstraint(double expectedCost, double maxCost)
        {
            return expectedCost <= maxCost;
        }

        private double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        private double Mean(double[] arr)
        {
            double sum = 0.0;
            foreach (var val in arr)
                sum += val;
            return sum / arr.Length;
        }
    }
}
