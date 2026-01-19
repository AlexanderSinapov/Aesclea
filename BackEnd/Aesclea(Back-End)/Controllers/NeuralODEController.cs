// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov
// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Microsoft.AspNetCore.Mvc;
using Aesclea_Back_End_.AIModel.NeuralODE;
using System.ComponentModel.DataAnnotations;

namespace Aesclea_Back_End_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NeuralODEController : ControllerBase
    {
        private static NeuralODEMedicalService? _service;
        private static readonly object _lock = new object();

        /// <summary>
        /// Initialize the Neural ODE service
        /// </summary>
        private NeuralODEMedicalService GetService()
        {
            if (_service == null)
            {
                lock (_lock)
                {
                    if (_service == null)
                    {
                        _service = new NeuralODEMedicalService();
                        Console.WriteLine("✓ Neural ODE Medical Service initialized");
                        Console.WriteLine("  Architecture: Continuous-Depth Transformer with Integral Attention");
                        Console.WriteLine("  Features: Uncertainty Quantification, Adaptive Compute");
                    }
                }
            }
            return _service;
        }

        /// <summary>
        /// Analyze medical text using Neural ODE LLM
        /// POST api/neuralode/analyze
        /// </summary>
        [HttpPost("analyze")]
        public async Task<ActionResult<NeuralODEAnalysisResponse>> AnalyzeMedicalText(
            [FromBody] MedicalAnalysisRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.ClinicalText))
                {
                    return BadRequest(new { error = "Clinical text is required" });
                }

                var service = GetService();
                var result = await service.AnalyzeMedicalText(
                    request.ClinicalText, 
                    request.Category ?? DiagnosisCategory.General
                );

                return Ok(new NeuralODEAnalysisResponse
                {
                    Success = true,
                    Diagnosis = result.Diagnosis,
                    Confidence = result.Confidence,
                    Uncertainty = result.Uncertainty,
                    Interpretation = result.Interpretation,
                    Recommendations = result.Recommendations,
                    ComputeCost = result.ComputeCost,
                    ProcessingTime = result.ProcessingTime,
                    ExtractedEntities = result.ExtractedEntities.Select(e => new EntityDto
                    {
                        Text = e.Text,
                        Type = e.Type,
                        StartIndex = e.StartIndex,
                        EndIndex = e.EndIndex
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in NeuralODE analysis: {ex.Message}");
                return StatusCode(500, new { error = "Analysis failed", details = ex.Message });
            }
        }

        /// <summary>
        /// Analyze patient symptoms with detailed uncertainty breakdown
        /// POST api/neuralode/symptoms
        /// </summary>
        [HttpPost("symptoms")]
        public async Task<ActionResult<SymptomAnalysisResponse>> AnalyzeSymptoms(
            [FromBody] SymptomAnalysisRequest request)
        {
            try
            {
                if (request.Symptoms == null || !request.Symptoms.Any())
                {
                    return BadRequest(new { error = "At least one symptom is required" });
                }

                var service = GetService();
                var result = await service.AnalyzeSymptoms(
                    request.Symptoms,
                    request.VitalSigns
                );

                return Ok(new SymptomAnalysisResponse
                {
                    Success = true,
                    Symptoms = result.Symptoms,
                    OverallConfidence = result.OverallConfidence,
                    EpistemicUncertainty = result.EpistemicUncertainty,
                    AleatoricUncertainty = result.AleatoricUncertainty,
                    TotalUncertainty = result.EpistemicUncertainty + result.AleatoricUncertainty,
                    SeverityLevel = result.SeverityLevel,
                    UrgencyScore = result.UrgencyScore,
                    SymptomContributions = result.SymptomContributions,
                    SuggestedTests = result.SuggestedTests,
                    VitalSigns = result.VitalSigns
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in symptom analysis: {ex.Message}");
                return StatusCode(500, new { error = "Symptom analysis failed", details = ex.Message });
            }
        }

        /// <summary>
        /// Get model information and status
        /// GET api/neuralode/info
        /// </summary>
        [HttpGet("info")]
        public ActionResult<ModelInfoResponse> GetModelInfo()
        {
            try
            {
                var service = GetService();
                
                return Ok(new ModelInfoResponse
                {
                    ModelType = "Neural ODE Language Model",
                    Architecture = new Dictionary<string, string>
                    {
                        { "Type", "Continuous-Depth Transformer" },
                        { "Attention", "Integral Attention with Gaussian Mixture Kernels" },
                        { "Uncertainty", "Bayesian Neural ODE with KL Regularization" },
                        { "Compute", "Adaptive Compute Control" },
                        { "Solver", "4th-order Runge-Kutta (RK4)" }
                    },
                    Features = new List<string>
                    {
                        "Continuous-depth processing",
                        "Multi-scale temporal attention",
                        "Epistemic & aleatoric uncertainty quantification",
                        "Adaptive compute allocation",
                        "FFT-optimized attention for long sequences",
                        "Medical entity extraction",
                        "Confidence-calibrated predictions"
                    },
                    IsTrained = service.IsModelTrained,
                    Status = service.IsModelTrained ? "Ready" : "Untrained - requires training data"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting model info: {ex.Message}");
                return StatusCode(500, new { error = "Failed to retrieve model info", details = ex.Message });
            }
        }

        /// <summary>
        /// Train the Neural ODE model
        /// POST api/neuralode/train
        /// </summary>
        [HttpPost("train")]
        public async Task<ActionResult<TrainingResponse>> TrainModel(
            [FromBody] TrainingRequest request)
        {
            try
            {
                if (request.TrainingData == null || !request.TrainingData.Any())
                {
                    return BadRequest(new { error = "Training data is required" });
                }

                var service = GetService();
                
                var trainingExamples = request.TrainingData.Select(td => new MedicalTrainingExample
                {
                    ClinicalText = td.ClinicalText,
                    DiagnosisClass = td.DiagnosisClass,
                    Metadata = td.Metadata
                }).ToList();

                await service.TrainModel(
                    trainingExamples,
                    request.Epochs ?? 10,
                    request.BatchSize ?? 32
                );

                return Ok(new TrainingResponse
                {
                    Success = true,
                    Message = $"Model trained successfully on {trainingExamples.Count} examples",
                    Epochs = request.Epochs ?? 10,
                    BatchSize = request.BatchSize ?? 32
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error training model: {ex.Message}");
                return StatusCode(500, new { error = "Training failed", details = ex.Message });
            }
        }

        /// <summary>
        /// Evaluate the model on test data
        /// POST api/neuralode/evaluate
        /// </summary>
        [HttpPost("evaluate")]
        public async Task<ActionResult<EvaluationResponse>> EvaluateModel(
            [FromBody] EvaluationRequest request)
        {
            try
            {
                if (request.TestData == null || !request.TestData.Any())
                {
                    return BadRequest(new { error = "Test data is required" });
                }

                var service = GetService();
                
                var testExamples = request.TestData.Select(td => new MedicalTrainingExample
                {
                    ClinicalText = td.ClinicalText,
                    DiagnosisClass = td.DiagnosisClass,
                    Metadata = td.Metadata
                }).ToList();

                var result = await service.EvaluateModel(testExamples);

                return Ok(new EvaluationResponse
                {
                    Success = true,
                    Accuracy = result.Accuracy,
                    AverageLoss = result.AverageLoss,
                    AverageUncertainty = result.AverageUncertainty,
                    AverageComputeCost = result.AverageComputeCost,
                    TotalSamples = result.TotalSamples
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error evaluating model: {ex.Message}");
                return StatusCode(500, new { error = "Evaluation failed", details = ex.Message });
            }
        }

        /// <summary>
        /// Batch analyze multiple medical texts
        /// POST api/neuralode/batch-analyze
        /// </summary>
        [HttpPost("batch-analyze")]
        public async Task<ActionResult<NeuralODEBatchResponse>> BatchAnalyze(
            [FromBody] BatchAnalysisRequest request)
        {
            try
            {
                if (request.ClinicalTexts == null || !request.ClinicalTexts.Any())
                {
                    return BadRequest(new { error = "At least one clinical text is required" });
                }

                var service = GetService();
                var results = new List<NeuralODEAnalysisResponse>();

                foreach (var text in request.ClinicalTexts)
                {
                    var result = await service.AnalyzeMedicalText(text, DiagnosisCategory.General);
                    
                    results.Add(new NeuralODEAnalysisResponse
                    {
                        Success = true,
                        Diagnosis = result.Diagnosis,
                        Confidence = result.Confidence,
                        Uncertainty = result.Uncertainty,
                        Interpretation = result.Interpretation,
                        Recommendations = result.Recommendations,
                        ComputeCost = result.ComputeCost
                    });
                }

                return Ok(new NeuralODEBatchResponse
                {
                    Success = true,
                    Results = results,
                    TotalProcessed = results.Count,
                    AverageConfidence = results.Average(r => r.Confidence),
                    AverageUncertainty = results.Average(r => r.Uncertainty)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in batch analysis: {ex.Message}");
                return StatusCode(500, new { error = "Batch analysis failed", details = ex.Message });
            }
        }
    }

    #region DTOs (Data Transfer Objects)

    public class MedicalAnalysisRequest
    {
        [Required]
        public string ClinicalText { get; set; }
        public DiagnosisCategory? Category { get; set; }
    }

    public class NeuralODEAnalysisResponse
    {
        public bool Success { get; set; }
        public string Diagnosis { get; set; }
        public double Confidence { get; set; }
        public double Uncertainty { get; set; }
        public string Interpretation { get; set; }
        public List<string> Recommendations { get; set; }
        public double ComputeCost { get; set; }
        public double ProcessingTime { get; set; }
        public List<EntityDto> ExtractedEntities { get; set; }
    }

    public class SymptomAnalysisRequest
    {
        [Required]
        public List<string> Symptoms { get; set; }
        public Dictionary<string, double> VitalSigns { get; set; }
    }

    public class SymptomAnalysisResponse
    {
        public bool Success { get; set; }
        public List<string> Symptoms { get; set; }
        public double OverallConfidence { get; set; }
        public double EpistemicUncertainty { get; set; }
        public double AleatoricUncertainty { get; set; }
        public double TotalUncertainty { get; set; }
        public string SeverityLevel { get; set; }
        public double UrgencyScore { get; set; }
        public Dictionary<string, double> SymptomContributions { get; set; }
        public List<string> SuggestedTests { get; set; }
        public Dictionary<string, double> VitalSigns { get; set; }
    }

    public class ModelInfoResponse
    {
        public string ModelType { get; set; }
        public Dictionary<string, string> Architecture { get; set; }
        public List<string> Features { get; set; }
        public bool IsTrained { get; set; }
        public string Status { get; set; }
    }

    public class TrainingRequest
    {
        [Required]
        public List<TrainingDataDto> TrainingData { get; set; }
        public int? Epochs { get; set; }
        public int? BatchSize { get; set; }
    }

    public class TrainingDataDto
    {
        [Required]
        public string ClinicalText { get; set; }
        [Required]
        public int DiagnosisClass { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class TrainingResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Epochs { get; set; }
        public int BatchSize { get; set; }
    }

    public class EvaluationRequest
    {
        [Required]
        public List<TrainingDataDto> TestData { get; set; }
    }

    public class EvaluationResponse
    {
        public bool Success { get; set; }
        public double Accuracy { get; set; }
        public double AverageLoss { get; set; }
        public double AverageUncertainty { get; set; }
        public double AverageComputeCost { get; set; }
        public int TotalSamples { get; set; }
    }

    public class BatchAnalysisRequest
    {
        [Required]
        public List<string> ClinicalTexts { get; set; }
    }

    public class NeuralODEBatchResponse
    {
        public bool Success { get; set; }
        public List<NeuralODEAnalysisResponse> Results { get; set; }
        public int TotalProcessed { get; set; }
        public double AverageConfidence { get; set; }
        public double AverageUncertainty { get; set; }
    }

    public class EntityDto
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }

    #endregion
}
