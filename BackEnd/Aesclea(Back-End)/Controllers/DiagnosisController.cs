// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Aesclea_Back_End_.AIModel;
using Aesclea_Back_End_.AIModel.Helpers;

namespace Aesclea_Back_End_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DiagnosisController : ControllerBase
    {
        private readonly MedicalDiagnosisClassifier _medicalClassifier;
        private readonly ILogger<DiagnosisController> _logger;

        public DiagnosisController(MedicalDiagnosisClassifier medicalClassifier, ILogger<DiagnosisController> logger)
        {
            _medicalClassifier = medicalClassifier;
            _logger = logger;
        }

        [HttpPost("suggest")]
        public async Task<IActionResult> SuggestDiagnosis([FromBody] DiagnosisRequest request)
        {
            try
            {
                _logger.LogInformation("Processing diagnosis suggestion for symptoms: {Symptoms}", request.Symptoms);

                if (request == null || string.IsNullOrWhiteSpace(request.Symptoms))
                {
                    return BadRequest(new { message = "Symptoms are required for diagnosis suggestion" });
                }

                // Process symptoms and get AI prediction
                var prediction = await Task.Run(() => 
                {
                    return _medicalClassifier.AnalyzeMedicalText(request.Symptoms);
                });

                // Generate comprehensive diagnosis response
                var response = new DiagnosisResponse
                {
                    PrimaryDiagnosis = prediction.PrimaryDiagnosticCategory,
                    Confidence = prediction.DiagnosticConfidences?.Values.Max() ?? 0.0,
                    Severity = prediction.SeverityLevel,
                    Urgency = prediction.UrgencyLevel,
                    PossibleConditions = GeneratePossibleConditions(request.Symptoms, prediction),
                    Recommendations = prediction.Recommendations ?? GenerateRecommendations(prediction),
                    NextSteps = GenerateNextSteps(prediction),
                    Timestamp = DateTime.UtcNow
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing diagnosis suggestion");
                return StatusCode(500, new { message = "Internal server error during diagnosis processing" });
            }
        }

        [HttpPost("analyze-symptoms")]
        public Task<IActionResult> AnalyzeSymptoms([FromBody] SymptomsAnalysisRequest request)
        {
            try
            {
                _logger.LogInformation("Analyzing symptoms for patient");

                if (request == null || request.Symptoms == null || !request.Symptoms.Any())
                {
                    return Task.FromResult<IActionResult>(BadRequest(new { message = "Symptoms list is required" }));
                }

                var analysis = new SymptomsAnalysis
                {
                    SymptomsSeverity = AnalyzeSymptomsSeverity(request.Symptoms),
                    SystemsAffected = IdentifyAffectedSystems(request.Symptoms),
                    RedFlags = IdentifyRedFlags(request.Symptoms),
                    TriageLevel = DetermineTriageLevel(request.Symptoms),
                    Duration = request.Duration,
                    PatientFactors = new PatientFactors
                    {
                        Age = request.PatientAge,
                        Gender = request.PatientGender,
                        MedicalHistory = request.MedicalHistory
                    }
                };

                return Task.FromResult<IActionResult>(Ok(analysis));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing symptoms");
                return Task.FromResult<IActionResult>(StatusCode(500, new { message = "Internal server error during symptoms analysis" }));
            }
        }

        private double[] ProcessSymptomsToInput(string symptoms, int patientAge, string patientGender)
        {
            // Convert symptoms text to numerical input for AI model
            var symptomsLower = symptoms.ToLower();
            
            // Basic symptom encoding (this should be more sophisticated in production)
            var features = new List<double>();
            
            // Basic symptom presence indicators
            features.Add(symptomsLower.Contains("fever") ? 1.0 : 0.0);
            features.Add(symptomsLower.Contains("cough") ? 1.0 : 0.0);
            features.Add(symptomsLower.Contains("pain") ? 1.0 : 0.0);
            features.Add(symptomsLower.Contains("headache") ? 1.0 : 0.0);
            features.Add(symptomsLower.Contains("nausea") ? 1.0 : 0.0);
            features.Add(symptomsLower.Contains("fatigue") ? 1.0 : 0.0);
            features.Add(symptomsLower.Contains("shortness of breath") || symptomsLower.Contains("breathing") ? 1.0 : 0.0);
            features.Add(symptomsLower.Contains("chest") ? 1.0 : 0.0);
            
            // Patient demographics (normalized)
            features.Add(patientAge / 100.0); // Normalize age
            features.Add(patientGender.ToLower() == "male" ? 0.0 : 1.0); // Gender encoding
            
            return features.ToArray();
        }

        private List<string> GeneratePossibleConditions(string symptoms, MedicalDiagnosisResult prediction)
        {
            var conditions = new List<string>();
            var symptomsLower = symptoms.ToLower();

            // Rule-based condition suggestions based on symptoms
            if (symptomsLower.Contains("fever") && symptomsLower.Contains("cough"))
            {
                conditions.AddRange(new[] { "Upper Respiratory Infection", "Pneumonia", "Influenza" });
            }
            
            if (symptomsLower.Contains("chest pain"))
            {
                conditions.AddRange(new[] { "Myocardial Infarction", "Angina", "Pulmonary Embolism", "Costochondritis" });
            }
            
            if (symptomsLower.Contains("headache"))
            {
                conditions.AddRange(new[] { "Tension Headache", "Migraine", "Cluster Headache", "Sinusitis" });
            }

            // Add AI prediction category
            if (!string.IsNullOrEmpty(prediction.PrimaryDiagnosticCategory))
            {
                conditions.Insert(0, prediction.PrimaryDiagnosticCategory);
            }

            return conditions.Distinct().Take(5).ToList();
        }

        private List<string> GenerateRecommendations(MedicalDiagnosisResult prediction)
        {
            var recommendations = new List<string>();

            switch (prediction.SeverityLevel)
            {
                case 1:
                case 2:
                    recommendations.Add("Monitor symptoms at home");
                    recommendations.Add("Increase fluid intake");
                    recommendations.Add("Rest as needed");
                    break;
                case 3:
                    recommendations.Add("Schedule appointment with primary care physician");
                    recommendations.Add("Monitor symptoms closely");
                    recommendations.Add("Consider over-the-counter medications if appropriate");
                    break;
                case 4:
                case 5:
                    recommendations.Add("Seek immediate medical attention");
                    recommendations.Add("Consider emergency department evaluation");
                    recommendations.Add("Do not delay medical care");
                    break;
            }

            return recommendations;
        }

        private List<string> GenerateNextSteps(MedicalDiagnosisResult prediction)
        {
            var nextSteps = new List<string>();

            if (prediction.UrgencyLevel >= 1)
            {
                nextSteps.Add("Contact healthcare provider within 24 hours");
            }

            if (prediction.UrgencyLevel >= 2)
            {
                nextSteps.Add("Seek emergency medical care immediately");
                nextSteps.Add("Call emergency services if symptoms worsen");
            }
            else
            {
                nextSteps.Add("Schedule follow-up appointment");
                nextSteps.Add("Keep symptom diary");
                nextSteps.Add("Return if symptoms worsen or new symptoms develop");
            }

            return nextSteps;
        }

        private Dictionary<string, int> AnalyzeSymptomsSeverity(List<SymptomReport> symptoms)
        {
            var severity = new Dictionary<string, int>();
            foreach (var symptom in symptoms)
            {
                severity[symptom.Name] = symptom.Severity;
            }
            return severity;
        }

        private List<string> IdentifyAffectedSystems(List<SymptomReport> symptoms)
        {
            var systems = new HashSet<string>();
            
            foreach (var symptom in symptoms)
            {
                var symptomLower = symptom.Name.ToLower();
                
                if (symptomLower.Contains("heart") || symptomLower.Contains("chest") || symptomLower.Contains("palpitation"))
                    systems.Add("Cardiovascular");
                if (symptomLower.Contains("lung") || symptomLower.Contains("breathing") || symptomLower.Contains("cough"))
                    systems.Add("Respiratory");
                if (symptomLower.Contains("stomach") || symptomLower.Contains("nausea") || symptomLower.Contains("vomiting"))
                    systems.Add("Gastrointestinal");
                if (symptomLower.Contains("head") || symptomLower.Contains("dizzy") || symptomLower.Contains("confusion"))
                    systems.Add("Neurological");
            }
            
            return systems.ToList();
        }

        private List<string> IdentifyRedFlags(List<SymptomReport> symptoms)
        {
            var redFlags = new List<string>();
            
            foreach (var symptom in symptoms)
            {
                if (symptom.Severity >= 8)
                {
                    redFlags.Add($"Severe {symptom.Name}");
                }
                
                var symptomLower = symptom.Name.ToLower();
                if (symptomLower.Contains("chest pain") && symptom.Severity >= 6)
                    redFlags.Add("Severe chest pain - possible cardiac event");
                if (symptomLower.Contains("shortness of breath") && symptom.Severity >= 7)
                    redFlags.Add("Severe respiratory distress");
                if (symptomLower.Contains("confusion") || symptomLower.Contains("altered mental"))
                    redFlags.Add("Altered mental status");
            }
            
            return redFlags;
        }

        private string DetermineTriageLevel(List<SymptomReport> symptoms)
        {
            var maxSeverity = symptoms.Max(s => s.Severity);
            var redFlags = IdentifyRedFlags(symptoms);
            
            if (redFlags.Any() || maxSeverity >= 8)
                return "Emergency";
            if (maxSeverity >= 6)
                return "Urgent";
            if (maxSeverity >= 4)
                return "Semi-urgent";
            return "Non-urgent";
        }
    }

    // Request and Response Models
    public class DiagnosisRequest
    {
        public string Symptoms { get; set; } = string.Empty;
        public int PatientAge { get; set; } = 0;
        public string PatientGender { get; set; } = string.Empty;
        public string? MedicalHistory { get; set; }
    }

    public class SymptomsAnalysisRequest
    {
        public List<SymptomReport> Symptoms { get; set; } = new();
        public string Duration { get; set; } = string.Empty;
        public int PatientAge { get; set; } = 0;
        public string PatientGender { get; set; } = string.Empty;
        public string? MedicalHistory { get; set; }
    }

    public class SymptomReport
    {
        public string Name { get; set; } = string.Empty;
        public int Severity { get; set; } = 1; // 1-10 scale
        public string Duration { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class DiagnosisResponse
    {
        public string PrimaryDiagnosis { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public int Severity { get; set; }
        public int Urgency { get; set; }
        public List<string> PossibleConditions { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public List<string> NextSteps { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }

    public class SymptomsAnalysis
    {
        public Dictionary<string, int> SymptomsSeverity { get; set; } = new();
        public List<string> SystemsAffected { get; set; } = new();
        public List<string> RedFlags { get; set; } = new();
        public string TriageLevel { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public PatientFactors PatientFactors { get; set; } = new();
    }

    public class PatientFactors
    {
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? MedicalHistory { get; set; }
    }
}
