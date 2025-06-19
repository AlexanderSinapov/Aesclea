using System;
using System.Collections.Generic;
using System.Linq;
using Aesclea_Back_End_.AIModel.Helpers;

namespace Aesclea_Back_End_.AIModel
{
    public class MedicalDiagnosisClassifier
    {
        private NeuronNetwork diagnosticNetwork; // Main diagnostic network
        private NeuronNetwork severityNetwork; // Severity assessment network
        private NeuronNetwork urgencyNetwork; // Urgency classification network
        private MedicalDiagnosisHelper diagnosisHelper; // Helper with medical knowledge

        public MedicalDiagnosisClassifier()
        {
            this.diagnosisHelper = new MedicalDiagnosisHelper();

            // Initialize networks
            // Diagnostic network - classifies into major diagnostic categories
            this.diagnosticNetwork = new NeuronNetwork(new int[] { 512, 256, 128, 64, diagnosisHelper.DiagnosticCategories.Length });

            // Severity network - assesses severity level (1-5 scale)
            this.severityNetwork = new NeuronNetwork(new int[] { 512, 128, 32, 8, 5 });

            // Urgency network - determines urgency level (immediate, urgent, routine)
            this.urgencyNetwork = new NeuronNetwork(new int[] { 512, 128, 32, 8, 3 });
        }

        /// <summary>
        /// Analyzes medical text and provides diagnostic insights
        /// </summary>
        /// <param name="medicalText">Raw medical text (symptoms, history, notes)</param>
        /// <returns>Comprehensive diagnostic analysis</returns>
        public MedicalDiagnosisResult AnalyzeMedicalText(string medicalText)
        {
            // Process text into numerical features
            var textFeatures = TextHelper.ProcessMedicalText(medicalText, 512);

            // Get predictions from all networks
            var diagnosticOutput = diagnosticNetwork.FeedForward(textFeatures);
            var severityOutput = severityNetwork.FeedForward(textFeatures);
            var urgencyOutput = urgencyNetwork.FeedForward(textFeatures);

            // Create result object
            var result = new MedicalDiagnosisResult
            {
                InputText = medicalText,
                ProcessedOn = DateTime.Now
            };

            // Analyze diagnostic categories
            result.PrimaryDiagnosticCategory = GetTopDiagnosticCategory(diagnosticOutput);
            result.DiagnosticConfidences = GetDiagnosticConfidences(diagnosticOutput);

            // Analyze severity
            result.SeverityLevel = GetSeverityLevel(severityOutput);
            result.SeverityDescription = diagnosisHelper.GetSeverityDescription(result.SeverityLevel);
            result.SeverityConfidence = severityOutput[result.SeverityLevel - 1];

            // Analyze urgency
            result.UrgencyLevel = GetUrgencyLevel(urgencyOutput);
            result.UrgencyDescription = diagnosisHelper.GetUrgencyDescription(result.UrgencyLevel);
            result.UrgencyConfidence = urgencyOutput[result.UrgencyLevel];

            // Extract additional medical information
            result.ExtractedInfo = TextHelper.ExtractMedicalInfo(medicalText);

            // Generate recommendations
            result.Recommendations = GenerateRecommendations(result);

            return result;
        }

        /// <summary>
        /// Trains the diagnostic networks using labeled medical text data
        /// </summary>
        public void TrainDiagnosticNetworks(
            List<string> medicalTexts,
            List<string> diagnosticCategories,
            List<int> severityLevels,
            List<int> urgencyLevels,
            int epochs,
            double learningRate)
        {
            Console.WriteLine("Processing medical texts for training...");
            
            // Convert texts to features
            var inputs = new List<List<double>>();
            foreach (var text in medicalTexts)
            {
                var features = TextHelper.ProcessMedicalText(text, 512);
                inputs.Add(features);
            }

            Console.WriteLine($"Processed {inputs.Count} medical texts into feature vectors.");

            // Train diagnostic network
            Console.WriteLine("Training diagnostic classification network...");
            TrainDiagnosticNetwork(inputs, diagnosticCategories, epochs, learningRate);

            // Train severity network
            Console.WriteLine("Training severity assessment network...");
            TrainSeverityNetwork(inputs, severityLevels, epochs, learningRate);

            // Train urgency network
            Console.WriteLine("Training urgency classification network...");
            TrainUrgencyNetwork(inputs, urgencyLevels, epochs, learningRate);

            Console.WriteLine("Medical diagnosis network training complete!");
        }

        private void TrainDiagnosticNetwork(List<List<double>> inputs, List<string> categories, int epochs, double learningRate)
        {
            var outputs = new List<List<double>>();

            // Create one-hot encoded outputs for diagnostic categories
            foreach (string category in categories)
            {
                var output = new List<double>(new double[diagnosisHelper.DiagnosticCategories.Length]);
                int index = Array.IndexOf(diagnosisHelper.DiagnosticCategories, category);
                if (index >= 0)
                    output[index] = 1.0;
                outputs.Add(output);
            }

            diagnosticNetwork.Train(inputs, outputs, epochs, learningRate);
        }

