// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Aesclea_Back_End_.AIModel;
using Aesclea_Back_End_.AIModel.Helpers;
using Aesclea_Back_End_.DDOs;

namespace Aesclea_Back_End_
{
    public class ConsoleApplication
    {
        private static FileHelper fileHelper = new FileHelper();
        private static TumorClassifier tumorClassifier;
        private static MedicalDiagnosisClassifier medicalDiagnosisClassifier;        public static void Run()
        {
            // Initialize the file helper
            fileHelper.OpenFolder();

            // Network with layers sized for 128x128 grayscale images (16384 inputs)
            var network = new NeuronNetwork(new int[] { 16384, 256, 64, 16, 1 });

            // Initialize the tumor classifier with the base network
            tumorClassifier = new TumorClassifier(network);

            // Auto-load existing weights if available
            Console.WriteLine("Loading existing trained weights...");
            try
            {
                tumorClassifier.LoadWeights(fileHelper, "tgl");
                Console.WriteLine("✓ Successfully loaded pre-trained tumor classifier weights");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Could not load existing weights: {ex.Message}");
                Console.WriteLine("   You may need to train the classifier first.");
            }

            // Initialize medical diagnosis classifier
            medicalDiagnosisClassifier = new MedicalDiagnosisClassifier();

            // Initialize recurrent network with similar architecture (keep for compatibility)
            var recurrentNetwork = new RecurrentNeuralNetwork(new int[] { 256, 64, 16 }, new int[] { 256, 64, 16 }, 500);

            while (true)
            {                // Display menu options
                Console.Clear();                Console.WriteLine("========================================");
                Console.WriteLine("         AESCLEA MEDICAL AI SYSTEM");
                Console.WriteLine("========================================");
                Console.WriteLine("Select an option:");
                Console.WriteLine("");
                Console.WriteLine("IMAGE ANALYSIS:");
                Console.WriteLine("1. Analyze Single Image (Enhanced)");
                Console.WriteLine("2. Batch Analyze Images");
                Console.WriteLine("3. Analyze Image with Annotation");
                Console.WriteLine("4. Advanced Tumor Analysis");
                Console.WriteLine("5. Compare Analysis Results");
                Console.WriteLine("");
                Console.WriteLine("MODEL TRAINING:");
                Console.WriteLine("6. Train Base Tumor Detector");
                Console.WriteLine("7. Train Tumor Classifiers");
                Console.WriteLine("8. Train with Custom Parameters");
                Console.WriteLine("");
                Console.WriteLine("MODEL TESTING:");
                Console.WriteLine("9. Test Base Network");
                Console.WriteLine("10. Test Tumor Classifiers");
                Console.WriteLine("11. Validate Model Performance");
                Console.WriteLine("");
                Console.WriteLine("MEDICAL TEXT ANALYSIS:");
                Console.WriteLine("12. Analyze Medical Text");
                Console.WriteLine("13. Train Medical Text Classifier");
                Console.WriteLine("14. Test Medical Text Classifier");
                Console.WriteLine("15. Batch Analyze Medical Records");
                Console.WriteLine("");
                Console.WriteLine("SYSTEM MANAGEMENT:");
                Console.WriteLine("16. Load Weights");
                Console.WriteLine("17. Save Weights");
                Console.WriteLine("18. Configure Analysis Settings");
                Console.WriteLine("19. Export Analysis Results");
                Console.WriteLine("20. Exit");
                Console.WriteLine("========================================");
                Console.Write("Enter your choice: ");                var choice = Console.ReadLine();                switch (choice)
                {
                    case "1":
                        EnhancedAnalyzeImage(network);
                        break;
                    case "2":
                        BatchAnalyzeImages(network);
                        break;
                    case "3":
                        AnalyzeImageWithAnnotation(network);
                        break;
                    case "4":
                        AdvancedTumorAnalysis(network);
                        break;
                    case "5":
                        CompareAnalysisResults(network);
                        break;
                    case "6":
                        TrainBaseTumorDetector(network);
                        break;
                    case "7":
                        TrainTumorClassifiers();
                        break;
                    case "8":
                        TrainWithCustomParameters(network);
                        break;
                    case "9":
                        MassTest(network);
                        break;
                    case "10":
                        TestTumorClassifiers();
                        break;
                    case "11":
                        ValidateModelPerformance(network);
                        break;
                    case "12":
                        AnalyzeMedicalText();
                        break;
                    case "13":
                        TrainMedicalTextClassifier();
                        break;
                    case "14":
                        TestMedicalTextClassifier();
                        break;
                    case "15":
                        BatchAnalyzeMedicalRecords();
                        break;
                    case "16":
                        LoadAllWeights(network);
                        break;
                    case "17":
                        SaveAllWeights(network);
                        break;
                    case "18":
                        ConfigureAnalysisSettings();
                        break;
                    case "19":
                        ExportAnalysisResults();
                        break;
                    case "20":
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

        private static void AnalyzeMedicalText()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("        MEDICAL TEXT ANALYSIS");
            Console.WriteLine("========================================");
            Console.WriteLine("");
            Console.WriteLine("Choose input method:");
            Console.WriteLine("1. Type medical text directly");
            Console.WriteLine("2. Load from text file");
            Console.WriteLine("3. Return to main menu");
            Console.Write("Enter your choice: ");

            var choice = Console.ReadLine();

            string medicalText = "";

            switch (choice)
            {
                case "1":
                    Console.WriteLine("\nEnter medical text (symptoms, history, notes):");
                    Console.WriteLine("Type 'END' on a new line to finish:");
                    string line;
                    while ((line = Console.ReadLine()) != "END")
                    {
                        medicalText += line + " ";
                    }
                    break;

                case "2":
                    Console.Write("Enter path to text file: ");
                    string filePath = Console.ReadLine();
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            medicalText = File.ReadAllText(filePath);
                            Console.WriteLine($"Loaded text from: {filePath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading file: {ex.Message}");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("File not found!");
                        return;
                    }
                    break;

                case "3":
                    return;

                default:
                    Console.WriteLine("Invalid choice!");
                    return;
            }

            if (string.IsNullOrWhiteSpace(medicalText))
            {
                Console.WriteLine("No medical text provided!");
                return;
            }

            Console.WriteLine("\nAnalyzing medical text...");

            try
            {
                // Analyze the medical text
                var result = medicalDiagnosisClassifier.AnalyzeMedicalText(medicalText);

                // Display results
                Console.WriteLine("\n" + result.GetSummary());

                // Ask if user wants to save results
                Console.WriteLine("\nWould you like to save this analysis? (y/n)");
                if (Console.ReadLine().ToLower() == "y")
                {
                    SaveMedicalAnalysis(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing medical text: {ex.Message}");
            }
        }

        private static void TrainMedicalTextClassifier()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("    TRAIN MEDICAL TEXT CLASSIFIER");
            Console.WriteLine("========================================");
            Console.WriteLine("");
            Console.WriteLine("Training data format required:");
            Console.WriteLine("CSV file with columns: text,diagnostic_category,severity_level,urgency_level");
            Console.WriteLine("Example: \"Patient has chest pain and shortness of breath\",Cardiovascular,4,1");
            Console.WriteLine("");

            Console.Write("Enter path to training data CSV file: ");
            string csvPath = Console.ReadLine();

            if (!File.Exists(csvPath))
            {
                Console.WriteLine("Training data file not found!");
                return;
            }

            Console.Write("Enter number of training epochs (e.g., 100): ");
            if (!int.TryParse(Console.ReadLine(), out int epochs) || epochs <= 0)
            {
                Console.WriteLine("Invalid epoch count!");
                return;
            }

            Console.Write("Enter learning rate (e.g., 0.01): ");
            if (!double.TryParse(Console.ReadLine(), CultureInfo.InvariantCulture, out double learningRate) || learningRate <= 0)
            {
                Console.WriteLine("Invalid learning rate!");
                return;
            }

            try
            {
                Console.WriteLine("Loading training data...");

                // Parse CSV data
                var trainingData = ParseMedicalTrainingData(csvPath);

                if (trainingData.texts.Count == 0)
                {
                    Console.WriteLine("No valid training data found!");
                    return;
                }

                Console.WriteLine($"Loaded {trainingData.texts.Count} training examples.");
                Console.WriteLine("Starting training...");

                // Train the classifier
                medicalDiagnosisClassifier.TrainDiagnosticNetworks(
                    trainingData.texts,
                    trainingData.categories,
                    trainingData.severities,
                    trainingData.urgencies,
                    epochs,
                    learningRate
                );

                Console.WriteLine("Training completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during training: {ex.Message}");
            }
        }

        private static void TestMedicalTextClassifier()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("    TEST MEDICAL TEXT CLASSIFIER");
            Console.WriteLine("========================================");
            Console.WriteLine("");

            Console.Write("Enter path to test data CSV file: ");
            string csvPath = Console.ReadLine();

            if (!File.Exists(csvPath))
            {
                Console.WriteLine("Test data file not found!");
                return;
            }

            try
            {
                Console.WriteLine("Loading test data...");

                // Parse test data
                var testData = ParseMedicalTrainingData(csvPath);

                if (testData.texts.Count == 0)
                {
                    Console.WriteLine("No valid test data found!");
                    return;
                }

                Console.WriteLine($"Testing with {testData.texts.Count} examples...");

                // Evaluate the classifier
                var results = medicalDiagnosisClassifier.EvaluateNetworks(
                    testData.texts,
                    testData.categories,
                    testData.severities,
                    testData.urgencies
                );

                // Display results
                Console.WriteLine("\n========================================");
                Console.WriteLine("           EVALUATION RESULTS");
                Console.WriteLine("========================================");
                Console.WriteLine($"Diagnostic Accuracy: {results["diagnostic_accuracy"] * 100:F2}%");
                Console.WriteLine($"Severity Accuracy:   {results["severity_accuracy"] * 100:F2}%");
                Console.WriteLine($"Urgency Accuracy:    {results["urgency_accuracy"] * 100:F2}%");
                Console.WriteLine($"Overall Accuracy:    {results["overall_accuracy"] * 100:F2}%");
                Console.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during testing: {ex.Message}");
            }
        }

        private static void BatchAnalyzeMedicalRecords()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("    BATCH ANALYZE MEDICAL RECORDS");
            Console.WriteLine("========================================");
            Console.WriteLine("");

            Console.Write("Enter path to folder containing text files: ");
            string folderPath = Console.ReadLine();

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("Folder not found!");
                return;
            }

