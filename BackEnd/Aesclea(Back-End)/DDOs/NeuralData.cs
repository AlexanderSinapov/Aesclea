// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

namespace Aesclea_Back_End_.DDOs
{
    public class NeuralData
    {
        public List<List<List<double>>> Weights { get; set; } = new List<List<List<double>>>();
        public List<List<double>> Biases { get; set; } = new List<List<double>>();
        public NeuralData() { }
        public NeuralData(List<List<List<double>>> weights, List<List<double>> biases)
        {
            Weights = weights;
            Biases = biases;
        }
    }
}
