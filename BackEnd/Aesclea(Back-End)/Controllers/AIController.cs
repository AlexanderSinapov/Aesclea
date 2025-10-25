// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Microsoft.AspNetCore.Mvc;
using Aesclea_Back_End_.AIModel;
using Aesclea_Back_End_.AIModel.Helpers;

namespace Aesclea_Back_End_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private static MedicalDiagnosisClassifier? _classifier;
        private static readonly object _lock = new object();

        /// <summary>
        /// Initializes the classifier if not already initialized
        /// </summary>
        private void EnsureClassifierInitialized()
        {
            if (_classifier == null)
            {
                lock (_lock)
                {
                    if (_classifier == null)
                    {
                        _classifier = new MedicalDiagnosisClassifier();
                        
                        // Try to load pre-trained weights from NeuronData folder
                        try
                        {
                            var fileHelper = new FileHelper();
                            fileHelper.OpenFolder(); // Opens default NeuronData folder
                            _classifier.LoadWeights(fileHelper, "medical_diagnosis");
                            Console.WriteLine("✓ Successfully loaded trained medical diagnosis model from NeuronData/");
                            Console.WriteLine("  - Using: medical_diagnosis_diagnostic_NeuralData.wbn");
                            Console.WriteLine("  - Using: medical_diagnosis_severity_NeuralData.wbn");
                            Console.WriteLine("  - Using: medical_diagnosis_urgency_NeuralData.wbn");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⚠️  No trained weights found in NeuronData/, using untrained model");
                            Console.WriteLine($"   Run training command (Console option #16) to train the model");
                            Console.WriteLine($"   Details: {ex.Message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Analyze medical text and provide diagnostic insights
        /// POST: api/ai/analyze
        /// </summary>
        [HttpPost("analyze")]
        public IActionResult AnalyzeMedicalText([FromBody] MedicalTextRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(new { error = "Medical text is required" });
            }

            try
            {
                EnsureClassifierInitialized();
                
                var result = _classifier!.AnalyzeMedicalText(request.Text);
                
                return Ok(new MedicalAnalysisResponse
                {
                    PrimaryCategory = result.PrimaryDiagnosticCategory,
                    SeverityLevel = result.SeverityLevel,
                    SeverityDescription = result.SeverityDescription,
                    SeverityConfidence = result.SeverityConfidence,
                    UrgencyLevel = result.UrgencyLevel,
                    UrgencyDescription = result.UrgencyDescription,
                    UrgencyConfidence = result.UrgencyConfidence,
                    DiagnosticConfidences = result.DiagnosticConfidences,
                    ClinicalAlerts = result.ExtractedInfo.ContainsKey("clinical_alerts") 
                        ? result.ExtractedInfo["clinical_alerts"] as List<string> 
                        : new List<string>(),
                    Symptoms = result.ExtractedInfo.ContainsKey("symptoms") 
                        ? result.ExtractedInfo["symptoms"] as List<string> 
                        : new List<string>(),
                    SymptomClusters = result.ExtractedInfo.ContainsKey("symptom_clusters") 
                        ? result.ExtractedInfo["symptom_clusters"] as Dictionary<string, List<string>> 
                        : new Dictionary<string, List<string>>(),
                    VitalSignsAnalysis = result.ExtractedInfo.ContainsKey("vital_signs_analysis") 
                        ? result.ExtractedInfo["vital_signs_analysis"] as Dictionary<string, string> 
                        : new Dictionary<string, string>(),
                    TemporalPatterns = result.ExtractedInfo.ContainsKey("temporal_patterns") 
                        ? result.ExtractedInfo["temporal_patterns"] as Dictionary<string, string> 
                        : new Dictionary<string, string>(),
                    FunctionalImpact = result.ExtractedInfo.ContainsKey("functional_impact") 
                        ? result.ExtractedInfo["functional_impact"] as Dictionary<string, string> 
                        : new Dictionary<string, string>(),
                    RiskFactors = result.ExtractedInfo.ContainsKey("risk_factors") 
                        ? result.ExtractedInfo["risk_factors"] as List<string> 
                        : new List<string>(),
                    Medications = result.ExtractedInfo.ContainsKey("medication_mentions") 
                        ? result.ExtractedInfo["medication_mentions"] as List<string> 
                        : new List<string>(),
                    DifferentialDiagnosis = result.ExtractedInfo.ContainsKey("differential_diagnosis") 
                        ? result.ExtractedInfo["differential_diagnosis"] as List<string> 
                        : new List<string>(),
                    SuggestedTests = result.ExtractedInfo.ContainsKey("suggested_tests") 
                        ? result.ExtractedInfo["suggested_tests"] as List<string> 
                        : new List<string>(),
                    Recommendations = result.Recommendations,
                    Summary = result.GetSummary(),
                    ProcessedOn = result.ProcessedOn
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error analyzing medical text", details = ex.Message });
            }
        }

        /// <summary>
        /// Chat endpoint for conversational medical assistant
        /// POST: api/ai/chat
        /// </summary>
        [HttpPost("chat")]
        public IActionResult Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { error = "Message is required" });
            }

            try
            {
                EnsureClassifierInitialized();
                
                // Analyze the medical text
                var analysis = _classifier!.AnalyzeMedicalText(request.Message);
                
                // Generate a conversational response based on the analysis
                var response = GenerateChatResponse(request.Message, analysis, request.Context);
                
                return Ok(new ChatResponse
                {
                    Message = response,
                    Analysis = new MedicalAnalysisResponse
                    {
                        PrimaryCategory = analysis.PrimaryDiagnosticCategory,
                        SeverityLevel = analysis.SeverityLevel,
                        SeverityDescription = analysis.SeverityDescription,
                        SeverityConfidence = analysis.SeverityConfidence,
                        UrgencyLevel = analysis.UrgencyLevel,
                        UrgencyDescription = analysis.UrgencyDescription,
                        ClinicalAlerts = analysis.ExtractedInfo.ContainsKey("clinical_alerts") 
                            ? analysis.ExtractedInfo["clinical_alerts"] as List<string> 
                            : new List<string>(),
                        Symptoms = analysis.ExtractedInfo.ContainsKey("symptoms") 
                            ? analysis.ExtractedInfo["symptoms"] as List<string> 
                            : new List<string>(),
                        DifferentialDiagnosis = analysis.ExtractedInfo.ContainsKey("differential_diagnosis") 
                            ? analysis.ExtractedInfo["differential_diagnosis"] as List<string> 
                            : new List<string>(),
                        SuggestedTests = analysis.ExtractedInfo.ContainsKey("suggested_tests") 
                            ? analysis.ExtractedInfo["suggested_tests"] as List<string> 
                            : new List<string>(),
                        Recommendations = analysis.Recommendations
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error processing chat message", details = ex.Message });
            }
        }

        /// <summary>
        /// Generate a conversational response based on analysis
        /// </summary>
        private string GenerateChatResponse(string userMessage, MedicalDiagnosisResult analysis, ChatContext? context)
        {
            var response = "";
            var lowerMessage = userMessage.ToLower();

            // Greeting detection
            if (lowerMessage.Contains("hello") || lowerMessage.Contains("hi ") || lowerMessage.StartsWith("hi"))
            {
                response += "Hello! I'm Aesclea's Medical AI Assistant. I can help you with medical analysis, differential diagnosis, treatment recommendations, and more. ";
                if (context != null && !string.IsNullOrEmpty(context.PatientId))
                {
                    response += $"I see we're discussing patient {context.PatientId}. ";
                }
                response += "How can I assist you today?\n\n";
                return response;
            }

            // Check for clinical alerts first
            var alerts = analysis.ExtractedInfo.ContainsKey("clinical_alerts") 
                ? analysis.ExtractedInfo["clinical_alerts"] as List<string> 
                : new List<string>();
            
            if (alerts != null && alerts.Count > 0)
            {
                response += "🚨 **CLINICAL ALERT:**\n";
                foreach (var alert in alerts)
                {
                    response += $"⚠️ {alert}\n";
                }
                response += "\n";
            }

            // Add urgency warning if needed
            if (analysis.UrgencyLevel >= 1)
            {
                response += $"**{analysis.UrgencyDescription}** (Confidence: {analysis.UrgencyConfidence * 100:F1}%)\n\n";
            }

            // Main analysis
            response += $"Based on my analysis of the provided information:\n\n";
            
            response += $"**Primary Category:** {analysis.PrimaryDiagnosticCategory}\n";
            response += $"**Severity:** Level {analysis.SeverityLevel} - {analysis.SeverityDescription}\n\n";

            // Symptoms
            var symptoms = analysis.ExtractedInfo.ContainsKey("symptoms") 
                ? analysis.ExtractedInfo["symptoms"] as List<string> 
                : new List<string>();
            
            if (symptoms != null && symptoms.Count > 0)
            {
                response += $"**Identified Symptoms:** {string.Join(", ", symptoms)}\n\n";
            }

            // Differential diagnosis
            var differentials = analysis.ExtractedInfo.ContainsKey("differential_diagnosis") 
                ? analysis.ExtractedInfo["differential_diagnosis"] as List<string> 
                : new List<string>();
            
            if (differentials != null && differentials.Count > 0)
            {
                response += "**Differential Diagnosis to Consider:**\n";
                foreach (var diff in differentials.Take(5))
                {
                    response += $"• {diff}\n";
                }
                response += "\n";
            }

            // Suggested tests
            var tests = analysis.ExtractedInfo.ContainsKey("suggested_tests") 
                ? analysis.ExtractedInfo["suggested_tests"] as List<string> 
                : new List<string>();
            
            if (tests != null && tests.Count > 0)
            {
                response += "**Recommended Diagnostic Tests:**\n";
                foreach (var test in tests.Take(5))
                {
                    response += $"• {test}\n";
                }
                response += "\n";
            }

            // Recommendations
            if (analysis.Recommendations != null && analysis.Recommendations.Count > 0)
            {
                response += "**Clinical Recommendations:**\n";
                foreach (var rec in analysis.Recommendations.Take(5))
                {
                    response += $"• {rec}\n";
                }
                response += "\n";
            }

            // Add context-aware note
            if (context != null)
            {
                if (!string.IsNullOrEmpty(context.PatientId))
                    response += $"\n*Analysis for Patient ID: {context.PatientId}*";
                if (!string.IsNullOrEmpty(context.Age))
                    response += $" (Age: {context.Age})";
                if (!string.IsNullOrEmpty(context.Gender))
                    response += $" ({context.Gender})";
                response += "\n";
            }

            response += "\n*This analysis is provided for clinical decision support. Always use professional medical judgment and follow established clinical guidelines.*";

            return response;
        }
    }

    // Request/Response models
    public class MedicalTextRequest
    {
        public string Text { get; set; } = string.Empty;
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public ChatContext? Context { get; set; }
    }

    public class ChatContext
    {
        public string? PatientId { get; set; }
        public string? Age { get; set; }
        public string? Gender { get; set; }
        public string? MedicalHistory { get; set; }
        public string? Medications { get; set; }
        public string? Allergies { get; set; }
    }

    public class MedicalAnalysisResponse
    {
        public string PrimaryCategory { get; set; } = string.Empty;
        public int SeverityLevel { get; set; }
        public string SeverityDescription { get; set; } = string.Empty;
        public double SeverityConfidence { get; set; }
        public int UrgencyLevel { get; set; }
        public string UrgencyDescription { get; set; } = string.Empty;
        public double UrgencyConfidence { get; set; }
        public Dictionary<string, double>? DiagnosticConfidences { get; set; }
        public List<string>? ClinicalAlerts { get; set; }
        public List<string>? Symptoms { get; set; }
        public Dictionary<string, List<string>>? SymptomClusters { get; set; }
        public Dictionary<string, string>? VitalSignsAnalysis { get; set; }
        public Dictionary<string, string>? TemporalPatterns { get; set; }
        public Dictionary<string, string>? FunctionalImpact { get; set; }
        public List<string>? RiskFactors { get; set; }
        public List<string>? Medications { get; set; }
        public List<string>? DifferentialDiagnosis { get; set; }
        public List<string>? SuggestedTests { get; set; }
        public List<string>? Recommendations { get; set; }
        public string? Summary { get; set; }
        public DateTime ProcessedOn { get; set; }
    }

    public class ChatResponse
    {
        public string Message { get; set; } = string.Empty;
        public MedicalAnalysisResponse? Analysis { get; set; }
    }
}
