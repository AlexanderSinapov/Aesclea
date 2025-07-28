namespace Aesclea_Back_End_.Models
{
    public class EnhancedTextAnalysisResult
    {
        public required string OriginalText { get; set; }
        public required string ProcessedText { get; set; }
        public DateTime AnalyzedAt { get; set; }
        
        public List<MedicalEntity> MedicalEntities { get; set; } = new();
        public SentimentAnalysisResult Sentiment { get; set; } = new();
        public SymptomAnalysisResult Symptoms { get; set; } = new();
        public RiskAssessmentResult RiskAssessment { get; set; } = new();
        public List<VitalSignMention> VitalSigns { get; set; } = new();
        public List<MedicationMention> Medications { get; set; } = new();
        public List<TemporalPattern> TemporalPatterns { get; set; } = new();
        public List<SeverityIndicator> SeverityIndicators { get; set; } = new();
        public List<ClinicalInsight> ClinicalInsights { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
    }

    public class TextAnalysisOptions
    {
        public bool IncludeSentimentAnalysis { get; set; } = true;
        public bool IncludeEntityExtraction { get; set; } = true;
        public bool IncludeSymptomAnalysis { get; set; } = true;
        public bool IncludeRiskAssessment { get; set; } = true;
        public bool IncludeVitalSigns { get; set; } = true;
        public bool IncludeMedications { get; set; } = true;
        public bool GenerateInsights { get; set; } = true;
        public bool GenerateRecommendations { get; set; } = true;
    }

    public class MedicalEntity
    {
        public required string Text { get; set; }
        public required string Category { get; set; }
        public int Position { get; set; }
        public int Length { get; set; }
        public decimal Confidence { get; set; }
        public string? SubCategory { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class SentimentAnalysisResult
    {
        public SentimentType Sentiment { get; set; }
        public decimal Confidence { get; set; }
        public int PositiveIndicators { get; set; }
        public int NegativeIndicators { get; set; }
        public int ConcernIndicators { get; set; }
        public List<string> Keywords { get; set; } = new();
    }

    public class SymptomAnalysisResult
    {
        public List<SymptomMention> Symptoms { get; set; } = new();
        public List<string> AffectedBodyParts { get; set; } = new();
        public int SymptomCount { get; set; }
        public Dictionary<int, int> SeverityDistribution { get; set; } = new();
        public List<string> SymptomClusters { get; set; } = new();
    }

    public class SymptomMention
    {
        public required string Symptom { get; set; }
        public int Severity { get; set; } // 1-4 scale
        public string BodyPart { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public DateTime? OnsetTime { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
    }

    public class RiskAssessmentResult
    {
        public List<RiskFactor> RiskFactors { get; set; } = new();
        public Models.VitalSigns.RiskLevel OverallRiskLevel { get; set; }
        public decimal RiskScore { get; set; } // 0-10 scale
        public List<string> Recommendations { get; set; } = new();
        public Dictionary<string, int> RiskCategories { get; set; } = new();
    }

    public class RiskFactor
    {
        public required string Factor { get; set; }
        public required string Category { get; set; }
        public Models.VitalSigns.RiskLevel RiskLevel { get; set; }
        public required string Description { get; set; }
        public decimal Confidence { get; set; } = 0.8m;
        public string? Source { get; set; }
    }

    public class VitalSignMention
    {
        public required string Type { get; set; }
        public required string Value { get; set; }
        public required string RawText { get; set; }
        public bool IsNormal { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime? Timestamp { get; set; }
    }

    public class MedicationMention
    {
        public required string Name { get; set; }
        public required string Category { get; set; }
        public string Context { get; set; } = string.Empty;
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public string? Route { get; set; }
        public bool IsCurrentlyTaking { get; set; }
    }

    public class TemporalPattern
    {
        public required string Indicator { get; set; }
        public required string Type { get; set; }
        public string Context { get; set; } = string.Empty;
        public DateTime? SpecificTime { get; set; }
        public string Duration { get; set; } = string.Empty;
    }

    public class SeverityIndicator
    {
        public required string Term { get; set; }
        public int Level { get; set; } // 1-4 scale
        public string Context { get; set; } = string.Empty;
        public decimal Confidence { get; set; } = 0.8m;
    }

    public class ClinicalInsight
    {
        public required string Type { get; set; }
        public required string Description { get; set; }
        public decimal Confidence { get; set; }
        public string Category { get; set; } = "General";
        public List<string> SupportingEvidence { get; set; } = new();
        public string? Recommendation { get; set; }
    }

    public enum SentimentType
    {
        Positive,
        Negative,
        Neutral,
        Concerned,
        Anxious
    }
}
