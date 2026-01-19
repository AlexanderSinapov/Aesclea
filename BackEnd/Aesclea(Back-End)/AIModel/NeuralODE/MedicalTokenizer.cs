// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov
// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aesclea_Back_End_.AIModel.NeuralODE
{
    /// <summary>
    /// Medical domain tokenizer for clinical text
    /// Handles medical terminology, abbreviations, and special tokens
    /// </summary>
    public class MedicalTokenizer
    {
        // Vocabulary
        private Dictionary<string, int> tokenToId;
        private Dictionary<int, string> idToToken;
        
        // Special tokens
        public int PadTokenId { get; private set; }
        public int UnkTokenId { get; private set; }
        public int StartTokenId { get; private set; }
        public int EndTokenId { get; private set; }
        public int SepTokenId { get; private set; }
        
        public int VocabSize => tokenToId.Count;

        public MedicalTokenizer()
        {
            InitializeVocabulary();
        }

        private void InitializeVocabulary()
        {
            tokenToId = new Dictionary<string, int>();
            idToToken = new Dictionary<int, string>();
            
            // Special tokens
            AddToken("[PAD]");
            AddToken("[UNK]");
            AddToken("[START]");
            AddToken("[END]");
            AddToken("[SEP]");
            
            PadTokenId = 0;
            UnkTokenId = 1;
            StartTokenId = 2;
            EndTokenId = 3;
            SepTokenId = 4;
            
            // Common medical terms
            string[] medicalTerms = new string[]
            {
                // Symptoms
                "pain", "fever", "cough", "headache", "nausea", "vomiting", "diarrhea", 
                "fatigue", "weakness", "dizziness", "shortness", "breath", "chest", 
                "abdominal", "back", "joint", "muscle", "swelling", "rash", "itch",
                
                // Body parts
                "heart", "lung", "liver", "kidney", "brain", "stomach", "intestine",
                "pancreas", "spleen", "bone", "skin", "eye", "ear", "nose", "throat",
                "arm", "leg", "hand", "foot", "head", "neck", "shoulder", "knee",
                
                // Conditions
                "diabetes", "hypertension", "asthma", "copd", "cancer", "tumor",
                "infection", "inflammation", "fracture", "bleeding", "clot",
                "stroke", "attack", "failure", "disease", "syndrome", "disorder",
                
                // Measurements
                "blood", "pressure", "temperature", "pulse", "rate", "level",
                "count", "high", "low", "normal", "elevated", "decreased",
                
                // Medical procedures
                "surgery", "operation", "procedure", "test", "scan", "xray",
                "mri", "ct", "ultrasound", "biopsy", "examination", "diagnosis",
                
                // Medications
                "medication", "drug", "antibiotic", "painkiller", "insulin",
                "aspirin", "therapy", "treatment", "dose", "prescription",
                
                // Time/severity
                "acute", "chronic", "severe", "mild", "moderate", "sudden",
                "gradual", "persistent", "intermittent", "daily", "weekly",
                
                // Medical abbreviations
                "bp", "hr", "temp", "rr", "spo2", "ecg", "ekg", "cbc", "bmp",
                
                // Common words
                "patient", "history", "present", "complaint", "onset", "duration",
                "location", "quality", "severity", "timing", "context", "factors",
                "symptoms", "signs", "diagnosis", "prognosis", "plan", "follow",
                "up", "monitor", "evaluate", "assess", "treat", "manage"
            };
            
            foreach (var term in medicalTerms)
            {
                AddToken(term);
            }
            
            // Add numbers
            for (int i = 0; i <= 200; i++)
            {
                AddToken(i.ToString());
            }
            
            // Add common punctuation and connectors
            string[] common = new string[] { ".", ",", ":", ";", "-", "/", "(", ")", 
                "the", "a", "an", "and", "or", "of", "in", "on", "at", "to", "for", 
                "with", "by", "from", "has", "have", "had", "is", "are", "was", "were" };
            
            foreach (var word in common)
            {
                AddToken(word);
            }
        }

        private void AddToken(string token)
        {
            if (!tokenToId.ContainsKey(token))
            {
                int id = tokenToId.Count;
                tokenToId[token] = id;
                idToToken[id] = token;
            }
        }

        /// <summary>
        /// Tokenize clinical text into token IDs
        /// </summary>
        public int[] Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new int[] { PadTokenId };
            
            // Normalize text
            text = text.ToLower();
            
            // Split into tokens (simple word tokenization)
            var tokens = Regex.Split(text, @"(\s+|[.,;:()\-/])")
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();
            
            // Convert to IDs
            var ids = new List<int> { StartTokenId };
            
            foreach (var token in tokens)
            {
                if (tokenToId.ContainsKey(token))
                {
                    ids.Add(tokenToId[token]);
                }
                else
                {
                    // Try to handle compound words or unknown terms
                    ids.Add(UnkTokenId);
                }
            }
            
            ids.Add(EndTokenId);
            
            return ids.ToArray();
        }

        /// <summary>
        /// Decode token IDs back to text
        /// </summary>
        public string Decode(int[] tokenIds)
        {
            var tokens = tokenIds
                .Where(id => id != PadTokenId && id != StartTokenId && id != EndTokenId)
                .Select(id => idToToken.ContainsKey(id) ? idToToken[id] : "[UNK]");
            
            return string.Join(" ", tokens);
        }

        /// <summary>
        /// Pad or truncate sequence to fixed length
        /// </summary>
        public int[] PadSequence(int[] tokens, int maxLength)
        {
            if (tokens.Length >= maxLength)
            {
                return tokens.Take(maxLength).ToArray();
            }
            
            var padded = new int[maxLength];
            Array.Copy(tokens, padded, tokens.Length);
            
            for (int i = tokens.Length; i < maxLength; i++)
            {
                padded[i] = PadTokenId;
            }
            
            return padded;
        }

        /// <summary>
        /// Create attention mask (1 for real tokens, 0 for padding)
        /// </summary>
        public int[] CreateAttentionMask(int[] tokens)
        {
            return tokens.Select(t => t == PadTokenId ? 0 : 1).ToArray();
        }

        /// <summary>
        /// Extract medical entities from tokenized text
        /// </summary>
        public List<MedicalEntity> ExtractEntities(string text)
        {
            var entities = new List<MedicalEntity>();
            
            // Simple pattern matching for medical entities
            var patterns = new Dictionary<string, string>
            {
                { @"\b\d+/\d+\s*mmhg\b", "BloodPressure" },
                { @"\b\d+\.?\d*\s*°?[cf]\b", "Temperature" },
                { @"\b\d+\s*bpm\b", "HeartRate" },
                { @"\b\d+\s*%\b", "OxygenSaturation" },
                { @"\bpain\b", "Symptom" },
                { @"\bfever\b", "Symptom" },
                { @"\bcough\b", "Symptom" }
            };
            
            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(text.ToLower(), pattern.Key);
                foreach (Match match in matches)
                {
                    entities.Add(new MedicalEntity
                    {
                        Text = match.Value,
                        Type = pattern.Value,
                        StartIndex = match.Index,
                        EndIndex = match.Index + match.Length
                    });
                }
            }
            
            return entities;
        }
    }

    /// <summary>
    /// Represents a medical entity extracted from text
    /// </summary>
    public class MedicalEntity
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
}
