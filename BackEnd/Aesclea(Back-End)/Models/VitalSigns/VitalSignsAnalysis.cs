namespace Aesclea_Back_End_.Models.VitalSigns
{
    public class VitalSignsAnalysis
    {
        public required VitalSignsReading Reading { get; set; }
        public required VitalSignsAssessment Assessment { get; set; }
        public List<VitalSignsAlert> Alerts { get; set; } = new();
        public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class VitalSignsAssessment
    {
        public required BloodPressureAssessment BloodPressure { get; set; }
        public required HeartRateAssessment HeartRate { get; set; }
        public required TemperatureAssessment Temperature { get; set; }
        public required OxygenSaturationAssessment OxygenSaturation { get; set; }
        public required RespiratoryRateAssessment RespiratoryRate { get; set; }
        public required BloodGlucoseAssessment BloodGlucose { get; set; }
        public required BMIAssessment BMI { get; set; }
        public required PainAssessment Pain { get; set; }
        public OverallRiskLevel OverallRisk { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }
    
    public class BloodPressureAssessment
    {
        public BloodPressureCategory Category { get; set; }
        public required string Description { get; set; }
        public RiskLevel RiskLevel { get; set; }
    }
    
    public class HeartRateAssessment
    {
        public HeartRateCategory Category { get; set; }
        public required string Description { get; set; }
        public RiskLevel RiskLevel { get; set; }
    }
    
    public class TemperatureAssessment
    {
        public TemperatureCategory Category { get; set; }
        public required string Description { get; set; }
        public RiskLevel RiskLevel { get; set; }
    }
    
    public class OxygenSaturationAssessment
    {
        public OxygenSaturationCategory Category { get; set; }
        public required string Description { get; set; }
        public RiskLevel RiskLevel { get; set; }
    }
    
    public class RespiratoryRateAssessment
    {
        public RespiratoryRateCategory Category { get; set; }
        public required string Description { get; set; }
        public RiskLevel RiskLevel { get; set; }
    }
    
    public class BloodGlucoseAssessment
    {
        public BloodGlucoseCategory Category { get; set; }
        public required string Description { get; set; }
        public RiskLevel RiskLevel { get; set; }
    }
    
    public class BMIAssessment
    {
        public BMICategory Category { get; set; }
        public required string Description { get; set; }
        public RiskLevel RiskLevel { get; set; }
    }
    
    public class PainAssessment
    {
        public PainCategory Category { get; set; }
        public required string Description { get; set; }
        public RiskLevel RiskLevel { get; set; }
    }
    
    public class VitalSignsAlert
    {
        public required string Parameter { get; set; }
        public required string Message { get; set; }
        public AlertSeverity Severity { get; set; }
        public bool RequiresImmediateAttention { get; set; }
    }
    
    // Enums for categorization
    public enum BloodPressureCategory
    {
        Normal,
        Elevated,
        HighStage1,
        HighStage2,
        HypertensiveCrisis,
        Low
    }
    
    public enum HeartRateCategory
    {
        Bradycardia,
        Normal,
        Tachycardia
    }
    
    public enum TemperatureCategory
    {
        Hypothermia,
        Normal,
        LowGradeFever,
        Fever,
        HighFever
    }
    
    public enum OxygenSaturationCategory
    {
        Critical,
        Low,
        Normal
    }
    
    public enum RespiratoryRateCategory
    {
        Bradypnea,
        Normal,
        Tachypnea
    }
    
    public enum BloodGlucoseCategory
    {
        Hypoglycemia,
        Normal,
        Prediabetes,
        Diabetes
    }
    
    public enum BMICategory
    {
        Underweight,
        Normal,
        Overweight,
        ObeseClass1,
        ObeseClass2,
        ObeseClass3
    }
    
    public enum PainCategory
    {
        None,
        Mild,
        Moderate,
        Severe
    }
    
    public enum RiskLevel
    {
        Normal,
        Low,
        Moderate,
        High,
        Critical
    }
    
    public enum OverallRiskLevel
    {
        Normal,
        LowRisk,
        ModerateRisk,
        HighRisk,
        CriticalRisk
    }
    
    public enum AlertSeverity
    {
        Info,
        Warning,
        Critical
    }
}
