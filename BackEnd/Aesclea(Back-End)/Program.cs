using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using Aesclea_Back_End_.AIModel;
using Aesclea_Back_End_.AIModel.Helpers;
using Aesclea_Back_End_.DDOs;

namespace Aesclea_Back_End_
{
    public class Program
    {
        private static FileHelper fileHelper = new FileHelper();
        private static TumorClassifier tumorClassifier;

        public static void Main(string[] args)
        {
            // Initialize the file helper
            fileHelper.OpenFolder();

            // Network with layers sized for 128x128 grayscale images (16384 inputs)
            var network = new NeuronNetwork(new int[] { 16384, 256, 64, 16, 1 });

            // Initialize the tumor classifier with the base network
            tumorClassifier = new TumorClassifier(network);

            // Initialize recurrent network with similar architecture (keep for compatibility)
            var recurrentNetwork = new RecurrentNeuralNetwork(new int[] { 256, 64, 16 }, new int[] { 256, 64, 16 }, 500);

            while (true)
            {
                // Display menu options
                Console.Clear();
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Analyze Image (Detect & Classify Tumor)");
                Console.WriteLine("2. Train Base Tumor Detector");
                Console.WriteLine("3. Train Tumor Classifiers");
                Console.WriteLine("4. Mass Test Base Network");
                Console.WriteLine("5. Load Weights");
                Console.WriteLine("6. Save Weights");
                Console.WriteLine("7. Convert Old Weights to New");
                Console.WriteLine("8. Recurrent Network Options");
                Console.WriteLine("9. Exit");
                Console.Write("Enter your choice: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AnalyzeImage(network);
                        break;
                    case "2":
                        TrainBaseTumorDetector(network);
                        break;
                    case "3":
                        TrainTumorClassifiers();
                        break;
                    case "4":
                        MassTest(network);
                        break;
                    case "5":
                        LoadAllWeights(network);
                        break;
                    case "6":
                        SaveAllWeights(network);
                        break;
                    case "7":
                        ConvertWeights();
                        break;
                    case "8":
                        //RecurrentNeuralNetworkOptions(recurrentNetwork);
                        break;
                    case "9":
                        return; // Exit the program
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }

                Console.WriteLine("\nPress Enter to return to the menu...");
                Console.ReadLine();
            }
        }

        private static void AnalyzeImage(NeuronNetwork network)
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
                    var processedImage = ImageHelper.ProcessImage(image, 128);

                    // Perform comprehensive tumor analysis
                    var result = tumorClassifier.AnalyzeImage(processedImage);

                    // Display the detailed results
                    Console.WriteLine(result.GetSummary());

                    // Generate visualization
                    if (result.HasTumor)
                    {
                        Console.WriteLine("\nWould you like to save a visualization? (y/n)");
                        if (Console.ReadLine().ToLower() == "y")
                        {
                            string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "Visualizations");
                            Directory.CreateDirectory(outputFolder);

                            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                            string outputPath = Path.Combine(outputFolder, $"tumor_analysis_{timestamp}.png");

                            // Save the processed image
                            ImageHelper.SaveImageData(processedImage, 128, 128, outputPath);

                            // Create a text file with the analysis results
                            string infoPath = Path.Combine(outputFolder, $"tumor_analysis_{timestamp}_info.txt");
                            File.WriteAllText(infoPath, result.GetSummary());

                            Console.WriteLine($"Visualization saved to {outputPath}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing image: {ex.Message}");
            }
        }

