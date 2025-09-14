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
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VitalSignsController : ControllerBase
    {
        private readonly MedicalDiagnosisClassifier _medicalClassifier;
        private readonly ILogger<VitalSignsController> _logger;

        public VitalSignsController(MedicalDiagnosisClassifier medicalClassifier, ILogger<VitalSignsController> logger)
        {
            _medicalClassifier = medicalClassifier;
            _logger = logger;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeVitalSigns([FromBody] VitalSignsRequest request)
        {
            try
            {
                _logger.LogInformation("Analyzing vital signs for patient: {PatientId}", request.PatientId);

                // Validate input
                if (request == null || !ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid vital signs data provided" });
                }

                // Prepare input data for the AI model and get AI prediction
                var prediction = await Task.Run(() => 
                {
                    var medicalText = $"Patient vital signs: Heart rate {request.HeartRate}, Blood pressure {request.BloodPressureSystolic}/{request.BloodPressureDiastolic}, Temperature {request.Temperature}, Respiratory rate {request.RespiratoryRate}, Oxygen saturation {request.OxygenSaturation}. Age: {request.Age}, Gender: {request.Gender}. Notes: {request.Notes}";
                    return _medicalClassifier.AnalyzeMedicalText(medicalText);
                });

                // Analyze vital signs
                var analysis = AnalyzeVitalSignsData(request);

                var response = new VitalSignsAnalysisResponse
                {
                    PatientId = request.PatientId,
                    Analysis = analysis,
                    AiPrediction = prediction,
                    Timestamp = DateTime.UtcNow,
                    Recommendations = GenerateRecommendations(analysis, prediction)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing vital signs");
                return StatusCode(500, new { message = "Internal server error during vital signs analysis" });
            }
        }

        private VitalSignsAnalysis AnalyzeVitalSignsData(VitalSignsRequest request)
        {
            var analysis = new VitalSignsAnalysis();
            var alerts = new List<string>();

            // Heart Rate Analysis
            if (request.HeartRate < 60)
            {
                analysis.HeartRateStatus = "Low (Bradycardia)";
                alerts.Add("Heart rate is below normal range");
            }
            else if (request.HeartRate > 100)
            {
                analysis.HeartRateStatus = "High (Tachycardia)";
                alerts.Add("Heart rate is above normal range");
            }
            else
            {
                analysis.HeartRateStatus = "Normal";
            }

            // Blood Pressure Analysis
            if (request.BloodPressureSystolic >= 140 || request.BloodPressureDiastolic >= 90)
            {
                analysis.BloodPressureStatus = "High (Hypertension)";
                alerts.Add("Blood pressure is elevated");
            }
            else if (request.BloodPressureSystolic < 90 || request.BloodPressureDiastolic < 60)
            {
                analysis.BloodPressureStatus = "Low (Hypotension)";
                alerts.Add("Blood pressure is below normal range");
            }
            else
            {
                analysis.BloodPressureStatus = "Normal";
            }

            // Temperature Analysis
            if (request.Temperature >= 38.0)
            {
                analysis.TemperatureStatus = "High (Fever)";
                alerts.Add("Patient has fever");
            }
            else if (request.Temperature < 36.0)
            {
                analysis.TemperatureStatus = "Low (Hypothermia)";
                alerts.Add("Body temperature is below normal");
            }
            else
            {
                analysis.TemperatureStatus = "Normal";
            }

            // Respiratory Rate Analysis
            if (request.RespiratoryRate > 20)
            {
                analysis.RespiratoryStatus = "High (Tachypnea)";
                alerts.Add("Respiratory rate is elevated");
            }
            else if (request.RespiratoryRate < 12)
            {
                analysis.RespiratoryStatus = "Low (Bradypnea)";
                alerts.Add("Respiratory rate is below normal");
            }
            else
            {
                analysis.RespiratoryStatus = "Normal";
            }

            // Oxygen Saturation Analysis
            if (request.OxygenSaturation < 95)
            {
                analysis.OxygenSaturationStatus = "Low (Hypoxemia)";
                alerts.Add("Oxygen saturation is below normal");
            }
            else
            {
                analysis.OxygenSaturationStatus = "Normal";
            }

            analysis.Alerts = alerts;
            analysis.OverallStatus = alerts.Count == 0 ? "Normal" : "Abnormal";
            
            return analysis;
        }

        private List<string> GenerateRecommendations(VitalSignsAnalysis analysis, MedicalDiagnosisResult aiPrediction)
        {
            var recommendations = new List<string>();

            if (analysis.Alerts.Count == 0)
            {
                recommendations.Add("Continue regular monitoring");
                recommendations.Add("Maintain current lifestyle and medications");
            }
            else
            {
                recommendations.Add("Consult with physician immediately");
                recommendations.Add("Monitor vital signs closely");
                
                if (analysis.BloodPressureStatus.Contains("High"))
                {
                    recommendations.Add("Consider antihypertensive medication");
                    recommendations.Add("Implement lifestyle modifications for blood pressure control");
                }
                
                if (analysis.TemperatureStatus.Contains("High"))
                {
                    recommendations.Add("Consider antipyretic medication");
                    recommendations.Add("Increase fluid intake");
                }
                
                if (analysis.OxygenSaturationStatus.Contains("Low"))
                {
                    recommendations.Add("Consider oxygen therapy");
                    recommendations.Add("Investigate underlying respiratory conditions");
                }
            }

            return recommendations;
        }
    }

    // Request and Response Models
    public class VitalSignsRequest
    {
        public string PatientId { get; set; } = string.Empty;
        public double HeartRate { get; set; }
        public double BloodPressureSystolic { get; set; }
        public double BloodPressureDiastolic { get; set; }
        public double Temperature { get; set; }
        public double RespiratoryRate { get; set; }
        public double OxygenSaturation { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class VitalSignsAnalysisResponse
    {
        public string PatientId { get; set; } = string.Empty;
        public VitalSignsAnalysis Analysis { get; set; } = new();
        public MedicalDiagnosisResult AiPrediction { get; set; } = new();
        public DateTime Timestamp { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }

    public class VitalSignsAnalysis
    {
        public string HeartRateStatus { get; set; } = string.Empty;
        public string BloodPressureStatus { get; set; } = string.Empty;
        public string TemperatureStatus { get; set; } = string.Empty;
        public string RespiratoryStatus { get; set; } = string.Empty;
        public string OxygenSaturationStatus { get; set; } = string.Empty;
        public string OverallStatus { get; set; } = string.Empty;
        public List<string> Alerts { get; set; } = new();
    }
}
