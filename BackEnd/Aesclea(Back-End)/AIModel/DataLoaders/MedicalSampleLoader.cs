using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Aesclea_Back_End_.AIModel;

namespace Aesclea_Back_End_.AIModel.DataLoaders
{
    /// <summary>
    /// Loads medical transcription samples from mtsamples.csv
    /// </summary>
    public class MedicalSampleLoader
    {
        public class MedicalSample
        {
            public string Description { get; set; } = "";
            public string MedicalSpecialty { get; set; } = "";
            public string SampleName { get; set; } = "";
            public string Transcription { get; set; } = "";
            public string Keywords { get; set; } = "";
        }

        /// <summary>
        /// Loads medical samples from CSV file
        /// </summary>
        public static List<MedicalSample> LoadFromCsv(string csvPath)
        {
            var samples = new List<MedicalSample>();
            
            if (!File.Exists(csvPath))
            {
                Console.WriteLine($"CSV file not found: {csvPath}");
                return samples;
            }

            try
            {
                var lines = File.ReadAllLines(csvPath, Encoding.UTF8);
                
                // Skip header row
                for (int i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        var sample = ParseCsvLine(lines[i]);
                        if (sample != null && !string.IsNullOrWhiteSpace(sample.Transcription))
                        {
                            samples.Add(sample);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing line {i}: {ex.Message}");
                    }
                }

                Console.WriteLine($"Loaded {samples.Count} medical samples from CSV");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
            }

            return samples;
        }

        /// <summary>
        /// Parses a CSV line handling quoted fields with commas
        /// </summary>
        private static MedicalSample? ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var currentField = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }
            
            // Add last field
            fields.Add(currentField.ToString());

            // CSV format: index,description,medical_specialty,sample_name,transcription,keywords
            if (fields.Count >= 6)
            {
                return new MedicalSample
                {
                    Description = fields[1].Trim(),
                    MedicalSpecialty = fields[2].Trim(),
                    SampleName = fields[3].Trim(),
                    Transcription = fields[4].Trim(),
                    Keywords = fields[5].Trim()
                };
            }

            return null;
        }

        /// <summary>
        /// Converts samples into training data format
        /// </summary>
        public static List<TrainingData> ConvertToTrainingData(List<MedicalSample> samples)
        {
            var trainingData = new List<TrainingData>();

            foreach (var sample in samples)
            {
                // Use full transcription as input text
                var inputText = sample.Transcription;

                // Determine urgency based on keywords and content
                var urgency = DetermineUrgency(sample);

                // Determine severity based on specialty and content
                var severity = DetermineSeverity(sample);

                // Use specialty as primary diagnosis category
                var diagnosis = MapSpecialtyToDiagnosis(sample.MedicalSpecialty);

                trainingData.Add(new TrainingData
                {
                    InputText = inputText,
                    DiagnosisCategory = diagnosis,
                    SeverityLevel = severity,
                    UrgencyLevel = urgency,
                    Specialty = sample.MedicalSpecialty,
                    Keywords = sample.Keywords
                });
            }

            return trainingData;
        }

        private static int DetermineUrgency(MedicalSample sample)
        {
            var text = (sample.Transcription + " " + sample.Keywords).ToLower();

            // Emergency keywords
            if (text.Contains("emergency") || text.Contains("urgent") || 
                text.Contains("acute") || text.Contains("critical") ||
                text.Contains("trauma") || text.Contains("hemorrhage") ||
                text.Contains("cardiac arrest") || text.Contains("stroke"))
                return 3; // High urgency

            // Moderate urgency
            if (text.Contains("pain") || text.Contains("fever") || 
                text.Contains("infection") || text.Contains("fracture"))
                return 2; // Moderate urgency

            // Routine/elective
            if (text.Contains("follow-up") || text.Contains("routine") || 
                text.Contains("screening") || text.Contains("consultation"))
                return 0; // Low urgency

            return 1; // Default moderate
        }

        private static int DetermineSeverity(MedicalSample sample)
        {
            var text = (sample.Transcription + " " + sample.Keywords).ToLower();

            // Critical conditions
            if (text.Contains("cancer") || text.Contains("malignant") || 
                text.Contains("metastatic") || text.Contains("carcinoma") ||
                text.Contains("acute myocardial") || text.Contains("sepsis") ||
                text.Contains("respiratory failure"))
                return 3; // Severe

            // Serious but not critical
            if (text.Contains("chronic") || text.Contains("disease") || 
                text.Contains("syndrome") || text.Contains("disorder"))
                return 2; // Moderate

            // Minor conditions
            if (text.Contains("mild") || text.Contains("minor") || 
                text.Contains("benign") || text.Contains("resolved"))
                return 1; // Mild

            return 2; // Default moderate
        }

        private static int MapSpecialtyToDiagnosis(string specialty)
        {
            // Map medical specialties to diagnosis categories (0-9)
            var specialtyLower = specialty.ToLower();

            if (specialtyLower.Contains("cardiovascular") || specialtyLower.Contains("cardiology"))
                return 0; // Cardiovascular

            if (specialtyLower.Contains("neurology") || specialtyLower.Contains("neurosurgery"))
                return 1; // Neurological

            if (specialtyLower.Contains("respiratory") || specialtyLower.Contains("pulmonology"))
                return 2; // Respiratory

            if (specialtyLower.Contains("gastroenterology") || specialtyLower.Contains("gi"))
                return 3; // Gastrointestinal

            if (specialtyLower.Contains("orthopedic") || specialtyLower.Contains("musculoskeletal"))
                return 4; // Musculoskeletal

            if (specialtyLower.Contains("oncology") || specialtyLower.Contains("hematology"))
                return 5; // Oncology/Hematology

            if (specialtyLower.Contains("endocrinology") || specialtyLower.Contains("diabetes"))
                return 6; // Endocrine

            if (specialtyLower.Contains("nephrology") || specialtyLower.Contains("urology"))
                return 7; // Renal/Urological

            if (specialtyLower.Contains("dermatology") || specialtyLower.Contains("allergy"))
                return 8; // Dermatological

            return 9; // Other/General
        }
    }
}
