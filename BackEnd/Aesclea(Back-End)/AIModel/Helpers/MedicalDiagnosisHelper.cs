using System;
using System.Collections.Generic;
using System.Linq;

namespace Aesclea_Back_End_.AIModel.Helpers
{
    /// <summary>
    /// Helper class providing medical knowledge for diagnosis classification
    /// </summary>
    public class MedicalDiagnosisHelper
    {
        /// <summary>
        /// Major diagnostic categories for medical conditions
        /// </summary>
        public string[] DiagnosticCategories = {
            "Cardiovascular", "Respiratory", "Neurological", "Gastrointestinal", "Musculoskeletal",
            "Dermatological", "Psychiatric", "Endocrine", "Infectious", "Hematological",
            "Renal", "Reproductive", "Ophthalmological", "ENT", "Emergency", "Trauma",
            "Pediatric", "Geriatric", "Surgical", "Oncological"
        };

        /// <summary>
        /// Severity level descriptions
        /// </summary>
        private readonly Dictionary<int, string> SeverityDescriptions = new Dictionary<int, string>
        {
            { 1, "Minimal - Minor symptoms, no significant impact" },
            { 2, "Mild - Some discomfort, minimal functional impact" },
            { 3, "Moderate - Notable symptoms, some functional limitation" },
            { 4, "Severe - Significant symptoms, major functional impairment" },
            { 5, "Critical - Life-threatening, immediate intervention required" }
        };

        /// <summary>
        /// Urgency level descriptions
        /// </summary>
        private readonly Dictionary<int, string> UrgencyDescriptions = new Dictionary<int, string>
        {
            { 0, "Routine - Can wait for regular appointment" },
            { 1, "Urgent - Requires prompt medical attention within 24 hours" },
            { 2, "Immediate - Emergency care required now" }
        };

        /// <summary>
        /// Category-specific recommendations
        /// </summary>
        private readonly Dictionary<string, List<string>> CategoryRecommendations = new Dictionary<string, List<string>>
        {
            ["Cardiovascular"] = new List<string> {
                "Monitor blood pressure and heart rate",
                "Consider ECG if chest pain present",
                "Assess cardiac risk factors",
                "Consider stress testing if indicated"
            },
            ["Respiratory"] = new List<string> {
                "Monitor oxygen saturation",
                "Consider chest X-ray",
                "Assess respiratory rate and effort",
                "Consider spirometry if chronic symptoms"
            },
            ["Neurological"] = new List<string> {
                "Perform neurological examination",
                "Consider brain imaging if indicated",
                "Assess cognitive function",
                "Monitor for seizure activity"
            },
            ["Gastrointestinal"] = new List<string> {
                "Monitor hydration status",
                "Consider dietary modifications",
                "Assess for dehydration",
                "Consider endoscopy if indicated"
            },
            ["Musculoskeletal"] = new List<string> {
                "Rest and protect affected area",
                "Apply ice for acute injuries",
                "Consider physical therapy",
                "Monitor range of motion"
            },
            ["Emergency"] = new List<string> {
                "Activate emergency protocols",
                "Ensure airway, breathing, circulation",
                "Obtain vital signs immediately",
                "Prepare for potential interventions"
            },
            ["Infectious"] = new List<string> {
                "Consider isolation precautions",
                "Monitor temperature closely",
                "Consider antibiotic therapy",
                "Assess for sepsis signs"
            },
            ["Psychiatric"] = new List<string> {
                "Assess safety and suicide risk",
                "Provide supportive environment",
                "Consider psychiatric consultation",
                "Monitor medication compliance"
            }
        };

        /// <summary>
        /// Common symptoms by category
        /// </summary>
        public Dictionary<string, List<string>> CategorySymptoms = new Dictionary<string, List<string>>
        {
            ["Cardiovascular"] = new List<string> {
                "chest pain", "shortness of breath", "palpitations", "dizziness", "syncope",
                "leg swelling", "fatigue", "orthopnea", "paroxysmal nocturnal dyspnea", "elevated heart rate",
                "hr"
            },
            ["Respiratory"] = new List<string> {
                "cough", "shortness of breath", "wheezing", "chest tightness", "sputum production",
                "hemoptysis", "pleuritic pain", "dyspnea on exertion"
            },
            ["Neurological"] = new List<string> {
                "headache", "dizziness", "weakness", "numbness", "tingling", "seizures",
                "confusion", "memory loss", "vision changes", "speech difficulties"
            },
            ["Gastrointestinal"] = new List<string> {
                "nausea", "vomiting", "diarrhea", "constipation", "abdominal pain",
                "bloating", "heartburn", "dysphagia", "hematemesis", "melena"
            },
            ["Musculoskeletal"] = new List<string> {
                "joint pain", "muscle pain", "stiffness", "swelling", "limited range of motion",
                "back pain", "neck pain", "muscle weakness", "joint deformity"
            },
            ["Infectious"] = new List<string> {
                "fever", "chills", "fatigue", "malaise", "body aches", "headache",
                "sore throat", "cough", "runny nose", "skin rash"
            }
        };

