// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov
// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Aesclea_Back_End_.AIModel.NeuralODE;
using System;
using System.Collections.Generic;

namespace Aesclea_Back_End_.Examples
{
    /// <summary>
    /// Quick start examples for using the Neural ODE Medical LLM
    /// </summary>
    public class NeuralODEQuickStart
    {
        public static void Main()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   Neural ODE Medical LLM - Quick Start Examples          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

            // Example 1: Basic medical text analysis
            Example1_BasicAnalysis();

            // Example 2: Symptom analysis with vital signs
            Example2_SymptomAnalysis();

            // Example 3: Different kernels comparison
            Example3_KernelComparison();

            // Example 4: Uncertainty quantification
            Example4_UncertaintyDemo();
        }

        static void Example1_BasicAnalysis()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("EXAMPLE 1: Basic Medical Text Analysis");
            Console.WriteLine(new string('=', 60));

            // Create tokenizer
            var tokenizer = new MedicalTokenizer();
            
            // Clinical text
            string clinicalText = "Patient presents with persistent cough, fever 38.5°C, and shortness of breath";
            Console.WriteLine($"Input: {clinicalText}\n");
            
            // Tokenize
            var tokens = tokenizer.Tokenize(clinicalText);
            Console.WriteLine($"Tokens generated: {tokens.Length}");
            Console.WriteLine($"Decoded: {tokenizer.Decode(tokens)}\n");
            
            // Extract entities
            var entities = tokenizer.ExtractEntities(clinicalText);
            Console.WriteLine($"Extracted {entities.Count} medical entities:");
            foreach (var entity in entities)
            {
                Console.WriteLine($"  - {entity.Text} ({entity.Type})");
            }
        }

        static void Example2_SymptomAnalysis()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("EXAMPLE 2: Symptom Analysis with Uncertainty");
            Console.WriteLine(new string('=', 60));

            // Create model with Gaussian kernel
            var tokenizer = new MedicalTokenizer();
            var model = new NeuralODELLM(
                vocabSize: tokenizer.VocabSize,
                hiddenDim: 64,
                embeddingDim: 32,
                uncertaintyDim: 16,
                kernel: new GaussianKernel(sigma: 1.0),
                seed: 42
            );

            // Analyze symptoms
            string[] symptoms = { "headache", "nausea", "dizziness" };
            string clinicalText = "Patient reports " + string.Join(", ", symptoms);
            
            Console.WriteLine($"Analyzing: {clinicalText}\n");
            
            var tokens = tokenizer.Tokenize(clinicalText);
            var result = model.PredictMedical(tokens, T: 1.0);
            
            Console.WriteLine($"Predicted Class: {result.PredictedClass}");
            Console.WriteLine($"Confidence: {result.Confidence:P2}");
            Console.WriteLine($"Uncertainty: {result.Uncertainty:F4}");
            Console.WriteLine($"Compute Cost: {result.ComputeCost:F4}");
            Console.WriteLine($"Interpretation: {result.GetInterpretation()}");
        }

        static void Example3_KernelComparison()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("EXAMPLE 3: Attention Kernel Comparison");
            Console.WriteLine(new string('=', 60));

            var tokenizer = new MedicalTokenizer();
            string text = "Patient has chest pain and irregular heartbeat";
            var tokens = tokenizer.Tokenize(text);

            // Test different kernels
            var kernels = new IAttentionKernel[]
            {
                new GaussianKernel(sigma: 0.5),
                new GaussianKernel(sigma: 1.0),
                new GaussianMixtureKernel(
                    sigmas: new double[] { 0.1, 0.5, 1.0 },
                    alphas: new double[] { 0.5, 0.3, 0.2 }
                ),
                new RandomFourierKernel(d: 32, scale: 1.0, seed: 42)
            };

            Console.WriteLine($"Analyzing: {text}\n");

            foreach (var kernel in kernels)
            {
                var model = new NeuralODELLM(
                    vocabSize: tokenizer.VocabSize,
                    hiddenDim: 64,
                    embeddingDim: 32,
                    uncertaintyDim: 16,
                    kernel: kernel,
                    seed: 42
                );

                var result = model.PredictMedical(tokens);
                Console.WriteLine($"{kernel.Name,-30} | Confidence: {result.Confidence:P2} | Cost: {result.ComputeCost:F3}");
            }
        }

        static void Example4_UncertaintyDemo()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("EXAMPLE 4: Uncertainty Quantification Demo");
            Console.WriteLine(new string('=', 60));

            var tokenizer = new MedicalTokenizer();
            var model = new NeuralODELLM(
                vocabSize: tokenizer.VocabSize,
                hiddenDim: 128,
                embeddingDim: 64,
                uncertaintyDim: 32,
                kernel: new GaussianMixtureKernel(
                    sigmas: new double[] { 0.1, 0.5, 1.0 },
                    alphas: new double[] { 0.5, 0.3, 0.2 }
                ),
                seed: 42
            );

            // Test cases with varying clarity
            var testCases = new Dictionary<string, string>
            {
                { "Clear case", "Patient has confirmed diabetes type 2 with HbA1c 8.5%" },
                { "Moderate case", "Patient reports occasional chest discomfort and fatigue" },
                { "Unclear case", "Patient feels unwell with vague symptoms" }
            };

            foreach (var testCase in testCases)
            {
                Console.WriteLine($"\n{testCase.Key}: {testCase.Value}");
                
                var tokens = tokenizer.Tokenize(testCase.Value);
                var result = model.PredictMedical(tokens);
                
                Console.WriteLine($"  Confidence: {result.Confidence:P2}");
                Console.WriteLine($"  Uncertainty: {result.Uncertainty:F4}");
                Console.WriteLine($"  Status: {result.GetInterpretation()}");
                
                // Show class probabilities
                Console.WriteLine("  Class Probabilities:");
                for (int i = 0; i < Math.Min(5, result.ClassProbabilities.Length); i++)
                {
                    Console.WriteLine($"    Class {i}: {result.ClassProbabilities[i]:P2}");
                }
            }
        }
    }
}