            var textFiles = Directory.GetFiles(folderPath, "*.txt");

            if (textFiles.Length == 0)
            {
                Console.WriteLine("No text files found in the specified folder!");
                return;
            }

            Console.WriteLine($"Found {textFiles.Length} text files. Processing...");

            var batchResults = new List<MedicalDiagnosisResult>();

            foreach (var filePath in textFiles)
            {
                try
                {
                    string medicalText = File.ReadAllText(filePath);

                    if (!string.IsNullOrWhiteSpace(medicalText))
                    {
                        var result = medicalDiagnosisClassifier.AnalyzeMedicalText(medicalText);
                        result.InputText = $"File: {Path.GetFileName(filePath)}\n{result.InputText}";
                        batchResults.Add(result);

                        Console.WriteLine($"Processed: {Path.GetFileName(filePath)}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {Path.GetFileName(filePath)}: {ex.Message}");
                }
            }

            // Generate batch summary
            Console.WriteLine($"\nProcessed {batchResults.Count} files successfully.");

            // Ask if user wants to save batch results
            Console.WriteLine("\nWould you like to save batch analysis results? (y/n)");
            if (Console.ReadLine().ToLower() == "y")
            {
                SaveBatchResults(batchResults);
            }

            // Display summary statistics
            DisplayBatchSummary(batchResults);
        }

        private static (List<string> texts, List<string> categories, List<int> severities, List<int> urgencies)
            ParseMedicalTrainingData(string csvPath)
        {
            var texts = new List<string>();
            var categories = new List<string>();
            var severities = new List<int>();
            var urgencies = new List<int>();

            var lines = File.ReadAllLines(csvPath);
            var diagnosisHelper = new MedicalDiagnosisHelper();

            // Skip header if present
            int startLine = lines[0].Contains("text,diagnostic") ? 1 : 0;

            for (int i = startLine; i < lines.Length; i++)
            {
                try
                {
                    // Simple CSV parsing (assuming no commas in text)
                    var parts = lines[i].Split(',');

                    if (parts.Length < 4) continue;

                    string text = parts[0].Trim('"');
                    string category = parts[1].Trim('"');
                    int severity = int.Parse(parts[2].Trim());
                    int urgency = int.Parse(parts[3].Trim());

                    // Validate data
                    if (string.IsNullOrWhiteSpace(text)) continue;
                    if (!Array.Exists(diagnosisHelper.DiagnosticCategories, c => c == category)) continue;
                    if (severity < 1 || severity > 5) continue;
                    if (urgency < 0 || urgency > 2) continue;

                    texts.Add(text);
                    categories.Add(category);
                    severities.Add(severity);
                    urgencies.Add(urgency);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing line {i + 1}: {ex.Message}");
                }
            }

            return (texts, categories, severities, urgencies);
        }

        private static void SaveMedicalAnalysis(MedicalDiagnosisResult result)
        {
            try
            {
                string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "MedicalAnalysis");
                Directory.CreateDirectory(outputFolder);

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputPath = Path.Combine(outputFolder, $"medical_analysis_{timestamp}.txt");

                File.WriteAllText(outputPath, result.GetSummary());
                Console.WriteLine($"Analysis saved to: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving analysis: {ex.Message}");
            }
        }