        private static void TrainBaseTumorDetector(NeuronNetwork network)
        {
            Console.WriteLine("Starting training basic tumor detector with MRI/X-RAY images...");

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

        private static void TrainTumorClassifiers()
        {
            Console.WriteLine("Starting training for tumor classifiers (type, grade, location)...");

            try
            {
                // Get tumor images with metadata
                Console.WriteLine("Enter the path to the folder containing labeled tumor images:");
                string tumorFolderPath = Console.ReadLine();

                // Get metadata file path
                Console.WriteLine("Enter the path to the tumor metadata CSV file:");
                Console.WriteLine("Format: filename,type,grade,location");
                string metadataPath = Console.ReadLine();

                if (!File.Exists(metadataPath))
                {
                    Console.WriteLine("Metadata file not found!");
                    return;
                }

                Console.WriteLine("Enter the number of epochs:");
                int epochs = int.Parse(Console.ReadLine());

                Console.WriteLine("Enter the learning rate (e.g., 0.01):");
                double learningRate = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

                // Load tumor images
                Console.WriteLine("Loading and processing tumor images...");
                var tumorHelper = new TumorHelper();

                var inputs = new List<List<double>>();
                var types = new List<string>();
                var grades = new List<int>();
                var locations = new List<string>();

                // Read metadata
                string[] metadataLines = File.ReadAllLines(metadataPath);

                // Skip header line if present
                int startLine = metadataLines[0].Contains("filename,type") ? 1 : 0;

                for (int i = startLine; i < metadataLines.Length; i++)
                {
                    string[] parts = metadataLines[i].Split(',');
                    if (parts.Length < 4) continue;

                    string filename = parts[0].Trim();
                    string type = parts[1].Trim();
                    int grade = int.Parse(parts[2].Trim());
                    string location = parts[3].Trim();

                    // Validate the data
                    if (!Array.Exists(tumorHelper.TumorType, t => t == type))
                    {
                        Console.WriteLine($"Warning: Invalid tumor type '{type}' for {filename}, skipping.");
                        continue;
                    }

                    if (grade < 1 || grade > 4)
                    {
                        Console.WriteLine($"Warning: Invalid grade '{grade}' for {filename}, skipping.");
                        continue;
                    }

                    if (!tumorHelper.TumorLocation.ContainsKey(location))
                    {
                        Console.WriteLine($"Warning: Invalid location '{location}' for {filename}, skipping.");
                        continue;
                    }

                    // Load and process the image
                    string imagePath = Path.Combine(tumorFolderPath, filename);
                    if (!File.Exists(imagePath))
                    {
                        Console.WriteLine($"Warning: Image file not found: {imagePath}, skipping.");
                        continue;
                    }

                    try
                    {
                        using (var image = new Bitmap(imagePath))
                        {
                            var processedImage = ImageHelper.ProcessImage(image, 128);

                            inputs.Add(processedImage);
                            types.Add(type);
                            grades.Add(grade);
                            locations.Add(location);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing image {filename}: {ex.Message}");
                    }
                }

                if (inputs.Count == 0)
                {
                    Console.WriteLine("No valid training data found.");
                    return;
                }

                Console.WriteLine($"Successfully loaded {inputs.Count} labeled tumor images.");

                // Train the classifiers
                tumorClassifier.TrainClassifiers(inputs, types, grades, locations, epochs, learningRate);

                Console.WriteLine("Tumor classifier training complete!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during tumor classifier training: {ex.Message}");
            }
        }

        private static void LoadAllWeights(NeuronNetwork network)
        {
            Console.WriteLine("Load Options:");
            Console.WriteLine("1. Load Base Tumor Detection Network");
            Console.WriteLine("2. Load All Networks (Detection + Classification)");
            Console.WriteLine("3. Return to Main Menu");

            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    LoadWeights(network);
                    break;
                case "2":
                    // First load the base network
                    LoadWeights(network);

                    // Then load classifier networks
                    Console.WriteLine("\nNow loading classifier networks...");
                    Console.WriteLine("Enter the base name for classifier networks:");
                    string baseName = Console.ReadLine();

                    try
                    {
                        tumorClassifier.LoadWeights(fileHelper, baseName);
                        Console.WriteLine("Successfully loaded all classifier networks.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading classifier networks: {ex.Message}");
                    }
                    break;
                case "3":
                    return;
            }
        }

        private static void SaveAllWeights(NeuronNetwork network)
        {
            Console.WriteLine("Save Options:");
            Console.WriteLine("1. Save Base Tumor Detection Network");
            Console.WriteLine("2. Save All Networks (Detection + Classification)");
            Console.WriteLine("3. Return to Main Menu");

            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SaveWeights(network);
                    break;
                case "2":
                    // First save the base network
                    SaveWeights(network);

                    // Then save classifier networks
                    Console.WriteLine("\nNow saving classifier networks...");
                    Console.Write("Enter a base name for the classifier networks: ");
                    string baseName = Console.ReadLine();

                    try
                    {
                        tumorClassifier.SaveWeights(fileHelper, baseName);
                        Console.WriteLine("Successfully saved all classifier networks.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving classifier networks: {ex.Message}");
                    }
                    break;
                case "3":
                    return;
            }
        }

        // Below are the existing methods reused from your original code
        // The implementations are kept mostly the same

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
                        Console.WriteLine("Weights loaded successfully.");
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