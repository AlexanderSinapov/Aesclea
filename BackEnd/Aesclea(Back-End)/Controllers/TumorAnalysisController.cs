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
        }

        [HttpPost("analyze")]
        public async Task<ActionResult<TumorAnalysisResponse>> AnalyzeImage(IFormFile imageFile)
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
                var result = await _tumorAnalysisService.AnalyzeImageAsync(imageFile);
                
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
    }
}