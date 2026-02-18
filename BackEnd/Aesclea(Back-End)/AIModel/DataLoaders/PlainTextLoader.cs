// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Aesclea_Back_End_.AIModel.DataLoaders
{
    /// <summary>
    /// Loads training data from plain text files (TXT format)
    /// Supports various formats including ICD-11, medical textbooks, clinical notes, etc.
    /// </summary>
    public class PlainTextLoader
    {
        public class TextDocument
        {
            public string FilePath { get; set; } = "";
            public string FileName { get; set; } = "";
            public string Content { get; set; } = "";
            public List<string> Sections { get; set; } = new List<string>();
            public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
        }

        /// <summary>
        /// Configuration for text parsing
        /// </summary>
        public class ParsingConfig
        {
            // Section delimiters
            public string[] SectionDelimiters { get; set; } = new[] { "\n\n", "\r\n\r\n" };
            
            // Minimum characters per section to be considered valid
            public int MinSectionLength { get; set; } = 50;
            
            // Maximum characters per section (split longer sections)
            public int MaxSectionLength { get; set; } = 2000;
            
            // For ICD-11 and structured data: detect code patterns
            public bool DetectMedicalCodes { get; set; } = true;
            
            // Extract titles/headers (lines ending with :, all caps, etc.)
            public bool ExtractHeaders { get; set; } = true;
            
            // Clean special characters and formatting
            public bool CleanText { get; set; } = true;
            
            // Overlap between chunks for context preservation
            public int ChunkOverlap { get; set; } = 100;
        }

        /// <summary>
        /// Loads all TXT files from a directory
        /// </summary>
        public static List<TextDocument> LoadFromDirectory(string directoryPath, ParsingConfig? config = null)
        {
            config ??= new ParsingConfig();
            var documents = new List<TextDocument>();

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Directory not found: {directoryPath}");
                return documents;
            }

            try
            {
                var txtFiles = Directory.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories);
                Console.WriteLine($"Found {txtFiles.Length} TXT files in {directoryPath}");

                foreach (var filePath in txtFiles)
                {
                    try
                    {
                        var doc = LoadFromFile(filePath, config);
                        if (doc != null)
                        {
                            documents.Add(doc);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading {filePath}: {ex.Message}");
                    }
                }

                Console.WriteLine($"Successfully loaded {documents.Count} text documents");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scanning directory: {ex.Message}");
            }

            return documents;
        }

        /// <summary>
        /// Loads a single TXT file
        /// </summary>
        public static TextDocument? LoadFromFile(string filePath, ParsingConfig? config = null)
        {
            config ??= new ParsingConfig();

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return null;
            }

            try
            {
                var content = File.ReadAllText(filePath, Encoding.UTF8);
                
                if (string.IsNullOrWhiteSpace(content))
                {
                    Console.WriteLine($"Empty file: {filePath}");
                    return null;
                }

                var document = new TextDocument
                {
                    FilePath = filePath,
                    FileName = Path.GetFileName(filePath),
                    Content = content
                };

                // Extract metadata from filename or content
                ExtractMetadata(document, config);

                // Parse content into sections
                document.Sections = ParseSections(content, config);

                return document;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Parses content into meaningful sections/chunks
        /// </summary>
        private static List<string> ParseSections(string content, ParsingConfig config)
        {
            var sections = new List<string>();

            if (config.CleanText)
            {
                content = CleanTextContent(content);
            }

            // Try multiple section splitting strategies
            var potentialSections = new List<string>();

            // Strategy 1: Split by double newlines (paragraphs)
            var paragraphs = content.Split(config.SectionDelimiters, StringSplitOptions.RemoveEmptyEntries);
            
            // Strategy 2: Detect ICD-11 or other medical code sections
            if (config.DetectMedicalCodes)
            {
                potentialSections.AddRange(SplitByMedicalCodes(content));
            }
            else
            {
                potentialSections.AddRange(paragraphs);
            }

            // Strategy 3: Split by headers if enabled
            if (config.ExtractHeaders)
            {
                potentialSections = SplitByHeaders(content).ToList();
            }

            // If no sections found, use paragraphs
            if (potentialSections.Count == 0)
            {
                potentialSections.AddRange(paragraphs);
            }

            // Process sections: filter, chunk, and overlap
            foreach (var section in potentialSections)
            {
                var trimmed = section.Trim();
                
                if (trimmed.Length < config.MinSectionLength)
                    continue;

                // If section is too long, split it into chunks with overlap
                if (trimmed.Length > config.MaxSectionLength)
                {
                    var chunks = ChunkLongSection(trimmed, config.MaxSectionLength, config.ChunkOverlap);
                    sections.AddRange(chunks);
                }
                else
                {
                    sections.Add(trimmed);
                }
            }

            return sections;
        }

        /// <summary>
        /// Splits content by medical codes (ICD-11, ICD-10, etc.)
        /// </summary>
        private static List<string> SplitByMedicalCodes(string content)
        {
            var sections = new List<string>();
            
            // Pattern for ICD-11 codes: e.g., "1A00", "5A10.1", "MG30.6"
            var icd11Pattern = @"(?:^|\n)([A-Z]{1,2}\d{1,2}(?:\.\d+)?)\s+(.+?)(?=\n[A-Z]{1,2}\d{1,2}|\Z)";
            
            // Pattern for ICD-10 codes: e.g., "A00.0", "J45.9"
            var icd10Pattern = @"(?:^|\n)([A-Z]\d{2}(?:\.\d+)?)\s+(.+?)(?=\n[A-Z]\d{2}|\Z)";

            var matches = Regex.Matches(content, icd11Pattern, RegexOptions.Singleline);
            
            if (matches.Count == 0)
            {
                matches = Regex.Matches(content, icd10Pattern, RegexOptions.Singleline);
            }

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    var code = match.Groups[1].Value;
                    var description = match.Groups[2].Value.Trim();
                    
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        sections.Add($"{code}: {description}");
                    }
                }
            }
            else
            {
                // Fallback: look for any code-like patterns
                var lines = content.Split('\n');
                var currentSection = new StringBuilder();
                
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    
                    // Detect if line starts with a code pattern
                    if (Regex.IsMatch(trimmed, @"^[A-Z]{1,2}\d{1,3}"))
                    {
                        if (currentSection.Length > 0)
                        {
                            sections.Add(currentSection.ToString().Trim());
                            currentSection.Clear();
                        }
                    }
                    
                    currentSection.AppendLine(trimmed);
                }
                
                if (currentSection.Length > 0)
                {
                    sections.Add(currentSection.ToString().Trim());
                }
            }

            return sections;
        }

        /// <summary>
        /// Splits content by headers (lines ending with :, all caps, numbered sections)
        /// </summary>
        private static List<string> SplitByHeaders(string content)
        {
            var sections = new List<string>();
            var lines = content.Split('\n');
            var currentSection = new StringBuilder();
            var currentHeader = "";

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                // Detect headers:
                // 1. Lines ending with ':'
                // 2. All uppercase lines (min 3 chars)
                // 3. Numbered sections (1., 2.1, etc.)
                bool isHeader = (trimmed.EndsWith(':') && trimmed.Length < 100) ||
                               (trimmed == trimmed.ToUpper() && trimmed.Length >= 3 && trimmed.Length < 100) ||
                               Regex.IsMatch(trimmed, @"^\d+(\.\d+)*\.?\s+[A-Z]");

                if (isHeader)
                {
                    // Save previous section
                    if (currentSection.Length > 0)
                    {
                        var sectionText = currentSection.ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(sectionText))
                        {
                            sections.Add($"{currentHeader}\n{sectionText}");
                        }
                        currentSection.Clear();
                    }
                    
                    currentHeader = trimmed;
                }
                else
                {
                    currentSection.AppendLine(trimmed);
                }
            }

            // Add last section
            if (currentSection.Length > 0)
            {
                var sectionText = currentSection.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(sectionText))
                {
                    sections.Add($"{currentHeader}\n{sectionText}");
                }
            }

            return sections;
        }

        /// <summary>
        /// Chunks long sections with overlap for context preservation
        /// </summary>
        private static List<string> ChunkLongSection(string text, int maxLength, int overlap)
        {
            var chunks = new List<string>();
            var words = text.Split(' ');
            var currentChunk = new StringBuilder();
            var overlapBuffer = new List<string>();

            for (int i = 0; i < words.Length; i++)
            {
                currentChunk.Append(words[i]).Append(' ');

                if (currentChunk.Length >= maxLength || i == words.Length - 1)
                {
                    chunks.Add(currentChunk.ToString().Trim());
                    
                    // Prepare overlap for next chunk
                    currentChunk.Clear();
                    
                    // Add overlap words from the end of current chunk
                    var overlapText = string.Join(' ', words.Skip(Math.Max(0, i - overlap)).Take(overlap));
                    currentChunk.Append(overlapText).Append(' ');
                }
            }

            return chunks;
        }

        /// <summary>
        /// Cleans text content (remove excessive whitespace, special chars, etc.)
        /// </summary>
        private static string CleanTextContent(string content)
        {
            // Remove excessive whitespace
            content = Regex.Replace(content, @"[ \t]+", " ");
            
            // Normalize line endings
            content = content.Replace("\r\n", "\n").Replace("\r", "\n");
            
            // Remove more than 2 consecutive newlines
            content = Regex.Replace(content, @"\n{3,}", "\n\n");
            
            // Remove common formatting artifacts
            content = content.Replace("�", "");
            content = content.Replace("\0", "");
            
            return content;
        }

        /// <summary>
        /// Extracts metadata from filename and content
        /// </summary>
        private static void ExtractMetadata(TextDocument document, ParsingConfig config)
        {
            var filename = Path.GetFileNameWithoutExtension(document.FileName);
            
            // Extract metadata from filename
            if (filename.Contains("icd", StringComparison.OrdinalIgnoreCase))
            {
                document.Metadata["type"] = "medical_codes";
                document.Metadata["source"] = "ICD";
            }
            else if (filename.Contains("textbook", StringComparison.OrdinalIgnoreCase))
            {
                document.Metadata["type"] = "textbook";
            }
            else if (filename.Contains("clinical", StringComparison.OrdinalIgnoreCase))
            {
                document.Metadata["type"] = "clinical_notes";
            }
            else if (filename.Contains("guideline", StringComparison.OrdinalIgnoreCase))
            {
                document.Metadata["type"] = "clinical_guideline";
            }
            
            // Try to detect specialty from content or filename
            var medicalSpecialties = new Dictionary<string, string[]>
            {
                ["cardiology"] = new[] { "cardiac", "heart", "cardio", "ecg" },
                ["neurology"] = new[] { "neuro", "brain", "seizure", "stroke" },
                ["oncology"] = new[] { "cancer", "tumor", "oncology", "chemotherapy" },
                ["radiology"] = new[] { "xray", "ct scan", "mri", "imaging" },
                ["emergency"] = new[] { "emergency", "trauma", "acute" }
            };

            var contentLower = document.Content.ToLower();
            foreach (var specialty in medicalSpecialties)
            {
                if (specialty.Value.Any(keyword => contentLower.Contains(keyword)))
                {
                    document.Metadata["specialty"] = specialty.Key;
                    break;
                }
            }
        }

        /// <summary>
        /// Converts text documents to TrainingData format
        /// </summary>
        public static List<TrainingData> ConvertToTrainingData(List<TextDocument> documents)
        {
            var trainingData = new List<TrainingData>();

            foreach (var doc in documents)
            {
                foreach (var section in doc.Sections)
                {
                    if (string.IsNullOrWhiteSpace(section) || section.Length < 50)
                        continue;

                    // Infer diagnosis category from content
                    int diagnosisCategory = InferDiagnosisCategory(section, doc.Metadata);
                    int severityLevel = InferSeverityLevel(section);
                    int urgencyLevel = InferUrgencyLevel(section);
                    
                    trainingData.Add(new TrainingData
                    {
                        InputText = section,
                        DiagnosisCategory = diagnosisCategory,
                        SeverityLevel = severityLevel,
                        UrgencyLevel = urgencyLevel,
                        Specialty = doc.Metadata.ContainsKey("specialty") ? doc.Metadata["specialty"] : "general",
                        Keywords = ExtractKeywords(section)
                    });
                }
            }

            Console.WriteLine($"Converted {documents.Count} documents into {trainingData.Count} training samples");
            return trainingData;
        }

        /// <summary>
        /// Infers diagnosis category from text content
        /// </summary>
        private static int InferDiagnosisCategory(string text, Dictionary<string, string> metadata)
        {
            var textLower = text.ToLower();

            // Category mapping based on medical content
            if (textLower.Contains("cancer") || textLower.Contains("tumor") || textLower.Contains("malignant"))
                return 2; // Oncology
            
            if (textLower.Contains("infection") || textLower.Contains("bacterial") || textLower.Contains("viral"))
                return 1; // Infectious
            
            if (textLower.Contains("chronic") || textLower.Contains("diabetes") || textLower.Contains("hypertension"))
                return 3; // Chronic disease
            
            if (textLower.Contains("injury") || textLower.Contains("trauma") || textLower.Contains("fracture"))
                return 4; // Trauma
            
            if (textLower.Contains("heart") || textLower.Contains("cardiac") || textLower.Contains("cardiovascular"))
                return 5; // Cardiovascular

            return 0; // General/unknown
        }

        /// <summary>
        /// Infers severity level from text content
        /// </summary>
        private static int InferSeverityLevel(string text)
        {
            var textLower = text.ToLower();

            if (textLower.Contains("severe") || textLower.Contains("critical") || textLower.Contains("life-threatening"))
                return 3; // Severe
            
            if (textLower.Contains("moderate") || textLower.Contains("significant"))
                return 2; // Moderate
            
            if (textLower.Contains("mild") || textLower.Contains("minor"))
                return 1; // Mild

            return 1; // Default to mild
        }

        /// <summary>
        /// Infers urgency level from text content
        /// </summary>
        private static int InferUrgencyLevel(string text)
        {
            var textLower = text.ToLower();

            if (textLower.Contains("emergency") || textLower.Contains("urgent") || textLower.Contains("immediate"))
                return 3; // Emergency
            
            if (textLower.Contains("soon") || textLower.Contains("timely"))
                return 2; // Soon
            
            if (textLower.Contains("routine") || textLower.Contains("scheduled"))
                return 1; // Routine

            return 1; // Default to routine
        }

        /// <summary>
        /// Extracts keywords from text content
        /// </summary>
        private static string ExtractKeywords(string text)
        {
            // Extract medical terms (simplified approach)
            var keywords = new HashSet<string>();
            var words = text.Split(new[] { ' ', ',', '.', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                var cleaned = word.Trim().ToLower();
                
                // Keep medical terms (3+ chars, contains medical patterns)
                if (cleaned.Length >= 3 && 
                    (cleaned.EndsWith("itis") || cleaned.EndsWith("osis") || 
                     cleaned.EndsWith("emia") || cleaned.EndsWith("pathy") ||
                     cleaned.Contains("cardio") || cleaned.Contains("neuro") ||
                     cleaned.Contains("hepat") || cleaned.Contains("renal")))
                {
                    keywords.Add(cleaned);
                    if (keywords.Count >= 10) break; // Limit keywords
                }
            }

            return string.Join(", ", keywords);
        }
    }
}