        /// <summary>
        /// Red flag symptoms that indicate high urgency
        /// </summary>
        public List<string> RedFlagSymptoms = new List<string>
        {
            "chest pain", "difficulty breathing", "severe headache", "loss of consciousness",
            "seizure", "severe bleeding", "severe abdominal pain", "inability to speak",
            "severe confusion", "high fever", "signs of stroke", "cardiac arrest",
            "anaphylaxis", "severe trauma", "poisoning", "overdose"
        };

        /// <summary>
        /// Common diagnostic tests by category
        /// </summary>
        public Dictionary<string, List<string>> CategoryTests = new Dictionary<string, List<string>>
        {
            ["Cardiovascular"] = new List<string> {
                "ECG", "Echocardiogram", "Stress test", "Cardiac catheterization", "BNP", "Troponins"
            },
            ["Respiratory"] = new List<string> {
                "Chest X-ray", "CT chest", "Pulmonary function tests", "ABG", "Sputum culture"
            },
            ["Neurological"] = new List<string> {
                "CT head", "MRI brain", "EEG", "Lumbar puncture", "EMG", "Nerve conduction studies"
            },
            ["Gastrointestinal"] = new List<string> {
                "CT abdomen", "Endoscopy", "Colonoscopy", "Liver function tests", "Lipase", "Amylase"
            },
            ["Infectious"] = new List<string> {
                "Blood culture", "CBC with differential", "ESR", "CRP", "Procalcitonin", "Urinalysis"
            }
        };

        public string GetSeverityDescription(int level)
        {
            return SeverityDescriptions.ContainsKey(level) ? SeverityDescriptions[level] : "Unknown severity";
        }

        public string GetUrgencyDescription(int level)
        {
            return UrgencyDescriptions.ContainsKey(level) ? UrgencyDescriptions[level] : "Unknown urgency";
        }

        public List<string> GetCategoryRecommendations(string category)
        {
            return CategoryRecommendations.ContainsKey(category) ? 
                CategoryRecommendations[category] : 
                new List<string> { "Consult with healthcare provider for appropriate evaluation" };
        }

        /// <summary>
        /// Determines if text contains red flag symptoms
        /// </summary>
        public bool HasRedFlagSymptoms(string text)
        {
            var lowerText = text.ToLower();
            return RedFlagSymptoms.Any(symptom => lowerText.Contains(symptom.ToLower()));
        }

        /// <summary>
        /// Gets suggested tests for a diagnostic category
        /// </summary>
        public List<string> GetSuggestedTests(string category)
        {
            return CategoryTests.ContainsKey(category) ? CategoryTests[category] : new List<string>();
        }

        /// <summary>
        /// Calculates symptom overlap with categories
        /// </summary>
        public Dictionary<string, double> CalculateSymptomOverlap(List<string> extractedSymptoms)
        {
            var overlap = new Dictionary<string, double>();

            foreach (var category in CategorySymptoms.Keys)
            {
                var categorySymptoms = CategorySymptoms[category];
                int matches = 0;

                foreach (var symptom in extractedSymptoms)
                {
                    if (categorySymptoms.Any(cs => cs.ToLower().Contains(symptom.ToLower()) || 
                                                   symptom.ToLower().Contains(cs.ToLower())))
                    {
                        matches++;
                    }
                }

                overlap[category] = extractedSymptoms.Count > 0 ? 
                    (double)matches / extractedSymptoms.Count : 0.0;
            }

            return overlap.OrderByDescending(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        /// <summary>
        /// Provides differential diagnosis suggestions
        /// </summary>
        public List<string> GetDifferentialDiagnosis(string category, List<string> symptoms)
        {
            var differentials = new Dictionary<string, List<string>>
            {
                ["Cardiovascular"] = new List<string> {
                    "Myocardial infarction", "Angina pectoris", "Heart failure", "Arrhythmia",
                    "Pericarditis", "Aortic dissection", "Pulmonary embolism"
                },
                ["Respiratory"] = new List<string> {
                    "Pneumonia", "Asthma", "COPD", "Pneumothorax", "Bronchitis",
                    "Pulmonary embolism", "Lung cancer", "Pleural effusion"
                },
                ["Neurological"] = new List<string> {
                    "Migraine", "Tension headache", "Stroke", "Seizure disorder",
                    "Multiple sclerosis", "Parkinson's disease", "Brain tumor"
                },
                ["Gastrointestinal"] = new List<string> {
                    "Gastroenteritis", "Peptic ulcer", "Appendicitis", "Cholecystitis",
                    "Pancreatitis", "IBD", "IBS", "GERD"
                }
            };

            return differentials.ContainsKey(category) ? differentials[category] : new List<string>();
        }
    }
}
