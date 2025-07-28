namespace Aesclea_Back_End_.Models.VitalSigns
{
    public class VitalSignsTrends
    {
        public int PatientId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<VitalSignsReading> Readings { get; set; } = new();
        
        // Trend data for charts/graphs
        public BloodPressureTrend BloodPressureTrend { get; set; } = new();
        public HeartRateTrend HeartRateTrend { get; set; } = new();
        public TemperatureTrend TemperatureTrend { get; set; } = new();
        public WeightTrend WeightTrend { get; set; } = new();
        
        // Summary statistics
        public VitalSignsStatistics Statistics { get; set; } = new();
    }
    
    public class BloodPressureTrend
    {
        public List<DataPoint> SystolicPoints { get; set; } = new();
        public List<DataPoint> DiastolicPoints { get; set; } = new();
        public TrendDirection Direction { get; set; }
        public decimal? AverageSystolic { get; set; }
        public decimal? AverageDiastolic { get; set; }
    }
    
    public class HeartRateTrend
    {
        public List<DataPoint> Points { get; set; } = new();
        public TrendDirection Direction { get; set; }
        public decimal? Average { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
    }
    
    public class TemperatureTrend
    {
        public List<DataPoint> Points { get; set; } = new();
        public TrendDirection Direction { get; set; }
        public decimal? Average { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
    
    public class WeightTrend
    {
        public List<DataPoint> Points { get; set; } = new();
        public TrendDirection Direction { get; set; }
        public decimal? Average { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public decimal? WeightChange { get; set; } // Change from first to last reading
    }
    
    public class DataPoint
    {
        public DateTime Timestamp { get; set; }
        public decimal Value { get; set; }
    }
    
    public class VitalSignsStatistics
    {
        public int TotalReadings { get; set; }
        public int CriticalAlerts { get; set; }
        public int WarningAlerts { get; set; }
        public DateTime? LastReading { get; set; }
        public TimeSpan? AverageTimeBetweenReadings { get; set; }
        
        // Compliance metrics
        public decimal ReadingComplianceRate { get; set; } // Percentage of expected readings
        public List<string> MissedReadingDates { get; set; } = new();
    }
    
    public enum TrendDirection
    {
        Stable,
        Improving,
        Declining,
        Increasing,
        Decreasing
    }
}
