// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        }        /// <summary>
        /// Analyzes medical text and provides enhanced diagnostic insights
        /// </summary>
        /// <param name="medicalText">Raw medical text (symptoms, history, notes)</param>
        /// <returns>Comprehensive diagnostic analysis with enhanced features</returns>
        public MedicalDiagnosisResult AnalyzeMedicalText(string medicalText)
        {
            // Process text into numerical features with enhanced processing
            var textFeatures = TextHelper.ProcessMedicalText(medicalText, 512);

            // Get predictions from all networks
            var diagnosticOutput = diagnosticNetwork.FeedForward(textFeatures);
            var severityOutput = severityNetwork.FeedForward(textFeatures);
            var urgencyOutput = urgencyNetwork.FeedForward(textFeatures);

            // Create enhanced result object
            var result = new MedicalDiagnosisResult
            {
                InputText = medicalText,
                ProcessedOn = DateTime.Now
            };

            // Enhanced diagnostic analysis
            result.PrimaryDiagnosticCategory = GetTopDiagnosticCategory(diagnosticOutput);
            result.DiagnosticConfidences = GetDiagnosticConfidences(diagnosticOutput);

            // Enhanced severity analysis with context
            result.SeverityLevel = GetSeverityLevel(severityOutput);
            result.SeverityDescription = diagnosisHelper.GetSeverityDescription(result.SeverityLevel);
            result.SeverityConfidence = severityOutput[result.SeverityLevel - 1];

            // Enhanced urgency analysis with red flag detection
            result.UrgencyLevel = GetUrgencyLevel(urgencyOutput);
            result.UrgencyDescription = diagnosisHelper.GetUrgencyDescription(result.UrgencyLevel);
            result.UrgencyConfidence = urgencyOutput[result.UrgencyLevel];

            // Enhanced medical information extraction
            result.ExtractedInfo = ExtractEnhancedMedicalInfo(medicalText);

            // Generate enhanced recommendations with clinical context
            result.Recommendations = GenerateEnhancedRecommendations(result);

            // Add clinical decision support features
            result.ExtractedInfo["clinical_alerts"] = GenerateClinicalAlerts(result, medicalText);
            result.ExtractedInfo["differential_diagnosis"] = GenerateDifferentialDiagnosis(result);
            result.ExtractedInfo["suggested_tests"] = GenerateSuggestedTests(result);
            result.ExtractedInfo["risk_factors"] = ExtractRiskFactors(medicalText);

            return result;
        }

        /// <summary>
        /// Extract enhanced medical information with clinical context
        /// </summary>
        private Dictionary<string, object> ExtractEnhancedMedicalInfo(string medicalText)
        {
            var info = TextHelper.ExtractMedicalInfo(medicalText);
            
            // Add enhanced clinical features
            info["symptom_clusters"] = IdentifySymptomClusters(medicalText);
            info["temporal_patterns"] = ExtractTemporalPatterns(medicalText);
            info["functional_impact"] = AssessFunctionalImpact(medicalText);
            info["psychosocial_factors"] = ExtractPsychosocialFactors(medicalText);
            info["medication_mentions"] = ExtractMedicationMentions(medicalText);
            info["vital_signs_analysis"] = AnalyzeVitalSigns(medicalText);
            
            return info;
        }

        /// <summary>
        /// Generate enhanced clinical recommendations
        /// </summary>
        private List<string> GenerateEnhancedRecommendations(MedicalDiagnosisResult result)
        {
            var recommendations = new List<string>();
            
            // Basic category recommendations
            recommendations.AddRange(diagnosisHelper.GetCategoryRecommendations(result.PrimaryDiagnosticCategory));
            
            // Severity-based recommendations
            switch (result.SeverityLevel)
            {
                case 1:
                    recommendations.Add("Consider outpatient management with scheduled follow-up");
                    recommendations.Add("Patient education on symptom monitoring");
                    break;
                case 2:
                    recommendations.Add("Consider same-day evaluation or urgent care visit");
                    recommendations.Add("Provide clear instructions for symptom worsening");
                    break;
                case 3:
                    recommendations.Add("Recommend prompt medical evaluation within 24 hours");
                    recommendations.Add("Consider emergency department if symptoms worsen");
                    break;
                case 4:
                case 5:
                    recommendations.Add("URGENT: Immediate medical evaluation required");
                    recommendations.Add("Consider emergency department presentation");
                    recommendations.Add("Monitor vital signs closely");
                    break;
            }
            
            // Urgency-based recommendations
            if (result.UrgencyLevel >= 1)
            {
                recommendations.Add("Ensure patient has emergency contact information");
                recommendations.Add("Consider activating emergency protocols if available");
            }
            
            // Red flag symptom recommendations
            if (result.ExtractedInfo.ContainsKey("clinical_alerts"))
            {
                var alerts = result.ExtractedInfo["clinical_alerts"] as List<string>;
                if (alerts?.Count > 0)
                {
                    recommendations.Add("⚠️ RED FLAG SYMPTOMS DETECTED - Immediate evaluation required");
                }
            }
            
            return recommendations.Distinct().ToList();
        }

        /// <summary>
        /// Generate clinical alerts for red flag symptoms
        /// </summary>
        private List<string> GenerateClinicalAlerts(MedicalDiagnosisResult result, string medicalText)
        {
            var alerts = new List<string>();
            var lowerText = medicalText.ToLower();
            
            // Cardiovascular alerts
            if (lowerText.Contains("chest pain") || lowerText.Contains("cardiac"))
            {
                if (lowerText.Contains("crushing") || lowerText.Contains("pressure") || lowerText.Contains("radiating"))
                    alerts.Add("Possible acute coronary syndrome - Consider ECG and cardiac enzymes");
            }
            
            // Neurological alerts
            if (lowerText.Contains("headache"))
            {
                if (lowerText.Contains("sudden") || lowerText.Contains("worst") || lowerText.Contains("thunderclap"))
                    alerts.Add("Possible subarachnoid hemorrhage - Consider urgent neuroimaging");
            }
            
            if (lowerText.Contains("weakness") || lowerText.Contains("numbness"))
            {
                if (lowerText.Contains("one side") || lowerText.Contains("facial") || lowerText.Contains("speech"))
                    alerts.Add("Possible stroke - Consider immediate neurological evaluation");
            }
            
            // Respiratory alerts
            if (lowerText.Contains("shortness") || lowerText.Contains("breathing"))
            {
                if (lowerText.Contains("sudden") || lowerText.Contains("severe"))
                    alerts.Add("Possible pulmonary embolism or pneumothorax - Consider chest imaging");
            }
            
            // Gastrointestinal alerts
            if (lowerText.Contains("abdominal pain"))
            {
                if (lowerText.Contains("severe") || lowerText.Contains("rigid") || lowerText.Contains("rebound"))
                    alerts.Add("Possible acute abdomen - Consider surgical consultation");
            }
            
            return alerts;
        }

        /// <summary>
        /// Generate differential diagnosis suggestions
        /// </summary>
        private List<string> GenerateDifferentialDiagnosis(MedicalDiagnosisResult result)
        {
            var symptoms = new List<string>();
            if (result.ExtractedInfo.ContainsKey("symptoms"))
            {
                symptoms = result.ExtractedInfo["symptoms"] as List<string> ?? new List<string>();
            }
            
            return diagnosisHelper.GetDifferentialDiagnosis(result.PrimaryDiagnosticCategory, symptoms);
        }

        /// <summary>
        /// Generate suggested diagnostic tests
        /// </summary>
        private List<string> GenerateSuggestedTests(MedicalDiagnosisResult result)
        {
            var tests = diagnosisHelper.GetSuggestedTests(result.PrimaryDiagnosticCategory);
            
            // Add severity-based test recommendations
            if (result.SeverityLevel >= 4)
            {
                tests.Add("Complete blood count (CBC)");
                tests.Add("Basic metabolic panel (BMP)");
                tests.Add("Vital signs monitoring");
            }
            
            return tests.Distinct().ToList();
        }

        /// <summary>
        /// Identify symptom clusters in medical text
        /// </summary>
        private Dictionary<string, List<string>> IdentifySymptomClusters(string medicalText)
        {
            var clusters = new Dictionary<string, List<string>>();
            var lowerText = medicalText.ToLower();
            
            var symptomClusters = new Dictionary<string, List<string>>
            {
                ["Constitutional"] = new List<string> { "fever", "fatigue", "weakness", "weight loss", "malaise" },
                ["Cardiovascular"] = new List<string> { "chest pain", "palpitations", "shortness of breath", "edema" },
                ["Respiratory"] = new List<string> { "cough", "wheeze", "dyspnea", "sputum" },
                ["Gastrointestinal"] = new List<string> { "nausea", "vomiting", "diarrhea", "constipation", "abdominal pain" },
                ["Neurological"] = new List<string> { "headache", "dizziness", "weakness", "numbness", "confusion" },
                ["Musculoskeletal"] = new List<string> { "joint pain", "muscle pain", "stiffness", "swelling" }
            };
            
            foreach (var cluster in symptomClusters)
            {
                var foundSymptoms = cluster.Value.Where(symptom => lowerText.Contains(symptom)).ToList();
                if (foundSymptoms.Count > 0)
                {
                    clusters[cluster.Key] = foundSymptoms;
                }
            }
            
            return clusters;
        }

        /// <summary>
        /// Extract temporal patterns from medical text
        /// </summary>
        private Dictionary<string, string> ExtractTemporalPatterns(string medicalText)
        {
            var patterns = new Dictionary<string, string>();
            var lowerText = medicalText.ToLower();
            
            if (lowerText.Contains("sudden") || lowerText.Contains("acute"))
                patterns["onset"] = "Acute";
            else if (lowerText.Contains("gradual") || lowerText.Contains("slowly"))
                patterns["onset"] = "Gradual";
            else if (lowerText.Contains("chronic") || lowerText.Contains("long-term"))
                patterns["onset"] = "Chronic";
            
            if (lowerText.Contains("worse") || lowerText.Contains("worsening"))
                patterns["progression"] = "Worsening";
            else if (lowerText.Contains("better") || lowerText.Contains("improving"))
                patterns["progression"] = "Improving";
            else if (lowerText.Contains("stable") || lowerText.Contains("unchanged"))
                patterns["progression"] = "Stable";
            
            return patterns;
        }

        /// <summary>
        /// Assess functional impact from medical text
        /// </summary>
        private Dictionary<string, string> AssessFunctionalImpact(string medicalText)
        {
            var impact = new Dictionary<string, string>();
            var lowerText = medicalText.ToLower();
            
            if (lowerText.Contains("unable") || lowerText.Contains("cannot"))
                impact["mobility"] = "Severely limited";
            else if (lowerText.Contains("difficulty") || lowerText.Contains("hard"))
                impact["mobility"] = "Moderately limited";
            else if (lowerText.Contains("some trouble") || lowerText.Contains("mild difficulty"))
                impact["mobility"] = "Mildly limited";
            
            if (lowerText.Contains("work") || lowerText.Contains("job"))
            {
                if (lowerText.Contains("unable to work") || lowerText.Contains("off work"))
                    impact["work"] = "Unable to work";
                else if (lowerText.Contains("modified") || lowerText.Contains("light duty"))
                    impact["work"] = "Modified duties";
            }
            
            return impact;
        }

        /// <summary>
        /// Extract psychosocial factors
        /// </summary>
        private List<string> ExtractPsychosocialFactors(string medicalText)
        {
            var factors = new List<string>();
            var lowerText = medicalText.ToLower();
            
            if (lowerText.Contains("stress") || lowerText.Contains("anxiety"))
                factors.Add("Psychological stress");
            if (lowerText.Contains("family") || lowerText.Contains("caregiver"))
                factors.Add("Family support considerations");
            if (lowerText.Contains("work") || lowerText.Contains("occupation"))
                factors.Add("Occupational factors");
            if (lowerText.Contains("financial") || lowerText.Contains("insurance"))
                factors.Add("Financial concerns");
            
            return factors;
        }

        /// <summary>
        /// Extract medication mentions
        /// </summary>
        private List<string> ExtractMedicationMentions(string medicalText)
        {
            var medications = new List<string>();
            var commonMeds = new List<string>
            {
                "aspirin", "ibuprofen", "acetaminophen", "lisinopril", "metformin", "atorvastatin",
                "amlodipine", "metoprolol", "omeprazole", "albuterol", "prednisone", "warfarin"
            };
            
            var lowerText = medicalText.ToLower();
            medications.AddRange(commonMeds.Where(med => lowerText.Contains(med)));
            
            return medications;
        }

        /// <summary>
        /// Analyze vital signs mentioned in text
        /// </summary>
        private Dictionary<string, string> AnalyzeVitalSigns(string medicalText)
        {
            var vitals = new Dictionary<string, string>();
            
            // Blood pressure pattern
            var bpMatch = Regex.Match(medicalText, @"(\d{2,3})/(\d{2,3})", RegexOptions.IgnoreCase);
            if (bpMatch.Success)
            {
                int systolic = int.Parse(bpMatch.Groups[1].Value);
                int diastolic = int.Parse(bpMatch.Groups[2].Value);
                
                if (systolic >= 180 || diastolic >= 120)
                    vitals["blood_pressure"] = $"Hypertensive crisis: {systolic}/{diastolic}";
                else if (systolic >= 140 || diastolic >= 90)
                    vitals["blood_pressure"] = $"Hypertensive: {systolic}/{diastolic}";
                else if (systolic < 90)
                    vitals["blood_pressure"] = $"Hypotensive: {systolic}/{diastolic}";
                else
                    vitals["blood_pressure"] = $"Normal: {systolic}/{diastolic}";
            }
            
            // Heart rate pattern
            var hrMatch = Regex.Match(medicalText, @"hr\s*(\d{2,3})", RegexOptions.IgnoreCase);
            if (hrMatch.Success)
            {
                int hr = int.Parse(hrMatch.Groups[1].Value);
                if (hr > 100)
                    vitals["heart_rate"] = $"Tachycardic: {hr}";
                else if (hr < 60)
                    vitals["heart_rate"] = $"Bradycardic: {hr}";
                else
                    vitals["heart_rate"] = $"Normal: {hr}";
            }
            
            // Temperature pattern
            var tempMatch = Regex.Match(medicalText, @"temp(?:erature)?\s*(\d{2,3}\.?\d?)", RegexOptions.IgnoreCase);
            if (tempMatch.Success)
            {
                double temp = double.Parse(tempMatch.Groups[1].Value);
                if (temp > 100.4)
                    vitals["temperature"] = $"Febrile: {temp}°F";
                else if (temp < 96)
                    vitals["temperature"] = $"Hypothermic: {temp}°F";
                else
                    vitals["temperature"] = $"Normal: {temp}°F";
            }
            
            return vitals;
        }

        /// <summary>
        /// Extract risk factors from medical text
        /// </summary>
        private List<string> ExtractRiskFactors(string medicalText)
        {
            var riskFactors = new List<string>();
            var lowerText = medicalText.ToLower();
            
            var knownRiskFactors = new Dictionary<string, string>
            {
                ["smoking"] = "Tobacco use",
                ["diabetes"] = "Diabetes mellitus",
                ["hypertension"] = "Hypertension",
                ["obesity"] = "Obesity",
                ["family history"] = "Positive family history",
                ["alcohol"] = "Alcohol use",
                ["sedentary"] = "Sedentary lifestyle",
                ["stress"] = "Chronic stress"
            };
            
            foreach (var factor in knownRiskFactors)
            {
                if (lowerText.Contains(factor.Key))
                {
                    riskFactors.Add(factor.Value);
                }
            }
            
            return riskFactors;
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
        public List<string> Recommendations { get; set; }        /// <summary>
        /// Returns an enhanced formatted summary of the diagnostic analysis
        /// </summary>
        public string GetSummary()
        {
            var summary = $"═══════════════════════════════════════════════════════\n";
            summary += $"           ENHANCED MEDICAL DIAGNOSIS ANALYSIS\n";
            summary += $"═══════════════════════════════════════════════════════\n";
            summary += $"Processed: {ProcessedOn:yyyy-MM-dd HH:mm:ss}\n\n";
            
            summary += $"🏥 PRIMARY CATEGORY: {PrimaryDiagnosticCategory}\n";
            summary += $"⚠️  SEVERITY: Level {SeverityLevel} - {SeverityDescription} (confidence: {SeverityConfidence * 100:F1}%)\n";
            summary += $"🚨 URGENCY: {UrgencyDescription} (confidence: {UrgencyConfidence * 100:F1}%)\n\n";

            // Clinical alerts
            if (ExtractedInfo.ContainsKey("clinical_alerts") && ExtractedInfo["clinical_alerts"] is List<string> alerts && alerts.Count > 0)
            {
                summary += "🚨 CLINICAL ALERTS:\n";
                foreach (var alert in alerts)
                {
                    summary += $"   ⚠️  {alert}\n";
                }
                summary += "\n";
            }

            summary += "📊 TOP DIAGNOSTIC POSSIBILITIES:\n";
            foreach (var diagnostic in DiagnosticConfidences.Take(5))
            {
                summary += $"   • {diagnostic.Key}: {diagnostic.Value * 100:F1}%\n";
            }

            // Enhanced symptom display with clustering
            if (ExtractedInfo.ContainsKey("symptoms") && ExtractedInfo["symptoms"] is List<string> symptoms && symptoms.Count > 0)
            {
                summary += $"\n🔍 IDENTIFIED SYMPTOMS: {string.Join(", ", symptoms)}\n";
            }

            // Symptom clusters
            if (ExtractedInfo.ContainsKey("symptom_clusters") && ExtractedInfo["symptom_clusters"] is Dictionary<string, List<string>> clusters && clusters.Count > 0)
            {
                summary += "\n📋 SYMPTOM CLUSTERS:\n";
                foreach (var cluster in clusters)
                {
                    summary += $"   • {cluster.Key}: {string.Join(", ", cluster.Value)}\n";
                }
            }

            // Vital signs analysis
            if (ExtractedInfo.ContainsKey("vital_signs_analysis") && ExtractedInfo["vital_signs_analysis"] is Dictionary<string, string> vitals && vitals.Count > 0)
            {
                summary += "\n💓 VITAL SIGNS ANALYSIS:\n";
                foreach (var vital in vitals)
                {
                    summary += $"   • {vital.Key.Replace("_", " ").ToUpper()}: {vital.Value}\n";
                }
            }

            // Temporal patterns
            if (ExtractedInfo.ContainsKey("temporal_patterns") && ExtractedInfo["temporal_patterns"] is Dictionary<string, string> temporal && temporal.Count > 0)
            {
                summary += "\n⏱️  TEMPORAL PATTERNS:\n";
                foreach (var pattern in temporal)
                {
                    summary += $"   • {pattern.Key.ToUpper()}: {pattern.Value}\n";
                }
            }

            // Functional impact
            if (ExtractedInfo.ContainsKey("functional_impact") && ExtractedInfo["functional_impact"] is Dictionary<string, string> functional && functional.Count > 0)
            {
                summary += "\n🏃 FUNCTIONAL IMPACT:\n";
                foreach (var impact in functional)
                {
                    summary += $"   • {impact.Key.ToUpper()}: {impact.Value}\n";
                }
            }

            // Risk factors
            if (ExtractedInfo.ContainsKey("risk_factors") && ExtractedInfo["risk_factors"] is List<string> riskFactors && riskFactors.Count > 0)
            {
                summary += $"\n⚠️  RISK FACTORS: {string.Join(", ", riskFactors)}\n";
            }

            // Medications
            if (ExtractedInfo.ContainsKey("medication_mentions") && ExtractedInfo["medication_mentions"] is List<string> medications && medications.Count > 0)
            {
                summary += $"\n💊 MEDICATIONS MENTIONED: {string.Join(", ", medications)}\n";
            }

            // Differential diagnosis
            if (ExtractedInfo.ContainsKey("differential_diagnosis") && ExtractedInfo["differential_diagnosis"] is List<string> differentials && differentials.Count > 0)
            {
                summary += "\n🔬 DIFFERENTIAL DIAGNOSIS:\n";
                foreach (var differential in differentials.Take(5))
                {
                    summary += $"   • {differential}\n";
                }
            }

            // Suggested tests
            if (ExtractedInfo.ContainsKey("suggested_tests") && ExtractedInfo["suggested_tests"] is List<string> tests && tests.Count > 0)
            {
                summary += "\n🧪 SUGGESTED TESTS:\n";
                foreach (var test in tests.Take(5))
                {
                    summary += $"   • {test}\n";
                }
            }

            // Recommendations
            if (Recommendations?.Count > 0)
            {
                summary += "\n📝 CLINICAL RECOMMENDATIONS:\n";
                foreach (var recommendation in Recommendations)
                {
                    summary += $"   • {recommendation}\n";
                }
            }

            summary += "\n═══════════════════════════════════════════════════════\n";
            summary += "   ⚠️  This analysis is for educational purposes only.\n";
            summary += "   Always consult with healthcare professionals for\n";
            summary += "   actual medical diagnosis and treatment decisions.\n";
            summary += "═══════════════════════════════════════════════════════\n";

            return summary;
        }
    }
}