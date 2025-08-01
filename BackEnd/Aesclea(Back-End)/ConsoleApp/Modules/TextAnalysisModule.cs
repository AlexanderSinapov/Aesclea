using Aesclea_Back_End_.ConsoleApp.Modules;
using Aesclea_Back_End_.Services;
using Aesclea_Back_End_.Models;
using Microsoft.Extensions.Logging;

namespace Aesclea_Back_End_.ConsoleApp.Modules
{
    public class TextAnalysisModule : IConsoleModule
    {
        private IEnhancedTextAnalysisService? _textAnalysisService;
        private readonly List<EnhancedTextAnalysisResult> _analysisHistory = new();

        public async Task InitializeAsync()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<EnhancedTextAnalysisService>();
            _textAnalysisService = new EnhancedTextAnalysisService(logger);
            
            await Task.CompletedTask;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                DisplayTextAnalysisMenu();
                var choice = global::System.Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await AnalyzeSingleTextAsync();
                        break;
                    case "2":
                        await BatchAnalyzeTextsAsync();
                        break;
                    case "3":
                        await AnalyzeFromFileAsync();
                        break;
                    case "4":
                        await ViewAnalysisHistoryAsync();
                        break;
                    case "5":
                        await ExportAnalysisResultsAsync();
                        break;
                    case "6":
                        await CompareAnalysisResultsAsync();
                        break;
                    case "7":
                        await ClearAnalysisHistoryAsync();
                        break;
                    case "8":
                        return; // Return to main menu
                    default:
                        global::System.Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                if (choice != "8")
                {
                    global::System.Console.WriteLine("\nPress Enter to continue...");
                    global::System.Console.ReadLine();
                }
            }
        }

        public async Task<string> GetStatusAsync()
        {
            await Task.CompletedTask;
            return _textAnalysisService is not null ? "✅ Ready" : "❌ Not Initialized";
        }

        private void DisplayTextAnalysisMenu()
        {
            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("        MEDICAL TEXT ANALYSIS MODULE");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();
            global::System.Console.WriteLine("Options:");
            global::System.Console.WriteLine();
            global::System.Console.WriteLine("1. 📝 Analyze Single Medical Text");
            global::System.Console.WriteLine("2. 📦 Batch Analyze Multiple Texts");
            global::System.Console.WriteLine("3. 📄 Analyze Text from File");
            global::System.Console.WriteLine("4. 📋 View Analysis History");
            global::System.Console.WriteLine("5. 💾 Export Analysis Results");
            global::System.Console.WriteLine("6. 🔍 Compare Analysis Results");
            global::System.Console.WriteLine("7. 🗑️  Clear Analysis History");
            global::System.Console.WriteLine("8. ⬅️  Return to Main Menu");
            global::System.Console.WriteLine();
            global::System.Console.WriteLine($"Analysis History: {_analysisHistory.Count} entries");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.Write("Enter your choice (1-8): ");
        }

        private async Task AnalyzeSingleTextAsync()
        {
            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("        SINGLE TEXT ANALYSIS");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("Enter medical text to analyze:");
            global::System.Console.WriteLine("(Type your text and press Enter twice when finished)");
            global::System.Console.WriteLine();

            var textLines = new List<string>();
            string? line;
            var emptyLineCount = 0;

            while ((line = global::System.Console.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    emptyLineCount++;
                    if (emptyLineCount >= 2)
                        break;
                }
                else
                {
                    emptyLineCount = 0;
                    textLines.Add(line);
                }
            }

            if (!textLines.Any())
            {
                global::System.Console.WriteLine("❌ No text entered for analysis.");
                return;
            }

            var medicalText = string.Join(" ", textLines);
            
            global::System.Console.WriteLine();
            global::System.Console.WriteLine("🔍 Analyzing text...");

            if (_textAnalysisService is null)
            {
                global::System.Console.WriteLine("❌ Text analysis service not initialized.");
                return;
            }

            try
            {
                var result = await _textAnalysisService.AnalyzeMedicalTextAsync(medicalText);
                _analysisHistory.Add(result);

                await DisplayAnalysisResultAsync(result);
            }
            catch (Exception ex)
            {
                global::System.Console.WriteLine($"❌ Error analyzing text: {ex.Message}");
            }
        }

        private async Task DisplayAnalysisResultAsync(EnhancedTextAnalysisResult result)
        {
            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("         TEXT ANALYSIS RESULTS");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            // Basic info
            global::System.Console.WriteLine("📋 Analysis Summary:");
            global::System.Console.WriteLine($"   Analyzed at: {result.AnalyzedAt:yyyy-MM-dd HH:mm:ss}");
            global::System.Console.WriteLine($"   Original text length: {result.OriginalText.Length} characters");
            global::System.Console.WriteLine($"   Medical entities found: {result.MedicalEntities.Count}");
            global::System.Console.WriteLine();

            // Sentiment Analysis
            global::System.Console.WriteLine("😊 Sentiment Analysis:");
            global::System.Console.WriteLine($"   Overall sentiment: {result.Sentiment.Sentiment}");
            global::System.Console.WriteLine($"   Confidence: {result.Sentiment.Confidence:P1}");
            global::System.Console.WriteLine($"   Positive indicators: {result.Sentiment.PositiveIndicators}");
            global::System.Console.WriteLine($"   Negative indicators: {result.Sentiment.NegativeIndicators}");
            global::System.Console.WriteLine($"   Concern indicators: {result.Sentiment.ConcernIndicators}");
            global::System.Console.WriteLine();

            // Medical Entities
            if (result.MedicalEntities.Any())
            {
                global::System.Console.WriteLine("🏥 Medical Entities:");
                var entitiesByCategory = result.MedicalEntities.GroupBy(e => e.Category);
                foreach (var category in entitiesByCategory)
                {
                    global::System.Console.WriteLine($"   {category.Key.ToUpper()}:");
                    foreach (var entity in category.Take(5)) // Show first 5 of each category
                    {
                        global::System.Console.WriteLine($"     • {entity.Text} (confidence: {entity.Confidence:P1})");
                    }
                    if (category.Count() > 5)
                    {
                        global::System.Console.WriteLine($"     ... and {category.Count() - 5} more");
                    }
                }
                global::System.Console.WriteLine();
            }

            // Symptoms
            if (result.Symptoms.Symptoms.Any())
            {
                global::System.Console.WriteLine("🤒 Symptom Analysis:");
                global::System.Console.WriteLine($"   Total symptoms: {result.Symptoms.SymptomCount}");
                global::System.Console.WriteLine($"   Affected body parts: {string.Join(", ", result.Symptoms.AffectedBodyParts.Take(5))}");
                
                global::System.Console.WriteLine("   Symptoms by severity:");
                foreach (var severity in result.Symptoms.SeverityDistribution.OrderBy(s => s.Key))
                {
                    var severityName = severity.Key switch
                    {
                        1 => "Mild",
                        2 => "Moderate", 
                        3 => "Severe",
                        4 => "Extreme",
                        _ => "Unknown"
                    };
                    global::System.Console.WriteLine($"     {severityName}: {severity.Value}");
                }

                global::System.Console.WriteLine("   Top symptoms:");
                foreach (var symptom in result.Symptoms.Symptoms.Take(5))
                {
                    var severityName = symptom.Severity switch
                    {
                        1 => "Mild",
                        2 => "Moderate",
                        3 => "Severe", 
                        4 => "Extreme",
                        _ => "Unknown"
                    };
                    global::System.Console.WriteLine($"     • {symptom.Symptom} ({severityName})");
                    if (!string.IsNullOrEmpty(symptom.BodyPart))
                        global::System.Console.WriteLine($"       Location: {symptom.BodyPart}");
                }
                global::System.Console.WriteLine();
            }

            // Risk Assessment
            global::System.Console.WriteLine("⚠️  Risk Assessment:");
            global::System.Console.WriteLine($"   Overall risk level: {result.RiskAssessment.OverallRiskLevel}");
            global::System.Console.WriteLine($"   Risk score: {result.RiskAssessment.RiskScore:F1}/10");
            
            if (result.RiskAssessment.RiskFactors.Any())
            {
                global::System.Console.WriteLine("   Risk factors:");
                foreach (var risk in result.RiskAssessment.RiskFactors.Take(5))
                {
                    global::System.Console.WriteLine($"     • {risk.Factor} ({risk.RiskLevel})");
                    global::System.Console.WriteLine($"       {risk.Description}");
                }
            }
            global::System.Console.WriteLine();

            // Vital Signs
            if (result.VitalSigns.Any())
            {
                global::System.Console.WriteLine("📊 Vital Signs Mentioned:");
                foreach (var vital in result.VitalSigns)
                {
                    var status = vital.IsNormal ? "✅ Normal" : "⚠️  Abnormal";
                    global::System.Console.WriteLine($"   • {vital.Type}: {vital.Value} {status}");
                }
                global::System.Console.WriteLine();
            }

            // Medications
            if (result.Medications.Any())
            {
                global::System.Console.WriteLine("💊 Medications Mentioned:");
                foreach (var med in result.Medications.Take(10))
                {
                    global::System.Console.WriteLine($"   • {med.Name} ({med.Category})");
                }
                global::System.Console.WriteLine();
            }

            // Clinical Insights
            if (result.ClinicalInsights.Any())
            {
                global::System.Console.WriteLine("🔬 Clinical Insights:");
                foreach (var insight in result.ClinicalInsights)
                {
                    global::System.Console.WriteLine($"   • {insight.Type}: {insight.Description}");
                    global::System.Console.WriteLine($"     Confidence: {insight.Confidence:P1}");
                }
                global::System.Console.WriteLine();
            }

            // Recommendations
            if (result.Recommendations.Any())
            {
                global::System.Console.WriteLine("💡 Recommendations:");
                foreach (var recommendation in result.Recommendations)
                {
                    global::System.Console.WriteLine($"   • {recommendation}");
                }
                global::System.Console.WriteLine();
            }

            global::System.Console.WriteLine("==============================================");
            
            await Task.CompletedTask;
        }

        private async Task BatchAnalyzeTextsAsync()
        {
            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("         BATCH TEXT ANALYSIS");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            global::System.Console.Write("How many texts would you like to analyze? ");
            if (!int.TryParse(global::System.Console.ReadLine(), out int count) || count <= 0)
            {
                global::System.Console.WriteLine("Invalid number.");
                return;
            }

            var texts = new List<string>();

            for (int i = 1; i <= count; i++)
            {
                global::System.Console.WriteLine();
                global::System.Console.WriteLine($"Enter text #{i}:");
                global::System.Console.WriteLine("(Type your text and press Enter twice when finished)");
                global::System.Console.WriteLine();

                var textLines = new List<string>();
                string? line;
                var emptyLineCount = 0;

                while ((line = global::System.Console.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        emptyLineCount++;
                        if (emptyLineCount >= 2)
                            break;
                    }
                    else
                    {
                        emptyLineCount = 0;
                        textLines.Add(line);
                    }
                }

                if (textLines.Any())
                {
                    texts.Add(string.Join(" ", textLines));
                }
                else
                {
                    global::System.Console.WriteLine($"Skipping empty text #{i}");
                }
            }

            if (!texts.Any())
            {
                global::System.Console.WriteLine("❌ No texts to analyze.");
                return;
            }

            global::System.Console.WriteLine();
            global::System.Console.WriteLine($"🔍 Analyzing {texts.Count} texts...");

            if (_textAnalysisService is null)
            {
                global::System.Console.WriteLine("❌ Text analysis service not initialized.");
                return;
            }

            var results = new List<EnhancedTextAnalysisResult>();

            for (int i = 0; i < texts.Count; i++)
            {
                try
                {
                    global::System.Console.WriteLine($"Processing text {i + 1}/{texts.Count}...");
                    var result = await _textAnalysisService.AnalyzeMedicalTextAsync(texts[i]);
                    results.Add(result);
                    _analysisHistory.Add(result);
                }
                catch (Exception ex)
                {
                    global::System.Console.WriteLine($"❌ Error analyzing text {i + 1}: {ex.Message}");
                }
            }

            // Display batch summary
            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("       BATCH ANALYSIS SUMMARY");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine($"📊 Processed {results.Count} texts successfully");
            global::System.Console.WriteLine();

            // Sentiment distribution
            var sentimentDist = results.GroupBy(r => r.Sentiment.Sentiment)
                .ToDictionary(g => g.Key, g => g.Count());

            global::System.Console.WriteLine("Sentiment Distribution:");
            foreach (var sentiment in sentimentDist)
            {
                var percentage = (double)sentiment.Value / results.Count * 100;
                global::System.Console.WriteLine($"   {sentiment.Key}: {sentiment.Value} ({percentage:F1}%)");
            }
            global::System.Console.WriteLine();

            // Risk level distribution
            var riskDist = results.GroupBy(r => r.RiskAssessment.OverallRiskLevel)
                .ToDictionary(g => g.Key, g => g.Count());

            global::System.Console.WriteLine("Risk Level Distribution:");
            foreach (var risk in riskDist)
            {
                var percentage = (double)risk.Value / results.Count * 100;
                global::System.Console.WriteLine($"   {risk.Key}: {risk.Value} ({percentage:F1}%)");
            }
            global::System.Console.WriteLine();

            // Most common symptoms
            var allSymptoms = results.SelectMany(r => r.Symptoms.Symptoms.Select(s => s.Symptom))
                .GroupBy(s => s.ToLower())
                .OrderByDescending(g => g.Count())
                .Take(5);

            global::System.Console.WriteLine("Most Common Symptoms:");
            foreach (var symptom in allSymptoms)
            {
                global::System.Console.WriteLine($"   {symptom.Key}: {symptom.Count()} mentions");
            }

            global::System.Console.WriteLine();
            global::System.Console.WriteLine("✅ Batch analysis completed!");
        }

        private async Task AnalyzeFromFileAsync()
        {
            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("        ANALYZE TEXT FROM FILE");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            global::System.Console.Write("Enter file path: ");
            var filePath = global::System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                global::System.Console.WriteLine("❌ File not found.");
                return;
            }

            try
            {
                var fileContent = await File.ReadAllTextAsync(filePath);
                
                if (string.IsNullOrWhiteSpace(fileContent))
                {
                    global::System.Console.WriteLine("❌ File is empty.");
                    return;
                }

                global::System.Console.WriteLine($"📄 File loaded: {new FileInfo(filePath).Name}");
                global::System.Console.WriteLine($"   Content length: {fileContent.Length} characters");
                global::System.Console.WriteLine();
                global::System.Console.WriteLine("🔍 Analyzing file content...");

                if (_textAnalysisService is null)
                {
                    global::System.Console.WriteLine("❌ Text analysis service not initialized.");
                    return;
                }

                var result = await _textAnalysisService.AnalyzeMedicalTextAsync(fileContent);
                _analysisHistory.Add(result);

                await DisplayAnalysisResultAsync(result);
            }
            catch (Exception ex)
            {
                global::System.Console.WriteLine($"❌ Error reading/analyzing file: {ex.Message}");
            }
        }

        private async Task ViewAnalysisHistoryAsync()
        {
            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("         ANALYSIS HISTORY");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            if (!_analysisHistory.Any())
            {
                global::System.Console.WriteLine("📭 No analysis history available.");
                return;
            }

            global::System.Console.WriteLine($"📊 Total analyses: {_analysisHistory.Count}");
            global::System.Console.WriteLine();

            for (int i = _analysisHistory.Count - 1; i >= 0 && i >= _analysisHistory.Count - 10; i--)
            {
                var analysis = _analysisHistory[i];
                var textPreview = analysis.OriginalText.Length > 100 
                    ? analysis.OriginalText.Substring(0, 100) + "..."
                    : analysis.OriginalText;

                global::System.Console.WriteLine($"{_analysisHistory.Count - i}. {analysis.AnalyzedAt:yyyy-MM-dd HH:mm:ss}");
                global::System.Console.WriteLine($"   Text: \"{textPreview}\"");
                global::System.Console.WriteLine($"   Sentiment: {analysis.Sentiment.Sentiment}");
                global::System.Console.WriteLine($"   Risk Level: {analysis.RiskAssessment.OverallRiskLevel}");
                global::System.Console.WriteLine($"   Entities: {analysis.MedicalEntities.Count}");
                global::System.Console.WriteLine();
            }

            if (_analysisHistory.Count > 10)
            {
                global::System.Console.WriteLine($"... and {_analysisHistory.Count - 10} more entries");
            }

            global::System.Console.WriteLine();
            global::System.Console.Write("Enter analysis number to view details (or press Enter to continue): ");
            var input = global::System.Console.ReadLine();

            if (int.TryParse(input, out int index) && index >= 1 && index <= _analysisHistory.Count)
            {
                await DisplayAnalysisResultAsync(_analysisHistory[_analysisHistory.Count - index]);
            }

            await Task.CompletedTask;
        }

        private async Task ExportAnalysisResultsAsync()
        {
            if (!_analysisHistory.Any())
            {
                global::System.Console.WriteLine("❌ No analysis results to export.");
                return;
            }

            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("        EXPORT ANALYSIS RESULTS");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            var exportPath = Path.Combine(Environment.CurrentDirectory, "Exports", "TextAnalysis");
            Directory.CreateDirectory(exportPath);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var filename = Path.Combine(exportPath, $"text_analysis_results_{timestamp}.json");

            var exportData = new
            {
                ExportDate = DateTime.Now,
                TotalAnalyses = _analysisHistory.Count,
                Results = _analysisHistory.Select(r => new
                {
                    r.AnalyzedAt,
                    TextLength = r.OriginalText.Length,
                    TextPreview = r.OriginalText.Length > 200 ? r.OriginalText.Substring(0, 200) + "..." : r.OriginalText,
                    Sentiment = new
                    {
                        r.Sentiment.Sentiment,
                        r.Sentiment.Confidence,
                        r.Sentiment.PositiveIndicators,
                        r.Sentiment.NegativeIndicators,
                        r.Sentiment.ConcernIndicators
                    },
                    RiskAssessment = new
                    {
                        r.RiskAssessment.OverallRiskLevel,
                        r.RiskAssessment.RiskScore,
                        RiskFactorCount = r.RiskAssessment.RiskFactors.Count
                    },
                    EntityCount = r.MedicalEntities.Count,
                    SymptomCount = r.Symptoms.SymptomCount,
                    VitalSignCount = r.VitalSigns.Count,
                    MedicationCount = r.Medications.Count,
                    InsightCount = r.ClinicalInsights.Count,
                    RecommendationCount = r.Recommendations.Count
                })
            };

            await File.WriteAllTextAsync(filename, System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));

            global::System.Console.WriteLine($"✅ Analysis results exported successfully!");
            global::System.Console.WriteLine($"📁 File location: {filename}");
            global::System.Console.WriteLine($"📊 Analyses exported: {_analysisHistory.Count}");
        }

        private async Task CompareAnalysisResultsAsync()
        {
            if (_analysisHistory.Count < 2)
            {
                global::System.Console.WriteLine("❌ Need at least 2 analyses to compare.");
                return;
            }

            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("        COMPARE ANALYSIS RESULTS");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("Available analyses:");
            for (int i = 0; i < _analysisHistory.Count; i++)
            {
                var analysis = _analysisHistory[i];
                var textPreview = analysis.OriginalText.Length > 50 
                    ? analysis.OriginalText.Substring(0, 50) + "..."
                    : analysis.OriginalText;
                global::System.Console.WriteLine($"{i + 1}. {analysis.AnalyzedAt:yyyy-MM-dd HH:mm} - \"{textPreview}\"");
            }

            global::System.Console.WriteLine();
            global::System.Console.Write("Select first analysis (number): ");
            if (!int.TryParse(global::System.Console.ReadLine(), out int first) || first < 1 || first > _analysisHistory.Count)
            {
                global::System.Console.WriteLine("Invalid selection.");
                return;
            }

            global::System.Console.Write("Select second analysis (number): ");
            if (!int.TryParse(global::System.Console.ReadLine(), out int second) || second < 1 || second > _analysisHistory.Count || second == first)
            {
                global::System.Console.WriteLine("Invalid selection.");
                return;
            }

            var analysis1 = _analysisHistory[first - 1];
            var analysis2 = _analysisHistory[second - 1];

            // Display comparison
            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("         ANALYSIS COMPARISON");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("📊 SENTIMENT COMPARISON:");
            global::System.Console.WriteLine($"   Analysis 1: {analysis1.Sentiment.Sentiment} ({analysis1.Sentiment.Confidence:P1})");
            global::System.Console.WriteLine($"   Analysis 2: {analysis2.Sentiment.Sentiment} ({analysis2.Sentiment.Confidence:P1})");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("⚠️  RISK COMPARISON:");
            global::System.Console.WriteLine($"   Analysis 1: {analysis1.RiskAssessment.OverallRiskLevel} (Score: {analysis1.RiskAssessment.RiskScore:F1})");
            global::System.Console.WriteLine($"   Analysis 2: {analysis2.RiskAssessment.OverallRiskLevel} (Score: {analysis2.RiskAssessment.RiskScore:F1})");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("🏥 ENTITY COMPARISON:");
            global::System.Console.WriteLine($"   Analysis 1: {analysis1.MedicalEntities.Count} entities");
            global::System.Console.WriteLine($"   Analysis 2: {analysis2.MedicalEntities.Count} entities");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("🤒 SYMPTOM COMPARISON:");
            global::System.Console.WriteLine($"   Analysis 1: {analysis1.Symptoms.SymptomCount} symptoms");
            global::System.Console.WriteLine($"   Analysis 2: {analysis2.Symptoms.SymptomCount} symptoms");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("💊 MEDICATION COMPARISON:");
            global::System.Console.WriteLine($"   Analysis 1: {analysis1.Medications.Count} medications");
            global::System.Console.WriteLine($"   Analysis 2: {analysis2.Medications.Count} medications");

            await Task.CompletedTask;
        }

        private async Task ClearAnalysisHistoryAsync()
        {
            global::System.Console.Write($"Are you sure you want to clear {_analysisHistory.Count} analysis results? (y/n): ");
            if (global::System.Console.ReadLine()?.ToLower() == "y")
            {
                _analysisHistory.Clear();
                global::System.Console.WriteLine("✅ Analysis history cleared successfully.");
            }
            else
            {
                global::System.Console.WriteLine("Operation cancelled.");
            }
            
            await Task.CompletedTask;
        }
    }
}
