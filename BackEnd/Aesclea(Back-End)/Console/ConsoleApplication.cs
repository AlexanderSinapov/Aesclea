using System;
using System.Drawing;
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
        private static MedicalDiagnosisClassifier medicalDiagnosisClassifier;

        public static void Run()
        {
            // Initialize the file helper
            fileHelper.OpenFolder();

            // Network with layers sized for 128x128 grayscale images (16384 inputs)
            var network = new NeuronNetwork(new int[] { 16384, 256, 64, 16, 1 });

            // Initialize the tumor classifier with the base network
            tumorClassifier = new TumorClassifier(network);

            // Initialize medical diagnosis classifier
            medicalDiagnosisClassifier = new MedicalDiagnosisClassifier();

            // Initialize recurrent network with similar architecture (keep for compatibility)
            var recurrentNetwork = new RecurrentNeuralNetwork(new int[] { 256, 64, 16 }, new int[] { 256, 64, 16 }, 500);

            while (true)
            {
                // Display menu options
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("         AESCLEA MEDICAL AI SYSTEM");
                Console.WriteLine("========================================");
                Console.WriteLine("Select an option:");
                Console.WriteLine("");
                Console.WriteLine("IMAGE ANALYSIS:");
                Console.WriteLine("1. Analyze Image (Detect & Classify Tumor)");
                Console.WriteLine("2. Train Base Tumor Detector");
                Console.WriteLine("3. Train Tumor Classifiers");
                Console.WriteLine("4. Mass Test Base Network");
                Console.WriteLine("5. Test Tumor Classifiers");
                Console.WriteLine("");
                Console.WriteLine("MEDICAL TEXT ANALYSIS:");
                Console.WriteLine("6. Analyze Medical Text");
                Console.WriteLine("7. Train Medical Text Classifier");
                Console.WriteLine("8. Test Medical Text Classifier");
                Console.WriteLine("9. Batch Analyze Medical Records");
                Console.WriteLine("");
                Console.WriteLine("SYSTEM MANAGEMENT:");
                Console.WriteLine("10. Load Weights");
                Console.WriteLine("11. Save Weights");
                Console.WriteLine("12. Convert Old Weights to New");
                Console.WriteLine("13. Recurrent Network Options");
                Console.WriteLine("14. Exit");
                Console.WriteLine("========================================");
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
                        TestTumorClassifiers();
                        break;
                    case "6":
                        AnalyzeMedicalText();
                        break;
                    case "7":
                        TrainMedicalTextClassifier();
                        break;
                    case "8":
                        TestMedicalTextClassifier();
                        break;
                    case "9":
                        BatchAnalyzeMedicalRecords();
                        break;
                    case "10":
                        LoadAllWeights(network);
                        break;
                    case "11":
                        SaveAllWeights(network);
                        break;
                    case "12":
                        ConvertWeights();
                        break;
                    case "13":
                        //RecurrentNeuralNetworkOptions(recurrentNetwork);
                        break;
                    case "14":
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
    }
}