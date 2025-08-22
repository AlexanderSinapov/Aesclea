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
            Console.WriteLine("=== ENHANCED TUMOR CLASSIFIER DEMO ===");
            Console.WriteLine("This demo showcases the new enhanced features.\n");

            // Initialize the classifier
            InitializeClassifier();

            // Demo different features
            DemoConfigurableThresholds();
            DemoBatchAnalysis();
            DemoAdvancedMetrics();
            DemoRiskAssessment();

            Console.WriteLine("\n=== DEMO COMPLETED ===");
            Console.WriteLine("The enhanced tumor classifier now provides:");
            Console.WriteLine("✓ Configurable detection and classification thresholds");
            Console.WriteLine("✓ Batch processing capabilities");
            Console.WriteLine("✓ Advanced confidence metrics");
            Console.WriteLine("✓ Risk assessment calculations");
            Console.WriteLine("✓ Detailed analysis timestamps");
            Console.WriteLine("✓ Clinical recommendations");
            Console.WriteLine("✓ Batch summary statistics");
        }

        private static void InitializeClassifier()
        {
            Console.WriteLine("1. Initializing Enhanced Tumor Classifier...");
            
            // Create a base network for tumor detection
            var baseNetwork = new NeuronNetwork(new int[] { 16384, 256, 64, 16, 1 });
            
            // Initialize the enhanced classifier
            tumorClassifier = new TumorClassifier(baseNetwork);
            
            Console.WriteLine("   ✓ Base network created with 16,384 input neurons");
            Console.WriteLine("   ✓ Specialized classification networks initialized");
            Console.WriteLine("   ✓ Default thresholds set (Detection: 0.5, Classification: 0.3)");
            Console.WriteLine();
        }

        private static void DemoConfigurableThresholds()
        {
            Console.WriteLine("2. Demonstrating Configurable Thresholds...");
            
            if (tumorClassifier == null) return;

            // Show default settings
            Console.WriteLine($"   Default Detection Threshold: {tumorClassifier.DetectionThreshold}");
            Console.WriteLine($"   Default Classification Threshold: {tumorClassifier.ClassificationThreshold}");
            Console.WriteLine($"   Detailed Analysis Enabled: {tumorClassifier.EnableDetailedAnalysis}");

            // Demonstrate threshold adjustment
            Console.WriteLine("\n   Adjusting for high-sensitivity screening:");
            tumorClassifier.DetectionThreshold = 0.3;
            tumorClassifier.ClassificationThreshold = 0.2;
            Console.WriteLine($"   → Detection Threshold: {tumorClassifier.DetectionThreshold} (more sensitive)");
            Console.WriteLine($"   → Classification Threshold: {tumorClassifier.ClassificationThreshold} (more inclusive)");

            Console.WriteLine("\n   Adjusting for high-specificity diagnosis:");
            tumorClassifier.DetectionThreshold = 0.7;
            tumorClassifier.ClassificationThreshold = 0.5;
            Console.WriteLine($"   → Detection Threshold: {tumorClassifier.DetectionThreshold} (more specific)");
            Console.WriteLine($"   → Classification Threshold: {tumorClassifier.ClassificationThreshold} (more strict)");

            // Reset to defaults
            tumorClassifier.DetectionThreshold = 0.5;
            tumorClassifier.ClassificationThreshold = 0.3;
            Console.WriteLine("\n   ✓ Thresholds reset to defaults for remaining demos");
            Console.WriteLine();
        }

        private static void DemoBatchAnalysis()
        {
            Console.WriteLine("3. Demonstrating Batch Analysis Capabilities...");
            
            if (tumorClassifier == null) return;

            // Simulate multiple image data (normally these would be real processed images)
            var simulatedImageData = GenerateSimulatedImageData(5);
            
            Console.WriteLine($"   Processing batch of {simulatedImageData.Count} images...");

            // Perform batch analysis with progress callback
            var results = tumorClassifier.AnalyzeBatch(simulatedImageData, (current, total) =>
            {
                double percentage = (double)current / total * 100;
                Console.Write($"\r   Progress: {current}/{total} ({percentage:F0}%)");
            });

            Console.WriteLine("\n\n   Batch Analysis Results:");
            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                Console.WriteLine($"   Image {i + 1}: {(result.HasTumor ? "TUMOR DETECTED" : "No tumor")} " +
                                $"(Confidence: {result.TumorProbability:P1})");
                if (result.HasTumor)
                {
                    Console.WriteLine($"            Type: {result.TumorType}, Grade: {result.TumorGrade}, Risk: {result.RiskAssessment}");
                }
            }

            // Generate and display batch summary
            var summary = tumorClassifier.GetBatchSummary(results);
            Console.WriteLine("\n   Batch Summary:");
            Console.WriteLine($"   → Total Images: {summary.TotalImages}");
            Console.WriteLine($"   → Tumors Detected: {summary.TumorsDetected} ({(double)summary.TumorsDetected / summary.TotalImages:P1})");
            Console.WriteLine($"   → Average Confidence: {summary.AverageConfidence:P1}");
            Console.WriteLine($"   → High Risk Cases: {summary.HighRiskCases}");
            Console.WriteLine($"   → Most Common Type: {summary.MostCommonType ?? "None"}");
            Console.WriteLine();
        }

        private static void DemoAdvancedMetrics()
        {
            Console.WriteLine("4. Demonstrating Advanced Metrics...");
            
            if (tumorClassifier == null) return;

            // Simulate a single image analysis
            var simulatedImage = GenerateSimulatedImageData(1)[0];
            var result = tumorClassifier.AnalyzeImage(simulatedImage);

            Console.WriteLine("   Enhanced Analysis Result:");
            Console.WriteLine($"   → Analysis Timestamp: {result.AnalysisTimestamp}");
            Console.WriteLine($"   → Overall Confidence: {result.OverallConfidence:P2}");
            Console.WriteLine($"   → Risk Assessment: {result.RiskAssessment}");
            
            if (result.HasTumor)
            {
                Console.WriteLine($"   → Tumor Detection: {result.TumorProbability:P2}");
                Console.WriteLine($"   → Type Classification: {result.TumorType} ({result.TypeConfidence:P2})");
                Console.WriteLine($"   → Grade Assessment: Grade {result.TumorGrade} ({result.GradeConfidence:P2})");
                Console.WriteLine($"   → Location Analysis: {result.TumorLocation} ({result.LocationConfidence:P2})");
                Console.WriteLine($"   → Stage Estimation: Stage {result.EstimatedStage} - {result.StageDescription}");
            }
            
            Console.WriteLine();
        }

        private static void DemoRiskAssessment()
        {
            Console.WriteLine("5. Demonstrating Risk Assessment System...");
            
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
                
                Console.WriteLine($"   Scenario: {description}");
                Console.WriteLine($"   → Risk Level: {result.RiskAssessment}");
                Console.WriteLine($"   → Recommended Action: {GetRecommendedAction(result.RiskAssessment)}");
                Console.WriteLine();
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
