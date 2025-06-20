using Aesclea_Back_End_.AIModel;
using Aesclea_Back_End_.AIModel.Helpers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.AspNetCore.Http;

namespace Aesclea_Back_End_.Services
{
    public class TumorAnalysisService
    {
        private readonly TumorClassifier _tumorClassifier;
        private readonly ImageProcessingService _imageProcessingService;
        private readonly FileService _fileService;
        private readonly ILogger<TumorAnalysisService> _logger;
        private readonly List<TumorAnalysisResponse> _analysisHistory;

        public TumorAnalysisService(
            TumorClassifier tumorClassifier,
            ImageProcessingService imageProcessingService,
            FileService fileService,
            ILogger<TumorAnalysisService> logger)
        {
            _tumorClassifier = tumorClassifier;
            _imageProcessingService = imageProcessingService;
            _fileService = fileService;
            _logger = logger;
            _analysisHistory = new List<TumorAnalysisResponse>();
        }        public async Task<TumorAnalysisResponse> AnalyzeImageAsync(IFormFile imageFile, bool saveAnnotated = true)
        {
            try
            {
                // Save the uploaded image
                var originalImagePath = await _fileService.SaveUploadedImageAsync(imageFile);
                
                // Process image for AI analysis
                var imageData = await _imageProcessingService.ProcessImageForAnalysisAsync(originalImagePath);
                
                // Perform tumor analysis
                var analysisResult = _tumorClassifier.AnalyzeImage(imageData);
                
                // Create annotated image if tumor is detected and user wants to save it
                string? annotatedImagePath = null;
                if (analysisResult.HasTumor && saveAnnotated)
                {
                    annotatedImagePath = await CreateAnnotatedImageAsync(originalImagePath, analysisResult);
                }                // Create response
                var response = new TumorAnalysisResponse
                {
                    Id = Guid.NewGuid().ToString(),
                    AnalysisTimestamp = DateTime.UtcNow,
                    OriginalFileName = imageFile.FileName,
                    OriginalImagePath = originalImagePath,
                    AnnotatedImagePath = annotatedImagePath,
                    AnnotatedFileName = annotatedImagePath != null ? Path.GetFileName(annotatedImagePath) : null,
                    CanCreateAnnotatedImage = analysisResult.HasTumor, // Can create if tumor is detected
                    
                    // Analysis results
                    HasTumor = analysisResult.HasTumor,
                    TumorProbability = analysisResult.TumorProbability,
                    TumorType = analysisResult.TumorType,
                    TypeConfidence = analysisResult.TypeConfidence,
                    TumorGrade = analysisResult.TumorGrade,
                    GradeDescription = analysisResult.GradeDescription,
                    GradeConfidence = analysisResult.GradeConfidence,
                    TumorLocation = analysisResult.TumorLocation,
                    LocationConfidence = analysisResult.LocationConfidence,
                    EstimatedStage = analysisResult.EstimatedStage,
                    StageDescription = analysisResult.StageDescription,
                    Summary = analysisResult.GetSummary()
                };

                // Add to history
                _analysisHistory.Add(response);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in tumor analysis service");
                throw;
            }
        }        private async Task<string> CreateAnnotatedImageAsync(string originalImagePath, TumorAnalysisResult analysisResult, string outlineColor = "Red")
        {
            try
            {
                if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                {
                    using var originalImage = Image.FromFile(originalImagePath);
                    using var annotatedImage = new Bitmap(originalImage.Width, originalImage.Height);
                    using var graphics = Graphics.FromImage(annotatedImage);
                    
                    // Set high quality rendering
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    
                    // Draw the original image
                    graphics.DrawImage(originalImage, 0, 0);

                    if (analysisResult.HasTumor)
                    {
                        // Calculate approximate tumor region (this is simplified - in real implementation, 
                        // you'd need actual bounding box coordinates from your AI model)
                        var tumorRegion = CalculateEstimatedTumorRegion(originalImage.Width, originalImage.Height, analysisResult);
                        
                        // Choose outline color based on tumor grade and malignancy
                        Color penColor = GetTumorOutlineColor(analysisResult, outlineColor);
                        
                        // Draw bold outline around estimated tumor area
                        using var pen = new Pen(penColor, 6);
                        graphics.DrawRectangle(pen, tumorRegion);
                        
                        // Add semi-transparent fill to highlight the region
                        using var brush = new SolidBrush(Color.FromArgb(60, penColor));
                        graphics.FillRectangle(brush, tumorRegion);
                        
                        // Draw confidence and type information
                        DrawAnalysisInfo(graphics, tumorRegion, analysisResult);
                    }

                    // Save annotated image
                    var annotatedPath = _fileService.GetAnnotatedImagePath(originalImagePath);
                    annotatedImage.Save(annotatedPath, ImageFormat.Png);
                    
                    return annotatedPath;
                }
                else
                {
                    // For non-Windows platforms, just copy the original file and return it
                    var annotatedPath = _fileService.GetAnnotatedImagePath(originalImagePath);
                    using var sourceStream = File.OpenRead(originalImagePath);
                    using var destStream = File.Create(annotatedPath);
                    await sourceStream.CopyToAsync(destStream);
                    return annotatedPath;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating annotated image");
                throw;
            }
        }

        private Rectangle CalculateEstimatedTumorRegion(int imageWidth, int imageHeight, TumorAnalysisResult result)
        {
            // This is a simplified approach - in a real implementation, your AI model should provide
            // actual bounding box coordinates. For now, we'll estimate based on confidence and type.
            
            int centerX = imageWidth / 2;
            int centerY = imageHeight / 2;
            
            // Size based on confidence (higher confidence = larger detected area)
            int regionSize = (int)(Math.Min(imageWidth, imageHeight) * 0.3 * result.TumorProbability);
            
            // Offset based on tumor location (simplified)
            var locationOffset = GetLocationOffset(result.TumorLocation, imageWidth, imageHeight);
            
            int x = Math.Max(0, centerX + locationOffset.X - regionSize / 2);
            int y = Math.Max(0, centerY + locationOffset.Y - regionSize / 2);
            int width = Math.Min(regionSize, imageWidth - x);
            int height = Math.Min(regionSize, imageHeight - y);
            
            return new Rectangle(x, y, width, height);
        }

        private Point GetLocationOffset(string location, int imageWidth, int imageHeight)
        {
            // Simple location-based offset calculation
            return location switch
            {
                "Brain & CNS" => new Point(0, -imageHeight / 4),
                "Head & Neck" => new Point(0, -imageHeight / 6),
                "Thorax" => new Point(0, -imageHeight / 8),
                "Abdomen" => new Point(0, imageHeight / 8),
                "Pelvis" => new Point(0, imageHeight / 4),
                _ => new Point(0, 0)
            };
        }        private void DrawAnalysisInfo(Graphics graphics, Rectangle tumorRegion, TumorAnalysisResult result)
        {
            if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            {
                var font = new Font("Arial", 14, FontStyle.Bold);
                var textBrush = new SolidBrush(Color.White);
                var backgroundBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                var borderPen = new Pen(Color.White, 2);
                
                var lines = new[]
                {
                    $"TUMOR DETECTED: {result.TumorProbability:P1}",
                    $"Type: {result.TumorType}",
                    $"Grade: {result.TumorGrade} ({result.GradeDescription})",
                    $"Location: {result.TumorLocation}",
                    $"Stage: {result.EstimatedStage} - {result.StageDescription}"
                };
                
                int lineHeight = 22;
                int padding = 10;
                int boxWidth = 320;
                int boxHeight = lines.Length * lineHeight + padding * 2;
                
                // Position the info box - try above the tumor region first
                int infoX = Math.Max(5, tumorRegion.X);
                int infoY = tumorRegion.Y - boxHeight - 15;
                
                // Adjust if it goes off screen
                if (infoY < 5) 
                    infoY = tumorRegion.Bottom + 15;
                if (infoX + boxWidth > graphics.ClipBounds.Width - 5) 
                    infoX = Math.Max(5, (int)graphics.ClipBounds.Width - boxWidth - 5);
                if (infoY + boxHeight > graphics.ClipBounds.Height - 5)
                    infoY = Math.Max(5, (int)graphics.ClipBounds.Height - boxHeight - 5);
                
                // Draw background with border
                graphics.FillRectangle(backgroundBrush, infoX, infoY, boxWidth, boxHeight);
                graphics.DrawRectangle(borderPen, infoX, infoY, boxWidth, boxHeight);
                
                // Draw text with better positioning
                for (int i = 0; i < lines.Length; i++)
                {
                    graphics.DrawString(lines[i], font, textBrush, infoX + padding, infoY + padding + i * lineHeight);
                }
                
                // Dispose of resources
                font.Dispose();
                textBrush.Dispose();
                backgroundBrush.Dispose();
                borderPen.Dispose();
            }
        }

        public async Task<FileDownloadResult?> GetAnnotatedImageAsync(string fileName)
        {
            try
            {
                var filePath = _fileService.GetAnnotatedImageFullPath(fileName);
                
                if (!File.Exists(filePath))
                    return null;

                var imageBytes = await File.ReadAllBytesAsync(filePath);
                var contentType = GetContentType(fileName);
                
                return new FileDownloadResult
                {
                    ImageBytes = imageBytes,
                    ContentType = contentType,
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving annotated image");
                throw;
            }
        }

        public async Task TrainModelAsync(TrainModelRequest request)
        {
            try
            {
                // Load training data from CSV files
                var trainingData = await LoadTrainingDataAsync(request.TrainingDataPath);
                
                // Train the classifier
                _tumorClassifier.TrainClassifiers(
                    trainingData.Inputs,
                    trainingData.Types,
                    trainingData.Grades,
                    trainingData.Locations,
                    request.Epochs,
                    request.LearningRate
                );
                
                // Save the trained model
                var fileHelper = new FileHelper();
                _tumorClassifier.SaveWeights(fileHelper, "tumor_classifier");
                
                _logger.LogInformation("Model training completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during model training");
                throw;
            }
        }

        public async Task<List<TumorAnalysisResponse>> GetAnalysisHistoryAsync()
        {
            return await Task.FromResult(_analysisHistory.OrderByDescending(x => x.AnalysisTimestamp).ToList());
        }        private async Task<TrainingData> LoadTrainingDataAsync(string dataPath)
        {
            // Implementation depends on your CSV format
            // This is a placeholder - you'll need to implement based on your metadata CSV files
            var trainingData = new TrainingData
            {
                Inputs = new List<List<double>>(),
                Types = new List<string>(),
                Grades = new List<int>(),
                Locations = new List<string>()
            };

            // Load from your tumor.csv and tumor-meningioma.csv files
            // Implementation needed based on your specific CSV format
            await Task.Delay(1); // Placeholder to make it truly async

            return trainingData;
        }

        public async Task<string?> CreateAnnotatedImageOnDemandAsync(string analysisId, string outlineColor = "Auto")
        {
            try
            {
                // Find the analysis result from history
                var analysis = _analysisHistory.FirstOrDefault(a => a.Id == analysisId);
                if (analysis == null)
                {
                    _logger.LogWarning($"Analysis with ID {analysisId} not found");
                    return null;
                }

                if (!analysis.HasTumor)
                {
                    _logger.LogWarning($"Analysis {analysisId} does not contain a tumor to annotate");
                    return null;
                }

                // Recreate the analysis result from the response data
                var analysisResult = new TumorAnalysisResult
                {
                    HasTumor = analysis.HasTumor,
                    TumorProbability = analysis.TumorProbability,
                    TumorType = analysis.TumorType,
                    TypeConfidence = analysis.TypeConfidence,
                    TumorGrade = analysis.TumorGrade,
                    GradeDescription = analysis.GradeDescription,
                    GradeConfidence = analysis.GradeConfidence,
                    TumorLocation = analysis.TumorLocation,
                    LocationConfidence = analysis.LocationConfidence,
                    EstimatedStage = analysis.EstimatedStage,
                    StageDescription = analysis.StageDescription
                };

                // Create new annotated image with specified color
                var annotatedPath = await CreateAnnotatedImageAsync(analysis.OriginalImagePath, analysisResult, outlineColor);
                
                // Update the analysis record with new annotated image path
                analysis.AnnotatedImagePath = annotatedPath;
                analysis.AnnotatedFileName = Path.GetFileName(annotatedPath);

                return annotatedPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating on-demand annotated image for analysis {analysisId}");
                return null;
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".bmp" => "image/bmp",
                ".tiff" => "image/tiff",
                _ => "application/octet-stream"
            };
        }

        private Color GetTumorOutlineColor(TumorAnalysisResult result, string preferredColor = "Red")
        {
            // Allow custom color preference
            if (!string.IsNullOrEmpty(preferredColor) && preferredColor != "Auto")
            {
                return Color.FromName(preferredColor);
            }
            
            // Automatic color selection based on tumor characteristics
            if (result.TumorGrade >= 4)
            {
                return Color.DarkRed; // High-grade malignant tumors - dark red
            }
            else if (result.TumorGrade >= 3)
            {
                return Color.Red; // High-grade tumors - bright red
            }
            else if (result.TumorGrade >= 2)
            {
                return Color.Orange; // Intermediate-grade tumors - orange
            }
            else
            {
                return Color.Yellow; // Low-grade tumors - yellow
            }
        }
    }    public class TumorAnalysisResponse
    {
        public string Id { get; set; } = string.Empty;
        public DateTime AnalysisTimestamp { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string OriginalImagePath { get; set; } = string.Empty;
        public string? AnnotatedImagePath { get; set; }
        public string? AnnotatedFileName { get; set; }
        public bool CanCreateAnnotatedImage { get; set; } // Indicates if an annotated image can be created/saved
        
        // Analysis results
        public bool HasTumor { get; set; }
        public double TumorProbability { get; set; }
        public string TumorType { get; set; } = string.Empty;
        public double TypeConfidence { get; set; }
        public int TumorGrade { get; set; }
        public string GradeDescription { get; set; } = string.Empty;
        public double GradeConfidence { get; set; }
        public string TumorLocation { get; set; } = string.Empty;
        public double LocationConfidence { get; set; }
        public int EstimatedStage { get; set; }
        public string StageDescription { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
    }

    public class FileDownloadResult
    {
        public byte[] ImageBytes { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
    }    public class TrainingData
    {
        public List<List<double>> Inputs { get; set; } = new();
        public List<int> Grades { get; set; } = new();
        public List<string> Locations { get; set; } = new();
        public List<string> Types { get; set; } = new();
    }

    public class TrainModelRequest
    {
        public required string TrainingDataPath { get; set; }
        public int Epochs { get; set; } = 100;
        public double LearningRate { get; set; } = 0.001;
    }

    public class FileService
    {
        private readonly string _uploadsPath;
        private readonly string _annotatedPath;
        private readonly ILogger<FileService> _logger;

        public FileService(IConfiguration configuration, ILogger<FileService> logger)
        {
            _uploadsPath = configuration["FileStorage:UploadsPath"] ?? "uploads/original";
            _annotatedPath = configuration["FileStorage:AnnotatedPath"] ?? "uploads/annotated";
            _logger = logger;
            
            // Ensure directories exist
            Directory.CreateDirectory(_uploadsPath);
            Directory.CreateDirectory(_annotatedPath);
        }

        public async Task<string> SaveUploadedImageAsync(IFormFile imageFile)
        {
            try
            {
                var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                var filePath = Path.Combine(_uploadsPath, fileName);
                
                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving uploaded image");
                throw;
            }
        }

        public string GetAnnotatedImagePath(string originalImagePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(originalImagePath);
            var annotatedFileName = $"annotated_{fileName}.png";
            return Path.Combine(_annotatedPath, annotatedFileName);
        }

        public string GetAnnotatedImageFullPath(string fileName)
        {
            return Path.Combine(_annotatedPath, fileName);
        }
    }