        private static void SaveBatchResults(List<MedicalDiagnosisResult> results)
        {
            try
            {
                string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "MedicalAnalysis");
                Directory.CreateDirectory(outputFolder);

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputPath = Path.Combine(outputFolder, $"batch_analysis_{timestamp}.txt");

                using (var writer = new StreamWriter(outputPath))
                {
                    writer.WriteLine("BATCH MEDICAL ANALYSIS RESULTS");
                    writer.WriteLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine($"Total Files Processed: {results.Count}");
                    writer.WriteLine(new string('=', 80));
                    writer.WriteLine();

                    foreach (var result in results)
                    {
                        writer.WriteLine(result.GetSummary());
                        writer.WriteLine(new string('-', 80));
                        writer.WriteLine();
                    }
                }

                Console.WriteLine($"Batch results saved to: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving batch results: {ex.Message}");
            }
        }

        private static void DisplayBatchSummary(List<MedicalDiagnosisResult> results)
        {
            Console.WriteLine("\n========================================");
            Console.WriteLine("          BATCH SUMMARY");
            Console.WriteLine("========================================");

            if (results.Count == 0) return;

            // Category distribution
            var categoryCount = new Dictionary<string, int>();
            var severityCount = new Dictionary<int, int>();
            var urgencyCount = new Dictionary<int, int>();

            foreach (var result in results)
            {
                // Count categories
                if (categoryCount.ContainsKey(result.PrimaryDiagnosticCategory))
                    categoryCount[result.PrimaryDiagnosticCategory]++;
                else
                    categoryCount[result.PrimaryDiagnosticCategory] = 1;

                // Count severities
                if (severityCount.ContainsKey(result.SeverityLevel))
                    severityCount[result.SeverityLevel]++;
                else
                    severityCount[result.SeverityLevel] = 1;

                // Count urgencies
                if (urgencyCount.ContainsKey(result.UrgencyLevel))
                    urgencyCount[result.UrgencyLevel]++;
                else
                    urgencyCount[result.UrgencyLevel] = 1;
            }

            Console.WriteLine("Diagnostic Category Distribution:");
            foreach (var category in categoryCount.OrderByDescending(kv => kv.Value))
            {
                Console.WriteLine($"  {category.Key}: {category.Value} ({category.Value * 100.0 / results.Count:F1}%)");
            }

            Console.WriteLine("\nSeverity Level Distribution:");
            for (int i = 1; i <= 5; i++)
            {
                int count = severityCount.ContainsKey(i) ? severityCount[i] : 0;
                Console.WriteLine($"  Level {i}: {count} ({count * 100.0 / results.Count:F1}%)");
            }

            Console.WriteLine("\nUrgency Level Distribution:");
            string[] urgencyLabels = { "Routine", "Urgent", "Immediate" };
            for (int i = 0; i < 3; i++)
            {
                int count = urgencyCount.ContainsKey(i) ? urgencyCount[i] : 0;
                Console.WriteLine($"  {urgencyLabels[i]}: {count} ({count * 100.0 / results.Count:F1}%)");
            }

            // High-priority cases
            var highPriority = results.Where(r => r.UrgencyLevel >= 1 || r.SeverityLevel >= 4).Count();
            Console.WriteLine($"\nHigh-Priority Cases: {highPriority} ({highPriority * 100.0 / results.Count:F1}%)");
        }

        private static void TestTumorClassifiers()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("      TEST TUMOR CLASSIFIERS");
            Console.WriteLine("========================================");
            Console.WriteLine("");

            Console.Write("Enter path to test images folder: ");
            string testImagesPath = Console.ReadLine();

            if (!Directory.Exists(testImagesPath))
            {
                Console.WriteLine("Test images folder not found!");
                return;
            }

            Console.Write("Enter path to test metadata CSV file: ");
            string metadataPath = Console.ReadLine();

            if (!File.Exists(metadataPath))
            {
                Console.WriteLine("Metadata file not found!");
                return;
            }

            try
            {
                Console.WriteLine("Loading test data...");

                // Parse test metadata
                var testData = ParseTumorTestData(testImagesPath, metadataPath);

                if (testData.images.Count == 0)
                {
                    Console.WriteLine("No valid test data found!");
                    return;
                }

                Console.WriteLine($"Testing with {testData.images.Count} tumor images...");

                // Test each image
                int correctTypes = 0;
                int correctGrades = 0;
                int correctLocations = 0;

                for (int i = 0; i < testData.images.Count; i++)
                {
                    var result = tumorClassifier.AnalyzeImage(testData.images[i]);

                    // Check type accuracy
                    if (result.TumorType == testData.types[i])
                        correctTypes++;

                    // Check grade accuracy
                    if (result.TumorGrade == testData.grades[i])
                        correctGrades++;

                    // Check location accuracy
                    if (result.TumorLocation == testData.locations[i])
                        correctLocations++;

                    Console.WriteLine($"Image {i + 1}: Expected Type={testData.types[i]}, Got={result.TumorType} " +
                                    $"| Expected Grade={testData.grades[i]}, Got={result.TumorGrade} " +
                                    $"| Expected Location={testData.locations[i]}, Got={result.TumorLocation}");
                }

                // Calculate accuracies
                double typeAccuracy = (double)correctTypes / testData.images.Count * 100;
                double gradeAccuracy = (double)correctGrades / testData.images.Count * 100;
                double locationAccuracy = (double)correctLocations / testData.images.Count * 100;

                Console.WriteLine("\n========================================");
                Console.WriteLine("         TUMOR CLASSIFIER RESULTS");
                Console.WriteLine("========================================");
                Console.WriteLine($"Type Classification Accuracy:     {typeAccuracy:F2}%");
                Console.WriteLine($"Grade Classification Accuracy:    {gradeAccuracy:F2}%");
                Console.WriteLine($"Location Classification Accuracy: {locationAccuracy:F2}%");
                Console.WriteLine($"Overall Average Accuracy:         {(typeAccuracy + gradeAccuracy + locationAccuracy) / 3:F2}%");
                Console.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during testing: {ex.Message}");
            }
        }

        private static (List<List<double>> images, List<string> types, List<int> grades, List<string> locations)
            ParseTumorTestData(string imagesPath, string metadataPath)
        {
            var images = new List<List<double>>();
            var types = new List<string>();
            var grades = new List<int>();
            var locations = new List<string>();

            var lines = File.ReadAllLines(metadataPath);
            var tumorHelper = new TumorHelper();

            // Skip header if present
            int startLine = lines[0].Contains("filename,type") ? 1 : 0;

            for (int i = startLine; i < lines.Length; i++)
            {
                try
                {
                    var parts = lines[i].Split(',');
                    if (parts.Length < 4) continue;

                    string filename = parts[0].Trim();
                    string type = parts[1].Trim();
                    int grade = int.Parse(parts[2].Trim());
                    string location = parts[3].Trim();

                    // Validate the data
                    if (!Array.Exists(tumorHelper.TumorType, t => t == type)) continue;
                    if (grade < 1 || grade > 4) continue;
                    if (!tumorHelper.TumorLocation.ContainsKey(location)) continue;

                    // Load and process the image
                    string imagePath = Path.Combine(imagesPath, filename);
                    if (!File.Exists(imagePath)) continue;

                    using (var image = new Bitmap(imagePath))
                    {
                        var processedImage = ImageHelper.ProcessImage(image, 128);

                        images.Add(processedImage);
                        types.Add(type);
                        grades.Add(grade);
                        locations.Add(location);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing line {i + 1}: {ex.Message}");
                }
            }

            return (images, types, grades, locations);
        }

        private static void LoadAllWeights(NeuronNetwork network)
        {
            Console.WriteLine("Load Options:");
            Console.WriteLine("1. Load Base Tumor Detection Network");
            Console.WriteLine("2. Load Tumor Classifier Networks");
            Console.WriteLine("3. Load Medical Text Classifier Networks");
            Console.WriteLine("4. Load All Networks (Base + Tumor + Medical Text)");
            Console.WriteLine("5. Return to Main Menu");

            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    LoadWeights(network);
                    break;
                case "2":
                    LoadTumorClassifierWeights();
                    break;
                case "3":
                    LoadMedicalTextWeights();
                    break;
                case "4":
                    // Load all networks
                    Console.WriteLine("Loading base tumor detection network...");
                    LoadWeights(network);
                    
                    Console.WriteLine("Loading tumor classifier networks...");
                    LoadTumorClassifierWeights();
                    
                    Console.WriteLine("Loading medical text classifier networks...");
                    LoadMedicalTextWeights();
                    break;
                case "5":
                    return;
            }
        }

        private static void SaveAllWeights(NeuronNetwork network)
        {
            Console.WriteLine("Save Options:");
            Console.WriteLine("1. Save Base Tumor Detection Network");
            Console.WriteLine("2. Save Tumor Classifier Networks");
            Console.WriteLine("3. Save Medical Text Classifier Networks");
            Console.WriteLine("4. Save All Networks (Base + Tumor + Medical Text)");
            Console.WriteLine("5. Return to Main Menu");

            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SaveWeights(network);
                    break;
                case "2":
                    SaveTumorClassifierWeights();
                    break;
                case "3":
                    SaveMedicalTextWeights();
                    break;
                case "4":
                    // Save all networks
                    Console.WriteLine("Saving base tumor detection network...");
                    SaveWeights(network);
                    
                    Console.WriteLine("Saving tumor classifier networks...");
                    SaveTumorClassifierWeights();
                    
                    Console.WriteLine("Saving medical text classifier networks...");
                    SaveMedicalTextWeights();
                    break;
                case "5":
                    return;
            }
        }

        private static void LoadTumorClassifierWeights()
        {
            Console.Write("Enter base name for tumor classifier networks: ");
            string baseName = Console.ReadLine();

            try
            {
                tumorClassifier.LoadWeights(fileHelper, baseName);
                Console.WriteLine("Successfully loaded tumor classifier networks.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tumor classifier networks: {ex.Message}");
            }
        }

        private static void SaveTumorClassifierWeights()
        {
            Console.Write("Enter a base name for the tumor classifier networks: ");
            string baseName = Console.ReadLine();

            try
            {
                tumorClassifier.SaveWeights(fileHelper, baseName);
                Console.WriteLine("Successfully saved tumor classifier networks.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tumor classifier networks: {ex.Message}");
            }
        }

        private static void LoadMedicalTextWeights()
        {
            Console.Write("Enter a base name for the medical text classifier networks: ");
            string baseName = Console.ReadLine();

            try
            {
                medicalDiagnosisClassifier.LoadWeights(fileHelper, baseName);
                Console.WriteLine("Successfully loaded medical text classifier networks.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading medical text classifier networks: {ex.Message}");
            }
        }

        private static void SaveMedicalTextWeights()
        {
            Console.Write("Enter a base name for the medical text classifier networks: ");
            string baseName = Console.ReadLine();

            try
            {
                medicalDiagnosisClassifier.SaveWeights(fileHelper, baseName);
                Console.WriteLine("Successfully saved medical text classifier networks.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving medical text classifier networks: {ex.Message}");
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
        private static void AnalyzeImageWithAnnotation(NeuronNetwork network)
        {
            Console.WriteLine("Enter the path to the image file to analyze:");
            string? imagePath = Console.ReadLine();

            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                Console.WriteLine($"File not found or invalid path: {imagePath}");
                return;
            }
            try
            {
                if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                {
                    using (var image = new Bitmap(imagePath))
                    {
                        // Use the ImageHelper class to preprocess the image
                        var processedImage = ImageHelper.ProcessImage(image, 128);

                        // Perform comprehensive tumor analysis
                        var result = tumorClassifier.AnalyzeImage(processedImage);

                        // Display the detailed results
                        Console.WriteLine(result.GetSummary());

                        // Enhanced visualization with annotation
                        if (result.HasTumor)
                        {
                            Console.WriteLine("\nTumor detected! Would you like to save an annotated image? (y/n)");
                            string? saveChoice = Console.ReadLine();
                            if (saveChoice?.ToLower() == "y")
                            {
                                // Ask for outline color preference
                                Console.WriteLine("Choose outline color:");
                                Console.WriteLine("1. Auto (based on tumor grade)");
                                Console.WriteLine("2. Red");
                                Console.WriteLine("3. Orange");
                                Console.WriteLine("4. Yellow");
                                Console.WriteLine("5. Green");
                                Console.WriteLine("6. Blue");
                                Console.WriteLine("7. Purple");
                                Console.Write("Enter choice (1-7): ");

                                string? colorChoice = Console.ReadLine();
                                string outlineColor = GetColorFromChoice(colorChoice ?? "1");

                                // Create annotated image
                                string annotatedPath = CreateConsoleAnnotatedImage(imagePath, result, outlineColor);

                                if (!string.IsNullOrEmpty(annotatedPath))
                                {
                                    Console.WriteLine($"Annotated image saved to: {annotatedPath}");

                                    // Also save analysis info
                                    string infoPath = Path.ChangeExtension(annotatedPath, ".txt");
                                    File.WriteAllText(infoPath, result.GetSummary());
                                    Console.WriteLine($"Analysis summary saved to: {infoPath}");
                                }
                                else
                                {
                                    Console.WriteLine("Failed to create annotated image.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nNo tumor detected. Would you like to save the analysis anyway? (y/n)");
                            string? saveChoice = Console.ReadLine();
                            if (saveChoice?.ToLower() == "y")
                            {
                                string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "AnalysisResults");
                                Directory.CreateDirectory(outputFolder);

                                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                                string infoPath = Path.Combine(outputFolder, $"no_tumor_analysis_{timestamp}.txt");
                                File.WriteAllText(infoPath, result.GetSummary());
                                Console.WriteLine($"Analysis summary saved to: {infoPath}");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Image annotation is only supported on Windows platforms in this console version.");
                    Console.WriteLine("Please use the web API version for cross-platform support.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing image: {ex.Message}");
            }
        }

        private static string GetColorFromChoice(string choice)
        {
            return choice switch
            {
                "1" => "Auto",
                "2" => "Red",
                "3" => "Orange",
                "4" => "Yellow",
                "5" => "Green",
                "6" => "Blue",
                "7" => "Purple",
                _ => "Auto"
            };
        }        private static string CreateConsoleAnnotatedImage(string originalImagePath, TumorAnalysisResult analysisResult, string outlineColor)
        {
            try
            {
                string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "AnnotatedImages");
                Directory.CreateDirectory(outputFolder);

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = Path.GetFileNameWithoutExtension(originalImagePath);
                string annotatedPath = Path.Combine(outputFolder, $"annotated_{fileName}_{timestamp}.png");                if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                {
                    using var originalImage = Image.FromFile(originalImagePath);
                    using var annotatedImage = new Bitmap(originalImage.Width, originalImage.Height);
                    using var graphics = Graphics.FromImage(annotatedImage);
                    
                    // Set high quality rendering
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    
                    // Draw the original image
                    graphics.DrawImage(originalImage, 0, 0);                    if (analysisResult.HasTumor)
                    {
                        // Calculate estimated tumor region with improved positioning
                        var tumorRegion = CalculateConsoleEstimatedTumorRegion(originalImage.Width, originalImage.Height, analysisResult);
                        
                        // Get outline color
                        Color penColor = GetConsoleOutlineColor(analysisResult, outlineColor);
                        
                        // Draw enhanced tumor outline with multiple visual elements
                        using var pen = new Pen(penColor, 4);
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        
                        // Draw main outline - use ellipse for more organic tumor shape
                        graphics.DrawEllipse(pen, tumorRegion);
                        
                        // Add inner highlight for better visibility
                        using var innerPen = new Pen(Color.FromArgb(120, Color.White), 2);
                        var innerRegion = new Rectangle(tumorRegion.X + 2, tumorRegion.Y + 2, 
                                                      tumorRegion.Width - 4, tumorRegion.Height - 4);
                        graphics.DrawEllipse(innerPen, innerRegion);
                        
                        // Add semi-transparent fill to highlight the region
                        using var brush = new SolidBrush(Color.FromArgb(40, penColor));
                        graphics.FillEllipse(brush, tumorRegion);
                        
                        // Add confidence indicators around the region
                        DrawConsoleConfidenceIndicators(graphics, tumorRegion, analysisResult.TumorProbability);
                        
                        // Draw analysis information
                        DrawConsoleAnalysisInfo(graphics, tumorRegion, analysisResult);
                    }

                    // Save annotated image
                    annotatedImage.Save(annotatedPath, ImageFormat.Png);
                    return annotatedPath;
                }
                else
                {
                    // For non-Windows platforms, just copy the original file
                    File.Copy(originalImagePath, annotatedPath, true);
                    return annotatedPath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating annotated image: {ex.Message}");
                return string.Empty;
            }
        }        private static Rectangle CalculateConsoleEstimatedTumorRegion(int imageWidth, int imageHeight, TumorAnalysisResult result)
        {
            // Use a seeded random for consistent but varied positioning based on analysis results
            var random = new Random((int)(result.TumorProbability * 10000 + 
                                        result.TypeConfidence * 1000 + 
                                        result.LocationConfidence * 100));
            
            // Get anatomically-based tumor region info
            var regionInfo = GetConsoleAnatomicalRegionInfo(result.TumorType, result.TumorLocation, result.TumorProbability);
            
            // Calculate base position using anatomical positioning
            int baseX = (int)(imageWidth * regionInfo.CenterX);
            int baseY = (int)(imageHeight * regionInfo.CenterY);
            
            // Add confidence-based variance (less confident = more spread)
            double confidenceVariance = (1.0 - result.TumorProbability) * 0.2; // 0-20% variance
            double xVariance = (random.NextDouble() - 0.5) * imageWidth * confidenceVariance;
            double yVariance = (random.NextDouble() - 0.5) * imageHeight * confidenceVariance;
            
            baseX += (int)xVariance;
            baseY += (int)yVariance;
            
            // Calculate size based on multiple factors
            double baseSizeMultiplier = 0.08; // Base 8% of image
            double confidenceMultiplier = result.TumorProbability * 0.12; // Up to 12% more for high confidence
            double gradeMultiplier = result.TumorGrade * 0.02; // 2% per grade level
            
            double totalSizeMultiplier = baseSizeMultiplier + confidenceMultiplier + gradeMultiplier;
            
            int regionWidth = (int)(Math.Min(imageWidth, imageHeight) * totalSizeMultiplier);
            int regionHeight = (int)(regionWidth * regionInfo.AspectRatio);
            
            // Add size variation for realism
            double sizeVariation = 1.0 + (random.NextDouble() - 0.5) * 0.3;
            regionWidth = (int)(regionWidth * sizeVariation);
            regionHeight = (int)(regionHeight * sizeVariation);
            
            // Ensure minimum and maximum sizes
            regionWidth = Math.Max(30, Math.Min(regionWidth, imageWidth / 3));
            regionHeight = Math.Max(30, Math.Min(regionHeight, imageHeight / 3));
            
            // Position with boundary checking
            int padding = 10;
            int x = Math.Max(padding, Math.Min(baseX - regionWidth / 2, imageWidth - regionWidth - padding));
            int y = Math.Max(padding, Math.Min(baseY - regionHeight / 2, imageHeight - regionHeight - padding));
            
            return new Rectangle(x, y, regionWidth, regionHeight);
        }        private static ConsoleRegionInfo GetConsoleAnatomicalRegionInfo(string? tumorType, string? location, double confidence)
        {
            // Enhanced anatomical positioning based on medical knowledge
            return (tumorType?.ToLower(), location?.ToLower()) switch
            {
                // Brain tumors - upper portion but not extreme top
                (var type, var loc) when type?.Contains("glioblastoma") == true || loc?.Contains("brain") == true =>
                    new ConsoleRegionInfo(0.45 + confidence * 0.1, 0.25 + confidence * 0.15, 1.0),
                
                (var type, var loc) when type?.Contains("meningioma") == true =>
                    new ConsoleRegionInfo(0.40 + confidence * 0.2, 0.30 + confidence * 0.1, 0.9),
                
                (var type, var loc) when type?.Contains("pituitary") == true =>
                    new ConsoleRegionInfo(0.48 + confidence * 0.04, 0.35 + confidence * 0.06, 0.8),
                
                // Chest/Lung tumors - middle portion
                (var type, var loc) when loc?.Contains("thorax") == true || loc?.Contains("lung") == true =>
                    new ConsoleRegionInfo(0.35 + confidence * 0.3, 0.45 + confidence * 0.2, 1.3),
                
                (var type, var loc) when loc?.Contains("chest") == true =>
                    new ConsoleRegionInfo(0.4 + confidence * 0.2, 0.5 + confidence * 0.15, 1.2),
                
                // Abdominal tumors - middle to lower portion
                (var type, var loc) when loc?.Contains("abdomen") == true || loc?.Contains("liver") == true =>
                    new ConsoleRegionInfo(0.55 + confidence * 0.2, 0.6 + confidence * 0.15, 1.4),
                
                (var type, var loc) when loc?.Contains("kidney") == true =>
                    new ConsoleRegionInfo(0.25 + confidence * 0.5, 0.55 + confidence * 0.2, 1.1),
                
                (var type, var loc) when loc?.Contains("pancreas") == true =>
                    new ConsoleRegionInfo(0.48 + confidence * 0.04, 0.6 + confidence * 0.1, 0.9),
                
                // Pelvic tumors - lower portion
                (var type, var loc) when loc?.Contains("pelvis") == true =>
                    new ConsoleRegionInfo(0.45 + confidence * 0.1, 0.75 + confidence * 0.1, 1.0),
                
                (var type, var loc) when loc?.Contains("prostate") == true =>
                    new ConsoleRegionInfo(0.48 + confidence * 0.04, 0.8 + confidence * 0.05, 0.9),
                
                (var type, var loc) when loc?.Contains("bladder") == true =>
                    new ConsoleRegionInfo(0.48 + confidence * 0.04, 0.75 + confidence * 0.08, 0.9),
                
                // Breast tumors - upper-middle chest area
                (var type, var loc) when loc?.Contains("breast") == true =>
                    new ConsoleRegionInfo(0.3 + confidence * 0.4, 0.4 + confidence * 0.15, 1.0),
                
                // Neck/Head tumors - upper portion
                (var type, var loc) when loc?.Contains("neck") == true || loc?.Contains("thyroid") == true =>
                    new ConsoleRegionInfo(0.45 + confidence * 0.1, 0.2 + confidence * 0.1, 0.8),
                
                // Bone tumors - variable positioning
                (var type, var loc) when type?.Contains("sarcoma") == true || loc?.Contains("bone") == true =>
                    new ConsoleRegionInfo(0.3 + confidence * 0.4, 0.5 + confidence * 0.3, 1.8),
                
                // Default - center region with better distribution
                _ => new ConsoleRegionInfo(0.45 + confidence * 0.1, 0.5 + confidence * 0.2, 1.0)
            };
        }

        private class ConsoleRegionInfo
        {
            public double CenterX { get; }
            public double CenterY { get; }
            public double AspectRatio { get; }

            public ConsoleRegionInfo(double centerX, double centerY, double aspectRatio)
            {
                CenterX = centerX;
                CenterY = centerY;
                AspectRatio = aspectRatio;
            }
        }

        private static Color GetConsoleOutlineColor(TumorAnalysisResult result, string preferredColor)
        {
            if (preferredColor != "Auto")
            {
                return Color.FromName(preferredColor);
            }
            
            // Automatic color selection based on tumor characteristics
            if (result.TumorGrade >= 4)
            {
                return Color.DarkRed; // High-grade malignant tumors
            }
            else if (result.TumorGrade >= 3)
            {
                return Color.Red; // High-grade tumors
            }
            else if (result.TumorGrade >= 2)
            {
                return Color.Orange; // Intermediate-grade tumors
            }
            else
            {
                return Color.Yellow; // Low-grade tumors
            }
        }        private static void DrawConsoleAnalysisInfo(Graphics graphics, Rectangle tumorRegion, TumorAnalysisResult result)
        {
            if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            {
                using var font = new Font("Arial", 12, FontStyle.Bold);
                using var textBrush = new SolidBrush(Color.White);
                using var backgroundBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
                using var borderPen = new Pen(Color.White, 2);
                
                var lines = new[]
                {
                    $"TUMOR DETECTED: {result.TumorProbability:P1}",
                    $"Type: {result.TumorType}",
                    $"Grade: {result.TumorGrade} ({result.GradeDescription})",
                    $"Location: {result.TumorLocation}",
                    $"Confidence: {result.OverallConfidence:P1}"
                };
                
                int lineHeight = 18;
                int padding = 8;
                int boxWidth = 280;
                int boxHeight = lines.Length * lineHeight + padding * 2;
                
                // Smart positioning - try multiple positions to avoid overlap
                int imageWidth = (int)graphics.ClipBounds.Width;
                int imageHeight = (int)graphics.ClipBounds.Height;
                
                var possiblePositions = new[]
                {
                    // Above tumor region
                    new Point(tumorRegion.X, tumorRegion.Y - boxHeight - 10),
                    // Below tumor region
                    new Point(tumorRegion.X, tumorRegion.Bottom + 10),
                    // Left of tumor region
                    new Point(tumorRegion.X - boxWidth - 10, tumorRegion.Y),
                    // Right of tumor region
                    new Point(tumorRegion.Right + 10, tumorRegion.Y),
                    // Top left corner
                    new Point(10, 10),
                    // Top right corner
                    new Point(imageWidth - boxWidth - 10, 10),
                    // Bottom left corner
                    new Point(10, imageHeight - boxHeight - 10),
                    // Bottom right corner
                    new Point(imageWidth - boxWidth - 10, imageHeight - boxHeight - 10)
                };
                
                // Find the first position that fits within image bounds
                Point infoPosition = new Point(10, 10); // Default fallback
                foreach (var pos in possiblePositions)
                {
                    if (pos.X >= 5 && pos.Y >= 5 && 
                        pos.X + boxWidth <= imageWidth - 5 && 
                        pos.Y + boxHeight <= imageHeight - 5)
                    {
                        infoPosition = pos;
                        break;
                    }
                }
                
                // Draw background with rounded corners effect
                var backgroundRect = new Rectangle(infoPosition.X, infoPosition.Y, boxWidth, boxHeight);
                graphics.FillRectangle(backgroundBrush, backgroundRect);
                graphics.DrawRectangle(borderPen, backgroundRect);
                
                // Draw text with better spacing
                for (int i = 0; i < lines.Length; i++)
                {
                    graphics.DrawString(lines[i], font, textBrush, 
                        infoPosition.X + padding, infoPosition.Y + padding + i * lineHeight);
                }
            }
        }
        
        // Enhanced console methods for improved tumor classifier functionality

        private static void EnhancedAnalyzeImage(NeuronNetwork network)
        {
            Console.WriteLine("=== ENHANCED IMAGE ANALYSIS ===");
            Console.WriteLine("Enter the path to the image file to analyze:");
            string? imagePath = Console.ReadLine();

            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                Console.WriteLine($"File not found: {imagePath}");
                return;
            }

            try
            {
                if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                {
                    using (var image = new Bitmap(imagePath))
                    {
                        // Use the ImageHelper class to preprocess the image
                        var processedImage = ImageHelper.ProcessImage(image, 128);

                        // Configure analysis settings
                        Console.WriteLine("Configure analysis settings:");
                        Console.WriteLine("1. Quick scan (faster, less detailed)");
                        Console.WriteLine("2. Standard analysis (default)");
                        Console.WriteLine("3. Detailed analysis (slower, more comprehensive)");
                        Console.Write("Choose option (1-3): ");
                        
                        string? choice = Console.ReadLine();
                        switch (choice)
                        {
                            case "1":
                                tumorClassifier.DetectionThreshold = 0.6;
                                tumorClassifier.EnableDetailedAnalysis = false;
                                break;
                            case "3":
                                tumorClassifier.DetectionThreshold = 0.3;
                                tumorClassifier.EnableDetailedAnalysis = true;
                                break;
                            default:
                                tumorClassifier.DetectionThreshold = 0.5;
                                tumorClassifier.EnableDetailedAnalysis = true;
                                break;
                        }

                        // Perform comprehensive tumor analysis
                        var result = tumorClassifier.AnalyzeImage(processedImage);

                        // Display the detailed results
                        Console.WriteLine("\n" + new string('=', 60));
                        Console.WriteLine("ENHANCED ANALYSIS RESULTS");
                        Console.WriteLine(new string('=', 60));
                        Console.WriteLine(result.GetSummary());
                        
                        if (result.HasTumor)
                        {
                            Console.WriteLine($"\nAnalysis Timestamp: {result.AnalysisTimestamp}");
                            Console.WriteLine($"Risk Assessment: {result.RiskAssessment}");
                            Console.WriteLine($"Overall Confidence: {result.OverallConfidence:P2}");
                        }

                        // Generate visualization
                        if (result.HasTumor)
                        {
                            Console.WriteLine("\nWould you like to save a detailed report? (y/n)");
                            string? saveChoice = Console.ReadLine();
                            if (saveChoice?.ToLower() == "y")
                            {
                                SaveDetailedReport(result, imagePath);
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Image analysis is only supported on Windows platforms.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing image: {ex.Message}");
            }
        }

        private static void BatchAnalyzeImages(NeuronNetwork network)
        {
            Console.WriteLine("=== BATCH IMAGE ANALYSIS ===");
            Console.WriteLine("Enter the path to the folder containing images:");
            string? folderPath = Console.ReadLine();

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                Console.WriteLine($"Folder not found: {folderPath}");
                return;
            }

            try
            {
                var imageFiles = Directory.GetFiles(folderPath, "*.*")
                    .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                               f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               f.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (imageFiles.Count == 0)
                {
                    Console.WriteLine("No image files found in the specified folder.");
                    return;
                }

                Console.WriteLine($"Found {imageFiles.Count} images. Processing...");

                var imageDataList = new List<List<double>>();
                var filenames = new List<string>();

                // Load and process all images
                if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                {
                    foreach (var imageFile in imageFiles)
                    {
                        try
                        {
                            using (var image = new Bitmap(imageFile))
                            {
                                var processedImage = ImageHelper.ProcessImage(image, 128);
                                imageDataList.Add(processedImage);
                                filenames.Add(Path.GetFileName(imageFile));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing {imageFile}: {ex.Message}");
                        }
                    }

                    // Perform batch analysis
                    var results = tumorClassifier.AnalyzeBatch(imageDataList, (current, total) =>
                    {
                        Console.Write($"\rAnalyzing: {current}/{total} ({(double)current / total:P0})");
                    });

                    Console.WriteLine("\n\n=== BATCH ANALYSIS RESULTS ===");

                    // Display individual results
                    for (int i = 0; i < results.Count; i++)
                    {
                        Console.WriteLine($"\nImage: {filenames[i]}");
                        if (results[i].HasTumor)
                        {
                            Console.WriteLine($"  TUMOR DETECTED - {results[i].TumorProbability:P2}");
                            Console.WriteLine($"  Type: {results[i].TumorType} ({results[i].TypeConfidence:P2})");
                            Console.WriteLine($"  Grade: {results[i].TumorGrade} - {results[i].GradeDescription}");
                            Console.WriteLine($"  Risk: {results[i].RiskAssessment}");
                        }
                        else
                        {
                            Console.WriteLine($"  No tumor detected ({(1 - results[i].TumorProbability):P2} confidence)");
                        }
                    }

                    // Display summary statistics
                    var summary = tumorClassifier.GetBatchSummary(results);
                    Console.WriteLine("\n=== SUMMARY STATISTICS ===");
                    Console.WriteLine($"Total Images Analyzed: {summary.TotalImages}");
                    Console.WriteLine($"Tumors Detected: {summary.TumorsDetected} ({(double)summary.TumorsDetected / summary.TotalImages:P2})");
                    
                    if (summary.TumorsDetected > 0)
                    {
                        Console.WriteLine($"Average Detection Confidence: {summary.AverageConfidence:P2}");
                        Console.WriteLine($"Most Common Type: {summary.MostCommonType ?? "Unknown"}");
                        Console.WriteLine($"High Risk Cases: {summary.HighRiskCases}");
                        
                        Console.WriteLine("\nGrade Distribution:");
                        foreach (var grade in summary.GradeDistribution)
                        {
                            Console.WriteLine($"  Grade {grade.Key}: {grade.Value} cases");
                        }
                    }

                    // Offer to save results
                    Console.WriteLine("\nWould you like to save the batch analysis results? (y/n)");
                    string? saveChoice = Console.ReadLine();
                    if (saveChoice?.ToLower() == "y")
                    {
                        SaveBatchResults(results, filenames, summary, folderPath);
                    }
                }
                else
                {
                    Console.WriteLine("Batch image analysis is only supported on Windows platforms.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during batch analysis: {ex.Message}");
            }
        }

        private static void AdvancedTumorAnalysis(NeuronNetwork network)
        {
            Console.WriteLine("=== ADVANCED TUMOR ANALYSIS ===");
            Console.WriteLine("Enter the path to the image file:");
            string? imagePath = Console.ReadLine();

            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                Console.WriteLine($"File not found: {imagePath}");
                return;
            }

            try
            {
                if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                {
                    using (var image = new Bitmap(imagePath))
                    {
                        // Configure advanced settings
                        Console.WriteLine("\nConfigure advanced analysis parameters:");
                        
                        Console.Write("Detection threshold (0.0-1.0, default 0.5): ");
                        string? thresholdInput = Console.ReadLine();
                        if (double.TryParse(thresholdInput, out double threshold))
                        {
                            tumorClassifier.DetectionThreshold = Math.Max(0.0, Math.Min(1.0, threshold));
                        }

                        Console.Write("Classification threshold (0.0-1.0, default 0.3): ");
                        string? classThresholdInput = Console.ReadLine();
                        if (double.TryParse(classThresholdInput, out double classThreshold))
                        {
                            tumorClassifier.ClassificationThreshold = Math.Max(0.0, Math.Min(1.0, classThreshold));
                        }

                        var processedImage = ImageHelper.ProcessImage(image, 128);
                        var result = tumorClassifier.AnalyzeImage(processedImage);

                        // Display comprehensive results
                        Console.WriteLine("\n" + new string('=', 80));
                        Console.WriteLine("ADVANCED ANALYSIS REPORT");
                        Console.WriteLine(new string('=', 80));
                        Console.WriteLine($"Analysis performed at: {result.AnalysisTimestamp}");
                        Console.WriteLine($"Detection threshold used: {tumorClassifier.DetectionThreshold:F3}");
                        Console.WriteLine($"Classification threshold used: {tumorClassifier.ClassificationThreshold:F3}");
                        Console.WriteLine();
                        Console.WriteLine(result.GetSummary());

                        if (result.HasTumor)
                        {
                            Console.WriteLine("\n--- DETAILED METRICS ---");
                            Console.WriteLine($"Overall Analysis Confidence: {result.OverallConfidence:P2}");
                            Console.WriteLine($"Risk Assessment: {result.RiskAssessment}");
                            
                            // Additional analysis based on type
                            Console.WriteLine("\n--- CLINICAL RECOMMENDATIONS ---");
                            GenerateClinicalRecommendations(result);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Advanced analysis is only supported on Windows platforms.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in advanced analysis: {ex.Message}");
            }
        }

        private static void CompareAnalysisResults(NeuronNetwork network)
        {
            Console.WriteLine("=== COMPARE ANALYSIS RESULTS ===");
            Console.WriteLine("This feature allows you to compare analysis results from multiple images.");
            
            var analysisResults = new List<(string filename, TumorAnalysisResult result)>();

            while (true)
            {
                Console.WriteLine($"\nCurrently have {analysisResults.Count} images for comparison.");
                Console.WriteLine("1. Add image for analysis");
                Console.WriteLine("2. Compare current results");
                Console.WriteLine("3. Clear all results");
                Console.WriteLine("4. Return to main menu");
                Console.Write("Choose option: ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddImageForComparison(network, analysisResults);
                        break;
                    case "2":
                        if (analysisResults.Count >= 2)
                            DisplayComparison(analysisResults);
                        else
                            Console.WriteLine("Need at least 2 images for comparison.");
                        break;
                    case "3":
                        analysisResults.Clear();
                        Console.WriteLine("All results cleared.");
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private static void TrainWithCustomParameters(NeuronNetwork network)
        {
            Console.WriteLine("=== TRAIN WITH CUSTOM PARAMETERS ===");
            Console.WriteLine("This allows you to train the model with custom hyperparameters.");

            try
            {
                Console.WriteLine("Enter the path to the folder containing tumor images:");
                string? tumorFolderPath = Console.ReadLine();

                Console.WriteLine("Enter the path to the folder containing non-tumor images:");
                string? nonTumorFolderPath = Console.ReadLine();

                if (string.IsNullOrEmpty(tumorFolderPath) || string.IsNullOrEmpty(nonTumorFolderPath) ||
                    !Directory.Exists(tumorFolderPath) || !Directory.Exists(nonTumorFolderPath))
                {
                    Console.WriteLine("Invalid folder paths provided.");
                    return;
                }

                // Custom parameters
                Console.Write("Enter number of epochs (default 1000): ");
                string? epochsInput = Console.ReadLine();
                int epochs = int.TryParse(epochsInput, out int e) ? e : 1000;

                Console.Write("Enter learning rate (default 0.01): ");
                string? lrInput = Console.ReadLine();
                double learningRate = double.TryParse(lrInput, out double lr) ? lr : 0.01;

                Console.Write("Enable data augmentation? (y/n): ");
                bool enableAugmentation = Console.ReadLine()?.ToLower() == "y";

                Console.Write("Validation split percentage (default 20): ");
                string? validationInput = Console.ReadLine();
                double validationSplit = double.TryParse(validationInput, out double vs) ? vs / 100.0 : 0.2;

                // Load and process images
                var tumorImages = ImageHelper.LoadImages(tumorFolderPath, 128, enableAugmentation, enableAugmentation ? 2 : 0);
                var nonTumorImages = ImageHelper.LoadImages(nonTumorFolderPath, 128, enableAugmentation, enableAugmentation ? 2 : 0);

                Console.WriteLine($"Loaded {tumorImages.Count} tumor images and {nonTumorImages.Count} non-tumor images");

                // Split data for validation
                var (trainImages, validImages, trainLabels, validLabels) = SplitTrainingData(tumorImages, nonTumorImages, validationSplit);

                Console.WriteLine($"Training set: {trainImages.Count} images");
                Console.WriteLine($"Validation set: {validImages.Count} images");

                // Train with validation
                TrainWithValidation(network, trainImages, trainLabels, validImages, validLabels, epochs, learningRate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in custom training: {ex.Message}");
            }
        }

        private static void ValidateModelPerformance(NeuronNetwork network)
        {
            Console.WriteLine("=== MODEL PERFORMANCE VALIDATION ===");
            Console.WriteLine("Enter the path to the test dataset folder:");
            string? testFolderPath = Console.ReadLine();

            if (string.IsNullOrEmpty(testFolderPath) || !Directory.Exists(testFolderPath))
            {
                Console.WriteLine($"Folder not found: {testFolderPath}");
                return;
            }

            try
            {
                // Look for subfolders indicating ground truth
                var tumorTestPath = Path.Combine(testFolderPath, "tumor");
                var nonTumorTestPath = Path.Combine(testFolderPath, "non_tumor");

                if (!Directory.Exists(tumorTestPath) || !Directory.Exists(nonTumorTestPath))
                {
                    Console.WriteLine("Expected folder structure: test_folder/tumor and test_folder/non_tumor");
                    return;
                }

                var tumorTestImages = ImageHelper.LoadImages(tumorTestPath);
                var nonTumorTestImages = ImageHelper.LoadImages(nonTumorTestPath);

                Console.WriteLine($"Validating with {tumorTestImages.Count} tumor images and {nonTumorTestImages.Count} non-tumor images");

                // Perform validation
                var validationResults = PerformValidation(tumorTestImages, nonTumorTestImages);
                
                Console.WriteLine("\n=== VALIDATION RESULTS ===");
                Console.WriteLine($"Accuracy: {validationResults.Accuracy:P2}");
                Console.WriteLine($"Sensitivity (True Positive Rate): {validationResults.Sensitivity:P2}");
                Console.WriteLine($"Specificity (True Negative Rate): {validationResults.Specificity:P2}");
                Console.WriteLine($"Precision: {validationResults.Precision:P2}");
                Console.WriteLine($"F1 Score: {validationResults.F1Score:P2}");
                Console.WriteLine($"AUC: {validationResults.AUC:F3}");

                Console.WriteLine("\nConfusion Matrix:");
                Console.WriteLine($"True Positives: {validationResults.TruePositives}");
                Console.WriteLine($"False Positives: {validationResults.FalsePositives}");
                Console.WriteLine($"True Negatives: {validationResults.TrueNegatives}");
                Console.WriteLine($"False Negatives: {validationResults.FalseNegatives}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in validation: {ex.Message}");
            }
        }

        private static void ConfigureAnalysisSettings()
        {
            Console.WriteLine("=== CONFIGURE ANALYSIS SETTINGS ===");
            Console.WriteLine($"Current Detection Threshold: {tumorClassifier.DetectionThreshold:F3}");
            Console.WriteLine($"Current Classification Threshold: {tumorClassifier.ClassificationThreshold:F3}");
            Console.WriteLine($"Detailed Analysis Enabled: {tumorClassifier.EnableDetailedAnalysis}");

            Console.WriteLine("\n1. Change detection threshold");
            Console.WriteLine("2. Change classification threshold");
            Console.WriteLine("3. Toggle detailed analysis");
            Console.WriteLine("4. Reset to defaults");
            Console.WriteLine("5. Return to main menu");
            Console.Write("Choose option: ");

            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Enter new detection threshold (0.0-1.0): ");
                    if (double.TryParse(Console.ReadLine(), out double detThreshold))
                    {
                        tumorClassifier.DetectionThreshold = Math.Max(0.0, Math.Min(1.0, detThreshold));
                        Console.WriteLine($"Detection threshold set to {tumorClassifier.DetectionThreshold:F3}");
                    }
                    break;
                case "2":
                    Console.Write("Enter new classification threshold (0.0-1.0): ");
                    if (double.TryParse(Console.ReadLine(), out double classThreshold))
                    {
                        tumorClassifier.ClassificationThreshold = Math.Max(0.0, Math.Min(1.0, classThreshold));
                        Console.WriteLine($"Classification threshold set to {tumorClassifier.ClassificationThreshold:F3}");
                    }
                    break;
                case "3":
                    tumorClassifier.EnableDetailedAnalysis = !tumorClassifier.EnableDetailedAnalysis;
                    Console.WriteLine($"Detailed analysis {(tumorClassifier.EnableDetailedAnalysis ? "enabled" : "disabled")}");
                    break;
                case "4":
                    tumorClassifier.DetectionThreshold = 0.5;
                    tumorClassifier.ClassificationThreshold = 0.3;
                    tumorClassifier.EnableDetailedAnalysis = true;
                    Console.WriteLine("Settings reset to defaults");
                    break;
            }
        }

        private static void ExportAnalysisResults()
        {
            Console.WriteLine("=== EXPORT ANALYSIS RESULTS ===");
            Console.WriteLine("This feature would export previous analysis results to various formats.");
            Console.WriteLine("Implementation depends on storing analysis history in a database or file system.");
            Console.WriteLine("This is a placeholder for future implementation.");
        }

        // Helper methods

        private static void SaveDetailedReport(TumorAnalysisResult result, string imagePath)
        {
            try
            {
                string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
                Directory.CreateDirectory(outputFolder);

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string reportPath = Path.Combine(outputFolder, $"detailed_report_{timestamp}.txt");

                var report = new System.Text.StringBuilder();
                report.AppendLine("DETAILED TUMOR ANALYSIS REPORT");
                report.AppendLine(new string('=', 50));
                report.AppendLine($"Analysis Date: {result.AnalysisTimestamp}");
                report.AppendLine($"Source Image: {Path.GetFileName(imagePath)}");
                report.AppendLine($"Full Path: {imagePath}");
                report.AppendLine();
                report.AppendLine(result.GetSummary());
                report.AppendLine();
                report.AppendLine("DETAILED METRICS:");
                report.AppendLine($"Detection Threshold Used: {tumorClassifier.DetectionThreshold:F3}");
                report.AppendLine($"Classification Threshold Used: {tumorClassifier.ClassificationThreshold:F3}");
                report.AppendLine($"Overall Confidence: {result.OverallConfidence:P2}");

                File.WriteAllText(reportPath, report.ToString());
                Console.WriteLine($"Detailed report saved to: {reportPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving report: {ex.Message}");
            }
        }

        private static void SaveBatchResults(List<TumorAnalysisResult> results, List<string> filenames, BatchAnalysisSummary summary, string folderPath)
        {
            try
            {
                string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "BatchResults");
                Directory.CreateDirectory(outputFolder);

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string csvPath = Path.Combine(outputFolder, $"batch_analysis_{timestamp}.csv");

                using (var writer = new StreamWriter(csvPath))
                {
                    // Write CSV header
                    writer.WriteLine("Filename,HasTumor,TumorProbability,TumorType,TypeConfidence,TumorGrade,GradeDescription,TumorLocation,EstimatedStage,RiskAssessment,OverallConfidence");

                    // Write data rows
                    for (int i = 0; i < results.Count; i++)
                    {
                        var result = results[i];
                        writer.WriteLine($"{filenames[i]},{result.HasTumor},{result.TumorProbability:F4}," +
                                       $"{result.TumorType ?? ""},{result.TypeConfidence:F4},{result.TumorGrade}," +
                                       $"{result.GradeDescription ?? ""},{result.TumorLocation ?? ""},{result.EstimatedStage}," +
                                       $"{result.RiskAssessment ?? ""},{result.OverallConfidence:F4}");
                    }
                }

                // Save summary report
                string summaryPath = Path.Combine(outputFolder, $"batch_summary_{timestamp}.txt");
                var summaryReport = new System.Text.StringBuilder();
                summaryReport.AppendLine("BATCH ANALYSIS SUMMARY REPORT");
                summaryReport.AppendLine(new string('=', 40));
                summaryReport.AppendLine($"Analysis Date: {DateTime.Now}");
                summaryReport.AppendLine($"Source Folder: {folderPath}");
                summaryReport.AppendLine($"Total Images: {summary.TotalImages}");
                summaryReport.AppendLine($"Tumors Detected: {summary.TumorsDetected} ({(double)summary.TumorsDetected / summary.TotalImages:P2})");
                summaryReport.AppendLine($"Average Confidence: {summary.AverageConfidence:P2}");
                summaryReport.AppendLine($"Most Common Type: {summary.MostCommonType ?? "Unknown"}");
                summaryReport.AppendLine($"High Risk Cases: {summary.HighRiskCases}");

                File.WriteAllText(summaryPath, summaryReport.ToString());

                Console.WriteLine($"Results saved to:");
                Console.WriteLine($"  CSV: {csvPath}");
                Console.WriteLine($"  Summary: {summaryPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving batch results: {ex.Message}");
            }
        }

        private static void GenerateClinicalRecommendations(TumorAnalysisResult result)
        {
            if (result.TumorGrade >= 4)
            {
                Console.WriteLine("⚠️  URGENT: High-grade tumor detected. Immediate oncological consultation recommended.");
            }
            else if (result.TumorGrade >= 3)
            {
                Console.WriteLine("⚠️  PRIORITY: Intermediate-grade tumor. Schedule oncological evaluation within 1-2 weeks.");
            }
            else if (result.TumorGrade >= 2)
            {
                Console.WriteLine("📋 FOLLOW-UP: Low-intermediate grade tumor. Consider follow-up imaging in 3-6 months.");
            }
            else
            {
                Console.WriteLine("📋 MONITOR: Low-grade lesion. Regular monitoring recommended.");
            }

            if (result.RiskAssessment == "High Risk")
            {
                Console.WriteLine("🔴 Risk Assessment: HIGH - Consider aggressive treatment options.");
            }
            else if (result.RiskAssessment == "Moderate Risk")
            {
                Console.WriteLine("🟡 Risk Assessment: MODERATE - Standard treatment protocols apply.");
            }
            else
            {
                Console.WriteLine("🟢 Risk Assessment: LOW - Conservative management may be appropriate.");
            }
        }

        private static void AddImageForComparison(NeuronNetwork network, List<(string filename, TumorAnalysisResult result)> analysisResults)
        {
            Console.Write("Enter image path: ");
            string? imagePath = Console.ReadLine();

            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                Console.WriteLine("Invalid image path.");
                return;
            }

            try
            {
                if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                {
                    using (var image = new Bitmap(imagePath))
                    {
                        var processedImage = ImageHelper.ProcessImage(image, 128);
                        var result = tumorClassifier.AnalyzeImage(processedImage);
                        
                        analysisResults.Add((Path.GetFileName(imagePath), result));
                        Console.WriteLine($"Added {Path.GetFileName(imagePath)} to comparison list.");
                        
                        if (result.HasTumor)
                        {
                            Console.WriteLine($"  Tumor detected: {result.TumorProbability:P2} confidence");
                        }
                        else
                        {
                            Console.WriteLine($"  No tumor detected: {(1 - result.TumorProbability):P2} confidence");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing image: {ex.Message}");
            }
        }

        private static void DisplayComparison(List<(string filename, TumorAnalysisResult result)> analysisResults)
        {
            Console.WriteLine("\n=== COMPARISON RESULTS ===");
            Console.WriteLine($"{"Image",-25} {"Tumor",-8} {"Confidence",-12} {"Type",-20} {"Grade",-6} {"Risk",-15}");
            Console.WriteLine(new string('-', 90));

            foreach (var (filename, result) in analysisResults)
            {
                string tumorStatus = result.HasTumor ? "YES" : "NO";
                string confidence = $"{result.TumorProbability:P1}";
                string type = result.TumorType ?? "N/A";
                string grade = result.HasTumor ? result.TumorGrade.ToString() : "N/A";
                string risk = result.RiskAssessment ?? "N/A";

                Console.WriteLine($"{filename,-25} {tumorStatus,-8} {confidence,-12} {type,-20} {grade,-6} {risk,-15}");
            }

            // Statistical comparison
            var tumorCount = analysisResults.Count(x => x.result.HasTumor);
            var avgConfidence = analysisResults.Where(x => x.result.HasTumor).Average(x => x.result.TumorProbability);
            var highRiskCount = analysisResults.Count(x => x.result.RiskAssessment == "High Risk");

            Console.WriteLine(new string('-', 90));
            Console.WriteLine($"Summary: {tumorCount}/{analysisResults.Count} tumors detected");
            if (tumorCount > 0)
            {
                Console.WriteLine($"Average tumor confidence: {avgConfidence:P2}");
                Console.WriteLine($"High risk cases: {highRiskCount}");
            }
        }

        private static (List<List<double>> trainImages, List<List<double>> validImages, 
                       List<List<double>> trainLabels, List<List<double>> validLabels) 
            SplitTrainingData(List<List<double>> tumorImages, List<List<double>> nonTumorImages, double validationSplit)
        {
            var random = new Random();
            
            // Shuffle and split tumor images
            var shuffledTumor = tumorImages.OrderBy(x => random.Next()).ToList();
            int tumorValidCount = (int)(shuffledTumor.Count * validationSplit);
            var tumorTrain = shuffledTumor.Skip(tumorValidCount).ToList();
            var tumorValid = shuffledTumor.Take(tumorValidCount).ToList();

            // Shuffle and split non-tumor images
            var shuffledNonTumor = nonTumorImages.OrderBy(x => random.Next()).ToList();
            int nonTumorValidCount = (int)(shuffledNonTumor.Count * validationSplit);
            var nonTumorTrain = shuffledNonTumor.Skip(nonTumorValidCount).ToList();
            var nonTumorValid = shuffledNonTumor.Take(nonTumorValidCount).ToList();

            // Combine training data
            var trainImages = new List<List<double>>();
            var trainLabels = new List<List<double>>();
            trainImages.AddRange(tumorTrain);
            trainImages.AddRange(nonTumorTrain);
            trainLabels.AddRange(tumorTrain.Select(x => new List<double> { 1.0 }));
            trainLabels.AddRange(nonTumorTrain.Select(x => new List<double> { 0.0 }));

            // Combine validation data
            var validImages = new List<List<double>>();
            var validLabels = new List<List<double>>();
            validImages.AddRange(tumorValid);
            validImages.AddRange(nonTumorValid);
            validLabels.AddRange(tumorValid.Select(x => new List<double> { 1.0 }));
            validLabels.AddRange(nonTumorValid.Select(x => new List<double> { 0.0 }));

            return (trainImages, validImages, trainLabels, validLabels);
        }

        private static void TrainWithValidation(NeuronNetwork network, List<List<double>> trainImages, 
            List<List<double>> trainLabels, List<List<double>> validImages, List<List<double>> validLabels, 
            int epochs, double learningRate)
        {
            Console.WriteLine("Starting training with validation...");
            
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                // Train on one epoch
                network.Train(trainImages, trainLabels, 1, learningRate);

                // Validate every 100 epochs
                if (epoch % 100 == 0 || epoch == epochs - 1)
                {
                    double validationAccuracy = CalculateAccuracy(network, validImages, validLabels);
                    Console.WriteLine($"Epoch {epoch + 1}/{epochs}, Validation Accuracy: {validationAccuracy:P2}");
                }
            }
        }

        private static double CalculateAccuracy(NeuronNetwork network, List<List<double>> images, List<List<double>> labels)
        {
            int correct = 0;
            for (int i = 0; i < images.Count; i++)
            {
                var output = network.FeedForward(images[i]);
                bool predicted = output[0] >= 0.5;
                bool actual = labels[i][0] >= 0.5;
                if (predicted == actual) correct++;
            }
            return (double)correct / images.Count;
        }

        private static ValidationResults PerformValidation(List<List<double>> tumorImages, List<List<double>> nonTumorImages)
        {
            var results = new ValidationResults();
            
            // Test tumor images (positive cases)
            foreach (var image in tumorImages)
            {
                var result = tumorClassifier.AnalyzeImage(image);
                if (result.HasTumor)
                    results.TruePositives++;
                else
                    results.FalseNegatives++;
            }

            // Test non-tumor images (negative cases)
            foreach (var image in nonTumorImages)
            {
                var result = tumorClassifier.AnalyzeImage(image);
                if (!result.HasTumor)
                    results.TrueNegatives++;
                else
                    results.FalsePositives++;
            }

            // Calculate metrics
            results.CalculateMetrics();
            return results;
        }

        public class ValidationResults
        {
            public int TruePositives { get; set; }
            public int FalsePositives { get; set; }
            public int TrueNegatives { get; set; }
            public int FalseNegatives { get; set; }

            public double Accuracy { get; private set; }
            public double Sensitivity { get; private set; }
            public double Specificity { get; private set; }
            public double Precision { get; private set; }
            public double F1Score { get; private set; }
            public double AUC { get; private set; }

            public void CalculateMetrics()
            {
                int total = TruePositives + TrueNegatives + FalsePositives + FalseNegatives;
                
                Accuracy = (double)(TruePositives + TrueNegatives) / total;
                Sensitivity = (double)TruePositives / (TruePositives + FalseNegatives);
                Specificity = (double)TrueNegatives / (TrueNegatives + FalsePositives);
                Precision = TruePositives + FalsePositives > 0 ? (double)TruePositives / (TruePositives + FalsePositives) : 0;
                F1Score = Precision + Sensitivity > 0 ? 2 * (Precision * Sensitivity) / (Precision + Sensitivity) : 0;
                AUC = (Sensitivity + Specificity) / 2; // Simplified AUC calculation
            }
        }

        private static void DrawConsoleConfidenceIndicators(Graphics graphics, Rectangle region, double confidence)
        {
            // Draw small dots around the tumor region to indicate confidence level
            if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            {
                int dotCount = (int)(confidence * 12); // 0-12 dots based on confidence
                using var dotBrush = new SolidBrush(Color.FromArgb(180, Color.Yellow));
                
                for (int i = 0; i < dotCount; i++)
                {
                    double angle = (2 * Math.PI * i) / 12;
                    int dotX = region.X + region.Width / 2 + (int)((region.Width / 2 + 20) * Math.Cos(angle)) - 3;
                    int dotY = region.Y + region.Height / 2 + (int)((region.Height / 2 + 20) * Math.Sin(angle)) - 3;
                    
                    graphics.FillEllipse(dotBrush, dotX, dotY, 6, 6);
                }
            }
        }
    }
}