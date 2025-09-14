// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Aesclea_Back_End_.Data;
using Aesclea_Back_End_.Models;
using Microsoft.EntityFrameworkCore;

namespace Aesclea_Back_End_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalysisController : ControllerBase
    {
        private readonly AescleaDbContext _context;
        private readonly ILogger<AnalysisController> _logger;

        public AnalysisController(AescleaDbContext context, ILogger<AnalysisController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnalyses([FromQuery] string? patientId = null, [FromQuery] string? department = null)
        {
            try
            {
                var userId = User.Identity?.Name; // Assuming user ID is stored in Name claim
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var query = _context.AnalysisRecords.AsQueryable();

                // Filter by patient if specified
                if (!string.IsNullOrEmpty(patientId))
                {
                    query = query.Where(a => a.PatientId == patientId);
                }

                // Filter by department if specified
                if (!string.IsNullOrEmpty(department))
                {
                    query = query.Where(a => a.Department == department);
                }

                var analyses = await query
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(100) // Limit to last 100 analyses
                    .ToListAsync();

                return Ok(analyses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching analyses");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnalysis([FromBody] CreateAnalysisRequest request)
        {
            try
            {
                var userId = User.Identity?.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var analysis = new AnalysisRecord
                {
                    PatientId = request.PatientId,
                    PatientName = request.PatientName,
                    AnalysisType = request.AnalysisType,
                    Status = "pending",
                    Department = request.Department,
                    Notes = request.Notes,
                    CreatedBy = userId,
                    Data = request.Data
                };

                _context.AnalysisRecords.Add(analysis);
                await _context.SaveChangesAsync();

                return Ok(new { id = analysis.Id, message = "Analysis created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating analysis");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnalysis(string id)
        {
            try
            {
                var userId = User.Identity?.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var analysis = await _context.AnalysisRecords.FindAsync(id);
                if (analysis == null)
                {
                    return NotFound(new { message = "Analysis not found" });
                }

                // Check if user owns this analysis or is admin
                if (analysis.CreatedBy != userId)
                {
                    return Forbid("You can only delete your own analyses");
                }

                _context.AnalysisRecords.Remove(analysis);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Analysis deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting analysis {AnalysisId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{analysisId}/report")]
        public async Task<IActionResult> GetAnalysisReport(string analysisId)
        {
            try
            {
                var userId = User.Identity?.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var analysis = await _context.AnalysisRecords.FindAsync(analysisId);
                if (analysis == null)
                {
                    return NotFound(new { message = "Analysis not found" });
                }

                // Check if user owns this analysis or is admin
                if (analysis.CreatedBy != userId)
                {
                    return Forbid("You can only access your own analysis reports");
                }

                // Generate report based on analysis type
                var report = GenerateAnalysisReport(analysis);

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating analysis report for {AnalysisId}", analysisId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("enhanced-text")]
        public async Task<IActionResult> ProcessEnhancedText([FromBody] EnhancedTextRequest request)
        {
            try
            {
                _logger.LogInformation("Processing enhanced text analysis");

                // For now, we'll provide a simulated enhanced text analysis
                // In a real implementation, this would use NLP models or external services
                var response = new EnhancedTextResponse
                {
                    ProcessedText = request.Text,
                    Summary = GenerateTextSummary(request.Text),
                    KeyPhrases = ExtractKeyPhrases(request.Text),
                    Sentiment = AnalyzeSentiment(request.Text),
                    MedicalEntities = ExtractMedicalEntities(request.Text),
                    Confidence = 0.87
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing enhanced text");
                return StatusCode(500, new { message = "Internal server error during text analysis" });
            }
        }

        private object GenerateAnalysisReport(AnalysisRecord analysis)
        {
            return new
            {
                analysisId = analysis.Id,
                patientId = analysis.PatientId,
                patientName = analysis.PatientName,
                analysisType = analysis.AnalysisType,
                status = analysis.Status,
                department = analysis.Department,
                createdAt = analysis.CreatedAt,
                completedAt = analysis.CompletedAt,
                results = analysis.Results,
                confidence = analysis.Confidence,
                recommendations = analysis.Recommendations,
                notes = analysis.Notes,
                generatedAt = DateTime.UtcNow
            };
        }

        private string GenerateTextSummary(string text)
        {
            // Simple text summarization - in production, use proper NLP models
            var sentences = text.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (sentences.Length <= 2)
                return text;

            return string.Join(". ", sentences.Take(2)) + ".";
        }

        private List<string> ExtractKeyPhrases(string text)
        {
            // Simple keyword extraction - in production, use proper NLP models
            var words = text.ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 4)
                .Distinct()
                .Take(10)
                .ToList();

            return words;
        }

        private string AnalyzeSentiment(string text)
        {
            // Simple sentiment analysis - in production, use proper sentiment analysis models
            var positiveWords = new[] { "good", "excellent", "positive", "healthy", "normal", "stable" };
            var negativeWords = new[] { "bad", "poor", "negative", "abnormal", "critical", "severe" };

            var lowerText = text.ToLower();
            var positiveCount = positiveWords.Count(word => lowerText.Contains(word));
            var negativeCount = negativeWords.Count(word => lowerText.Contains(word));

            if (positiveCount > negativeCount) return "Positive";
            if (negativeCount > positiveCount) return "Negative";
            return "Neutral";
        }

        private List<string> ExtractMedicalEntities(string text)
        {
            // Simple medical entity extraction - in production, use proper medical NER models
            var medicalTerms = new[]
            {
                "blood pressure", "heart rate", "temperature", "respiratory rate",
                "diabetes", "hypertension", "cardiovascular", "respiratory",
                "medication", "diagnosis", "treatment", "symptoms"
            };

            var lowerText = text.ToLower();
            return medicalTerms.Where(term => lowerText.Contains(term)).ToList();
        }
    }

    // Request and Response Models
    public class CreateAnalysisRequest
    {
        public string PatientId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string AnalysisType { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Data { get; set; } // JSON string for analysis data
    }

    public class EnhancedTextRequest
    {
        public string Text { get; set; } = string.Empty;
        public string? Context { get; set; }
        public string? Language { get; set; } = "en";
    }

    public class EnhancedTextResponse
    {
        public string ProcessedText { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public List<string> KeyPhrases { get; set; } = new();
        public string Sentiment { get; set; } = string.Empty;
        public List<string> MedicalEntities { get; set; } = new();
        public double Confidence { get; set; }
    }
}