        private void TrainSeverityNetwork(List<List<double>> inputs, List<int> severityLevels, int epochs, double learningRate)
        {
            var outputs = new List<List<double>>();

            // Create one-hot encoded outputs for severity levels (1-5)
            foreach (int level in severityLevels)
            {
                var output = new List<double>(new double[5]);
                if (level >= 1 && level <= 5)
                    output[level - 1] = 1.0;
                outputs.Add(output);
            }

            severityNetwork.Train(inputs, outputs, epochs, learningRate);
        }

        private void TrainUrgencyNetwork(List<List<double>> inputs, List<int> urgencyLevels, int epochs, double learningRate)
        {
            var outputs = new List<List<double>>();

            // Create one-hot encoded outputs for urgency levels (0-2)
            foreach (int level in urgencyLevels)
            {
                var output = new List<double>(new double[3]);
                if (level >= 0 && level <= 2)
                    output[level] = 1.0;
                outputs.Add(output);
            }

            urgencyNetwork.Train(inputs, outputs, epochs, learningRate);
        }

        private string GetTopDiagnosticCategory(List<double> diagnosticOutput)
        {
            int maxIndex = 0;
            double maxValue = diagnosticOutput[0];

            for (int i = 1; i < diagnosticOutput.Count; i++)
            {
                if (diagnosticOutput[i] > maxValue)
                {
                    maxValue = diagnosticOutput[i];
                    maxIndex = i;
                }
            }

            return diagnosisHelper.DiagnosticCategories[maxIndex];
        }

