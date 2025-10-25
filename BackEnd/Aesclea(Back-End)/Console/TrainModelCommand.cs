// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;
using System.IO;
using Aesclea_Back_End_.AIModel;
using Aesclea_Back_End_.AIModel.Helpers;
using Console = System.Console;

namespace Aesclea_Back_End_.Commands
{
    /// <summary>
    /// Console command for training the medical AI model with LLM datasets
    /// </summary>
    public class TrainModelCommand
    {
        public static void Execute()
        {
            System.Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            System.Console.WriteLine("║     AESCLEA MEDICAL AI MODEL TRAINING                    ║");
            System.Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            System.Console.WriteLine();

            try
            {
                // Get dataset path
                var solutionDir = FileHelper.TryGetSolutionDirectoryInfo().FullName;
                var datasetPath = Path.Combine(solutionDir, "LLMDatasets");

                if (!Directory.Exists(datasetPath))
                {
                    System.Console.WriteLine($"ERROR: Dataset directory not found: {datasetPath}");
                    System.Console.WriteLine("Please ensure LLMDatasets folder exists in the solution directory.");
                    return;
                }

                System.Console.WriteLine($"Dataset Path: {datasetPath}");
                System.Console.WriteLine();

                // Get user input for epochs
                System.Console.Write("Enter number of epochs (recommended: 50-100 for accuracy): ");
                int epochs = 50; // Default
                if (int.TryParse(System.Console.ReadLine(), out int userEpochs) && userEpochs > 0)
                {
                    epochs = userEpochs;
                }
                else
                {
                    System.Console.WriteLine($"Invalid input. Using default: {epochs} epochs");
                }

                System.Console.Write("Enter learning rate (recommended: 0.0001): ");
                double learningRate = 0.0001; // Default
                if (double.TryParse(System.Console.ReadLine(), System.Globalization.CultureInfo.InvariantCulture, out double userLR) && userLR > 0)
                {
                    learningRate = userLR;
                }
                else
                {
                    System.Console.WriteLine($"Invalid input. Using default: {learningRate}");
                }

                System.Console.Write("Max samples per dataset (recommended: 10000): ");
                int maxSamples = 10000; // Increased for better learning
                if (int.TryParse(System.Console.ReadLine(), out int userSamples) && userSamples > 0)
                {
                    maxSamples = userSamples;
                }
                else
                {
                    System.Console.WriteLine($"Invalid input. Using default: {maxSamples}");
                }

                System.Console.WriteLine();

                // Initialize classifier
                var classifier = new MedicalDiagnosisClassifier();

                // Initialize training service
                var trainingService = new TrainingService(classifier, datasetPath);

                // Configure training with safe parameters to prevent NaN
                var config = new TrainingService.TrainingConfig
                {
                    Epochs = epochs,
                    LearningRate = learningRate,
                    BatchSize = 32, // Keep moderate for stability
                    ValidationSplit = 0.2,
                    UseMedicalSamples = true,
                    UseTextbooks = true,
                    UsePubMedQA = true,
                    MaxSamplesPerSource = maxSamples,
                    EnableParallelProcessing = true, // NEW: Enable multi-core processing
                    MaxDegreeOfParallelism = Environment.ProcessorCount // Use all CPU cores
                };

                System.Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
                System.Console.WriteLine("║     TRAINING CONFIGURATION                                ║");
                System.Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
                System.Console.WriteLine($"  🔄 Epochs: {config.Epochs}");
                System.Console.WriteLine($"  📊 Learning Rate: {config.LearningRate}");
                System.Console.WriteLine($"  📦 Batch Size: {config.BatchSize}");
                System.Console.WriteLine($"  ✅ Validation Split: {config.ValidationSplit * 100}%");
                System.Console.WriteLine($"  📚 Max Samples Per Source: {config.MaxSamplesPerSource}");
                System.Console.WriteLine($"  ⚡ Parallel Processing: ENABLED");
                System.Console.WriteLine($"  🖥️  CPU Cores: {config.MaxDegreeOfParallelism}");
                System.Console.WriteLine();

                System.Console.Write("Start training? (y/n): ");
                var response = System.Console.ReadLine();

                if (response?.ToLower() == "y")
                {
                    // Progress callback
                    void ProgressCallback(TrainingService.TrainingProgress progress)
                    {
                        if (progress.Phase == "Loading Data")
                        {
                            System.Console.WriteLine($"📁 {progress.Message}");
                        }
                        else if (progress.Phase == "Training")
                        {
                            System.Console.WriteLine($"🔄 Epoch {progress.CurrentEpoch}/{progress.TotalEpochs} - {progress.Message}");
                        }
                        else if (progress.Phase == "Validation")
                        {
                            System.Console.WriteLine($"✓ {progress.Message}");
                        }
                        else if (progress.Phase == "Complete")
                        {
                            System.Console.WriteLine();
                            System.Console.WriteLine($"✅ {progress.Message}");
                        }
                    }

                    // Start training
                    trainingService.TrainModel(config, ProgressCallback);

                    System.Console.WriteLine();
                    System.Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
                    System.Console.WriteLine("║     TRAINING COMPLETE!                                   ║");
                    System.Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
                    System.Console.WriteLine();
                    System.Console.WriteLine("📌 IMPORTANT: To use the trained model:");
                    System.Console.WriteLine("   1. Restart the backend application");
                    System.Console.WriteLine("   2. The AI chatbot will automatically load the new weights");
                    System.Console.WriteLine("   3. Test with /api/ai/chat or /api/ai/analyze endpoints");
                    System.Console.WriteLine();
                    System.Console.WriteLine("💡 The trained model is now saved in NeuronData/ folder");
                    System.Console.WriteLine("   and will be used by all AI features automatically!");
                }
                else
                {
                    System.Console.WriteLine("Training cancelled.");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine();
                System.Console.WriteLine($"❌ ERROR: {ex.Message}");
                System.Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }

            System.Console.WriteLine();
            System.Console.WriteLine("Press any key to return to menu...");
            System.Console.ReadKey();
        }
    }
}
