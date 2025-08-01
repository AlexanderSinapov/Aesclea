using Aesclea_Back_End_.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Aesclea_Back_End_.Services
{
    public class EnhancedTextAnalysisService : IEnhancedTextAnalysisService
    {
        private readonly ILogger<EnhancedTextAnalysisService> _logger;

        public EnhancedTextAnalysisService(ILogger<EnhancedTextAnalysisService> logger)
        {
            _logger = logger;
        }

        public async Task<EnhancedTextAnalysisResult> AnalyzeMedicalTextAsync(string text, TextAnalysisOptions? options = null)
        {
            _logger.LogInformation("Starting medical text analysis for text of length {Length}", text.Length);

            options ??= new TextAnalysisOptions();

            var result = new EnhancedTextAnalysisResult
            {
                OriginalText = text,
                ProcessedText = ProcessText(text),
                AnalyzedAt = DateTime.UtcNow
            };

            // Analyze based on options
            if (options.IncludeEntityExtraction)
            {
                result.MedicalEntities = ExtractMedicalEntities(text);
            }

            if (options.IncludeSentimentAnalysis)
            {
                result.Sentiment = AnalyzeSentiment(text);
            }

            if (options.IncludeSymptomAnalysis)
            {
                result.Symptoms = AnalyzeSymptoms(text);
            }

            if (options.IncludeRiskAssessment)
            {
                result.RiskAssessment = AssessRisk(text, result.Symptoms);
            }

            if (options.IncludeVitalSigns)
            {
                result.VitalSigns = ExtractVitalSigns(text);
            }

            if (options.IncludeMedications)
            {
                result.Medications = ExtractMedications(text);
            }

            if (options.GenerateInsights)
            {
                result.ClinicalInsights = GenerateClinicalInsights(text, result);
            }

            if (options.GenerateRecommendations)
            {
                result.Recommendations = GenerateRecommendations(result);
            }

            _logger.LogInformation("Medical text analysis completed successfully");

            return await Task.FromResult(result);
        }

        private string ProcessText(string text)
        {
            // Basic text processing
            return text.Trim().ToLowerInvariant();
        }

        private List<MedicalEntity> ExtractMedicalEntities(string text)
        {
            var entities = new List<MedicalEntity>();

            // Simple entity extraction patterns
            var patterns = new Dictionary<string, string>
            {
                { "symptom", @"\b(pain|headache|fever|nausea|fatigue|dizziness|shortness of breath|chest pain|cough|weakness)\b" },
                { "anatomy", @"\b(head|chest|heart|lung|brain|stomach|back|neck|arm|leg|knee|shoulder)\b" },
                { "condition", @"\b(diabetes|hypertension|asthma|depression|anxiety|cancer|infection|inflammation)\b" },
                { "medication", @"\b(aspirin|ibuprofen|acetaminophen|insulin|metformin|lisinopril|atorvastatin)\b" }
            };

            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(text, pattern.Value, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    entities.Add(new MedicalEntity
                    {
                        Text = match.Value,
                        Category = pattern.Key,
                        Position = match.Index,
                        Length = match.Length,
                        Confidence = 0.8m
                    });
                }
            }

            return entities;
        }

        private SentimentAnalysisResult AnalyzeSentiment(string text)
        {
            var lowerText = text.ToLowerInvariant();
            
            var positiveWords = new[] { "good", "better", "improved", "healing", "recovery", "well", "fine", "normal" };
            var negativeWords = new[] { "pain", "bad", "worse", "terrible", "awful", "hurt", "sick", "ill" };
            var concernWords = new[] { "worried", "concerned", "anxious", "scared", "nervous", "urgent", "emergency" };

            var positiveCount = positiveWords.Count(word => lowerText.Contains(word));
            var negativeCount = negativeWords.Count(word => lowerText.Contains(word));
            var concernCount = concernWords.Count(word => lowerText.Contains(word));

            SentimentType sentiment;
            decimal confidence;

            if (concernCount > 0)
            {
                sentiment = SentimentType.Concerned;
                confidence = 0.7m + (concernCount * 0.1m);
            }
            else if (negativeCount > positiveCount)
            {
                sentiment = SentimentType.Negative;
                confidence = 0.6m + ((negativeCount - positiveCount) * 0.1m);
            }
            else if (positiveCount > negativeCount)
            {
                sentiment = SentimentType.Positive;
                confidence = 0.6m + ((positiveCount - negativeCount) * 0.1m);
            }
            else
            {
                sentiment = SentimentType.Neutral;
                confidence = 0.5m;
            }

            return new SentimentAnalysisResult
            {
                Sentiment = sentiment,
                Confidence = Math.Min(confidence, 1.0m),
                PositiveIndicators = positiveCount,
                NegativeIndicators = negativeCount,
                ConcernIndicators = concernCount
            };
        }

        private SymptomAnalysisResult AnalyzeSymptoms(string text)
        {
            var symptoms = new List<SymptomMention>();
            var bodyParts = new HashSet<string>();
            var severityDistribution = new Dictionary<int, int>();

            // Simple symptom detection
            var symptomPatterns = new Dictionary<string, int>
            {
                { "severe pain", 4 },
                { "extreme pain", 4 },
                { "moderate pain", 3 },
                { "mild pain", 2 },
                { "pain", 2 },
                { "headache", 2 },
                { "fever", 3 },
                { "nausea", 2 },
                { "fatigue", 2 },
                { "dizziness", 2 },
                { "shortness of breath", 3 },
                { "chest pain", 3 },
                { "cough", 1 }
            };

            var bodyPartPatterns = new[] { "head", "chest", "heart", "lung", "brain", "stomach", "back", "neck", "arm", "leg", "knee", "shoulder" };

            foreach (var symptomPattern in symptomPatterns)
            {
                if (text.ToLowerInvariant().Contains(symptomPattern.Key))
                {
                    var bodyPart = bodyPartPatterns.FirstOrDefault(bp => text.ToLowerInvariant().Contains(bp)) ?? "";
                    if (!string.IsNullOrEmpty(bodyPart))
                    {
                        bodyParts.Add(bodyPart);
                    }

                    symptoms.Add(new SymptomMention
                    {
                        Symptom = symptomPattern.Key,
                        Severity = symptomPattern.Value,
                        BodyPart = bodyPart
                    });

                    severityDistribution[symptomPattern.Value] = severityDistribution.GetValueOrDefault(symptomPattern.Value) + 1;
                }
            }

            return new SymptomAnalysisResult
            {
                Symptoms = symptoms,
                AffectedBodyParts = bodyParts.ToList(),
                SymptomCount = symptoms.Count,
                SeverityDistribution = severityDistribution
            };
        }

        private RiskAssessmentResult AssessRisk(string text, SymptomAnalysisResult symptoms)
        {
            var riskFactors = new List<RiskFactor>();
            var riskScore = 0m;

            // Assess based on symptoms
            foreach (var symptom in symptoms.Symptoms)
            {
                riskScore += symptom.Severity;

                if (symptom.Severity >= 3)
                {
                    riskFactors.Add(new RiskFactor
                    {
                        Factor = symptom.Symptom,
                        Category = "Symptom",
                        RiskLevel = symptom.Severity >= 4 ? Models.VitalSigns.RiskLevel.Critical : Models.VitalSigns.RiskLevel.High,
                        Description = $"High severity symptom: {symptom.Symptom}"
                    });
                }
            }

            // Check for emergency keywords
            var emergencyKeywords = new[] { "emergency", "urgent", "critical", "severe", "extreme", "unbearable" };
            if (emergencyKeywords.Any(keyword => text.ToLowerInvariant().Contains(keyword)))
            {
                riskScore += 3;
                riskFactors.Add(new RiskFactor
                {
                    Factor = "Emergency indicators",
                    Category = "Urgency",
                    RiskLevel = Models.VitalSigns.RiskLevel.Critical,
                    Description = "Text contains emergency or urgent language"
                });
            }

            // Determine overall risk level
            Models.VitalSigns.RiskLevel overallRisk;
            if (riskScore >= 10)
                overallRisk = Models.VitalSigns.RiskLevel.Critical;
            else if (riskScore >= 7)
                overallRisk = Models.VitalSigns.RiskLevel.High;
            else if (riskScore >= 4)
                overallRisk = Models.VitalSigns.RiskLevel.Moderate;
            else if (riskScore >= 2)
                overallRisk = Models.VitalSigns.RiskLevel.Low;
            else
                overallRisk = Models.VitalSigns.RiskLevel.Normal;

            return new RiskAssessmentResult
            {
                RiskFactors = riskFactors,
                OverallRiskLevel = overallRisk,
                RiskScore = Math.Min(riskScore, 10m)
            };
        }

        private List<VitalSignMention> ExtractVitalSigns(string text)
        {
            var vitalSigns = new List<VitalSignMention>();

            // Blood pressure pattern
            var bpPattern = @"(\d{2,3})/(\d{2,3})\s*(mmhg|mm hg)?";
            var bpMatches = Regex.Matches(text, bpPattern, RegexOptions.IgnoreCase);
            foreach (Match match in bpMatches)
            {
                var systolic = int.Parse(match.Groups[1].Value);
                var diastolic = int.Parse(match.Groups[2].Value);
                var isNormal = systolic >= 90 && systolic <= 140 && diastolic >= 60 && diastolic <= 90;

                vitalSigns.Add(new VitalSignMention
                {
                    Type = "Blood Pressure",
                    Value = match.Value,
                    RawText = match.Value,
                    IsNormal = isNormal,
                    Unit = "mmHg"
                });
            }

            // Temperature pattern
            var tempPattern = @"(\d{2,3}(?:\.\d)?)\s*°?[cf]?";
            var tempMatches = Regex.Matches(text, tempPattern, RegexOptions.IgnoreCase);
            foreach (Match match in tempMatches)
            {
                if (decimal.TryParse(match.Groups[1].Value, out var temp))
                {
                    var isNormal = temp >= 97.0m && temp <= 99.0m; // Assuming Fahrenheit
                    vitalSigns.Add(new VitalSignMention
                    {
                        Type = "Temperature",
                        Value = match.Groups[1].Value,
                        RawText = match.Value,
                        IsNormal = isNormal,
                        Unit = "°F"
                    });
                }
            }

            return vitalSigns;
        }

        private List<MedicationMention> ExtractMedications(string text)
        {
            var medications = new List<MedicationMention>();

            var commonMedications = new Dictionary<string, string>
            {
                { "aspirin", "Pain Relief" },
                { "ibuprofen", "Pain Relief" },
                { "acetaminophen", "Pain Relief" },
                { "tylenol", "Pain Relief" },
                { "insulin", "Diabetes" },
                { "metformin", "Diabetes" },
                { "lisinopril", "Blood Pressure" },
                { "atorvastatin", "Cholesterol" },
                { "omeprazole", "Acid Reflux" },
                { "albuterol", "Asthma" }
            };

            foreach (var medication in commonMedications)
            {
                if (text.ToLowerInvariant().Contains(medication.Key))
                {
                    medications.Add(new MedicationMention
                    {
                        Name = medication.Key,
                        Category = medication.Value
                    });
                }
            }

            return medications;
        }

        private List<ClinicalInsight> GenerateClinicalInsights(string text, EnhancedTextAnalysisResult result)
        {
            var insights = new List<ClinicalInsight>();

            // Insight based on symptom patterns
            if (result.Symptoms.SymptomCount > 3)
            {
                insights.Add(new ClinicalInsight
                {
                    Type = "Multi-symptom presentation",
                    Description = "Multiple symptoms reported, suggesting possible systemic condition",
                    Confidence = 0.7m,
                    Category = "Pattern Recognition"
                });
            }

            // Insight based on risk level
            if (result.RiskAssessment.OverallRiskLevel >= Models.VitalSigns.RiskLevel.High)
            {
                insights.Add(new ClinicalInsight
                {
                    Type = "High-risk presentation",
                    Description = "Patient presentation suggests high-risk condition requiring immediate attention",
                    Confidence = 0.8m,
                    Category = "Risk Assessment"
                });
            }

            // Insight based on sentiment
            if (result.Sentiment.Sentiment == SentimentType.Concerned)
            {
                insights.Add(new ClinicalInsight
                {
                    Type = "Patient anxiety indicators",
                    Description = "Language suggests patient anxiety or concern about symptoms",
                    Confidence = 0.6m,
                    Category = "Psychological"
                });
            }

            return insights;
        }

        private List<string> GenerateRecommendations(EnhancedTextAnalysisResult result)
        {
            var recommendations = new List<string>();

            // Recommendations based on risk level
            switch (result.RiskAssessment.OverallRiskLevel)
            {
                case Models.VitalSigns.RiskLevel.Critical:
                    recommendations.Add("Immediate medical attention required - consider emergency care");
                    recommendations.Add("Monitor vital signs closely");
                    break;
                case Models.VitalSigns.RiskLevel.High:
                    recommendations.Add("Schedule urgent medical evaluation");
                    recommendations.Add("Consider same-day appointment with healthcare provider");
                    break;
                case Models.VitalSigns.RiskLevel.Moderate:
                    recommendations.Add("Schedule medical evaluation within 1-2 days");
                    recommendations.Add("Monitor symptoms for changes");
                    break;
                case Models.VitalSigns.RiskLevel.Low:
                    recommendations.Add("Consider scheduling routine appointment");
                    recommendations.Add("Continue monitoring symptoms");
                    break;
                default:
                    recommendations.Add("Continue routine care as appropriate");
                    break;
            }

            // Recommendations based on symptoms
            if (result.Symptoms.Symptoms.Any(s => s.Symptom.Contains("pain") && s.Severity >= 3))
            {
                recommendations.Add("Consider pain management strategies");
                recommendations.Add("Document pain location and triggers");
            }

            // Recommendations based on sentiment
            if (result.Sentiment.Sentiment == SentimentType.Concerned)
            {
                recommendations.Add("Provide patient education and reassurance");
                recommendations.Add("Address patient concerns and questions");
            }

            return recommendations;
        }
    }
}