        private Dictionary<string, double> GetDiagnosticConfidences(List<double> diagnosticOutput)
        {
            var confidences = new Dictionary<string, double>();

            for (int i = 0; i < Math.Min(diagnosticOutput.Count, diagnosisHelper.DiagnosticCategories.Length); i++)
            {
                confidences[diagnosisHelper.DiagnosticCategories[i]] = diagnosticOutput[i];
            }

            return confidences.OrderByDescending(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private int GetSeverityLevel(List<double> severityOutput)
        {
            int maxIndex = 0;
            double maxValue = severityOutput[0];

            for (int i = 1; i < severityOutput.Count; i++)
            {
                if (severityOutput[i] > maxValue)
                {
                    maxValue = severityOutput[i];
                    maxIndex = i;
                }
            }

            return maxIndex + 1; // Convert 0-based index to 1-based severity level
        }

        private int GetUrgencyLevel(List<double> urgencyOutput)
        {
            int maxIndex = 0;
            double maxValue = urgencyOutput[0];

            for (int i = 1; i < urgencyOutput.Count; i++)
            {
                if (urgencyOutput[i] > maxValue)
                {
                    maxValue = urgencyOutput[i];
                    maxIndex = i;
                }
            }

            return maxIndex; // 0 = routine, 1 = urgent, 2 = immediate
        }

        private List<string> GenerateRecommendations(MedicalDiagnosisResult result)
        {
            var recommendations = new List<string>();

            // Base recommendations on urgency level
            switch (result.UrgencyLevel)
            {
                case 2: // Immediate
                    recommendations.Add("IMMEDIATE MEDICAL ATTENTION REQUIRED");
                    recommendations.Add("Contact emergency services or go to emergency department");
                    recommendations.Add("Do not delay treatment");
                    break;
                case 1: // Urgent
                    recommendations.Add("Seek medical attention within 24 hours");
                    recommendations.Add("Contact healthcare provider immediately");
                    recommendations.Add("Monitor symptoms closely");
                    break;
                case 0: // Routine
                    recommendations.Add("Schedule appointment with healthcare provider");
                    recommendations.Add("Continue monitoring symptoms");
                    recommendations.Add("Follow up as needed");
                    break;
            }

            // Add category-specific recommendations
            var categoryRecommendations = diagnosisHelper.GetCategoryRecommendations(result.PrimaryDiagnosticCategory);
            recommendations.AddRange(categoryRecommendations);

            // Add severity-specific recommendations
            if (result.SeverityLevel >= 4)
            {
                recommendations.Add("Consider hospitalization or intensive treatment");
                recommendations.Add("Frequent monitoring required");
            }
            else if (result.SeverityLevel >= 3)
            {
                recommendations.Add("Close medical supervision recommended");
                recommendations.Add("Regular follow-up appointments");
            }

            return recommendations;
        }

        /// <summary>
        /// Save all diagnostic network weights
        /// </summary>
        public void SaveWeights(FileHelper fileHelper, string baseName)
        {
            fileHelper.SaveNeuralData(diagnosticNetwork.GetNeuralNetworkData(), baseName + "_diagnostic");
            fileHelper.SaveNeuralData(severityNetwork.GetNeuralNetworkData(), baseName + "_severity");
            fileHelper.SaveNeuralData(urgencyNetwork.GetNeuralNetworkData(), baseName + "_urgency");
        }

        /// <summary>
        /// Load all diagnostic network weights
        /// </summary>
        public void LoadWeights(FileHelper fileHelper, string baseName)
        {
            var diagnosticData = fileHelper.GetNeuralData(baseName + "_diagnostic_NeuralData.wbn");
            if (diagnosticData != null)
                diagnosticNetwork.SetNeuralNetworkData(diagnosticData);

            var severityData = fileHelper.GetNeuralData(baseName + "_severity_NeuralData.wbn");
            if (severityData != null)
                severityNetwork.SetNeuralNetworkData(severityData);

            var urgencyData = fileHelper.GetNeuralData(baseName + "_urgency_NeuralData.wbn");
            if (urgencyData != null)
                urgencyNetwork.SetNeuralNetworkData(urgencyData);
        }

        /// <summary>
        /// Evaluate the diagnostic networks on test data
        /// </summary>
        public Dictionary<string, double> EvaluateNetworks(
            List<string> testTexts,
            List<string> actualCategories,
            List<int> actualSeverities,
            List<int> actualUrgencies)
        {
            var results = new Dictionary<string, double>();
            
            int diagnosticCorrect = 0;
            int severityCorrect = 0;
            int urgencyCorrect = 0;

            for (int i = 0; i < testTexts.Count; i++)
            {
                var analysis = AnalyzeMedicalText(testTexts[i]);

                // Check diagnostic accuracy
                if (analysis.PrimaryDiagnosticCategory == actualCategories[i])
                    diagnosticCorrect++;

                // Check severity accuracy
                if (analysis.SeverityLevel == actualSeverities[i])
                    severityCorrect++;

                // Check urgency accuracy
                if (analysis.UrgencyLevel == actualUrgencies[i])
                    urgencyCorrect++;
            }

            results["diagnostic_accuracy"] = (double)diagnosticCorrect / testTexts.Count;
            results["severity_accuracy"] = (double)severityCorrect / testTexts.Count;
            results["urgency_accuracy"] = (double)urgencyCorrect / testTexts.Count;
            results["overall_accuracy"] = (diagnosticCorrect + severityCorrect + urgencyCorrect) / (3.0 * testTexts.Count);

            return results;
        }
    }

    /// <summary>
    /// Stores the results of medical text analysis
    /// </summary>
    public class MedicalDiagnosisResult
    {
        public string InputText { get; set; }
        public DateTime ProcessedOn { get; set; }

        // Diagnostic classification
        public string PrimaryDiagnosticCategory { get; set; }
        public Dictionary<string, double> DiagnosticConfidences { get; set; }

        // Severity assessment
        public int SeverityLevel { get; set; } // 1-5 scale
        public string SeverityDescription { get; set; }
        public double SeverityConfidence { get; set; }

        // Urgency classification
        public int UrgencyLevel { get; set; } // 0=routine, 1=urgent, 2=immediate
        public string UrgencyDescription { get; set; }
        public double UrgencyConfidence { get; set; }

        // Additional extracted information
        public Dictionary<string, object> ExtractedInfo { get; set; }

        // Recommendations
        public List<string> Recommendations { get; set; }

        /// <summary>
        /// Returns a formatted summary of the diagnostic analysis
        /// </summary>
        public string GetSummary()
        {
            var summary = $"MEDICAL DIAGNOSIS ANALYSIS\n";
            summary += $"Processed: {ProcessedOn:yyyy-MM-dd HH:mm:ss}\n\n";
            
            summary += $"PRIMARY CATEGORY: {PrimaryDiagnosticCategory}\n";
            summary += $"SEVERITY: Level {SeverityLevel} - {SeverityDescription} (confidence: {SeverityConfidence * 100:F1}%)\n";
            summary += $"URGENCY: {UrgencyDescription} (confidence: {UrgencyConfidence * 100:F1}%)\n\n";

            summary += "TOP DIAGNOSTIC POSSIBILITIES:\n";
            foreach (var diagnostic in DiagnosticConfidences.Take(5))
            {
                summary += $"  • {diagnostic.Key}: {diagnostic.Value * 100:F1}%\n";
            }

            if (ExtractedInfo.ContainsKey("symptoms") && ExtractedInfo["symptoms"] is List<string> symptoms && symptoms.Count > 0)
            {
                summary += $"\nIDENTIFIED SYMPTOMS: {string.Join(", ", symptoms)}\n";
            }

            if (Recommendations?.Count > 0)
            {
                summary += "\nRECOMMENDATIONS:\n";
                foreach (var recommendation in Recommendations)
                {
                    summary += $"  • {recommendation}\n";
                }
            }

            return summary;
        }
    }
}