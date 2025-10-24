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
using static Aesclea_Back_End_.Services.TumorAnalysisService;

namespace Aesclea_Back_End_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TumorAnalysisController : ControllerBase
    {
        private readonly TumorAnalysisService _tumorAnalysisService;
        private readonly ILogger<TumorAnalysisController> _logger;

        public TumorAnalysisController(TumorAnalysisService tumorAnalysisService, ILogger<TumorAnalysisController> logger)
        {
            _tumorAnalysisService = tumorAnalysisService;
            _logger = logger;
        }        [HttpPost("analyze")]
        public async Task<ActionResult<TumorAnalysisResponse>> AnalyzeImage(IFormFile imageFile, [FromQuery] bool saveAnnotated = true)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    return BadRequest("No image file provided");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Invalid file format. Supported formats: JPG, PNG, BMP, TIFF");

                // Process the image
                var result = await _tumorAnalysisService.AnalyzeImageAsync(imageFile, saveAnnotated);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing tumor image");
                return StatusCode(500, new { Error = "Internal server error during analysis", Details = ex.Message });
            }
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadAnnotatedImage(string fileName)
        {
            try
            {
                var result = await _tumorAnalysisService.GetAnnotatedImageAsync(fileName);
                
                if (result == null)
                    return NotFound("Annotated image not found");

                return File(result.ImageBytes, result.ContentType, result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading annotated image");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("train")]
        public async Task<IActionResult> TrainModel([FromBody] TrainModelRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.TrainingDataPath))
                    return BadRequest("Training data path is required");

                await _tumorAnalysisService.TrainModelAsync(request);
                
                return Ok(new { Message = "Model training completed successfully", Timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error training model");
                return StatusCode(500, new { Error = "Internal server error during training", Details = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<ActionResult<List<TumorAnalysisResponse>>> GetAnalysisHistory()
        {
            try
            {
                var history = await _tumorAnalysisService.GetAnalysisHistoryAsync();
                return Ok(history);
            }            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving analysis history");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("annotate/{analysisId}")]
        public async Task<IActionResult> CreateAnnotatedImage(string analysisId, [FromQuery] string outlineColor = "Auto")
        {
            try
            {
                var allowedColors = new[] { "Auto", "Red", "Orange", "Yellow", "Green", "Blue", "Purple", "DarkRed", "Pink" };
                if (!allowedColors.Contains(outlineColor))
                    return BadRequest($"Invalid outline color. Allowed colors: {string.Join(", ", allowedColors)}");

                var annotatedImagePath = await _tumorAnalysisService.CreateAnnotatedImageOnDemandAsync(analysisId, outlineColor);
                
                if (annotatedImagePath == null)
                    return NotFound("Analysis not found or does not contain a detectable tumor");

                var fileName = Path.GetFileName(annotatedImagePath);
                return Ok(new 
                { 
                    Message = "Annotated image created successfully",
                    AnnotatedImagePath = annotatedImagePath,
                    FileName = fileName,
                    DownloadUrl = $"/api/tumoranalysis/download/{fileName}",
                    OutlineColor = outlineColor
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating annotated image");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("outline-colors")]
        public IActionResult GetAvailableOutlineColors()
        {
            var colors = new[]
            {
                new { Value = "Auto", Description = "Automatic color based on tumor grade (Yellow=Grade 1, Orange=Grade 2, Red=Grade 3, DarkRed=Grade 4)" },
                new { Value = "Red", Description = "Classic red outline" },
                new { Value = "Orange", Description = "Orange outline" },
                new { Value = "Yellow", Description = "Yellow outline" },
                new { Value = "Green", Description = "Green outline" },
                new { Value = "Blue", Description = "Blue outline" },
                new { Value = "Purple", Description = "Purple outline" },
                new { Value = "DarkRed", Description = "Dark red outline" },
                new { Value = "Pink", Description = "Pink outline" }
            };

            return Ok(colors);
        }

        [HttpGet("test-classifier")]
        public IActionResult TestClassifier()
        {
            try
            {
                _logger.LogInformation("🧪 Testing tumor classifier with synthetic data...");
                
                // Create test data: all zeros (should represent no tumor)
                var zeroData = new List<double>(new double[16384]); // All zeros
                
                // Create test data: all ones (should represent strong tumor signal)
                var oneData = Enumerable.Repeat(1.0, 16384).ToList();
                
                // Create test data: random pattern
                var random = new Random();
                var randomData = Enumerable.Range(0, 16384).Select(_ => random.NextDouble()).ToList();
                
                // Test the classifier with different inputs
                var result1 = _tumorAnalysisService.TestClassifierWithData(zeroData);
                var result2 = _tumorAnalysisService.TestClassifierWithData(oneData);
                var result3 = _tumorAnalysisService.TestClassifierWithData(randomData);
                
                var diagnostics = new
                {
                    Test1_AllZeros = new { 
                        HasTumor = result1.HasTumor, 
                        Probability = result1.TumorProbability,
                        Type = result1.TumorType,
                        Grade = result1.TumorGrade 
                    },
                    Test2_AllOnes = new { 
                        HasTumor = result2.HasTumor, 
                        Probability = result2.TumorProbability,
                        Type = result2.TumorType,
                        Grade = result2.TumorGrade 
                    },
                    Test3_Random = new { 
                        HasTumor = result3.HasTumor, 
                        Probability = result3.TumorProbability,
                        Type = result3.TumorType,
                        Grade = result3.TumorGrade 
                    },
                    Analysis = new
                    {
                        NetworkSeemsTrained = result1.TumorProbability != result2.TumorProbability || result2.TumorProbability != result3.TumorProbability,
                        Note = "If all probabilities are the same, the network may not be trained properly"
                    }
                };
                
                return Ok(diagnostics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing classifier");
                return StatusCode(500, new { Error = "Error testing classifier", Details = ex.Message });
            }
        }
    }
}