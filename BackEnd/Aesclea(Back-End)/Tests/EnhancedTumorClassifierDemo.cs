using Aesclea_Back_End_.AIModel;
using Aesclea_Back_End_.AIModel.Helpers;
using System.Drawing;

namespace Aesclea_Back_End_.Tests
{
    /// <summary>
    /// Test class to demonstrate enhanced tumor classifier functionality
    /// </summary>
    public class EnhancedTumorClassifierDemo
    {
        private static TumorClassifier? tumorClassifier;

        public static void RunDemo()
        {
            global::System.Console.WriteLine("=== ENHANCED TUMOR CLASSIFIER DEMO ===");
            global::System.Console.WriteLine("This demo showcases the new enhanced features.\n");

            // Initialize the classifier
            InitializeClassifier();

            // Demo different features
            DemoConfigurableThresholds();
            DemoBatchAnalysis();
            DemoAdvancedMetrics();
            DemoRiskAssessment();

            global::System.Console.WriteLine("\n=== DEMO COMPLETED ===");
            global::System.Console.WriteLine("The enhanced tumor classifier now provides:");
            global::System.Console.WriteLine("✓ Configurable detection and classification thresholds");
            global::System.Console.WriteLine("✓ Batch processing capabilities");
            global::System.Console.WriteLine("✓ Advanced confidence metrics");
            global::System.Console.WriteLine("✓ Risk assessment calculations");
            global::System.Console.WriteLine("✓ Detailed analysis timestamps");
            global::System.Console.WriteLine("✓ Clinical recommendations");
            global::System.Console.WriteLine("✓ Batch summary statistics");
        }

        private static void InitializeClassifier()
        {
            global::System.Console.WriteLine("1. Initializing Enhanced Tumor Classifier...");

            // Create a base network for tumor detection
            var baseNetwork = new NeuronNetwork(new int[] { 16384, 256, 64, 16, 1 });
            
            // Initialize the enhanced classifier
            tumorClassifier = new TumorClassifier(baseNetwork);
            
            global::System.Console.WriteLine("   ✓ Base network created with 16,384 input neurons");
            global::System.Console.WriteLine("   ✓ Specialized classification networks initialized");
            global::System.Console.WriteLine("   ✓ Default thresholds set (Detection: 0.5, Classification: 0.3)");
            global::System.Console.WriteLine();
        }

        private static void DemoConfigurableThresholds()
        {
            global::System.Console.WriteLine("2. Demonstrating Configurable Thresholds...");
            
            if (tumorClassifier == null) return;

            // Show default settings
            global::System.Console.WriteLine($"   Default Detection Threshold: {tumorClassifier.DetectionThreshold}");
            global::System.Console.WriteLine($"   Default Classification Threshold: {tumorClassifier.ClassificationThreshold}");
            global::System.Console.WriteLine($"   Detailed Analysis Enabled: {tumorClassifier.EnableDetailedAnalysis}");

            // Demonstrate threshold adjustment
            global::System.Console.WriteLine("\n   Adjusting for high-sensitivity screening:");
            tumorClassifier.DetectionThreshold = 0.3;
            tumorClassifier.ClassificationThreshold = 0.2;
            global::System.Console.WriteLine($"   → Detection Threshold: {tumorClassifier.DetectionThreshold} (more sensitive)");
            global::System.Console.WriteLine($"   → Classification Threshold: {tumorClassifier.ClassificationThreshold} (more inclusive)");

            global::System.Console.WriteLine("\n   Adjusting for high-specificity diagnosis:");
            tumorClassifier.DetectionThreshold = 0.7;
            tumorClassifier.ClassificationThreshold = 0.5;
            global::System.Console.WriteLine($"   → Detection Threshold: {tumorClassifier.DetectionThreshold} (more specific)");
            global::System.Console.WriteLine($"   → Classification Threshold: {tumorClassifier.ClassificationThreshold} (more strict)");

            // Reset to defaults
            tumorClassifier.DetectionThreshold = 0.5;
            tumorClassifier.ClassificationThreshold = 0.3;
            global::System.Console.WriteLine("\n   ✓ Thresholds reset to defaults for remaining demos");
            global::System.Console.WriteLine();
        }

        private static void DemoBatchAnalysis()
        {
            global::System.Console.WriteLine("3. Demonstrating Batch Analysis Capabilities...");
            
            if (tumorClassifier == null) return;

            // Simulate multiple image data (normally these would be real processed images)
            var simulatedImageData = GenerateSimulatedImageData(5);

            global::System.Console.WriteLine($"   Processing batch of {simulatedImageData.Count} images...");

            // Perform batch analysis with progress callback
            var results = tumorClassifier.AnalyzeBatch(simulatedImageData, (current, total) =>
            {
                double percentage = (double)current / total * 100;
                global::System.Console.Write($"\r   Progress: {current}/{total} ({percentage:F0}%)");
            });

            global::System.Console.WriteLine("\n\n   Batch Analysis Results:");
            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                global::System.Console.WriteLine($"   Image {i + 1}: {(result.HasTumor ? "TUMOR DETECTED" : "No tumor")} " +
                                $"(Confidence: {result.TumorProbability:P1})");
                if (result.HasTumor)
                {
                    global::System.Console.WriteLine($"            Type: {result.TumorType}, Grade: {result.TumorGrade}, Risk: {result.RiskAssessment}");
                }
            }

