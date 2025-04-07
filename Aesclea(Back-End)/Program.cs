using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Aesclea_Back_End_.AIModel;
using System.Globalization;
using Aesclea_Back_End_.AIModel.Helpers;
using Aesclea_Back_End_.DDOs;

namespace Aesclea_Back_End_
{
    public class Program
    {
        private static FileHelper fileHelper = new FileHelper();

        public static void Main(string[] args)
        {
            // Initialize the file helper
            fileHelper.OpenFolder();

            // Network with layers sized for 512x512 grayscale images
            var network = new NeuronNetwork(new int[] { 16384, 256, 64, 16, 1 });

            while (true)
            {
                // Display menu options
                Console.Clear();
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Prompt (Test image for tumor)");
                Console.WriteLine("2. Train");
                Console.WriteLine("3. Mass Test");
                Console.WriteLine("4. Load Weights");
                Console.WriteLine("5. Save Weights");
                Console.WriteLine("6. Convert Old Weights to New");
                Console.WriteLine("7. Exit");
                Console.Write("Enter your choice: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Prompt(network);
                        break;
                    case "2":
                        Train(network);
                        break;
                    case "3":
                        MassTest(network);
                        break;
                    case "4":
                        LoadWeights(network);
                        break;
                    case "5":
                        SaveWeights(network);
                        break;
                    case "6":
                        ConvertWeights();
                        break;
                    case "7":
                        return; // Exit the program
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }

                Console.WriteLine("\nPress Enter to return to the menu...");
                Console.ReadLine();
            }
        }

        private static void Prompt(NeuronNetwork network)
        {
            Console.WriteLine("Enter the path to the image file to analyze:");
            string imagePath = Console.ReadLine();

            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"File not found: {imagePath}");
                return;
            }

