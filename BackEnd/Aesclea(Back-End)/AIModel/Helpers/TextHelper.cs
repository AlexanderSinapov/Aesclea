using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Aesclea_Back_End_.AIModel.Helpers
{
    /// <summary>
    /// Helper class for processing medical text data for neural network input
    /// </summary>
    public static class TextHelper
    {
        // Medical terminology dictionary for feature extraction
        private static readonly Dictionary<string, double> MedicalTermWeights = new Dictionary<string, double>
        {
            // Symptoms - High importance
            { "pain", 1.0 }, { "fever", 0.9 }, { "headache", 0.8 }, { "nausea", 0.7 }, { "fatigue", 0.6 },
            { "dizziness", 0.7 }, { "shortness", 0.8 }, { "breathing", 0.8 }, { "chest", 0.9 }, { "abdominal", 0.8 },
            { "vomiting", 0.7 }, { "diarrhea", 0.6 }, { "constipation", 0.5 }, { "swelling", 0.7 }, { "rash", 0.6 },
            
            // Severity indicators
            { "severe", 1.0 }, { "acute", 0.9 }, { "chronic", 0.8 }, { "mild", 0.3 }, { "moderate", 0.6 },
            { "critical", 1.0 }, { "emergency", 1.0 }, { "urgent", 0.9 }, { "stable", 0.2 },
            
            // Body systems
            { "cardiac", 0.9 }, { "respiratory", 0.9 }, { "neurological", 0.9 }, { "gastrointestinal", 0.8 },
            { "musculoskeletal", 0.7 }, { "dermatological", 0.6 }, { "psychiatric", 0.8 }, { "endocrine", 0.8 },
            
            // Common conditions
            { "hypertension", 0.8 }, { "diabetes", 0.8 }, { "pneumonia", 0.9 }, { "asthma", 0.7 },
            { "depression", 0.7 }, { "anxiety", 0.6 }, { "arthritis", 0.6 }, { "infection", 0.8 },
            
            // Diagnostic terms
            { "positive", 0.7 }, { "negative", -0.3 }, { "abnormal", 0.8 }, { "normal", -0.2 },
            { "elevated", 0.6 }, { "decreased", 0.5 }, { "enlarged", 0.7 }, { "inflammation", 0.8 }
        };

        private static readonly string[] StopWords = {
            "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for", "of", "with", "by",
            "is", "are", "was", "were", "be", "been", "being", "have", "has", "had", "do", "does", "did",
            "will", "would", "could", "should", "may", "might", "can", "this", "that", "these", "those"
        };

        /// <summary>
        /// Converts medical text into numerical features for neural network input
        /// </summary>
        /// <param name="text">Raw medical text</param>
        /// <param name="maxFeatures">Maximum number of features to extract</param>
        /// <returns>List of numerical features</returns>
        public static List<double> ProcessMedicalText(string text, int maxFeatures = 512)
        {
            if (string.IsNullOrEmpty(text))
                return new List<double>(new double[maxFeatures]);

            // Clean and tokenize text
            var tokens = TokenizeText(text);
            
            // Extract features
            var features = ExtractMedicalFeatures(tokens, maxFeatures);
            
            // Normalize features
            return NormalizeFeatures(features, maxFeatures);
        }

        /// <summary>
        /// Tokenizes and cleans medical text
        /// </summary>
        private static List<string> TokenizeText(string text)
        {
            // Convert to lowercase and remove special characters except medical notation
            text = text.ToLower();
            text = Regex.Replace(text, @"[^\w\s\-\./]", " ");
            
            // Split into words
            var words = text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Remove stop words and very short words
            var tokens = words.Where(w => w.Length > 2 && !StopWords.Contains(w)).ToList();
            
            // Handle medical abbreviations and compound terms
            tokens = ProcessMedicalTerms(tokens);
            
            return tokens;
        }

        /// <summary>
        /// Processes medical terms and abbreviations
        /// </summary>
        private static List<string> ProcessMedicalTerms(List<string> tokens)
        {
            var processedTokens = new List<string>();
            
            for (int i = 0; i < tokens.Count; i++)
            {
                string token = tokens[i];
                
                // Handle compound medical terms
                if (i < tokens.Count - 1)
                {
                    string compound = token + "_" + tokens[i + 1];
                    if (MedicalTermWeights.ContainsKey(compound))
                    {
                        processedTokens.Add(compound);
                        i++; // Skip next token as it's part of compound
                        continue;
                    }
                }
                
                // Expand common medical abbreviations
                token = ExpandMedicalAbbreviations(token);
                processedTokens.Add(token);
            }
            
            return processedTokens;
        }

        /// <summary>
        /// Expands common medical abbreviations
        /// </summary>
        private static string ExpandMedicalAbbreviations(string token)
        {
            var abbreviations = new Dictionary<string, string>
            {
                { "bp", "blood_pressure" }, { "hr", "heart_rate" }, { "temp", "temperature" },
                { "wbc", "white_blood_cells" }, { "rbc", "red_blood_cells" }, { "ecg", "electrocardiogram" },
                { "mri", "magnetic_resonance_imaging" }, { "ct", "computed_tomography" },
                { "sob", "shortness_of_breath" }, { "rom", "range_of_motion" }, { "npo", "nothing_by_mouth" },
                { "prn", "as_needed" }, { "bid", "twice_daily" }, { "tid", "three_times_daily" },
                { "qid", "four_times_daily" }, { "qd", "once_daily" }, { "od", "once_daily" }
            };

            return abbreviations.ContainsKey(token) ? abbreviations[token] : token;
        }

        /// <summary>
        /// Extracts medical features from tokenized text
        /// </summary>
        private static List<double> ExtractMedicalFeatures(List<string> tokens, int maxFeatures)
        {
            var features = new List<double>();
            
            // 1. Term frequency features
            var termFrequency = CalculateTermFrequency(tokens);
            features.AddRange(GetTopTermFeatures(termFrequency, maxFeatures / 4));
            
            // 2. Medical term weight features
            features.AddRange(GetMedicalTermFeatures(tokens, maxFeatures / 4));
            
            // 3. Sentiment and severity features
            features.AddRange(GetSentimentFeatures(tokens, maxFeatures / 4));
            
            // 4. Statistical features
            features.AddRange(GetStatisticalFeatures(tokens, maxFeatures / 4));
            
            return features;
        }

        /// <summary>
        /// Calculates term frequency
        /// </summary>
        private static Dictionary<string, int> CalculateTermFrequency(List<string> tokens)
        {
            var frequency = new Dictionary<string, int>();
            
            foreach (var token in tokens)
            {
                if (frequency.ContainsKey(token))
                    frequency[token]++;
                else
                    frequency[token] = 1;
            }
            
            return frequency;
        }

        /// <summary>
        /// Gets top term frequency features
        /// </summary>
        private static List<double> GetTopTermFeatures(Dictionary<string, int> termFreq, int count)
        {
            var topTerms = termFreq.OrderByDescending(kv => kv.Value).Take(count);
            var features = new List<double>();
            
            foreach (var term in topTerms)
            {
                features.Add(Math.Log(1 + term.Value)); // Log transform to reduce impact of very frequent terms
            }
            
            // Pad with zeros if needed
            while (features.Count < count)
                features.Add(0.0);
            
            return features;
        }

        /// <summary>
        /// Gets medical term weight features
        /// </summary>
        private static List<double> GetMedicalTermFeatures(List<string> tokens, int count)
        {
            var features = new List<double>();
            var medicalScores = new List<double>();
            
            foreach (var token in tokens)
            {
                if (MedicalTermWeights.ContainsKey(token))
                {
                    medicalScores.Add(MedicalTermWeights[token]);
                }
            }
            
            if (medicalScores.Count > 0)
            {
                features.Add(medicalScores.Average()); // Average medical term weight
                features.Add(medicalScores.Max()); // Maximum medical term weight
                features.Add(medicalScores.Min()); // Minimum medical term weight
                features.Add(medicalScores.Count); // Count of medical terms
            }
            else
            {
                features.AddRange(new double[] { 0, 0, 0, 0 });
            }
            
            // Pad with additional medical category features
            var categoryScores = CalculateCategoryScores(tokens);
            features.AddRange(categoryScores);
            
            // Pad with zeros if needed
            while (features.Count < count)
                features.Add(0.0);
            
            return features.Take(count).ToList();
        }

        /// <summary>
        /// Calculates scores for different medical categories
        /// </summary>
        private static List<double> CalculateCategoryScores(List<string> tokens)
        {
            var categories = new Dictionary<string, List<string>>
            {
                ["symptoms"] = new List<string> { "pain", "fever", "headache", "nausea", "fatigue", "dizziness" },
                ["severity"] = new List<string> { "severe", "acute", "chronic", "mild", "moderate", "critical" },
                ["systems"] = new List<string> { "cardiac", "respiratory", "neurological", "gastrointestinal" },
                ["conditions"] = new List<string> { "hypertension", "diabetes", "pneumonia", "asthma", "depression" }
            };
            
            var categoryScores = new List<double>();
            
            foreach (var category in categories)
            {
                double score = 0;
                foreach (var token in tokens)
                {
                    if (category.Value.Any(term => token.Contains(term)))
                    {
                        score += MedicalTermWeights.ContainsKey(token) ? MedicalTermWeights[token] : 0.5;
                    }
                }
                categoryScores.Add(score / Math.Max(1, tokens.Count)); // Normalize by token count
            }
            
            return categoryScores;
        }

        /// <summary>
        /// Gets sentiment and severity features
        /// </summary>
        private static List<double> GetSentimentFeatures(List<string> tokens, int count)
        {
            var features = new List<double>();
            
            // Severity indicators
            var severityTerms = new[] { "severe", "critical", "emergency", "acute", "urgent" };
            var mildTerms = new[] { "mild", "slight", "minor", "stable" };
            
            double severityScore = tokens.Count(t => severityTerms.Any(s => t.Contains(s)));
            double mildnessScore = tokens.Count(t => mildTerms.Any(m => t.Contains(m)));
            
            features.Add(severityScore / Math.Max(1, tokens.Count));
            features.Add(mildnessScore / Math.Max(1, tokens.Count));
            
            // Positive/negative indicators
            var positiveTerms = new[] { "positive", "abnormal", "elevated", "high", "increased" };
            var negativeTerms = new[] { "negative", "normal", "stable", "improved", "decreased" };
            
            double positiveScore = tokens.Count(t => positiveTerms.Any(p => t.Contains(p)));
            double negativeScore = tokens.Count(t => negativeTerms.Any(n => t.Contains(n)));
            
            features.Add(positiveScore / Math.Max(1, tokens.Count));
            features.Add(negativeScore / Math.Max(1, tokens.Count));
            
            // Pain and discomfort indicators
            var painTerms = new[] { "pain", "ache", "hurt", "discomfort", "tender", "sore" };
            double painScore = tokens.Count(t => painTerms.Any(p => t.Contains(p)));
            features.Add(painScore / Math.Max(1, tokens.Count));
            
            // Pad with zeros if needed
            while (features.Count < count)
                features.Add(0.0);
            
            return features.Take(count).ToList();
        }

        /// <summary>
        /// Gets statistical features from text
        /// </summary>
        private static List<double> GetStatisticalFeatures(List<string> tokens, int count)
        {
            var features = new List<double>();
            
            if (tokens.Count == 0)
            {
                return new List<double>(new double[count]);
            }
            
            // Text length features
            features.Add(Math.Log(1 + tokens.Count)); // Log of token count
            features.Add(tokens.Average(t => t.Length)); // Average token length
            
            // Lexical diversity
            var uniqueTokens = tokens.Distinct().Count();
            features.Add((double)uniqueTokens / tokens.Count); // Type-token ratio
            
            // Medical term density
            var medicalTermCount = tokens.Count(t => MedicalTermWeights.ContainsKey(t));
            features.Add((double)medicalTermCount / tokens.Count);
            
            // Number extraction (for vital signs, lab values, etc.)
            var numbers = ExtractNumbers(tokens);
            if (numbers.Count > 0)
            {
                features.Add(numbers.Average());
                features.Add(numbers.Max());
                features.Add(numbers.Min());
                features.Add(numbers.Count);
            }
            else
            {
                features.AddRange(new double[] { 0, 0, 0, 0 });
            }
            
            // Pad with zeros if needed
            while (features.Count < count)
                features.Add(0.0);
            
            return features.Take(count).ToList();
        }

        /// <summary>
        /// Extracts numerical values from tokens (vital signs, lab values, etc.)
        /// </summary>
        private static List<double> ExtractNumbers(List<string> tokens)
        {
            var numbers = new List<double>();
            
            foreach (var token in tokens)
            {
                // Extract numbers with decimal points
                var matches = Regex.Matches(token, @"\d+\.?\d*");
                foreach (Match match in matches)
                {
                    if (double.TryParse(match.Value, out double value))
                    {
                        numbers.Add(value);
                    }
                }
            }
            
            return numbers;
        }

        /// <summary>
        /// Normalizes features to a consistent range
        /// </summary>
        private static List<double> NormalizeFeatures(List<double> features, int targetSize)
        {
            // Ensure we have the target size
            while (features.Count < targetSize)
                features.Add(0.0);
            
            if (features.Count > targetSize)
                features = features.Take(targetSize).ToList();
            
            // Z-score normalization with robust statistics
            var nonZeroFeatures = features.Where(f => Math.Abs(f) > 1e-10).ToList();
            
            if (nonZeroFeatures.Count > 0)
            {
                var mean = nonZeroFeatures.Average();
                var stdDev = Math.Sqrt(nonZeroFeatures.Average(f => Math.Pow(f - mean, 2)));
                
                if (stdDev > 1e-10) // Avoid division by zero
                {
                    for (int i = 0; i < features.Count; i++)
                    {
                        if (Math.Abs(features[i]) > 1e-10)
                        {
                            features[i] = (features[i] - mean) / stdDev;
                            // Clip extreme values
                            features[i] = Math.Max(-3, Math.Min(3, features[i]));
                        }
                    }
                }
            }
            
            return features;
        }

        /// <summary>
        /// Extracts key medical information from text for summary
        /// </summary>
        public static Dictionary<string, object> ExtractMedicalInfo(string text)
        {
            var info = new Dictionary<string, object>();
            var tokens = TokenizeText(text);
            
            // Extract symptoms
            var symptoms = new List<string>();
            var symptomTerms = new[] { "pain", "fever", "headache", "nausea", "fatigue", "dizziness", "shortness", "breathing" };
            
            foreach (var token in tokens)
            {
                if (symptomTerms.Any(s => token.Contains(s)))
                {
                    symptoms.Add(token);
                }
            }
            
            info["symptoms"] = symptoms.Distinct().ToList();
            
            // Extract severity
            var severityTerms = new[] { "severe", "critical", "emergency", "acute", "urgent", "mild", "moderate" };
            var severity = tokens.FirstOrDefault(t => severityTerms.Any(s => t.Contains(s))) ?? "unknown";
            info["severity"] = severity;
            
            // Extract numbers (vital signs, lab values)
            var numbers = ExtractNumbers(tokens);
            info["numerical_values"] = numbers;
            
            // Extract body systems mentioned
            var systems = new List<string>();
            var systemTerms = new[] { "cardiac", "respiratory", "neurological", "gastrointestinal", "musculoskeletal" };
            
            foreach (var token in tokens)
            {
                if (systemTerms.Any(s => token.Contains(s)))
                {
                    systems.Add(token);
                }
            }
            
            info["body_systems"] = systems.Distinct().ToList();
            
            return info;
        }
    }
}