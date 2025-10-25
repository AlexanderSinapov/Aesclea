using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Aesclea_Back_End_.AIModel;

namespace Aesclea_Back_End_.AIModel.DataLoaders
{
    /// <summary>
    /// Loads medical knowledge from textbook .txt files
    /// </summary>
    public class TextbookLoader
    {
        public class TextbookChunk
        {
            public string SourceBook { get; set; } = "";
            public string Content { get; set; } = "";
            public string Specialty { get; set; } = "";
        }

        /// <summary>
        /// Loads all textbooks from directory
        /// </summary>
        public static List<TextbookChunk> LoadFromDirectory(string directoryPath)
        {
            var chunks = new List<TextbookChunk>();

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Directory not found: {directoryPath}");
                return chunks;
            }

            try
            {
                var txtFiles = Directory.GetFiles(directoryPath, "*.txt", SearchOption.TopDirectoryOnly);

                Console.WriteLine($"Found {txtFiles.Length} textbook files");

                foreach (var filePath in txtFiles)
                {
                    try
                    {
                        var fileChunks = LoadTextbook(filePath);
                        chunks.AddRange(fileChunks);
                        Console.WriteLine($"Loaded {fileChunks.Count} chunks from {Path.GetFileName(filePath)}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading {Path.GetFileName(filePath)}: {ex.Message}");
                    }
                }

                Console.WriteLine($"Total textbook chunks loaded: {chunks.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading directory: {ex.Message}");
            }

            return chunks;
        }

        /// <summary>
        /// Loads and chunks a single textbook file
        /// </summary>
        private static List<TextbookChunk> LoadTextbook(string filePath)
        {
            var chunks = new List<TextbookChunk>();
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var specialty = DetermineSpecialty(fileName);

            var content = File.ReadAllText(filePath, Encoding.UTF8);

            // Split into chunks (paragraphs or sections)
            // Using chunk size of ~1000-2000 characters for better training
            var chunkSize = 1500;
            var overlapSize = 200; // Overlap to maintain context

            for (int i = 0; i < content.Length; i += (chunkSize - overlapSize))
            {
                var chunkLength = Math.Min(chunkSize, content.Length - i);
                var chunkText = content.Substring(i, chunkLength);

                // Only add chunks with substantial content
                if (!string.IsNullOrWhiteSpace(chunkText) && chunkText.Length > 100)
                {
                    chunks.Add(new TextbookChunk
                    {
                        SourceBook = fileName,
                        Content = chunkText.Trim(),
                        Specialty = specialty
                    });
                }
            }

            return chunks;
        }

        /// <summary>
        /// Determines medical specialty from filename
        /// </summary>
        private static string DetermineSpecialty(string fileName)
        {
            var nameLower = fileName.ToLower();

            if (nameLower.Contains("anatomy") || nameLower.Contains("gray"))
                return "Anatomy";

            if (nameLower.Contains("internal") || nameLower.Contains("harrison"))
                return "Internal Medicine";

            if (nameLower.Contains("pathology") || nameLower.Contains("robbins") || nameLower.Contains("pathoma"))
                return "Pathology";

            if (nameLower.Contains("surgery") || nameLower.Contains("schwartz"))
                return "Surgery";

            if (nameLower.Contains("pharmacology") || nameLower.Contains("katzung"))
                return "Pharmacology";

            if (nameLower.Contains("pediatric") || nameLower.Contains("nelson"))
                return "Pediatrics";

            if (nameLower.Contains("neurology") || nameLower.Contains("adams"))
                return "Neurology";

            if (nameLower.Contains("obstetric") || nameLower.Contains("williams"))
                return "Obstetrics";

            if (nameLower.Contains("gynecology") || nameLower.Contains("novak"))
                return "Gynecology";

            if (nameLower.Contains("physiology") || nameLower.Contains("levy"))
                return "Physiology";

            if (nameLower.Contains("immunology") || nameLower.Contains("janeway"))
                return "Immunology";

            if (nameLower.Contains("histology") || nameLower.Contains("ross"))
                return "Histology";

            if (nameLower.Contains("biochemistry") || nameLower.Contains("lippincott"))
                return "Biochemistry";

            if (nameLower.Contains("first_aid") || nameLower.Contains("step"))
                return "Clinical Review";

            if (nameLower.Contains("psychiatry") || nameLower.Contains("dsm"))
                return "Psychiatry";

            if (nameLower.Contains("cell") || nameLower.Contains("alberts"))
                return "Cell Biology";

            return "General Medicine";
        }

        /// <summary>
        /// Converts textbook chunks to training data
        /// </summary>
        public static List<TrainingData> ConvertToTrainingData(List<TextbookChunk> chunks)
        {
            var trainingData = new List<TrainingData>();

            foreach (var chunk in chunks)
            {
                // Textbook content is educational, not clinical urgency
                // Use it to improve diagnostic accuracy and knowledge base
                var diagnosis = MapSpecialtyToDiagnosis(chunk.Specialty);

                trainingData.Add(new TrainingData
                {
                    InputText = chunk.Content,
                    DiagnosisCategory = diagnosis,
                    SeverityLevel = 1, // Educational content has neutral severity
                    UrgencyLevel = 0,  // Not a clinical case, no urgency
                    Specialty = chunk.Specialty,
                    Keywords = ExtractKeyTerms(chunk.Content)
                });
            }

            return trainingData;
        }

        private static int MapSpecialtyToDiagnosis(string specialty)
        {
            var specialtyLower = specialty.ToLower();

            if (specialtyLower.Contains("internal medicine"))
                return 9; // General/Internal

            if (specialtyLower.Contains("neurology"))
                return 1; // Neurological

            if (specialtyLower.Contains("surgery"))
                return 4; // Surgical/Musculoskeletal

            if (specialtyLower.Contains("pediatric"))
                return 9; // General

            if (specialtyLower.Contains("pathology"))
                return 5; // Related to oncology/disease processes

            if (specialtyLower.Contains("pharmacology"))
                return 9; // General

            if (specialtyLower.Contains("obstetric") || specialtyLower.Contains("gynecology"))
                return 7; // Reproductive/Urological

            if (specialtyLower.Contains("psychiatry"))
                return 1; // Neurological/Psychiatric

            return 9; // General/Other
        }

        private static string ExtractKeyTerms(string content)
        {
            // Simple extraction of medical terms (could be enhanced with NLP)
            var words = content.Split(new[] { ' ', '\n', '\r', '\t', '.', ',', ';' }, 
                StringSplitOptions.RemoveEmptyEntries);

            // Find words that look like medical terms (capitalized, longer than 5 chars)
            var medicalTerms = words
                .Where(w => w.Length > 5 && char.IsUpper(w[0]))
                .Distinct()
                .Take(10);

            return string.Join(", ", medicalTerms);
        }
    }
}