            try
            {
                using (var image = new Bitmap(imagePath))
                {
                    // Use the ImageHelper class to preprocess the image
                    var processedImage = ImageHelper.ProcessImage(image, 128); // Updated method call

                    // Feed the image data into the network
                    var output = network.FeedForward(processedImage);

                    // Interpret the result (output is a single value between 0 and 1)
                    double tumorProbability = output[0] * 100;
                    Console.WriteLine($"Analysis complete.");
                    Console.WriteLine($"Tumor probability: {tumorProbability:F2}%");

                    // Give a clear interpretation
                    if (tumorProbability > 75)
                    {
                        Console.WriteLine("Assessment: High probability of tumor detected.");
                    }
                    else if (tumorProbability > 40)
                    {
                        Console.WriteLine("Assessment: Moderate probability of tumor detected.");
                    }
                    else
                    {
                        Console.WriteLine("Assessment: Low probability of tumor detected.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing image: {ex.Message}");
            }
        }

        private static void Train(NeuronNetwork network)
        {
            Console.WriteLine("Starting training with MRI/X-RAY images...");

            try
            {
                // Get positive examples (with tumor)
                Console.WriteLine("Enter the path to the folder containing tumor images:");
                string tumorFolderPath = Console.ReadLine();

                // Get negative examples (without tumor)
                Console.WriteLine("Enter the path to the folder containing non-tumor images:");
                string nonTumorFolderPath = Console.ReadLine();

                Console.WriteLine("Enter the number of epochs:");
                int epochs = int.Parse(Console.ReadLine());

                Console.WriteLine("Enter the learning rate (e.g., 0.01):");
                double learningRate = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

                // Use ImageHelper to load and process images
                var tumorImages = ImageHelper.LoadImages(tumorFolderPath);
                Console.WriteLine($"Loaded {tumorImages.Count} tumor images.");

                var nonTumorImages = ImageHelper.LoadImages(nonTumorFolderPath);
                Console.WriteLine($"Loaded {nonTumorImages.Count} non-tumor images.");

                // Combine into training data
                var inputs = new List<List<double>>();
                var outputs = new List<List<double>>();

                // Add tumor images with label 1
                foreach (var imageData in tumorImages)
                {
                    inputs.Add(imageData);
                    outputs.Add(new List<double> { 1.0 });
                }

                // Add non-tumor images with label 0
                foreach (var imageData in nonTumorImages)
                {
                    inputs.Add(imageData);
                    outputs.Add(new List<double> { 0.0 });
                }

                // Train the network
                Console.WriteLine($"Starting training with {inputs.Count} images for {epochs} epochs...");
                network.Train(inputs, outputs, epochs, learningRate);

                Console.WriteLine("Training complete!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during training: {ex.Message}");
            }
        }

        private static void MassTest(NeuronNetwork network)
        {
            Console.WriteLine("Starting mass test...");

            try
            {
                // Get test data path
                Console.WriteLine("Enter path to folder with tumor test images:");
                string tumorTestPath = Console.ReadLine();

                Console.WriteLine("Enter path to folder with non-tumor test images:");
                string nonTumorTestPath = Console.ReadLine();

                // Load test images using ImageHelper
                var tumorTestImages = ImageHelper.LoadImages(tumorTestPath);
                var nonTumorTestImages = ImageHelper.LoadImages(nonTumorTestPath);

                var testInputs = new List<List<double>>();
                var expectedOutputs = new List<List<double>>();

                // Add tumor images with label 1
                foreach (var image in tumorTestImages)
                {
                    testInputs.Add(image);
                    expectedOutputs.Add(new List<double> { 1.0 });
                }

                // Add non-tumor images with label 0
                foreach (var image in nonTumorTestImages)
                {
                    testInputs.Add(image);
                    expectedOutputs.Add(new List<double> { 0.0 });
                }

                Console.WriteLine($"Testing network with {testInputs.Count} images...");

                // Calculate accuracy using the NeuronNetwork method
                double accuracy = network.CalculateAccuracy(testInputs, expectedOutputs);

                Console.WriteLine($"Mass test results: Accuracy = {accuracy * 100:F2}%");

                // Detailed evaluation
                int totalTumorImages = tumorTestImages.Count;
                int totalNonTumorImages = nonTumorTestImages.Count;

                // Test tumor detection rate (sensitivity)
                int correctTumorDetections = 0;
                foreach (var image in tumorTestImages)
                {
                    var output = network.FeedForward(image);
                    if (output[0] >= 0.5) // Predicted as tumor
                        correctTumorDetections++;
                }

                // Test non-tumor detection rate (specificity)
                int correctNonTumorDetections = 0;
                foreach (var image in nonTumorTestImages)
                {
                    var output = network.FeedForward(image);
                    if (output[0] < 0.5) // Predicted as non-tumor
                        correctNonTumorDetections++;
                }

                double sensitivity = (double)correctTumorDetections / totalTumorImages * 100;
                double specificity = (double)correctNonTumorDetections / totalNonTumorImages * 100;

                Console.WriteLine($"Tumor detection rate (sensitivity): {sensitivity:F2}%");
                Console.WriteLine($"Non-tumor detection rate (specificity): {specificity:F2}%");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during mass testing: {ex.Message}");
            }
        }

        private static void LoadWeights(NeuronNetwork network)
        {
            Console.WriteLine("Available weight files:");

            // Get all .wbn files in the NeuronData folder
            var files = fileHelper.GetAvailableWeightFiles();

            if (files.Count == 0)
            {
                Console.WriteLine("No weight files found in NeuronData directory.");
                return;
            }

            // List all available files
            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {files[i]}");
            }

            Console.Write("Select a file by number (or enter 0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int selection) && selection > 0 && selection <= files.Count)
            {
                string selectedFile = files[selection - 1];

                try
                {
                    // Load the neural data
                    NeuralData data = fileHelper.GetNeuralData(selectedFile);

                    if (data != null)
                    {
                        // Set the weights in the network
                        network.SetNeuralNetworkData(data);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading weights: {ex.Message}");
                }
            }
            else if (selection != 0)
            {
                Console.WriteLine("Invalid selection.");
            }
        }

        private static void SaveWeights(NeuronNetwork network)
        {
            Console.Write("Enter a name for the weights file (leave empty for timestamp): ");
            string fileName = Console.ReadLine();

            try
            {
                // Get current weights and biases
                NeuralData data = network.GetNeuralNetworkData();

                // Save to file
                bool success = fileHelper.SaveNeuralData(data, fileName);

                if (success)
                {
                    string displayName = string.IsNullOrEmpty(fileName) ?
                        DateTime.Now.ToString("yyyyMMddHHmmss") : fileName;

                    Console.WriteLine($"Weights saved successfully to {displayName}_NeuralData.wbn.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving weights: {ex.Message}");
            }
        }

        private static void ConvertWeights()
        {
            Console.WriteLine("Convert old weights to new .wbn format");
            Console.Write("Enter the path to the old weights JSON file: ");
            string inputFile = Console.ReadLine();

            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Error: File not found at {inputFile}");
                return;
            }

            Console.Write("Enter a name for the new weights file: ");
            string outputName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(outputName))
            {
                outputName = DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            bool success = fileHelper.ConvertOldDataToNew(inputFile, outputName);

            if (success)
            {
                Console.WriteLine($"Conversion complete. File saved to NeuronData/{outputName}_NeuralData.wbn");
            }
        }
    }
}