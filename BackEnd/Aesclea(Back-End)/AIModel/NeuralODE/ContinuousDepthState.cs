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
    /// Represents the continuous-depth state: x(t) = [h(t), e(t), u(t)]
    /// where h = hidden state, e = embedding state, u = uncertainty state
    /// </summary>
    public class ContinuousDepthState
    {
        public double[] Hidden { get; set; }        // h(t) - hidden representation
        public double[] Embedding { get; set; }     // e(t) - embedding state
        public double[] Uncertainty { get; set; }   // u(t) - uncertainty estimates
        
        public double Time { get; set; }            // Current time t
        
        public int Dimension => Hidden.Length + Embedding.Length + Uncertainty.Length;

        public ContinuousDepthState(int hiddenDim, int embeddingDim, int uncertaintyDim)
        {
            Hidden = new double[hiddenDim];
            Embedding = new double[embeddingDim];
            Uncertainty = new double[uncertaintyDim];
            Time = 0.0;
        }

        public ContinuousDepthState(double[] hidden, double[] embedding, double[] uncertainty, double time = 0.0)
        {
            Hidden = (double[])hidden.Clone();
            Embedding = (double[])embedding.Clone();
            Uncertainty = (double[])uncertainty.Clone();
            Time = time;
        }

        /// <summary>
        /// Get the full state vector [h(t), e(t), u(t)]
        /// </summary>
        public double[] ToVector()
        {
            var result = new double[Dimension];
            int idx = 0;
            
            Array.Copy(Hidden, 0, result, idx, Hidden.Length);
            idx += Hidden.Length;
            
            Array.Copy(Embedding, 0, result, idx, Embedding.Length);
            idx += Embedding.Length;
            
            Array.Copy(Uncertainty, 0, result, idx, Uncertainty.Length);
            
            return result;
        }

        /// <summary>
        /// Create state from full vector
        /// </summary>
        public static ContinuousDepthState FromVector(double[] vector, int hiddenDim, int embeddingDim, int uncertaintyDim, double time = 0.0)
        {
            var state = new ContinuousDepthState(hiddenDim, embeddingDim, uncertaintyDim)
            {
                Time = time
            };
            
            int idx = 0;
            Array.Copy(vector, idx, state.Hidden, 0, hiddenDim);
            idx += hiddenDim;
            
            Array.Copy(vector, idx, state.Embedding, 0, embeddingDim);
            idx += embeddingDim;
            
            Array.Copy(vector, idx, state.Uncertainty, 0, uncertaintyDim);
            
            return state;
        }

        public ContinuousDepthState Clone()
        {
            return new ContinuousDepthState(Hidden, Embedding, Uncertainty, Time);
        }
    }
}
