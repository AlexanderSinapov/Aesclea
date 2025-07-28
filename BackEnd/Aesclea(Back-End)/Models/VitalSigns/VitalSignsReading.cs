using System.ComponentModel.DataAnnotations;

namespace Aesclea_Back_End_.Models.VitalSigns
{
    public class VitalSignsReading
    {
        public int Id { get; set; }
        
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        
        // Blood Pressure
        public int? SystolicBP { get; set; }
        public int? DiastolicBP { get; set; }
        
        // Heart Rate
        public int? HeartRate { get; set; }
        
        // Temperature
        public decimal? Temperature { get; set; }
        public TemperatureUnit TemperatureUnit { get; set; } = TemperatureUnit.Celsius;
        
        // Respiratory Rate
        public int? RespiratoryRate { get; set; }
        
        // Oxygen Saturation
        public decimal? OxygenSaturation { get; set; }
        
        // Blood Sugar
        public decimal? BloodGlucose { get; set; }
        public BloodGlucoseUnit BloodGlucoseUnit { get; set; } = BloodGlucoseUnit.MgDl;
        
        // Weight and BMI
        public decimal? Weight { get; set; }
        public WeightUnit WeightUnit { get; set; } = WeightUnit.Kg;
        public decimal? Height { get; set; }
        public HeightUnit HeightUnit { get; set; } = HeightUnit.Cm;
        
        // Pain Scale
        public int? PainLevel { get; set; } // 0-10 scale
        
        // Additional notes
        public string? Notes { get; set; }
        
        // Recorded by
        public int? RecordedByUserId { get; set; }
        
        // Navigation properties
        public Patient Patient { get; set; }
        public User? RecordedBy { get; set; }
        
        // Calculated properties
        public decimal? BMI
        {
            get
            {
                if (Weight.HasValue && Height.HasValue && Height.Value > 0)
                {
                    var weightKg = ConvertToKg(Weight.Value, WeightUnit);
                    var heightM = ConvertToMeters(Height.Value, HeightUnit);
                    return Math.Round(weightKg / (heightM * heightM), 2);
                }
                return null;
            }
        }
        
        public string BloodPressureFormatted
        {
            get
            {
                if (SystolicBP.HasValue && DiastolicBP.HasValue)
                {
                    return $"{SystolicBP}/{DiastolicBP}";
                }
                return "N/A";
            }
        }
        
        private decimal ConvertToKg(decimal weight, WeightUnit unit)
        {
            return unit switch
            {
                WeightUnit.Kg => weight,
                WeightUnit.Lbs => weight * 0.453592m,
                _ => weight
            };
        }
        
        private decimal ConvertToMeters(decimal height, HeightUnit unit)
        {
            return unit switch
            {
                HeightUnit.Cm => height / 100,
                HeightUnit.Inches => height * 0.0254m,
                HeightUnit.Feet => height * 0.3048m,
                _ => height
            };
        }
    }
    
    public enum TemperatureUnit
    {
        Celsius = 0,
        Fahrenheit = 1
    }
    
    public enum BloodGlucoseUnit
    {
        MgDl = 0, // mg/dL
        MmolL = 1 // mmol/L
    }
    
    public enum WeightUnit
    {
        Kg = 0,
        Lbs = 1
    }
    
    public enum HeightUnit
    {
        Cm = 0,
        Inches = 1,
        Feet = 2
    }
}
