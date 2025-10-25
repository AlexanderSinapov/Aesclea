using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using Aesclea_Back_End_.AIModel;

namespace Aesclea_Back_End_.AIModel.DataLoaders
{
    /// <summary>
    /// Loads PubMed QA dataset for question-answering training
    /// </summary>
    public class PubMedQALoader
    {
        public class PubMedQuestion
        {
            public string QuestionId { get; set; } = "";
            public string Question { get; set; } = "";
            public List<string> Contexts { get; set; } = new();
            public List<string> Labels { get; set; } = new();
            public List<string> Meshes { get; set; } = new();
            public string Year { get; set; } = "";
            public string Answer { get; set; } = "";
            public string LongAnswer { get; set; } = "";
        }

        /// <summary>
        /// Loads PubMed QA from JSON file
        /// </summary>
        public static List<PubMedQuestion> LoadFromJson(string jsonPath, string? groundTruthPath = null)
        {
            var questions = new List<PubMedQuestion>();

            if (!File.Exists(jsonPath))
            {
                Console.WriteLine($"JSON file not found: {jsonPath}");
                return questions;
            }

            try
            {
                var jsonContent = File.ReadAllText(jsonPath);
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonContent);

                // Load ground truth if available
                Dictionary<string, string>? groundTruth = null;
                if (!string.IsNullOrEmpty(groundTruthPath) && File.Exists(groundTruthPath))
                {
                    var gtContent = File.ReadAllText(groundTruthPath);
                    groundTruth = JsonSerializer.Deserialize<Dictionary<string, string>>(gtContent);
                }

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        try
                        {
                            var questionId = item.Key;
                            var questionData = item.Value;

                            var question = new PubMedQuestion
                            {
                                QuestionId = questionId
                            };

                            // Parse question data
                            if (questionData.TryGetProperty("QUESTION", out var questionProp))
                                question.Question = questionProp.GetString() ?? "";

                            if (questionData.TryGetProperty("CONTEXTS", out var contextsProp))
                            {
                                question.Contexts = contextsProp.EnumerateArray()
                                    .Select(c => c.GetString() ?? "")
                                    .ToList();
                            }

                            if (questionData.TryGetProperty("LABELS", out var labelsProp))
                            {
                                question.Labels = labelsProp.EnumerateArray()
                                    .Select(l => l.GetString() ?? "")
                                    .ToList();
                            }

                            if (questionData.TryGetProperty("MESHES", out var meshesProp))
                            {
                                question.Meshes = meshesProp.EnumerateArray()
                                    .Select(m => m.GetString() ?? "")
                                    .ToList();
                            }

                            if (questionData.TryGetProperty("YEAR", out var yearProp))
                                question.Year = yearProp.GetString() ?? "";

                            if (questionData.TryGetProperty("final_decision", out var decisionProp))
                                question.Answer = decisionProp.GetString() ?? "";

                            if (questionData.TryGetProperty("LONG_ANSWER", out var longAnswerProp))
                                question.LongAnswer = longAnswerProp.GetString() ?? "";

                            // Use ground truth if available
                            if (groundTruth != null && groundTruth.ContainsKey(questionId))
                            {
                                question.Answer = groundTruth[questionId];
                            }

                            if (!string.IsNullOrWhiteSpace(question.Question))
                            {
                                questions.Add(question);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing question {item.Key}: {ex.Message}");
                        }
                    }

                    Console.WriteLine($"Loaded {questions.Count} PubMed QA samples");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading JSON file: {ex.Message}");
            }

            return questions;
        }

        /// <summary>
        /// Converts PubMed QA to training data
        /// </summary>
        public static List<TrainingData> ConvertToTrainingData(List<PubMedQuestion> questions)
        {
            var trainingData = new List<TrainingData>();

            foreach (var qa in questions)
            {
                // Combine question and context for rich input
                var inputText = $"{qa.Question}\n\n{string.Join("\n\n", qa.Contexts)}";

                // Determine diagnosis category from MeSH terms
                var diagnosis = DetermineDiagnosisFromMesh(qa.Meshes);

                // Questions are research-based, moderate severity and low urgency
                var severity = DetermineSeverityFromContext(qa.Contexts);
                var urgency = 1; // Research questions have low urgency

                trainingData.Add(new TrainingData
                {
                    InputText = inputText,
                    DiagnosisCategory = diagnosis,
                    SeverityLevel = severity,
                    UrgencyLevel = urgency,
                    Specialty = "Research/Evidence-Based Medicine",
                    Keywords = string.Join(", ", qa.Meshes)
                });
            }

            return trainingData;
        }

        private static int DetermineDiagnosisFromMesh(List<string> meshTerms)
        {
            var meshString = string.Join(" ", meshTerms).ToLower();

            if (meshString.Contains("heart") || meshString.Contains("cardiac") || 
                meshString.Contains("cardiovascular") || meshString.Contains("myocardial"))
                return 0; // Cardiovascular

            if (meshString.Contains("brain") || meshString.Contains("neural") || 
                meshString.Contains("nervous") || meshString.Contains("cognitive"))
                return 1; // Neurological

            if (meshString.Contains("lung") || meshString.Contains("respiratory") || 
                meshString.Contains("pulmonary"))
                return 2; // Respiratory

            if (meshString.Contains("gastro") || meshString.Contains("intestin") || 
                meshString.Contains("liver") || meshString.Contains("digest"))
                return 3; // Gastrointestinal

            if (meshString.Contains("bone") || meshString.Contains("muscle") || 
                meshString.Contains("joint") || meshString.Contains("orthopedic"))
                return 4; // Musculoskeletal

            if (meshString.Contains("cancer") || meshString.Contains("tumor") || 
                meshString.Contains("neoplasm") || meshString.Contains("oncology"))
                return 5; // Oncology

            if (meshString.Contains("diabetes") || meshString.Contains("thyroid") || 
                meshString.Contains("hormone") || meshString.Contains("endocrine"))
                return 6; // Endocrine

            if (meshString.Contains("kidney") || meshString.Contains("renal") || 
                meshString.Contains("urinary"))
                return 7; // Renal

            if (meshString.Contains("skin") || meshString.Contains("derma"))
                return 8; // Dermatological

            return 9; // General/Other
        }

        private static int DetermineSeverityFromContext(List<string> contexts)
        {
            var contextText = string.Join(" ", contexts).ToLower();

            if (contextText.Contains("mortality") || contextText.Contains("fatal") || 
                contextText.Contains("death") || contextText.Contains("severe"))
                return 3; // Severe

            if (contextText.Contains("chronic") || contextText.Contains("complication") || 
                contextText.Contains("significant"))
                return 2; // Moderate

            if (contextText.Contains("mild") || contextText.Contains("minor") || 
                contextText.Contains("benign"))
                return 1; // Mild

            return 2; // Default moderate
        }
    }
}