            // Generate and display batch summary
            var summary = tumorClassifier.GetBatchSummary(results);
            global::System.Console.WriteLine("\n   Batch Summary:");
            global::System.Console.WriteLine($"   → Total Images: {summary.TotalImages}");
            global::System.Console.WriteLine($"   → Tumors Detected: {summary.TumorsDetected} ({(double)summary.TumorsDetected / summary.TotalImages:P1})");
            global::System.Console.WriteLine($"   → Average Confidence: {summary.AverageConfidence:P1}");
            global::System.Console.WriteLine($"   → High Risk Cases: {summary.HighRiskCases}");
            global::System.Console.WriteLine($"   → Most Common Type: {summary.MostCommonType ?? "None"}");
            global::System.Console.WriteLine();
        }

        private static void DemoAdvancedMetrics()
        {
            global::System.Console.WriteLine("4. Demonstrating Advanced Metrics...");
            
            if (tumorClassifier == null) return;

            // Simulate a single image analysis
            var simulatedImage = GenerateSimulatedImageData(1)[0];
            var result = tumorClassifier.AnalyzeImage(simulatedImage);

            global::System.Console.WriteLine("   Enhanced Analysis Result:");
            global::System.Console.WriteLine($"   → Analysis Timestamp: {result.AnalysisTimestamp}");
            global::System.Console.WriteLine($"   → Overall Confidence: {result.OverallConfidence:P2}");
            global::System.Console.WriteLine($"   → Risk Assessment: {result.RiskAssessment}");

            if (result.HasTumor)
            {
                global::System.Console.WriteLine($"   → Tumor Detection: {result.TumorProbability:P2}");
                global::System.Console.WriteLine($"   → Type Classification: {result.TumorType} ({result.TypeConfidence:P2})");
                global::System.Console.WriteLine($"   → Grade Assessment: Grade {result.TumorGrade} ({result.GradeConfidence:P2})");
                global::System.Console.WriteLine($"   → Location Analysis: {result.TumorLocation} ({result.LocationConfidence:P2})");
                global::System.Console.WriteLine($"   → Stage Estimation: Stage {result.EstimatedStage} - {result.StageDescription}");
            }

            global::System.Console.WriteLine();
        }

        private static void DemoRiskAssessment()
        {
            global::System.Console.WriteLine("5. Demonstrating Risk Assessment System...");
            
            if (tumorClassifier == null) return;

            // Simulate different risk scenarios
            var riskScenarios = new[]
            {
                ("Low-grade benign lesion", GenerateSpecificTumorData(1, 0.6)),
                ("Moderate-grade tumor", GenerateSpecificTumorData(2, 0.75)),
                ("High-grade malignant tumor", GenerateSpecificTumorData(4, 0.95)),
                ("Suspicious lesion", GenerateSpecificTumorData(3, 0.85))
            };

            foreach (var (description, imageData) in riskScenarios)
            {
                var result = tumorClassifier.AnalyzeImage(imageData);
                
                global::System.Console.WriteLine($"   Scenario: {description}");
                global::System.Console.WriteLine($"   → Risk Level: {result.RiskAssessment}");
                global::System.Console.WriteLine($"   → Recommended Action: {GetRecommendedAction(result.RiskAssessment)}");
                global::System.Console.WriteLine();
            }
        }

        private static List<List<double>> GenerateSimulatedImageData(int count)
        {
            var random = new Random();
            var imageDataList = new List<List<double>>();

            for (int i = 0; i < count; i++)
            {
                var imageData = new List<double>();
                
                // Generate 16,384 random values (128x128 image)
                for (int j = 0; j < 16384; j++)
                {
                    imageData.Add(random.NextDouble());
                }
                
                imageDataList.Add(imageData);
            }

            return imageDataList;
        }

        private static List<double> GenerateSpecificTumorData(int grade, double probability)
        {
            var random = new Random();
            var imageData = new List<double>();

            // Generate data that would likely result in specific tumor characteristics
            // This is simulated - in real usage, this would be actual processed image data
            for (int i = 0; i < 16384; i++)
            {
                // Bias the data based on desired grade and probability
                double baseValue = random.NextDouble();
                double biasedValue = baseValue * probability + (grade / 4.0) * 0.1;
                imageData.Add(Math.Min(1.0, biasedValue));
            }

            return imageData;
        }

        private static string GetRecommendedAction(string? riskAssessment)
        {
            return riskAssessment switch
            {
                "High Risk" => "Immediate oncological consultation required",
                "Moderate Risk" => "Schedule follow-up within 2 weeks",
                "Low-Moderate Risk" => "Monitor with imaging in 3-6 months",
                "Low Risk" => "Routine follow-up as clinically indicated",
                _ => "Consult with radiologist for interpretation"
            };
        }
    }
}
