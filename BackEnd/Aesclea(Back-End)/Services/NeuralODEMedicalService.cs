// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov
// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aesclea_Back_End_.AIModel.NeuralODE
{
    /// <summary>
    /// Service layer for Neural ODE medical analysis
    /// Provides high-level interface for medical predictions
    /// </summary>
    public class NeuralODEMedicalService
    {
        private NeuralODELLM model;
        private MedicalTokenizer tokenizer;
        private NeuralODETrainer trainer;
        
        // Model configuration
        private const int HIDDEN_DIM = 128;
        private const int EMBEDDING_DIM = 64;
        private const int UNCERTAINTY_DIM = 32;
        private const int MAX_SEQUENCE_LENGTH = 256;
        
        public bool IsModelTrained { get; private set; } = false;

        public NeuralODEMedicalService()
        {
            tokenizer = new MedicalTokenizer();
            
            // Initialize model with medical-specific configuration
            var kernel = new GaussianMixtureKernel(
                sigmas: new double[] { 0.1, 0.5, 1.0 },  // Multi-scale attention
                alphas: new double[] { 0.5, 0.3, 0.2 }
            );
            
            model = new NeuralODELLM(
                vocabSize: tokenizer.VocabSize,
                hiddenDim: HIDDEN_DIM,
                embeddingDim: EMBEDDING_DIM,
                uncertaintyDim: UNCERTAINTY_DIM,
                kernel: kernel,
                seed: 42
            );
            
            trainer = new NeuralODETrainer(model)
            {
                LearningRate = 0.001,
                Lambda1 = 0.01,  // Uncertainty weight
                Lambda2 = 0.001, // Compute cost weight
                MaxComputeCost = 2.0
            };
        }

        /// <summary>
        /// Analyze medical text and provide diagnosis prediction
        /// </summary>
        public async Task<MedicalAnalysisResult> AnalyzeMedicalText(string clinicalText, 
            DiagnosisCategory category = DiagnosisCategory.General)
        {
            return await Task.Run(() =>
            {
                // Tokenize input
                var tokens = tokenizer.Tokenize(clinicalText);
                var paddedTokens = tokenizer.PadSequence(tokens, MAX_SEQUENCE_LENGTH);
                
                // Extract medical entities
                var entities = tokenizer.ExtractEntities(clinicalText);
                
                // Run prediction with different time horizons for different analyses
                double T = category switch
                {
                    DiagnosisCategory.Emergency => 0.5,  // Fast inference
                    DiagnosisCategory.Routine => 1.0,    // Normal depth
                    DiagnosisCategory.Complex => 2.0,    // Deep analysis
                    _ => 1.0
                };
                
                var prediction = model.PredictMedical(paddedTokens, T);
                
                // Map to diagnosis
                var diagnosis = MapPredictionToDiagnosis(prediction, category);
                
                // Create result
                return new MedicalAnalysisResult
                {
                    InputText = clinicalText,
                    Diagnosis = diagnosis,
                    Confidence = prediction.Confidence,
                    Uncertainty = prediction.Uncertainty,
                    ComputeCost = prediction.ComputeCost,
                    Interpretation = prediction.GetInterpretation(),
                    ExtractedEntities = entities,
                    ProcessingTime = T,
                    Recommendations = GenerateRecommendations(prediction, entities)
                };
            });
        }

        /// <summary>
        /// Analyze patient symptoms with detailed uncertainty breakdown
        /// </summary>
        public async Task<SymptomAnalysisResult> AnalyzeSymptoms(List<string> symptoms, 
            Dictionary<string, double> vitalSigns = null)
        {
            return await Task.Run(() =>
            {
                // Combine symptoms into clinical text
                string clinicalText = "Patient presents with: " + string.Join(", ", symptoms);
                
                if (vitalSigns != null && vitalSigns.Count > 0)
                {
                    clinicalText += ". Vital signs: ";
                    clinicalText += string.Join(", ", vitalSigns.Select(kv => $"{kv.Key}={kv.Value}"));
                }
                
                var tokens = tokenizer.Tokenize(clinicalText);
                var paddedTokens = tokenizer.PadSequence(tokens, MAX_SEQUENCE_LENGTH);
                
                // Run prediction
                var prediction = model.PredictMedical(paddedTokens);
                
                // Analyze each symptom's contribution
                var symptomContributions = new Dictionary<string, double>();
                foreach (var symptom in symptoms)
                {
                    var symptomTokens = tokenizer.Tokenize(symptom);
                    var symptomPred = model.PredictMedical(symptomTokens, T: 0.5);
                    symptomContributions[symptom] = symptomPred.Confidence;
                }
                
                return new SymptomAnalysisResult
                {
                    Symptoms = symptoms,
                    VitalSigns = vitalSigns,
                    OverallConfidence = prediction.Confidence,
                    EpistemicUncertainty = prediction.Uncertainty * 0.6,  // Model uncertainty
                    AleatoricUncertainty = prediction.Uncertainty * 0.4,  // Data uncertainty
                    SymptomContributions = symptomContributions,
                    SeverityLevel = DetermineSeverity(prediction, symptoms),
                    UrgencyScore = CalculateUrgency(prediction, symptoms, vitalSigns),
                    SuggestedTests = SuggestAdditionalTests(symptoms, prediction)
                };
            });
        }

        /// <summary>
        /// Train the model on medical data
        /// </summary>
        public async Task TrainModel(List<MedicalTrainingExample> trainingData, 
            int epochs = 10, int batchSize = 32)
        {
            await Task.Run(() =>
            {
                // Convert training data to token sequences
                var dataset = trainingData.Select(example =>
                {
                    var tokens = tokenizer.Tokenize(example.ClinicalText);
                    var paddedTokens = tokenizer.PadSequence(tokens, MAX_SEQUENCE_LENGTH);
                    
                    // Target is the diagnosis class
                    var targets = new int[] { example.DiagnosisClass };
                    
                    return (tokens: paddedTokens, targets: targets);
                }).ToList();
                
                // Train
                trainer.Train(dataset, epochs, batchSize);
                
                IsModelTrained = true;
            });
        }

        /// <summary>
        /// Evaluate model performance
        /// </summary>
        public async Task<ModelEvaluationResult> EvaluateModel(List<MedicalTrainingExample> testData)
        {
            return await Task.Run(() =>
            {
                var dataset = testData.Select(example =>
                {
                    var tokens = tokenizer.Tokenize(example.ClinicalText);
                    var paddedTokens = tokenizer.PadSequence(tokens, MAX_SEQUENCE_LENGTH);
                    var targets = new int[] { example.DiagnosisClass };
                    return (tokens: paddedTokens, targets: targets);
                }).ToList();
                
                var metrics = trainer.Validate(dataset);
                
                return new ModelEvaluationResult
                {
                    Accuracy = metrics.Accuracy,
                    AverageLoss = metrics.TotalLoss,
                    AverageUncertainty = metrics.AverageUncertainty,
                    AverageComputeCost = metrics.ComputeCost,
                    TotalSamples = testData.Count
                };
            });
        }

        // Helper methods
        private string MapPredictionToDiagnosis(MedicalPrediction prediction, DiagnosisCategory category)
        {
            // This would map to actual diagnosis codes/names
            // For now, return generic classifications
            var diagnoses = new Dictionary<int, string>
            {
                { 0, "Normal/Healthy" },
                { 1, "Mild Condition - Monitoring Recommended" },
                { 2, "Moderate Condition - Treatment Recommended" },
                { 3, "Severe Condition - Immediate Treatment Required" },
                { 4, "Critical Condition - Emergency Care Required" }
            };
            
            return diagnoses.ContainsKey(prediction.PredictedClass) 
                ? diagnoses[prediction.PredictedClass] 
                : "Uncertain - Further Evaluation Needed";
        }

        private List<string> GenerateRecommendations(MedicalPrediction prediction, List<MedicalEntity> entities)
        {
            var recommendations = new List<string>();
            
            if (prediction.Uncertainty > 0.5)
            {
                recommendations.Add("High uncertainty detected - recommend additional diagnostic tests");
            }
            
            if (prediction.Confidence < 0.6)
            {
                recommendations.Add("Low confidence - recommend specialist consultation");
            }
            
            if (entities.Any(e => e.Type == "Symptom"))
            {
                recommendations.Add("Monitor symptoms closely and reassess in 24-48 hours");
            }
            
            return recommendations;
        }

        private string DetermineSeverity(MedicalPrediction prediction, List<string> symptoms)
        {
            if (prediction.PredictedClass >= 3) return "Severe";
            if (prediction.PredictedClass == 2) return "Moderate";
            if (prediction.PredictedClass == 1) return "Mild";
            return "Normal";
        }

        private double CalculateUrgency(MedicalPrediction prediction, List<string> symptoms, 
            Dictionary<string, double> vitalSigns)
        {
            double urgency = prediction.PredictedClass * 0.2;
            
            // Increase urgency for critical symptoms
            var criticalSymptoms = new[] { "chest pain", "shortness of breath", "severe bleeding" };
            if (symptoms.Any(s => criticalSymptoms.Any(cs => s.ToLower().Contains(cs))))
            {
                urgency += 0.3;
            }
            
            // Check vital signs
            if (vitalSigns != null)
            {
                if (vitalSigns.ContainsKey("temperature") && vitalSigns["temperature"] > 39.0)
                    urgency += 0.15;
                if (vitalSigns.ContainsKey("heartrate") && vitalSigns["heartrate"] > 120)
                    urgency += 0.15;
            }
            
            return Math.Min(urgency, 1.0);
        }

        private List<string> SuggestAdditionalTests(List<string> symptoms, MedicalPrediction prediction)
        {
            var tests = new List<string>();
            
            if (prediction.Uncertainty > 0.4)
            {
                tests.Add("Complete Blood Count (CBC)");
                tests.Add("Comprehensive Metabolic Panel");
            }
            
            return tests;
        }
    }

    // Supporting classes
    public enum DiagnosisCategory
    {
        General,
        Emergency,
        Routine,
        Complex
    }

    public class MedicalAnalysisResult
    {
        public string InputText { get; set; }
        public string Diagnosis { get; set; }
        public double Confidence { get; set; }
        public double Uncertainty { get; set; }
        public double ComputeCost { get; set; }
        public string Interpretation { get; set; }
        public List<MedicalEntity> ExtractedEntities { get; set; }
        public double ProcessingTime { get; set; }
        public List<string> Recommendations { get; set; }
    }

    public class SymptomAnalysisResult
    {
        public List<string> Symptoms { get; set; }
        public Dictionary<string, double> VitalSigns { get; set; }
        public double OverallConfidence { get; set; }
        public double EpistemicUncertainty { get; set; }
        public double AleatoricUncertainty { get; set; }
        public Dictionary<string, double> SymptomContributions { get; set; }
        public string SeverityLevel { get; set; }
        public double UrgencyScore { get; set; }
        public List<string> SuggestedTests { get; set; }
    }

    public class MedicalTrainingExample
    {
        public string ClinicalText { get; set; }
        public int DiagnosisClass { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class ModelEvaluationResult
    {
        public double Accuracy { get; set; }
        public double AverageLoss { get; set; }
        public double AverageUncertainty { get; set; }
        public double AverageComputeCost { get; set; }
        public int TotalSamples { get; set; }
    }
}
