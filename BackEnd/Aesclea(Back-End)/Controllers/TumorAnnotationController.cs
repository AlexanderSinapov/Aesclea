using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Aesclea_Back_End_.Models;
using Aesclea_Back_End_.Services;
using Aesclea_Back_End_.AIModel;
using System.ComponentModel.DataAnnotations;

namespace Aesclea_Back_End_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TumorAnnotationController : ControllerBase
    {
        private readonly TumorAnnotationService _annotationService;
        private readonly TumorAnalysisService _analysisService;
        private readonly ILogger<TumorAnnotationController> _logger;

        public TumorAnnotationController(
            TumorAnnotationService annotationService,
            TumorAnalysisService analysisService,
            ILogger<TumorAnnotationController> logger)
        {
            _annotationService = annotationService;
            _analysisService = analysisService;
            _logger = logger;
        }

        /// <summary>
        /// Create intelligent annotations for a tumor analysis
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<AnnotationResponse>> CreateIntelligentAnnotation(
            [FromBody] CreateAnnotationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get the analysis result
                var analysisResult = await GetAnalysisResultById(request.AnalysisId);
                if (analysisResult == null)
                    return NotFound("Analysis not found");

                // Get original image path
                var originalImagePath = await GetOriginalImagePath(request.AnalysisId);
                if (string.IsNullOrEmpty(originalImagePath) || !System.IO.File.Exists(originalImagePath))
                    return NotFound("Original image not found");

                // Create intelligent annotation
                var response = await _annotationService.CreateIntelligentAnnotationAsync(
                    request.AnalysisId,
                    analysisResult,
                    originalImagePath,
                    request);

                _logger.LogInformation("Created intelligent annotation for analysis {AnalysisId}", request.AnalysisId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating intelligent annotation");
                return StatusCode(500, "An error occurred while creating annotation");
            }
        }

        /// <summary>
        /// Create annotation directly from uploaded image
        /// </summary>
        [HttpPost("analyze-and-annotate")]
        public async Task<ActionResult<AnnotationResponse>> AnalyzeAndAnnotate(
            IFormFile imageFile,
            [FromQuery] AnnotationColor color = AnnotationColor.Auto,
            [FromQuery] int strokeWidth = 3,
            [FromQuery] double opacity = 0.7,
            [FromQuery] bool enableDetailedAnalysis = true)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    return BadRequest("No image file provided");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Invalid file format. Supported formats: JPG, PNG, BMP, TIFF");

                // First perform tumor analysis
                var analysisResult = await _analysisService.AnalyzeImageAsync(imageFile, false);

                // Create annotation request
                var annotationRequest = new CreateAnnotationRequest
                {
                    AnalysisId = analysisResult.Id,
                    Color = color,
                    StrokeWidth = strokeWidth,
                    Opacity = opacity,
                    Type = AnnotationType.AutoDetected
                };

                // Convert analysis result to TumorAnalysisResult
                var tumorResult = ConvertToTumorAnalysisResult(analysisResult);

                // Create intelligent annotation
                var annotationResponse = await _annotationService.CreateIntelligentAnnotationAsync(
                    analysisResult.Id,
                    tumorResult,
                    analysisResult.OriginalImagePath,
                    annotationRequest);

                _logger.LogInformation("Analyzed and annotated image {FileName}", imageFile.FileName);

                return Ok(new
                {
                    Analysis = analysisResult,
                    Annotation = annotationResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in analyze and annotate");
                return StatusCode(500, "An error occurred during analysis and annotation");
            }
        }

        /// <summary>
        /// Get annotation by ID
        /// </summary>
        [HttpGet("{annotationId}")]
        public async Task<ActionResult<TumorAnnotation>> GetAnnotation(int annotationId)
        {
            try
            {
                // TODO: Implement annotation retrieval from database
                await Task.CompletedTask;
                return NotFound("Annotation not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving annotation {AnnotationId}", annotationId);
                return StatusCode(500, "An error occurred while retrieving annotation");
            }
        }

        /// <summary>
        /// Update annotation
        /// </summary>
        [HttpPut("{annotationId}")]
        public async Task<ActionResult<TumorAnnotation>> UpdateAnnotation(
            int annotationId,
            [FromBody] TumorAnnotation annotation)
        {
            try
            {
                if (annotationId != annotation.Id)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // TODO: Implement annotation update
                await Task.CompletedTask;
                return Ok(annotation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating annotation {AnnotationId}", annotationId);
                return StatusCode(500, "An error occurred while updating annotation");
            }
        }

        /// <summary>
        /// Delete annotation
        /// </summary>
        [HttpDelete("{annotationId}")]
        public async Task<ActionResult> DeleteAnnotation(int annotationId)
        {
            try
            {
                // TODO: Implement annotation deletion
                await Task.CompletedTask;
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting annotation {AnnotationId}", annotationId);
                return StatusCode(500, "An error occurred while deleting annotation");
            }
        }

        /// <summary>
        /// Get all annotations for an analysis
        /// </summary>
        [HttpGet("analysis/{analysisId}")]
        public async Task<ActionResult<List<TumorAnnotation>>> GetAnnotationsForAnalysis(string analysisId)
        {
            try
            {
                // TODO: Implement retrieval of annotations for analysis
                await Task.CompletedTask;
                return Ok(new List<TumorAnnotation>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving annotations for analysis {AnalysisId}", analysisId);
                return StatusCode(500, "An error occurred while retrieving annotations");
            }
        }

        /// <summary>
        /// Get annotation statistics
        /// </summary>
        [HttpGet("{annotationId}/statistics")]
        public async Task<ActionResult<AnnotationStatistics>> GetAnnotationStatistics(int annotationId)
        {
            try
            {
                // TODO: Implement annotation statistics calculation
                await Task.CompletedTask;
                return Ok(new AnnotationStatistics());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating annotation statistics for {AnnotationId}", annotationId);
                return StatusCode(500, "An error occurred while calculating statistics");
            }
        }

        /// <summary>
        /// Export annotation data
        /// </summary>
        [HttpPost("{annotationId}/export")]
        public async Task<ActionResult> ExportAnnotation(
            int annotationId,
            [FromQuery] string format = "json")
        {
            try
            {
                var supportedFormats = new[] { "json", "xml", "csv", "dicom" };
                if (!supportedFormats.Contains(format.ToLower()))
                    return BadRequest($"Unsupported format. Supported formats: {string.Join(", ", supportedFormats)}");

                // TODO: Implement annotation export
                await Task.CompletedTask;

                var fileName = $"annotation_{annotationId}.{format}";
                var contentType = format.ToLower() switch
                {
                    "json" => "application/json",
                    "xml" => "application/xml",
                    "csv" => "text/csv",
                    "dicom" => "application/dicom",
                    _ => "application/octet-stream"
                };

                // For now, return a placeholder
                var sampleData = System.Text.Encoding.UTF8.GetBytes($"Sample {format} export data for annotation {annotationId}");
                
                return File(sampleData, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting annotation {AnnotationId}", annotationId);
                return StatusCode(500, "An error occurred while exporting annotation");
            }
        }

        /// <summary>
        /// Validate annotation quality
        /// </summary>
        [HttpPost("{annotationId}/validate")]
        public async Task<ActionResult<AnnotationValidationResult>> ValidateAnnotation(int annotationId)
        {
            try
            {
                // TODO: Implement annotation validation
                var validationResult = new AnnotationValidationResult
                {
                    IsValid = true,
                    QualityScore = 0.85,
                    Issues = new List<string>(),
                    Suggestions = new List<string>
                    {
                        "Consider adding more detailed region boundaries",
                        "Verify tumor type classification"
                    }
                };

                await Task.CompletedTask;
                return Ok(validationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating annotation {AnnotationId}", annotationId);
                return StatusCode(500, "An error occurred while validating annotation");
            }
        }

        /// <summary>
        /// Get available annotation colors
        /// </summary>
        [HttpGet("colors")]
        public ActionResult<Dictionary<string, string>> GetAvailableColors()
        {
            var colors = new Dictionary<string, string>
            {
                { "Auto", "Automatically selected based on tumor grade" },
                { "Red", "High severity/malignant" },
                { "Orange", "Moderate severity" },
                { "Yellow", "Low severity/benign" },
                { "Green", "Normal/healthy tissue" },
                { "Blue", "Vascular structures" },
                { "Purple", "Specialized regions" },
                { "Pink", "Inflammatory regions" },
                { "Cyan", "Anatomical landmarks" },
                { "Magenta", "Contrast-enhanced regions" },
                { "Lime", "Measurement references" },
                { "DarkRed", "Critical/urgent" },
                { "DarkBlue", "Deep structures" },
                { "DarkGreen", "Background reference" }
            };

            return Ok(colors);
        }

        /// <summary>
        /// Get annotation templates
        /// </summary>
        [HttpGet("templates")]
        public ActionResult<List<AnnotationTemplate>> GetAnnotationTemplates()
        {
            var templates = new List<AnnotationTemplate>
            {
                new AnnotationTemplate
                {
                    Name = "Brain Tumor Standard",
                    Description = "Standard annotation template for brain tumors",
                    DefaultColor = AnnotationColor.Red,
                    RequiredRegions = new[] { "Primary Tumor", "Edema", "Necrosis" },
                    OptionalRegions = new[] { "Enhancement", "Infiltration" }
                },
                new AnnotationTemplate
                {
                    Name = "Lung Nodule",
                    Description = "Template for lung nodule analysis",
                    DefaultColor = AnnotationColor.Orange,
                    RequiredRegions = new[] { "Nodule" },
                    OptionalRegions = new[] { "Ground Glass", "Consolidation" }
                },
                new AnnotationTemplate
                {
                    Name = "Breast Mass",
                    Description = "Template for breast mass evaluation",
                    DefaultColor = AnnotationColor.Purple,
                    RequiredRegions = new[] { "Mass" },
                    OptionalRegions = new[] { "Calcifications", "Distortion" }
                }
            };

            return Ok(templates);
        }

        // Helper methods

        private async Task<TumorAnalysisResult?> GetAnalysisResultById(string analysisId)
        {
            // TODO: Implement retrieval from database or cache
            await Task.CompletedTask;
            return null;
        }

        private async Task<string?> GetOriginalImagePath(string analysisId)
        {
            // TODO: Implement retrieval from database
            await Task.CompletedTask;
            return null;
        }

        private TumorAnalysisResult ConvertToTumorAnalysisResult(TumorAnalysisResponse response)
        {
            return new TumorAnalysisResult
            {
                HasTumor = response.HasTumor,
                TumorProbability = response.TumorProbability,
                TumorType = response.TumorType,
                TypeConfidence = response.TypeConfidence,
                TumorGrade = response.TumorGrade,
                GradeDescription = response.GradeDescription,
                GradeConfidence = response.GradeConfidence,
                TumorLocation = response.TumorLocation,
                LocationConfidence = response.LocationConfidence,
                EstimatedStage = response.EstimatedStage,
                StageDescription = response.StageDescription,
                AnalysisTimestamp = response.AnalysisTimestamp
            };
        }
    }

    // Supporting models

    public class AnnotationValidationResult
    {
        public bool IsValid { get; set; }
        public double QualityScore { get; set; }
        public List<string> Issues { get; set; } = new();
        public List<string> Suggestions { get; set; } = new();
    }

    public class AnnotationTemplate
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AnnotationColor DefaultColor { get; set; }
        public string[] RequiredRegions { get; set; } = Array.Empty<string>();
        public string[] OptionalRegions { get; set; } = Array.Empty<string>();
    }
}
