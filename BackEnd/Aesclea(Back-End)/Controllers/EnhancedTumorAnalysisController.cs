// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Microsoft.AspNetCore.Mvc;
using Aesclea_Back_End_.AIModel;
using Aesclea_Back_End_.Services;
using Aesclea_Back_End_.AIModel.Helpers;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel.DataAnnotations;

namespace Aesclea_Back_End_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnhancedTumorAnalysisController : ControllerBase
    {
        private readonly TumorAnalysisService _tumorAnalysisService;
        private readonly ILogger<EnhancedTumorAnalysisController> _logger;
        private readonly TumorClassifier _tumorClassifier;

        public EnhancedTumorAnalysisController(
            TumorAnalysisService tumorAnalysisService, 
            ILogger<EnhancedTumorAnalysisController> logger,
            TumorClassifier tumorClassifier)
        {
            _tumorAnalysisService = tumorAnalysisService;
            _logger = logger;
            _tumorClassifier = tumorClassifier;
        }

        /// <summary>
        /// Enhanced single image analysis with configurable parameters
        /// </summary>
        [HttpPost("analyze/enhanced")]
        public async Task<ActionResult<EnhancedAnalysisResponse>> EnhancedAnalyze(
            IFormFile imageFile,
            [FromQuery] double detectionThreshold = 0.5,
            [FromQuery] double classificationThreshold = 0.3,
            [FromQuery] bool enableDetailedAnalysis = true,
            [FromQuery] bool saveAnnotated = true)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    return BadRequest("No image file provided");

                // Validate thresholds
                if (detectionThreshold < 0 || detectionThreshold > 1)
                    return BadRequest("Detection threshold must be between 0 and 1");

                if (classificationThreshold < 0 || classificationThreshold > 1)
                    return BadRequest("Classification threshold must be between 0 and 1");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Invalid file format. Supported formats: JPG, PNG, BMP, TIFF");

                // Configure classifier settings
                _tumorClassifier.DetectionThreshold = detectionThreshold;
                _tumorClassifier.ClassificationThreshold = classificationThreshold;
                _tumorClassifier.EnableDetailedAnalysis = enableDetailedAnalysis;

                // Process the image
                var result = await _tumorAnalysisService.AnalyzeImageAsync(imageFile, saveAnnotated);
                
                // Create enhanced response
                var enhancedResponse = new EnhancedAnalysisResponse
                {
                    BasicAnalysis = result,
                    AnalysisSettings = new AnalysisSettings
                    {
                        DetectionThreshold = detectionThreshold,
                        ClassificationThreshold = classificationThreshold,
                        EnableDetailedAnalysis = enableDetailedAnalysis
                    },
                    ClinicalRecommendations = GenerateClinicalRecommendations(result),
                    ConfidenceMetrics = new ConfidenceMetrics
                    {
                        OverallConfidence = result.Summary?.Contains("Overall Confidence") == true ? 
                            ExtractOverallConfidence(result.Summary) : 0,
                        DetectionConfidence = result.TumorProbability,
                        ClassificationReliability = CalculateClassificationReliability(result)
                    }
                };

                return Ok(enhancedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in enhanced tumor analysis");
                return StatusCode(500, new { Error = "Internal server error during enhanced analysis", Details = ex.Message });
            }
        }

        /// <summary>
        /// Batch analysis of multiple images
        /// </summary>
        [HttpPost("analyze/batch")]
        public async Task<ActionResult<BatchAnalysisResponse>> BatchAnalyze(
            List<IFormFile> imageFiles,
            [FromQuery] double detectionThreshold = 0.5,
            [FromQuery] bool saveIndividualReports = false)
        {
            try
            {
                if (imageFiles == null || imageFiles.Count == 0)
                    return BadRequest("No image files provided");

                if (imageFiles.Count > 50) // Limit batch size
                    return BadRequest("Maximum 50 images allowed per batch");

                _tumorClassifier.DetectionThreshold = detectionThreshold;

                var batchResults = new List<TumorAnalysisResponse>();
                var processingErrors = new List<string>();

                foreach (var imageFile in imageFiles)
                {
                    try
                    {
                        var result = await _tumorAnalysisService.AnalyzeImageAsync(imageFile, false);
                        batchResults.Add(result);
                    }
                    catch (Exception ex)
                    {
                        processingErrors.Add($"Error processing {imageFile.FileName}: {ex.Message}");
                        _logger.LogWarning(ex, "Error processing file {FileName} in batch", imageFile.FileName);
                    }
                }

                // Generate batch summary
                var summary = GenerateBatchSummary(batchResults);

                var batchResponse = new BatchAnalysisResponse
                {
                    TotalSubmitted = imageFiles.Count,
                    SuccessfullyProcessed = batchResults.Count,
                    ProcessingErrors = processingErrors,
                    Results = batchResults,
                    Summary = summary,
                    ProcessedAt = DateTime.UtcNow
                };

                return Ok(batchResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch tumor analysis");
                return StatusCode(500, new { Error = "Internal server error during batch analysis", Details = ex.Message });
            }
        }

        /// <summary>
        /// Compare analysis results between multiple images
        /// </summary>
        [HttpPost("analyze/compare")]
        public async Task<ActionResult<ComparisonResponse>> CompareImages(List<IFormFile> imageFiles)
        {
            try
            {
                if (imageFiles == null || imageFiles.Count < 2)
                    return BadRequest("At least 2 images required for comparison");

                if (imageFiles.Count > 10)
                    return BadRequest("Maximum 10 images allowed for comparison");

                var analysisResults = new List<TumorAnalysisResponse>();

                foreach (var imageFile in imageFiles)
                {
                    var result = await _tumorAnalysisService.AnalyzeImageAsync(imageFile, false);
                    analysisResults.Add(result);
                }

                var comparison = new ComparisonResponse
                {
                    ComparedImages = analysisResults.Select(r => new ComparisonItem
                    {
                        FileName = r.OriginalFileName ?? "Unknown",
                        HasTumor = r.HasTumor,
                        TumorProbability = r.TumorProbability,
                        TumorType = r.TumorType,
                        TumorGrade = r.TumorGrade,
                        RiskLevel = DetermineRiskLevel(r)
                    }).ToList(),
                    
                    ComparisonSummary = new ComparisonSummary
                    {
                        TotalImages = analysisResults.Count,
                        TumorsDetected = analysisResults.Count(r => r.HasTumor),
                        HighestConfidence = analysisResults.Max(r => r.TumorProbability),
                        AverageConfidence = analysisResults.Where(r => r.HasTumor).DefaultIfEmpty().Average(r => r?.TumorProbability ?? 0),
                        MostCommonType = GetMostCommonType(analysisResults),
                        RiskDistribution = GetRiskDistribution(analysisResults)
                    },
                    
                    ComparedAt = DateTime.UtcNow
                };

                return Ok(comparison);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in image comparison");
                return StatusCode(500, new { Error = "Internal server error during comparison", Details = ex.Message });
            }
        }

        /// <summary>
        /// Get detailed analysis report for a specific analysis
        /// </summary>
        [HttpGet("report/detailed/{analysisId}")]
        public async Task<ActionResult<DetailedReportResponse>> GetDetailedReport(string analysisId)
        {
            try
            {
                // This would typically retrieve from a database
                // For now, return a placeholder response
                var report = new DetailedReportResponse
                {
                    AnalysisId = analysisId,
                    GeneratedAt = DateTime.UtcNow,
                    ReportSections = new List<ReportSection>
                    {
                        new ReportSection
                        {
                            Title = "Executive Summary",
                            Content = "Comprehensive tumor analysis completed with detailed classification results."
                        },
                        new ReportSection
                        {
                            Title = "Technical Analysis",
                            Content = "AI model performance metrics and confidence intervals for all classifications."
                        },
                        new ReportSection
                        {
                            Title = "Clinical Recommendations",
                            Content = "Evidence-based recommendations for follow-up care and treatment considerations."
                        }
                    }
                };

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating detailed report");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update classifier configuration
        /// </summary>
        [HttpPost("configuration/update")]
        public ActionResult<ConfigurationResponse> UpdateConfiguration([FromBody] ClassifierConfiguration config)
        {
            try
            {
                if (config.DetectionThreshold < 0 || config.DetectionThreshold > 1)
                    return BadRequest("Detection threshold must be between 0 and 1");

                if (config.ClassificationThreshold < 0 || config.ClassificationThreshold > 1)
                    return BadRequest("Classification threshold must be between 0 and 1");

                _tumorClassifier.DetectionThreshold = config.DetectionThreshold;
                _tumorClassifier.ClassificationThreshold = config.ClassificationThreshold;
                _tumorClassifier.EnableDetailedAnalysis = config.EnableDetailedAnalysis;

                var response = new ConfigurationResponse
                {
                    Success = true,
                    Message = "Configuration updated successfully",
                    CurrentConfiguration = new ClassifierConfiguration
                    {
                        DetectionThreshold = _tumorClassifier.DetectionThreshold,
                        ClassificationThreshold = _tumorClassifier.ClassificationThreshold,
                        EnableDetailedAnalysis = _tumorClassifier.EnableDetailedAnalysis
                    },
                    UpdatedAt = DateTime.UtcNow
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating configuration");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get current classifier configuration
        /// </summary>
        [HttpGet("configuration")]
        public ActionResult<ClassifierConfiguration> GetConfiguration()
        {
            return Ok(new ClassifierConfiguration
            {
                DetectionThreshold = _tumorClassifier.DetectionThreshold,
                ClassificationThreshold = _tumorClassifier.ClassificationThreshold,
                EnableDetailedAnalysis = _tumorClassifier.EnableDetailedAnalysis
            });
        }

        /// <summary>
        /// Export analysis results in various formats
        /// </summary>
        [HttpPost("export")]
        public ActionResult<ExportResponse> ExportResults(
            [FromBody] ExportRequest request)
        {
            try
            {
                // This would implement actual export functionality
                var response = new ExportResponse
                {
                    ExportId = Guid.NewGuid().ToString(),
                    Format = request.Format,
                    Status = "Completed",
                    DownloadUrl = $"/api/enhancedtumoranalysis/download-export/{Guid.NewGuid()}",
                    ExportedAt = DateTime.UtcNow,
                    RecordCount = request.AnalysisIds?.Count ?? 0
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting results");
                return StatusCode(500, "Internal server error");
            }
        }        /// <summary>
        /// Load or reload classifier weights from storage
        /// </summary>
        [HttpPost("weights/load")]
        public ActionResult<WeightLoadResponse> LoadWeights(
            [FromBody] WeightLoadRequest? request = null)
        {
            try
            {
                var fileHelper = new FileHelper();
                string baseName = request?.WeightSetName ?? "tgl"; // Default to 'tgl' weights
                
                // Check if weight files exist
                var availableWeightSets = fileHelper.GetAvailableWeightFiles();
                bool weightsExist = availableWeightSets.Any(w => w.Contains(baseName));
                
                if (!weightsExist)
                {
                    return BadRequest($"Weight set '{baseName}' not found. Available weight sets: {string.Join(", ", availableWeightSets)}");
                }

                var loadStartTime = DateTime.UtcNow;
                var loadResults = new List<string>();

                try
                {
                    // Load weights into the classifier
                    _tumorClassifier.LoadWeights(fileHelper, baseName);
                    loadResults.Add($"Successfully loaded classifier weights from '{baseName}'");

                    // Verify weights are loaded by checking if networks respond to test data
                    var testData = Enumerable.Range(0, 16384).Select(x => 0.5).ToList();
                    var testResult = _tumorClassifier.AnalyzeImage(testData);
                    
                    loadResults.Add("Weight loading verified - classifier is responding");
                    
                    _logger.LogInformation($"Weights loaded successfully from '{baseName}' at {loadStartTime}");

                    return Ok(new WeightLoadResponse
                    {
                        Success = true,
                        WeightSetName = baseName,
                        LoadedAt = loadStartTime,
                        LoadDurationMs = (int)(DateTime.UtcNow - loadStartTime).TotalMilliseconds,
                        AvailableWeightSets = availableWeightSets,
                        LoadResults = loadResults,
                        Message = $"Successfully loaded weights from '{baseName}'"
                    });
                }
                catch (Exception loadEx)
                {
                    _logger.LogError(loadEx, $"Failed to load weights from '{baseName}'");
                    return StatusCode(500, new WeightLoadResponse
                    {
                        Success = false,
                        WeightSetName = baseName,
                        LoadedAt = loadStartTime,
                        LoadDurationMs = (int)(DateTime.UtcNow - loadStartTime).TotalMilliseconds,
                        AvailableWeightSets = availableWeightSets,
                        LoadResults = loadResults,
                        Message = $"Failed to load weights: {loadEx.Message}"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in weight loading endpoint");
                return StatusCode(500, $"Error loading weights: {ex.Message}");
            }
        }        /// <summary>
        /// Get information about available weight sets
        /// </summary>
        [HttpGet("weights/available")]
        public ActionResult<AvailableWeightsResponse> GetAvailableWeights()
        {
            try
            {
                var fileHelper = new FileHelper();
                var availableWeightSets = fileHelper.GetAvailableWeightFiles();
                
                var weightSetInfo = new List<WeightSetInfo>();
                
                foreach (var weightSet in availableWeightSets)
                {
                    var info = new WeightSetInfo
                    {
                        Name = weightSet,
                        IsDefault = weightSet == "tgl",
                        Description = GetWeightSetDescription(weightSet)
                    };                    // Try to get file info
                    try
                    {
                        var tempHelper = new FileHelper();
                        tempHelper.OpenFolder(); // Initialize the data path
                        var fullPath = Path.Combine("NeuronData", $"{weightSet}_type_NeuralData.wbn");
                        if (System.IO.File.Exists(fullPath))
                        {
                            var fileInfo = new FileInfo(fullPath);
                            info.LastModified = fileInfo.LastWriteTime;
                            info.SizeBytes = fileInfo.Length;
                        }
                    }
                    catch { }

                    weightSetInfo.Add(info);
                }

                return Ok(new AvailableWeightsResponse
                {
                    AvailableWeightSets = weightSetInfo,
                    DefaultWeightSet = "tgl",
                    TotalCount = weightSetInfo.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available weights");
                return StatusCode(500, $"Error getting available weights: {ex.Message}");
            }
        }

        private string GetWeightSetDescription(string weightSetName)
        {
            return weightSetName switch
            {
                "tgl" => "Default tumor classification weights for glioma detection",
                "V2" => "Version 2 weights with enhanced classification",
                _ => $"Weight set: {weightSetName}"
            };
        }

        // Helper methods
        private static List<string> GenerateClinicalRecommendations(TumorAnalysisResponse result)
        {
            var recommendations = new List<string>();

            if (!result.HasTumor)
            {
                recommendations.Add("No tumor detected. Continue routine screening as appropriate.");
                return recommendations;
            }

            if (result.TumorGrade >= 4)
            {
                recommendations.Add("URGENT: High-grade tumor detected. Immediate oncological consultation recommended.");
                recommendations.Add("Consider expedited staging workup including additional imaging studies.");
            }
            else if (result.TumorGrade >= 3)
            {
                recommendations.Add("PRIORITY: Intermediate-grade tumor. Schedule oncological evaluation within 1-2 weeks.");
                recommendations.Add("Consider multidisciplinary team discussion for treatment planning.");
            }
            else if (result.TumorGrade >= 2)
            {
                recommendations.Add("FOLLOW-UP: Low-intermediate grade tumor. Consider follow-up imaging in 3-6 months.");
                recommendations.Add("Monitor for progression with serial imaging.");
            }
            else
            {
                recommendations.Add("MONITOR: Low-grade lesion. Regular monitoring recommended.");
                recommendations.Add("Consider watchful waiting with periodic reassessment.");
            }

            // Add type-specific recommendations
            if (!string.IsNullOrEmpty(result.TumorType))
            {
                recommendations.Add($"Type-specific considerations for {result.TumorType} should be reviewed.");
            }

            return recommendations;
        }

        private static double ExtractOverallConfidence(string summary)
        {
            // Simple extraction logic - in real implementation, this would be more robust
            if (summary.Contains("Overall Confidence:"))
            {
                var parts = summary.Split("Overall Confidence:");
                if (parts.Length > 1)
                {
                    var confidencePart = parts[1].Split('%')[0].Trim();
                    if (double.TryParse(confidencePart, out double confidence))
                    {
                        return confidence / 100.0;
                    }
                }
            }
            return 0;
        }

        private static double CalculateClassificationReliability(TumorAnalysisResponse result)
        {
            if (!result.HasTumor) return 0;
            
            var confidences = new List<double>
            {
                result.TypeConfidence,
                result.GradeConfidence,
                result.LocationConfidence
            };

            return confidences.Average();
        }

        private static BatchSummary GenerateBatchSummary(List<TumorAnalysisResponse> results)
        {
            var tumorResults = results.Where(r => r.HasTumor).ToList();
            
            return new BatchSummary
            {
                TotalAnalyzed = results.Count,
                TumorsDetected = tumorResults.Count,
                DetectionRate = (double)tumorResults.Count / results.Count,
                AverageConfidence = tumorResults.DefaultIfEmpty().Average(r => r?.TumorProbability ?? 0),
                HighRiskCount = tumorResults.Count(r => r.TumorGrade >= 3),
                GradeDistribution = tumorResults.GroupBy(r => r.TumorGrade)
                                              .ToDictionary(g => g.Key, g => g.Count()),
                TypeDistribution = tumorResults.Where(r => !string.IsNullOrEmpty(r.TumorType))
                                              .GroupBy(r => r.TumorType)
                                              .ToDictionary(g => g.Key!, g => g.Count())
            };
        }

        private static string DetermineRiskLevel(TumorAnalysisResponse result)
        {
            if (!result.HasTumor) return "No Risk";
            
            if (result.TumorGrade >= 4) return "High Risk";
            if (result.TumorGrade >= 3) return "Moderate-High Risk";
            if (result.TumorGrade >= 2) return "Moderate Risk";
            return "Low Risk";
        }

        private static string? GetMostCommonType(List<TumorAnalysisResponse> results)
        {
            return results.Where(r => r.HasTumor && !string.IsNullOrEmpty(r.TumorType))
                         .GroupBy(r => r.TumorType)
                         .OrderByDescending(g => g.Count())
                         .FirstOrDefault()?.Key;
        }

        private static Dictionary<string, int> GetRiskDistribution(List<TumorAnalysisResponse> results)
        {
            return results.GroupBy(r => DetermineRiskLevel(r))
                         .ToDictionary(g => g.Key, g => g.Count());
        }
    }

    // Response DTOs
    public class EnhancedAnalysisResponse
    {
        public TumorAnalysisResponse? BasicAnalysis { get; set; }
        public AnalysisSettings? AnalysisSettings { get; set; }
        public List<string> ClinicalRecommendations { get; set; } = new();
        public ConfidenceMetrics? ConfidenceMetrics { get; set; }
    }

    public class AnalysisSettings
    {
        public double DetectionThreshold { get; set; }
        public double ClassificationThreshold { get; set; }
        public bool EnableDetailedAnalysis { get; set; }
    }

    public class ConfidenceMetrics
    {
        public double OverallConfidence { get; set; }
        public double DetectionConfidence { get; set; }
        public double ClassificationReliability { get; set; }
    }

    public class BatchAnalysisResponse
    {
        public int TotalSubmitted { get; set; }
        public int SuccessfullyProcessed { get; set; }
        public List<string> ProcessingErrors { get; set; } = new();
        public List<TumorAnalysisResponse> Results { get; set; } = new();
        public BatchSummary? Summary { get; set; }
        public DateTime ProcessedAt { get; set; }
    }

    public class BatchSummary
    {
        public int TotalAnalyzed { get; set; }
        public int TumorsDetected { get; set; }
        public double DetectionRate { get; set; }
        public double AverageConfidence { get; set; }
        public int HighRiskCount { get; set; }
        public Dictionary<int, int> GradeDistribution { get; set; } = new();
        public Dictionary<string, int> TypeDistribution { get; set; } = new();
    }

    public class ComparisonResponse
    {
        public List<ComparisonItem> ComparedImages { get; set; } = new();
        public ComparisonSummary? ComparisonSummary { get; set; }
        public DateTime ComparedAt { get; set; }
    }

    public class ComparisonItem
    {
        public string FileName { get; set; } = string.Empty;
        public bool HasTumor { get; set; }
        public double TumorProbability { get; set; }
        public string? TumorType { get; set; }
        public int TumorGrade { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
    }

    public class ComparisonSummary
    {
        public int TotalImages { get; set; }
        public int TumorsDetected { get; set; }
        public double HighestConfidence { get; set; }
        public double AverageConfidence { get; set; }
        public string? MostCommonType { get; set; }
        public Dictionary<string, int> RiskDistribution { get; set; } = new();
    }

    public class DetailedReportResponse
    {
        public string AnalysisId { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public List<ReportSection> ReportSections { get; set; } = new();
    }

    public class ReportSection
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class ClassifierConfiguration
    {
        [Range(0.0, 1.0)]
        public double DetectionThreshold { get; set; } = 0.5;
        
        [Range(0.0, 1.0)]
        public double ClassificationThreshold { get; set; } = 0.3;
        
        public bool EnableDetailedAnalysis { get; set; } = true;
    }

    public class ConfigurationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ClassifierConfiguration? CurrentConfiguration { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ExportRequest
    {
        public List<string>? AnalysisIds { get; set; }
        public string Format { get; set; } = "CSV"; // CSV, JSON, PDF
        public bool IncludeImages { get; set; } = false;
    }

    public class ExportResponse
    {
        public string ExportId { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public DateTime ExportedAt { get; set; }
        public int RecordCount { get; set; }
    }

    // Request/Response classes for weight loading
    public class WeightLoadRequest
    {
        public string WeightSetName { get; set; } = "tgl";
    }

    public class WeightLoadResponse
    {
        public bool Success { get; set; }
        public string WeightSetName { get; set; } = string.Empty;
        public DateTime LoadedAt { get; set; }
        public int LoadDurationMs { get; set; }
        public List<string> AvailableWeightSets { get; set; } = new();
        public List<string> LoadResults { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }

    public class AvailableWeightsResponse
    {
        public List<WeightSetInfo> AvailableWeightSets { get; set; } = new();
        public string DefaultWeightSet { get; set; } = string.Empty;
        public int TotalCount { get; set; }
    }

    public class WeightSetInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime? LastModified { get; set; }
        public long? SizeBytes { get; set; }
    }
}
