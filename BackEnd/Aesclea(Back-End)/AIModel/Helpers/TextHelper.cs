// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

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
        };        /// <summary>
        /// Converts medical text into numerical features for neural network input
        /// Enhanced version with improved medical context understanding
        /// </summary>
        /// <param name="text">Raw medical text</param>
        /// <param name="maxFeatures">Maximum number of features to extract</param>
        /// <returns>List of numerical features</returns>
        public static List<double> ProcessMedicalText(string text, int maxFeatures = 512)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    // Return exact size list filled with zeros
                    return Enumerable.Repeat(0.0, maxFeatures).ToList();
                }

                // Enhanced preprocessing with medical context
                text = PreprocessMedicalText(text);
                
                // Clean and tokenize text
                var tokens = TokenizeText(text);
                
                // Extract enhanced medical features - this should return exactly maxFeatures
                var features = ExtractEnhancedMedicalFeatures(tokens, maxFeatures);
                
                // CRITICAL VALIDATION: Verify exact size before any processing
                if (features.Count != maxFeatures)
                {
                    if (features.Count > maxFeatures)
                    {
                        features = features.Take(maxFeatures).ToList();
                    }
                    else
                    {
                        // Pad with zeros
                        features.AddRange(Enumerable.Repeat(0.0, maxFeatures - features.Count));
                    }
                }
                
                // Normalize features (this should maintain size)
                var result = NormalizeFeatures(features, maxFeatures);
                
                // PARANOID FINAL VALIDATION: Ensure exact size and no invalid values
                if (result.Count != maxFeatures)
                {
                    if (result.Count > maxFeatures)
                    {
                        result = result.Take(maxFeatures).ToList();
                    }
                    else
                    {
                        result.AddRange(Enumerable.Repeat(0.0, maxFeatures - result.Count));
                    }
                }
                
                // PARANOID CHECK: Ensure no NaN or Infinity
                for (int i = 0; i < result.Count; i++)
                {
                    if (double.IsNaN(result[i]) || double.IsInfinity(result[i]))
                    {
                        result[i] = 0.0;
                    }
                }
                
                // Final sanity check
                if (result.Count != maxFeatures)
                {
                    throw new InvalidOperationException($"CRITICAL ERROR: Result size {result.Count} does not match required size {maxFeatures}");
                }
                
                // Return a brand new array copy to prevent any external modification
                return result.ToArray().ToList();
            }
            catch (Exception ex)
            {
                // If anything fails, return a zero-filled list of correct size
                Console.WriteLine($"ERROR in ProcessMedicalText: {ex.Message}");
                return Enumerable.Repeat(0.0, maxFeatures).ToList();
            }
        }

        /// <summary>
        /// Enhanced medical text preprocessing
        /// </summary>
        private static string PreprocessMedicalText(string text)
        {
            // Normalize common medical abbreviations and units
            var medicalReplacements = new Dictionary<string, string>
            {
                { @"\bpt\b", "patient" },
                { @"\bhx\b", "history" },
                { @"\bsx\b", "symptoms" },
                { @"\bdx\b", "diagnosis" },
                { @"\btx\b", "treatment" },
                { @"\brx\b", "prescription" },
                { @"\bc/o\b", "complains of" },
                { @"\bp/o\b", "postoperative" },
                { @"\bw/\b", "with" },
                { @"\bw/o\b", "without" },
                { @"\bs/p\b", "status post" },
                { @"\bh/o\b", "history of" },
                { @"\bf/u\b", "follow up" },
                { @"\bNKDA\b", "no known drug allergies" },
                { @"\bNKA\b", "no known allergies" },
                { @"\bSOB\b", "shortness of breath" },
                { @"\bDOE\b", "dyspnea on exertion" },
                { @"\bCPR\b", "cardiopulmonary resuscitation" },
                { @"\bICU\b", "intensive care unit" },
                { @"\bER\b", "emergency room" },
                { @"\bOR\b", "operating room" },
                { @"\bIV\b", "intravenous" },
                { @"\bPO\b", "per oral" },
                { @"\bIM\b", "intramuscular" },
                { @"\bSC\b", "subcutaneous" }
            };

            foreach (var replacement in medicalReplacements)
            {
                text = Regex.Replace(text, replacement.Key, replacement.Value, RegexOptions.IgnoreCase);
            }

            // Normalize vital signs patterns
            text = Regex.Replace(text, @"(\d+)/(\d+)\s*mmhg", "$1 over $2 blood pressure", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"temp\s*(\d+\.?\d*)", "temperature $1", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"hr\s*(\d+)", "heart rate $1", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"rr\s*(\d+)", "respiratory rate $1", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"o2\s*sat\s*(\d+)", "oxygen saturation $1", RegexOptions.IgnoreCase);

            return text;
        }

        /// <summary>
        /// Enhanced medical feature extraction with improved context understanding
        /// </summary>
        private static List<double> ExtractEnhancedMedicalFeatures(List<string> tokens, int maxFeatures)
        {
            var features = new List<double>();
            
            // Calculate how many features each component should contribute
            int featuresPerComponent = maxFeatures / 6;
            
            // 1. Enhanced term frequency features with medical weighting
            var termFrequency = CalculateTermFrequency(tokens);
            var termFeatures = GetEnhancedTermFeatures(termFrequency, featuresPerComponent);
            // CRITICAL: Ensure exact size
            if (termFeatures.Count > featuresPerComponent) termFeatures = termFeatures.Take(featuresPerComponent).ToList();
            while (termFeatures.Count < featuresPerComponent) termFeatures.Add(0.0);
            features.AddRange(termFeatures);
            
            // 2. Medical context features (symptom clustering, body systems)
            var contextFeatures = GetMedicalContextFeatures(tokens, featuresPerComponent);
            if (contextFeatures.Count > featuresPerComponent) contextFeatures = contextFeatures.Take(featuresPerComponent).ToList();
            while (contextFeatures.Count < featuresPerComponent) contextFeatures.Add(0.0);
            features.AddRange(contextFeatures);
            
            // 3. Enhanced medical term weight features
            var medicalFeatures = GetMedicalTermFeatures(tokens, featuresPerComponent);
            if (medicalFeatures.Count > featuresPerComponent) medicalFeatures = medicalFeatures.Take(featuresPerComponent).ToList();
            while (medicalFeatures.Count < featuresPerComponent) medicalFeatures.Add(0.0);
            features.AddRange(medicalFeatures);
            
            // 4. Clinical urgency and severity indicators
            var urgencyFeatures = GetClinicalUrgencyFeatures(tokens, featuresPerComponent);
            if (urgencyFeatures.Count > featuresPerComponent) urgencyFeatures = urgencyFeatures.Take(featuresPerComponent).ToList();
            while (urgencyFeatures.Count < featuresPerComponent) urgencyFeatures.Add(0.0);
            features.AddRange(urgencyFeatures);
            
            // 5. Sentiment and severity features
            var sentimentFeatures = GetSentimentFeatures(tokens, featuresPerComponent);
            if (sentimentFeatures.Count > featuresPerComponent) sentimentFeatures = sentimentFeatures.Take(featuresPerComponent).ToList();
            while (sentimentFeatures.Count < featuresPerComponent) sentimentFeatures.Add(0.0);
            features.AddRange(sentimentFeatures);
            
            // 6. Enhanced statistical features
            var statisticalFeatures = GetStatisticalFeatures(tokens, featuresPerComponent);
            if (statisticalFeatures.Count > featuresPerComponent) statisticalFeatures = statisticalFeatures.Take(featuresPerComponent).ToList();
            while (statisticalFeatures.Count < featuresPerComponent) statisticalFeatures.Add(0.0);
            features.AddRange(statisticalFeatures);
            
            // CRITICAL: Ensure exact maxFeatures size
            if (features.Count > maxFeatures)
            {
                features = features.Take(maxFeatures).ToList();
            }
            while (features.Count < maxFeatures)
            {
                features.Add(0.0);
            }
            
            return features;
        }

        /// <summary>
        /// Extract medical context features including symptom clusters and body systems
        /// </summary>
        private static List<double> GetMedicalContextFeatures(List<string> tokens, int count)
        {
            var features = new List<double>();
            
            // For each token, calculate its relevance to each medical category
            // This creates a much richer feature space
            var contextCategories = new Dictionary<string, List<string>>
            {
                ["cardiovascular"] = new List<string> { "chest", "heart", "cardiac", "blood", "pressure", "circulation", "pulse", "rhythm" },
                ["respiratory"] = new List<string> { "lung", "breathing", "breath", "respiratory", "oxygen", "airway", "cough", "wheeze" },
                ["neurological"] = new List<string> { "brain", "nerve", "neural", "cognitive", "memory", "seizure", "consciousness", "reflex" },
                ["gastrointestinal"] = new List<string> { "stomach", "intestine", "digestive", "bowel", "liver", "pancreas", "bile", "digest" },
                ["musculoskeletal"] = new List<string> { "muscle", "bone", "joint", "spine", "skeletal", "movement", "mobility", "strength" },
                ["genitourinary"] = new List<string> { "kidney", "bladder", "urinary", "reproductive", "genital", "urine", "renal", "prostate" },
                ["endocrine"] = new List<string> { "hormone", "gland", "thyroid", "diabetes", "insulin", "metabolic", "glucose", "cortisol" },
                ["hematologic"] = new List<string> { "blood", "anemia", "bleeding", "clotting", "platelet", "hemoglobin", "transfusion", "coagulation" },
                ["immunologic"] = new List<string> { "immune", "allergy", "infection", "inflammatory", "autoimmune", "antibody", "vaccine", "reaction" },
                ["psychiatric"] = new List<string> { "mental", "mood", "depression", "anxiety", "psychiatric", "psychological", "behavior", "cognitive" }
            };
            
            // Create a vector for EACH token's category relevance (much richer representation)
            int tokensToProcess = Math.Min(tokens.Count, count / contextCategories.Count);
            for (int i = 0; i < tokensToProcess; i++)
            {
                var token = i < tokens.Count ? tokens[i] : "";
                foreach (var category in contextCategories)
                {
                    double score = category.Value.Any(term => token.Contains(term) || term.Contains(token)) ? 1.0 : 0.0;
                    features.Add(score);
                }
            }
            
            // Pad to target count
            while (features.Count < count)
                features.Add(0.0);
            
            return features.Take(count).ToList();
        }

        /// <summary>
        /// Extract clinical urgency and severity indicators
        /// </summary>
        private static List<double> GetClinicalUrgencyFeatures(List<string> tokens, int count)
        {
            var features = new List<double>();
            
            // Create binary features for each token against urgency indicators
            int tokensToProcess = Math.Min(tokens.Count, count / 3);
            
            var emergencyTerms = new List<string> 
            { 
                "emergency", "urgent", "critical", "severe", "acute", "immediate", "stat", "code", "arrest", 
                "trauma", "hemorrhage", "stroke", "infarction", "shock", "respiratory_distress", "cardiac_arrest"
            };
            
            var moderateTerms = new List<string>
            {
                "moderate", "significant", "concerning", "notable", "marked", "pronounced", "substantial"
            };
            
            var mildTerms = new List<string>
            {
                "mild", "minor", "slight", "minimal", "small", "trace", "stable", "improved"
            };
            
            for (int i = 0; i < tokensToProcess; i++)
            {
                var token = i < tokens.Count ? tokens[i] : "";
                features.Add(emergencyTerms.Any(e => token.Contains(e) || e.Contains(token)) ? 1.0 : 0.0);
                features.Add(moderateTerms.Any(m => token.Contains(m) || m.Contains(token)) ? 1.0 : 0.0);
                features.Add(mildTerms.Any(m => token.Contains(m) || m.Contains(token)) ? 1.0 : 0.0);
            }
            
            // Pad to target count
            while (features.Count < count)
                features.Add(0.0);
            
            return features.Take(count).ToList();
        }

        /// <summary>
        /// Enhanced term frequency features with medical term prioritization
        /// </summary>
        private static List<double> GetEnhancedTermFeatures(Dictionary<string, int> termFreq, int count)
        {
            var features = new List<double>();
            
            // Create a rich frequency-based feature vector
            var allTerms = termFreq.OrderByDescending(kv => kv.Value).ToList();
            
            // For each position in our feature vector, use the corresponding term's frequency
            for (int i = 0; i < count; i++)
            {
                if (i < allTerms.Count)
                {
                    // Use log transform to normalize extreme frequencies
                    double weight = MedicalTermWeights.ContainsKey(allTerms[i].Key) ? MedicalTermWeights[allTerms[i].Key] : 1.0;
                    features.Add(Math.Log(1 + allTerms[i].Value * weight));
                }
                else
                {
                    features.Add(0.0);
                }
            }
            
            return features;
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
            
            // Create a feature for each position based on medical term presence
            for (int i = 0; i < count; i++)
            {
                if (i < tokens.Count)
                {
                    var token = tokens[i];
                    if (MedicalTermWeights.ContainsKey(token))
                    {
                        features.Add(MedicalTermWeights[token]);
                    }
                    else
                    {
                        // Check partial matches
                        double maxWeight = 0.0;
                        foreach (var medTerm in MedicalTermWeights)
                        {
                            if (token.Contains(medTerm.Key) || medTerm.Key.Contains(token))
                            {
                                maxWeight = Math.Max(maxWeight, medTerm.Value);
                            }
                        }
                        features.Add(maxWeight);
                    }
                }
                else
                {
                    features.Add(0.0);
                }
            }
            
            return features;
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
            
            var severityTerms = new[] { "severe", "critical", "emergency", "acute", "urgent" };
            var mildTerms = new[] { "mild", "slight", "minor", "stable" };
            var positiveTerms = new[] { "positive", "abnormal", "elevated", "high", "increased" };
            var negativeTerms = new[] { "negative", "normal", "stable", "improved", "decreased" };
            var painTerms = new[] { "pain", "ache", "hurt", "discomfort", "tender", "sore" };
            
            // Create features for each token
            for (int i = 0; i < count / 5; i++)
            {
                if (i < tokens.Count)
                {
                    var token = tokens[i];
                    features.Add(severityTerms.Any(s => token.Contains(s)) ? 1.0 : 0.0);
                    features.Add(mildTerms.Any(m => token.Contains(m)) ? 1.0 : 0.0);
                    features.Add(positiveTerms.Any(p => token.Contains(p)) ? 1.0 : 0.0);
                    features.Add(negativeTerms.Any(n => token.Contains(n)) ? 1.0 : 0.0);
                    features.Add(painTerms.Any(p => token.Contains(p)) ? 1.0 : 0.0);
                }
                else
                {
                    features.AddRange(new double[] { 0, 0, 0, 0, 0 });
                }
            }
            
            // Pad to target count
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
                return Enumerable.Repeat(0.0, count).ToList();
            }
            
            // Create statistical features for each token position
            for (int i = 0; i < count; i++)
            {
                if (i < tokens.Count)
                {
                    var token = tokens[i];
                    // Token length normalized
                    features.Add(token.Length / 20.0); // Normalize by typical max word length
                }
                else
                {
                    features.Add(0.0);
                }
            }
            
            return features;
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
            // CRITICAL: Ensure input has exact target size before normalization
            if (features.Count > targetSize)
            {
                features = features.Take(targetSize).ToList();
            }
            else if (features.Count < targetSize)
            {
                features.AddRange(Enumerable.Repeat(0.0, targetSize - features.Count));
            }
            
            // Clean any NaN or infinity values first (prevents training crashes)
            for (int i = 0; i < features.Count; i++)
            {
                if (double.IsNaN(features[i]) || double.IsInfinity(features[i]))
                {
                    features[i] = 0.0;
                }
            }
            
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
                            // Clip extreme values to prevent overflow
                            features[i] = Math.Max(-5, Math.Min(5, features[i]));
                            
                            // Double-check for NaN after normalization
                            if (double.IsNaN(features[i]) || double.IsInfinity(features[i]))
                            {
                                features[i] = 0.0;
                            }
                        }
                    }
                }
            }
            
            // FINAL GUARANTEE: Return exactly targetSize elements
            if (features.Count != targetSize)
            {
                if (features.Count > targetSize)
                {
                    return features.Take(targetSize).ToList();
                }
                else
                {
                    features.AddRange(Enumerable.Repeat(0.0, targetSize - features.Count));
                }
            }
            
            return features;
        }        /// <summary>
        /// Extracts key medical information from text for summary - Enhanced version
        /// </summary>
        public static Dictionary<string, object> ExtractMedicalInfo(string text)
        {
            var info = new Dictionary<string, object>();
            var tokens = TokenizeText(text);
            
            // Enhanced symptom extraction with medical context
            var symptoms = new List<string>();
            var enhancedSymptomTerms = new[] { 
                "pain", "fever", "headache", "nausea", "fatigue", "dizziness", "shortness", "breathing",
                "chest_pain", "abdominal_pain", "back_pain", "joint_pain", "muscle_pain",
                "cough", "wheeze", "vomiting", "diarrhea", "constipation", "weakness", "numbness",
                "tingling", "swelling", "rash", "bleeding", "bruising", "palpitations", "syncope",
                "confusion", "memory_loss", "vision_changes", "hearing_loss", "weight_loss", "weight_gain"
            };
            
            foreach (var token in tokens)
            {
                if (enhancedSymptomTerms.Any(s => token.Contains(s) || s.Contains(token)))
                {
                    symptoms.Add(token);
                }
            }
            info["symptoms"] = symptoms.Distinct().ToList();
            
            // Enhanced severity extraction with context
            var severityTerms = new[] { 
                "severe", "critical", "emergency", "acute", "urgent", "mild", "moderate", "chronic",
                "life_threatening", "debilitating", "excruciating", "unbearable", "intense"
            };
            var severity = tokens.FirstOrDefault(t => severityTerms.Any(s => t.Contains(s))) ?? "unknown";
            info["severity"] = severity;
            
            // Enhanced numerical values extraction (vital signs, lab values)
            var numbers = ExtractNumbers(tokens);
            info["numerical_values"] = numbers;
            
            // Extract body systems mentioned with enhanced detection
            var systems = new List<string>();
            var enhancedSystemTerms = new[] { 
                "cardiac", "cardiovascular", "respiratory", "pulmonary", "neurological", "neural",
                "gastrointestinal", "digestive", "musculoskeletal", "orthopedic", "dermatological",
                "genitourinary", "renal", "endocrine", "hematologic", "immunologic", "psychiatric",
                "ophthalmologic", "otolaryngologic", "reproductive"
            };
            
            foreach (var token in tokens)
            {
                if (enhancedSystemTerms.Any(s => token.Contains(s)))
                {
                    systems.Add(token);
                }
            }
            info["body_systems"] = systems.Distinct().ToList();
            
            // Enhanced temporal pattern extraction
            var temporalPatterns = new Dictionary<string, string>();
            if (tokens.Any(t => t.Contains("sudden") || t.Contains("acute")))
                temporalPatterns["onset"] = "acute";
            else if (tokens.Any(t => t.Contains("gradual") || t.Contains("progressive")))
                temporalPatterns["onset"] = "gradual";
            else if (tokens.Any(t => t.Contains("chronic") || t.Contains("longstanding")))
                temporalPatterns["onset"] = "chronic";
                
            if (tokens.Any(t => t.Contains("worse") || t.Contains("worsening")))
                temporalPatterns["course"] = "worsening";
            else if (tokens.Any(t => t.Contains("better") || t.Contains("improving")))
                temporalPatterns["course"] = "improving";
            else if (tokens.Any(t => t.Contains("stable") || t.Contains("unchanged")))
                temporalPatterns["course"] = "stable";
                
            info["temporal_patterns"] = temporalPatterns;
            
            // Enhanced medication extraction
            var medications = new List<string>();
            var commonMedications = new[] {
                "aspirin", "ibuprofen", "acetaminophen", "tylenol", "advil", "motrin",
                "lisinopril", "metformin", "atorvastatin", "amlodipine", "metoprolol",
                "omeprazole", "albuterol", "prednisone", "warfarin", "insulin",
                "hydrochlorothiazide", "furosemide", "gabapentin", "tramadol", "morphine"
            };
            
            foreach (var token in tokens)
            {
                if (commonMedications.Any(med => token.Contains(med)))
                {
                    medications.Add(token);
                }
            }
            info["medications"] = medications.Distinct().ToList();
            
            // Enhanced functional status assessment
            var functionalImpact = new List<string>();
            if (tokens.Any(t => t.Contains("unable") || t.Contains("cannot")))
                functionalImpact.Add("severe_limitation");
            if (tokens.Any(t => t.Contains("difficulty") || t.Contains("trouble")))
                functionalImpact.Add("moderate_limitation");
            if (tokens.Any(t => t.Contains("independent") || t.Contains("normal_activity")))
                functionalImpact.Add("independent");
                
            info["functional_status"] = functionalImpact;
            
            // Enhanced psychosocial factors
            var psychosocial = new List<string>();
            if (tokens.Any(t => t.Contains("anxiety") || t.Contains("worried") || t.Contains("stress")))
                psychosocial.Add("psychological_distress");
            if (tokens.Any(t => t.Contains("family") || t.Contains("support")))
                psychosocial.Add("family_involvement");
            if (tokens.Any(t => t.Contains("work") || t.Contains("job") || t.Contains("occupation")))
                psychosocial.Add("work_related");
                
            info["psychosocial_factors"] = psychosocial;
            
            return info;
        }
    }
}