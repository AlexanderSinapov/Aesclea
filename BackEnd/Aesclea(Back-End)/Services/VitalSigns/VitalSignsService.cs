using Aesclea_Back_End_.Models.VitalSigns;
using Microsoft.Extensions.Logging;

namespace Aesclea_Back_End_.Services.VitalSigns
{
    public interface IVitalSignsService
    {
        Task<VitalSignsAnalysis> AnalyzeVitalSignsAsync(VitalSignsReading reading);
        Task<List<VitalSignsReading>> GetPatientVitalSignsHistoryAsync(int patientId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<VitalSignsReading> SaveVitalSignsAsync(VitalSignsReading reading);
        Task<List<VitalSignsAlert>> GetActiveAlertsAsync(int patientId);
        Task<VitalSignsTrends> GetVitalSignsTrendsAsync(int patientId, int daysBack = 30);
        Task<VitalSignsReading?> GetVitalSignsAsync(int id);
        Task<List<VitalSignsReading>> GetUserVitalSignsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null, int pageNumber = 1, int pageSize = 50);
        Task<VitalSignsReading> UpdateVitalSignsAsync(VitalSignsReading reading);
        Task DeleteVitalSignsAsync(int id);
        Task<object> GetVitalSignsStatisticsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<object> GetVitalSignsTrendsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null, string trendType = "all");
    }
    
    public class VitalSignsService : IVitalSignsService
    {
        private readonly ILogger<VitalSignsService> _logger;
        
        public VitalSignsService(ILogger<VitalSignsService> logger)
        {
            _logger = logger;
        }
        
        public Task<VitalSignsAnalysis> AnalyzeVitalSignsAsync(VitalSignsReading reading)
        {
            try
            {
                var assessment = new VitalSignsAssessment
                {
                    BloodPressure = AnalyzeBloodPressure(reading.SystolicBP, reading.DiastolicBP),
                    HeartRate = AnalyzeHeartRate(reading.HeartRate),
                    Temperature = AnalyzeTemperature(reading.Temperature, reading.TemperatureUnit),
                    OxygenSaturation = AnalyzeOxygenSaturation(reading.OxygenSaturation),
                    RespiratoryRate = AnalyzeRespiratoryRate(reading.RespiratoryRate),
                    BloodGlucose = AnalyzeBloodGlucose(reading.BloodGlucose, reading.BloodGlucoseUnit),
                    BMI = AnalyzeBMI(reading.BMI),
                    Pain = AnalyzePain(reading.PainLevel)
                };
                
                var analysis = new VitalSignsAnalysis
                {
                    Reading = reading,
                    Assessment = assessment,
                    Alerts = GenerateAlerts(assessment)
                };
                
                // Determine overall risk level
                analysis.Assessment.OverallRisk = CalculateOverallRiskLevel(assessment);
                analysis.Assessment.Recommendations = GenerateRecommendations(assessment);
                
                return Task.FromResult(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing vital signs for patient {PatientId}", reading.PatientId);
                throw;
            }
        }
        
        public async Task<List<VitalSignsReading>> GetPatientVitalSignsHistoryAsync(int patientId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            // TODO: Implement database retrieval
            // This would typically query your database for vital signs history
            await Task.CompletedTask;
            return new List<VitalSignsReading>();
        }
        
        public async Task<VitalSignsReading> SaveVitalSignsAsync(VitalSignsReading reading)
        {
            // TODO: Implement database save
            await Task.CompletedTask;
            return reading;
        }
        
        public async Task<List<VitalSignsAlert>> GetActiveAlertsAsync(int patientId)
        {
            // TODO: Implement active alerts retrieval
            await Task.CompletedTask;
            return new List<VitalSignsAlert>();
        }
        
        public async Task<VitalSignsTrends> GetVitalSignsTrendsAsync(int patientId, int daysBack = 30)
        {
            // TODO: Implement trends calculation
            await Task.CompletedTask;
            return new VitalSignsTrends();
        }
        
        public async Task<VitalSignsReading?> GetVitalSignsAsync(int id)
        {
            // TODO: Implement database retrieval by ID
            await Task.CompletedTask;
            return null; // Would return the reading from database
        }
        
        public async Task<List<VitalSignsReading>> GetUserVitalSignsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null, int pageNumber = 1, int pageSize = 50)
        {
            // TODO: Implement database retrieval with pagination
            await Task.CompletedTask;
            return new List<VitalSignsReading>();
        }
        
        public async Task<VitalSignsReading> UpdateVitalSignsAsync(VitalSignsReading reading)
        {
            // TODO: Implement database update
            await Task.CompletedTask;
            return reading;
        }
        
        public async Task DeleteVitalSignsAsync(int id)
        {
            // TODO: Implement database deletion
            await Task.CompletedTask;
        }
        
        public async Task<object> GetVitalSignsStatisticsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            // TODO: Implement statistics calculation
            await Task.CompletedTask;
            return new { 
                TotalReadings = 0, 
                AverageHeartRate = 0, 
                AverageBloodPressure = "0/0",
                LastReading = (DateTime?)null
            };
        }
        
        public async Task<object> GetVitalSignsTrendsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null, string trendType = "all")
        {
            // TODO: Implement trends calculation with different parameters
            await Task.CompletedTask;
            return new { 
                TrendType = trendType,
                DataPoints = new List<object>(),
                Summary = "No data available"
            };
        }
        
        private BloodPressureAssessment AnalyzeBloodPressure(int? systolic, int? diastolic)
        {
            if (!systolic.HasValue || !diastolic.HasValue)
            {
                return new BloodPressureAssessment
                {
                    Category = BloodPressureCategory.Normal,
                    Description = "Blood pressure not measured",
                    RiskLevel = RiskLevel.Normal
                };
            }
            
            var sys = systolic.Value;
            var dia = diastolic.Value;
            
            if (sys < 90 || dia < 60)
            {
                return new BloodPressureAssessment
                {
                    Category = BloodPressureCategory.Low,
                    Description = $"Low blood pressure ({sys}/{dia} mmHg) - may indicate hypotension",
                    RiskLevel = RiskLevel.Moderate
                };
            }
            else if (sys < 120 && dia < 80)
            {
                return new BloodPressureAssessment
                {
                    Category = BloodPressureCategory.Normal,
                    Description = $"Normal blood pressure ({sys}/{dia} mmHg)",
                    RiskLevel = RiskLevel.Normal
                };
            }
            else if (sys < 130 && dia < 80)
            {
                return new BloodPressureAssessment
                {
                    Category = BloodPressureCategory.Elevated,
                    Description = $"Elevated blood pressure ({sys}/{dia} mmHg) - monitor closely",
                    RiskLevel = RiskLevel.Low
                };
            }
            else if (sys < 140 || dia < 90)
            {
                return new BloodPressureAssessment
                {
                    Category = BloodPressureCategory.HighStage1,
                    Description = $"High blood pressure Stage 1 ({sys}/{dia} mmHg) - lifestyle changes recommended",
                    RiskLevel = RiskLevel.Moderate
                };
            }
            else if (sys < 180 || dia < 120)
            {
                return new BloodPressureAssessment
                {
                    Category = BloodPressureCategory.HighStage2,
                    Description = $"High blood pressure Stage 2 ({sys}/{dia} mmHg) - medication may be needed",
                    RiskLevel = RiskLevel.High
                };
            }
            else
            {
                return new BloodPressureAssessment
                {
                    Category = BloodPressureCategory.HypertensiveCrisis,
                    Description = $"Hypertensive crisis ({sys}/{dia} mmHg) - seek immediate medical attention",
                    RiskLevel = RiskLevel.Critical
                };
            }
        }
        
        private HeartRateAssessment AnalyzeHeartRate(int? heartRate)
        {
            if (!heartRate.HasValue)
            {
                return new HeartRateAssessment
                {
                    Category = HeartRateCategory.Normal,
                    Description = "Heart rate not measured",
                    RiskLevel = RiskLevel.Normal
                };
            }
            
            var hr = heartRate.Value;
            
            if (hr < 60)
            {
                return new HeartRateAssessment
                {
                    Category = HeartRateCategory.Bradycardia,
                    Description = $"Bradycardia ({hr} bpm) - below normal heart rate",
                    RiskLevel = hr < 50 ? RiskLevel.Moderate : RiskLevel.Low
                };
            }
            else if (hr <= 100)
            {
                return new HeartRateAssessment
                {
                    Category = HeartRateCategory.Normal,
                    Description = $"Normal heart rate ({hr} bpm)",
                    RiskLevel = RiskLevel.Normal
                };
            }
            else
            {
                return new HeartRateAssessment
                {
                    Category = HeartRateCategory.Tachycardia,
                    Description = $"Tachycardia ({hr} bpm) - above normal heart rate",
                    RiskLevel = hr > 120 ? RiskLevel.High : RiskLevel.Moderate
                };
            }
        }
        
        private TemperatureAssessment AnalyzeTemperature(decimal? temperature, TemperatureUnit unit)
        {
            if (!temperature.HasValue)
            {
                return new TemperatureAssessment
                {
                    Category = TemperatureCategory.Normal,
                    Description = "Temperature not measured",
                    RiskLevel = RiskLevel.Normal
                };
            }
            
            // Convert to Celsius for analysis
            var tempC = unit == TemperatureUnit.Fahrenheit ? (temperature.Value - 32) * 5 / 9 : temperature.Value;
            var tempDisplay = unit == TemperatureUnit.Fahrenheit ? $"{temperature}°F" : $"{temperature}°C";
            
            if (tempC < 35.0m)
            {
                return new TemperatureAssessment
                {
                    Category = TemperatureCategory.Hypothermia,
                    Description = $"Hypothermia ({tempDisplay}) - dangerously low body temperature",
                    RiskLevel = RiskLevel.Critical
                };
            }
            else if (tempC < 37.2m)
            {
                return new TemperatureAssessment
                {
                    Category = TemperatureCategory.Normal,
                    Description = $"Normal temperature ({tempDisplay})",
                    RiskLevel = RiskLevel.Normal
                };
            }
            else if (tempC < 38.0m)
            {
                return new TemperatureAssessment
                {
                    Category = TemperatureCategory.LowGradeFever,
                    Description = $"Low-grade fever ({tempDisplay}) - mild elevation",
                    RiskLevel = RiskLevel.Low
                };
            }
            else if (tempC < 39.0m)
            {
                return new TemperatureAssessment
                {
                    Category = TemperatureCategory.Fever,
                    Description = $"Fever ({tempDisplay}) - elevated body temperature",
                    RiskLevel = RiskLevel.Moderate
                };
            }
            else
            {
                return new TemperatureAssessment
                {
                    Category = TemperatureCategory.HighFever,
                    Description = $"High fever ({tempDisplay}) - significantly elevated temperature",
                    RiskLevel = RiskLevel.High
                };
            }
        }
        
        private OxygenSaturationAssessment AnalyzeOxygenSaturation(decimal? oxygenSaturation)
        {
            if (!oxygenSaturation.HasValue)
            {
                return new OxygenSaturationAssessment
                {
                    Category = OxygenSaturationCategory.Normal,
                    Description = "Oxygen saturation not measured",
                    RiskLevel = RiskLevel.Normal
                };
            }
            
            var spo2 = oxygenSaturation.Value;
            
            if (spo2 < 88)
            {
                return new OxygenSaturationAssessment
                {
                    Category = OxygenSaturationCategory.Critical,
                    Description = $"Critical oxygen saturation ({spo2}%) - immediate medical attention required",
                    RiskLevel = RiskLevel.Critical
                };
            }
            else if (spo2 < 95)
            {
                return new OxygenSaturationAssessment
                {
                    Category = OxygenSaturationCategory.Low,
                    Description = $"Low oxygen saturation ({spo2}%) - below normal range",
                    RiskLevel = RiskLevel.High
                };
            }
            else
            {
                return new OxygenSaturationAssessment
                {
                    Category = OxygenSaturationCategory.Normal,
                    Description = $"Normal oxygen saturation ({spo2}%)",
                    RiskLevel = RiskLevel.Normal
                };
            }
        }
        
        private RespiratoryRateAssessment AnalyzeRespiratoryRate(int? respiratoryRate)
        {
            if (!respiratoryRate.HasValue)
            {
                return new RespiratoryRateAssessment
                {
                    Category = RespiratoryRateCategory.Normal,
                    Description = "Respiratory rate not measured",
                    RiskLevel = RiskLevel.Normal
                };
            }
            
            var rr = respiratoryRate.Value;
            
            if (rr < 12)
            {
                return new RespiratoryRateAssessment
                {
                    Category = RespiratoryRateCategory.Bradypnea,
                    Description = $"Bradypnea ({rr} breaths/min) - below normal respiratory rate",
                    RiskLevel = rr < 8 ? RiskLevel.High : RiskLevel.Moderate
                };
            }
            else if (rr <= 20)
            {
                return new RespiratoryRateAssessment
                {
                    Category = RespiratoryRateCategory.Normal,
                    Description = $"Normal respiratory rate ({rr} breaths/min)",
                    RiskLevel = RiskLevel.Normal
                };
            }
            else
            {
                return new RespiratoryRateAssessment
                {
                    Category = RespiratoryRateCategory.Tachypnea,
                    Description = $"Tachypnea ({rr} breaths/min) - above normal respiratory rate",
                    RiskLevel = rr > 30 ? RiskLevel.High : RiskLevel.Moderate
                };
            }
        }
        
        private BloodGlucoseAssessment AnalyzeBloodGlucose(decimal? bloodGlucose, BloodGlucoseUnit unit)
        {
            if (!bloodGlucose.HasValue)
            {
                return new BloodGlucoseAssessment
                {
                    Category = BloodGlucoseCategory.Normal,
                    Description = "Blood glucose not measured",
                    RiskLevel = RiskLevel.Normal
                };
            }
            
            // Convert to mg/dL for analysis
            var glucoseMgDl = unit == BloodGlucoseUnit.MmolL ? bloodGlucose.Value * 18.0182m : bloodGlucose.Value;
            var glucoseDisplay = unit == BloodGlucoseUnit.MmolL ? $"{bloodGlucose} mmol/L" : $"{bloodGlucose} mg/dL";
            
            if (glucoseMgDl < 70)
            {
                return new BloodGlucoseAssessment
                {
                    Category = BloodGlucoseCategory.Hypoglycemia,
                    Description = $"Hypoglycemia ({glucoseDisplay}) - low blood sugar",
                    RiskLevel = glucoseMgDl < 54 ? RiskLevel.Critical : RiskLevel.High
                };
            }
            else if (glucoseMgDl < 100)
            {
                return new BloodGlucoseAssessment
                {
                    Category = BloodGlucoseCategory.Normal,
                    Description = $"Normal blood glucose ({glucoseDisplay})",
                    RiskLevel = RiskLevel.Normal
                };
            }
            else if (glucoseMgDl < 126)
            {
                return new BloodGlucoseAssessment
                {
                    Category = BloodGlucoseCategory.Prediabetes,
                    Description = $"Prediabetes range ({glucoseDisplay}) - elevated glucose levels",
                    RiskLevel = RiskLevel.Moderate
                };
            }
            else
            {
                return new BloodGlucoseAssessment
                {
                    Category = BloodGlucoseCategory.Diabetes,
                    Description = $"Diabetes range ({glucoseDisplay}) - high blood glucose",
                    RiskLevel = glucoseMgDl > 250 ? RiskLevel.Critical : RiskLevel.High
                };
            }
        }
        
        private BMIAssessment AnalyzeBMI(decimal? bmi)
        {
            if (!bmi.HasValue)
            {
                return new BMIAssessment
                {
                    Category = BMICategory.Normal,
                    Description = "BMI not calculated (height or weight missing)",
                    RiskLevel = RiskLevel.Normal
                };
            }
            
            var bmiValue = bmi.Value;
            
            if (bmiValue < 18.5m)
            {
                return new BMIAssessment
                {
                    Category = BMICategory.Underweight,
                    Description = $"Underweight (BMI: {bmiValue:F1}) - below normal weight range",
                    RiskLevel = RiskLevel.Moderate
                };
            }
            else if (bmiValue < 25.0m)
            {
                return new BMIAssessment
                {
                    Category = BMICategory.Normal,
                    Description = $"Normal weight (BMI: {bmiValue:F1})",
                    RiskLevel = RiskLevel.Normal
                };
            }
            else if (bmiValue < 30.0m)
            {
                return new BMIAssessment
                {
                    Category = BMICategory.Overweight,
                    Description = $"Overweight (BMI: {bmiValue:F1}) - above normal weight range",
                    RiskLevel = RiskLevel.Low
                };
            }
            else if (bmiValue < 35.0m)
            {
                return new BMIAssessment
                {
                    Category = BMICategory.ObeseClass1,
                    Description = $"Obesity Class I (BMI: {bmiValue:F1}) - moderate obesity",
                    RiskLevel = RiskLevel.Moderate
                };
            }
            else if (bmiValue < 40.0m)
            {
                return new BMIAssessment
                {
                    Category = BMICategory.ObeseClass2,
                    Description = $"Obesity Class II (BMI: {bmiValue:F1}) - severe obesity",
                    RiskLevel = RiskLevel.High
                };
            }
            else
            {
                return new BMIAssessment
                {
                    Category = BMICategory.ObeseClass3,
                    Description = $"Obesity Class III (BMI: {bmiValue:F1}) - extreme obesity",
                    RiskLevel = RiskLevel.High
                };
            }
        }
        
        private PainAssessment AnalyzePain(int? painLevel)
        {
            if (!painLevel.HasValue)
            {
                return new PainAssessment
                {
                    Category = PainCategory.None,
                    Description = "Pain level not assessed",
                    RiskLevel = RiskLevel.Normal
                };
            }
            
            var pain = painLevel.Value;
            
            if (pain == 0)
            {
                return new PainAssessment
                {
                    Category = PainCategory.None,
                    Description = "No pain reported",
                    RiskLevel = RiskLevel.Normal
                };
            }
            else if (pain <= 3)
            {
                return new PainAssessment
                {
                    Category = PainCategory.Mild,
                    Description = $"Mild pain (Level {pain}/10) - manageable discomfort",
                    RiskLevel = RiskLevel.Low
                };
            }
            else if (pain <= 6)
            {
                return new PainAssessment
                {
                    Category = PainCategory.Moderate,
                    Description = $"Moderate pain (Level {pain}/10) - interferes with activities",
                    RiskLevel = RiskLevel.Moderate
                };
            }
            else
            {
                return new PainAssessment
                {
                    Category = PainCategory.Severe,
                    Description = $"Severe pain (Level {pain}/10) - debilitating pain",
                    RiskLevel = RiskLevel.High
                };
            }
        }
        
        private List<VitalSignsAlert> GenerateAlerts(VitalSignsAssessment assessment)
        {
            var alerts = new List<VitalSignsAlert>();
            
            // Check each vital sign for critical values
            if (assessment.BloodPressure.RiskLevel == RiskLevel.Critical)
            {
                alerts.Add(new VitalSignsAlert
                {
                    Parameter = "Blood Pressure",
                    Message = assessment.BloodPressure.Description,
                    Severity = AlertSeverity.Critical,
                    RequiresImmediateAttention = true
                });
            }
            
            if (assessment.OxygenSaturation.RiskLevel == RiskLevel.Critical)
            {
                alerts.Add(new VitalSignsAlert
                {
                    Parameter = "Oxygen Saturation",
                    Message = assessment.OxygenSaturation.Description,
                    Severity = AlertSeverity.Critical,
                    RequiresImmediateAttention = true
                });
            }
            
            if (assessment.Temperature.RiskLevel == RiskLevel.Critical)
            {
                alerts.Add(new VitalSignsAlert
                {
                    Parameter = "Temperature",
                    Message = assessment.Temperature.Description,
                    Severity = AlertSeverity.Critical,
                    RequiresImmediateAttention = true
                });
            }
            
            if (assessment.BloodGlucose.RiskLevel == RiskLevel.Critical)
            {
                alerts.Add(new VitalSignsAlert
                {
                    Parameter = "Blood Glucose",
                    Message = assessment.BloodGlucose.Description,
                    Severity = AlertSeverity.Critical,
                    RequiresImmediateAttention = true
                });
            }
            
            // Add high-risk alerts
            var highRiskAssessments = new (object vital, string name)[]
            {
                (assessment.HeartRate, "Heart Rate"),
                (assessment.RespiratoryRate, "Respiratory Rate"),
                (assessment.Pain, "Pain Level")
            };
            
            foreach (var (vital, name) in highRiskAssessments)
            {
                var vitalAssessment = vital as dynamic;
                if (vitalAssessment?.RiskLevel == RiskLevel.High)
                {
                    alerts.Add(new VitalSignsAlert
                    {
                        Parameter = name,
                        Message = vitalAssessment.Description,
                        Severity = AlertSeverity.Warning,
                        RequiresImmediateAttention = false
                    });
                }
            }
            
            return alerts;
        }
        
        private OverallRiskLevel CalculateOverallRiskLevel(VitalSignsAssessment assessment)
        {
            var riskLevels = new[]
            {
                assessment.BloodPressure.RiskLevel,
                assessment.HeartRate.RiskLevel,
                assessment.Temperature.RiskLevel,
                assessment.OxygenSaturation.RiskLevel,
                assessment.RespiratoryRate.RiskLevel,
                assessment.BloodGlucose.RiskLevel,
                assessment.BMI.RiskLevel,
                assessment.Pain.RiskLevel
            };
            
            var maxRisk = riskLevels.Max();
            
            return maxRisk switch
            {
                RiskLevel.Critical => OverallRiskLevel.CriticalRisk,
                RiskLevel.High => OverallRiskLevel.HighRisk,
                RiskLevel.Moderate => OverallRiskLevel.ModerateRisk,
                RiskLevel.Low => OverallRiskLevel.LowRisk,
                _ => OverallRiskLevel.Normal
            };
        }
        
        private List<string> GenerateRecommendations(VitalSignsAssessment assessment)
        {
            var recommendations = new List<string>();
            
            // Blood pressure recommendations
            if (assessment.BloodPressure.Category == BloodPressureCategory.HypertensiveCrisis)
            {
                recommendations.Add("Seek immediate emergency medical care for hypertensive crisis");
            }
            else if (assessment.BloodPressure.Category == BloodPressureCategory.HighStage2)
            {
                recommendations.Add("Consult physician about blood pressure medication");
                recommendations.Add("Implement DASH diet and reduce sodium intake");
            }
            else if (assessment.BloodPressure.Category == BloodPressureCategory.HighStage1 || 
                     assessment.BloodPressure.Category == BloodPressureCategory.Elevated)
            {
                recommendations.Add("Increase physical activity and monitor blood pressure regularly");
                recommendations.Add("Reduce stress and maintain healthy weight");
            }
            
            // Temperature recommendations
            if (assessment.Temperature.Category == TemperatureCategory.HighFever)
            {
                recommendations.Add("Monitor temperature closely and consider fever-reducing medication");
                recommendations.Add("Stay hydrated and seek medical attention if fever persists");
            }
            else if (assessment.Temperature.Category == TemperatureCategory.Hypothermia)
            {
                recommendations.Add("Seek immediate medical attention for hypothermia");
            }
            
            // Oxygen saturation recommendations
            if (assessment.OxygenSaturation.Category == OxygenSaturationCategory.Critical)
            {
                recommendations.Add("Administer oxygen therapy immediately");
            }
            else if (assessment.OxygenSaturation.Category == OxygenSaturationCategory.Low)
            {
                recommendations.Add("Monitor breathing and consider supplemental oxygen");
            }
            
            // BMI recommendations
            if (assessment.BMI.Category == BMICategory.ObeseClass2 || assessment.BMI.Category == BMICategory.ObeseClass3)
            {
                recommendations.Add("Consult healthcare provider about weight management program");
            }
            else if (assessment.BMI.Category == BMICategory.Overweight || assessment.BMI.Category == BMICategory.ObeseClass1)
            {
                recommendations.Add("Consider lifestyle modifications for weight management");
            }
            
            // Pain recommendations
            if (assessment.Pain.Category == PainCategory.Severe)
            {
                recommendations.Add("Evaluate need for pain management intervention");
            }
            
            // Blood glucose recommendations
            if (assessment.BloodGlucose.Category == BloodGlucoseCategory.Diabetes)
            {
                recommendations.Add("Monitor blood glucose levels regularly and follow diabetes management plan");
            }
            else if (assessment.BloodGlucose.Category == BloodGlucoseCategory.Prediabetes)
            {
                recommendations.Add("Implement lifestyle changes to prevent progression to diabetes");
            }
            
            return recommendations;
        }
    }
}
