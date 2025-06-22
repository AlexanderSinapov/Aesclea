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
            if (string.IsNullOrEmpty(text))
                return new List<double>(new double[maxFeatures]);

            // Enhanced preprocessing with medical context
            text = PreprocessMedicalText(text);
            
            // Clean and tokenize text
            var tokens = TokenizeText(text);
            
            // Extract enhanced medical features
            var features = ExtractEnhancedMedicalFeatures(tokens, maxFeatures);
            
            // Normalize features
            return NormalizeFeatures(features, maxFeatures);
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
            
            // 1. Enhanced term frequency features with medical weighting
            var termFrequency = CalculateTermFrequency(tokens);
            features.AddRange(GetEnhancedTermFeatures(termFrequency, maxFeatures / 6));
            
            // 2. Medical context features (symptom clustering, body systems)
            features.AddRange(GetMedicalContextFeatures(tokens, maxFeatures / 6));
            
            // 3. Enhanced medical term weight features
            features.AddRange(GetMedicalTermFeatures(tokens, maxFeatures / 6));
            
            // 4. Clinical urgency and severity indicators
            features.AddRange(GetClinicalUrgencyFeatures(tokens, maxFeatures / 6));
            
            // 5. Sentiment and severity features
            features.AddRange(GetSentimentFeatures(tokens, maxFeatures / 6));
            
            // 6. Enhanced statistical features
            features.AddRange(GetStatisticalFeatures(tokens, maxFeatures / 6));
            
            return features;
        }

        /// <summary>
        /// Extract medical context features including symptom clusters and body systems
        /// </summary>
        private static List<double> GetMedicalContextFeatures(List<string> tokens, int count)
        {
            var features = new List<double>();
            
            // Define medical context categories
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
            
            // Calculate context scores
            foreach (var category in contextCategories)
            {
                double score = 0;
                foreach (var token in tokens)
                {
                    if (category.Value.Any(term => token.Contains(term) || term.Contains(token)))
                    {
                        score += 1.0;
                    }
                }
                features.Add(score / Math.Max(1, tokens.Count));
            }
            
            // Symptom clustering features
            var symptomClusters = new Dictionary<string, List<string>>
            {
                ["pain_cluster"] = new List<string> { "pain", "ache", "hurt", "tender", "sore", "cramp", "burning", "sharp", "dull" },
                ["fever_cluster"] = new List<string> { "fever", "temperature", "hot", "chills", "sweats", "hyperthermia", "pyrexia" },
                ["respiratory_cluster"] = new List<string> { "cough", "wheeze", "shortness", "dyspnea", "breathing", "sputum", "phlegm" },
                ["gastrointestinal_cluster"] = new List<string> { "nausea", "vomiting", "diarrhea", "constipation", "bloating", "cramping" },
                ["neurological_cluster"] = new List<string> { "headache", "dizziness", "confusion", "weakness", "numbness", "tingling" },
                ["fatigue_cluster"] = new List<string> { "fatigue", "tired", "exhausted", "weakness", "lethargy", "energy", "rest" }
            };
            
            foreach (var cluster in symptomClusters)
            {
                double clusterScore = 0;
                foreach (var token in tokens)
                {
                    if (cluster.Value.Any(symptom => token.Contains(symptom) || symptom.Contains(token)))
                    {
                        clusterScore += 1.0;
                    }
                }
                features.Add(clusterScore / Math.Max(1, tokens.Count));
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
            
            // Emergency indicators
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
            
            // Calculate urgency scores
            double emergencyScore = tokens.Count(t => emergencyTerms.Any(e => t.Contains(e) || e.Contains(t)));
            double moderateScore = tokens.Count(t => moderateTerms.Any(m => t.Contains(m) || m.Contains(t)));
            double mildScore = tokens.Count(t => mildTerms.Any(m => t.Contains(m) || m.Contains(t)));
            
            features.Add(emergencyScore / Math.Max(1, tokens.Count));
            features.Add(moderateScore / Math.Max(1, tokens.Count));
            features.Add(mildScore / Math.Max(1, tokens.Count));
            
            // Time-based urgency indicators
            var timeIndicators = new List<string>
            {
                "sudden", "sudden_onset", "rapid", "progressive", "chronic", "acute", "subacute"
            };
            
            double timeUrgency = tokens.Count(t => timeIndicators.Any(ti => t.Contains(ti) || ti.Contains(t)));
            features.Add(timeUrgency / Math.Max(1, tokens.Count));
            
            // Functional impact indicators
            var functionalImpact = new List<string>
            {
                "unable", "difficulty", "impaired", "reduced", "limited", "restricted", "compromised"
            };
            
            double functionalScore = tokens.Count(t => functionalImpact.Any(fi => t.Contains(fi) || fi.Contains(t)));
            features.Add(functionalScore / Math.Max(1, tokens.Count));
            
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
            
            // Prioritize medical terms in frequency analysis
            var medicalTermFreq = termFreq.Where(kv => MedicalTermWeights.ContainsKey(kv.Key))
                                         .OrderByDescending(kv => kv.Value * MedicalTermWeights[kv.Key])
                                         .Take(count / 2);
            
            var generalTermFreq = termFreq.Where(kv => !MedicalTermWeights.ContainsKey(kv.Key))
                                         .OrderByDescending(kv => kv.Value)
                                         .Take(count / 2);
            
            // Add medical term features
            foreach (var term in medicalTermFreq)
            {
                features.Add(Math.Log(1 + term.Value * MedicalTermWeights[term.Key]));
            }
            
            // Add general term features
            foreach (var term in generalTermFreq)
            {
                features.Add(Math.Log(1 + term.Value));
            }
            
            // Pad with zeros if needed
            while (features.Count < count)
                features.Add(0.0);
            
            return features.Take(count).ToList();
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