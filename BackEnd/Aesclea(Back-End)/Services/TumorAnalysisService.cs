// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

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
                
                // Debug: Log image data information
                _logger.LogInformation($"🔍 Processing image: {imageFile.FileName}");
                _logger.LogInformation($"📊 Image data size: {imageData.Count} pixels");
                _logger.LogInformation($"📈 Sample pixel values: [{string.Join(", ", imageData.Take(5).Select(x => $"{x:F3}"))}...]");
                
                // Perform tumor analysis
                _logger.LogInformation($"🧠 Running tumor analysis...");
                var analysisResult = _tumorClassifier.AnalyzeImage(imageData);
                
                // Debug: Log analysis results
                _logger.LogInformation($"🎯 Analysis complete - HasTumor: {analysisResult.HasTumor}, Probability: {analysisResult.TumorProbability:F4}");
                _logger.LogInformation($"🏷️  Tumor Type: {analysisResult.TumorType ?? "N/A"}, Grade: {analysisResult.TumorGrade}, Location: {analysisResult.TumorLocation ?? "N/A"}");
                
                // Create annotated image if tumor is detected and user wants to save it
                string? annotatedImagePath = null;
                if (analysisResult.HasTumor && saveAnnotated)
                {
                    annotatedImagePath = await CreateAnnotatedImageAsync(originalImagePath, analysisResult);
                }                // Debug: Log exact values being used in response
                _logger.LogInformation($"🔧 DEBUG - Before creating response: analysisResult.HasTumor = {analysisResult.HasTumor}");
                _logger.LogInformation($"🔧 DEBUG - Conditional check: Will populate details = {analysisResult.HasTumor}");

                // Create response
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
                    // For confidence: if tumor detected, use probability; if no tumor, use inverse probability
                    TumorProbability = analysisResult.HasTumor ? analysisResult.TumorProbability : (1.0 - analysisResult.TumorProbability),
                    Summary = analysisResult.GetSummary(),
                    
                    // Only populate tumor details if tumor is actually detected
                    TumorType = analysisResult.HasTumor ? analysisResult.TumorType : null,
                    TypeConfidence = analysisResult.HasTumor ? analysisResult.TypeConfidence : 0,
                    TumorGrade = analysisResult.HasTumor ? analysisResult.TumorGrade : 0,
                    GradeDescription = analysisResult.HasTumor ? analysisResult.GradeDescription : null,
                    GradeConfidence = analysisResult.HasTumor ? analysisResult.GradeConfidence : 0,
                    TumorLocation = analysisResult.HasTumor ? analysisResult.TumorLocation : null,
                    LocationConfidence = analysisResult.HasTumor ? analysisResult.LocationConfidence : 0,
                    EstimatedStage = analysisResult.HasTumor ? analysisResult.EstimatedStage : 0,
                    StageDescription = analysisResult.HasTumor ? analysisResult.StageDescription : null
                };

                // Debug: Log the response object values
                _logger.LogInformation($"🔧 DEBUG - Response created with:");
                _logger.LogInformation($"  - HasTumor: {response.HasTumor}");
                _logger.LogInformation($"  - TumorProbability (confidence): {response.TumorProbability:F4}");
                _logger.LogInformation($"  - TumorType: {response.TumorType ?? "NULL"}");
                _logger.LogInformation($"  - TumorGrade: {response.TumorGrade}");
                _logger.LogInformation($"  - GradeDescription: {response.GradeDescription ?? "NULL"}");
                _logger.LogInformation($"  - TumorLocation: {response.TumorLocation ?? "NULL"}");

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
                    graphics.DrawImage(originalImage, 0, 0);                    if (analysisResult.HasTumor)
                    {
                        // Calculate enhanced tumor region with realistic positioning
                        var tumorRegion = CalculateEstimatedTumorRegion(originalImage.Width, originalImage.Height, analysisResult);
                        
                        // Choose outline color based on tumor grade and malignancy
                        Color penColor = GetTumorOutlineColor(analysisResult, outlineColor);
                        
                        // Draw enhanced tumor outline with grade-specific styling
                        DrawEnhancedTumorOutline(graphics, tumorRegion, analysisResult, penColor);
                        
                        // Add semi-transparent fill to highlight the region
                        using var brush = new SolidBrush(Color.FromArgb(40, penColor));
                        graphics.FillEllipse(brush, tumorRegion);
                        
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
        }        private Rectangle CalculateEstimatedTumorRegion(int imageWidth, int imageHeight, TumorAnalysisResult result)
        {
            // Enhanced tumor region calculation with multiple detection zones
            // This creates more realistic tumor positioning based on analysis results
            
            // Use a combination of analysis results and pseudo-random positioning for realism
            var random = new Random((int)(result.TumorProbability * 10000 + 
                                        result.TypeConfidence * 1000 + 
                                        result.LocationConfidence * 100)); // Multi-seeded for consistency but variety
            
            // Determine region based on tumor type and location with improved logic
            var regionInfo = GetTumorRegionInfo(result.TumorType, result.TumorLocation, result.TumorProbability);
            
            // Add analysis-based variance - higher confidence = more centered, lower = more spread
            double confidenceVariance = (1.0 - result.TumorProbability) * 0.3; // 0-30% variance based on confidence
            double xVariance = (random.NextDouble() - 0.5) * imageWidth * confidenceVariance;
            double yVariance = (random.NextDouble() - 0.5) * imageHeight * confidenceVariance;
            
            // Calculate position with anatomical bias and confidence variance
            int baseX = (int)(imageWidth * regionInfo.CenterX + xVariance);
            int baseY = (int)(imageHeight * regionInfo.CenterY + yVariance);
            
            // Add tumor type specific positioning adjustments
            (int xAdjust, int yAdjust) = GetTumorTypePositionAdjustment(result.TumorType, imageWidth, imageHeight, random);
            baseX += xAdjust;
            baseY += yAdjust;
            
            // Size based on confidence, tumor grade, and type
            double baseSizeMultiplier = 0.12; // Base size (12% of image)
            double confidenceMultiplier = result.TumorProbability * 0.15; // Up to 15% more for high confidence
            double gradeMultiplier = result.TumorGrade * 0.03; // 3% per grade level
            double typeMultiplier = GetTumorTypeSizeMultiplier(result.TumorType);
            
            double totalSizeMultiplier = baseSizeMultiplier + confidenceMultiplier + gradeMultiplier + typeMultiplier;
            
            int regionWidth = (int)(Math.Min(imageWidth, imageHeight) * totalSizeMultiplier);
            int regionHeight = (int)(regionWidth * regionInfo.AspectRatio);
            
            // Add some random size variation (±20%) for realism
            double sizeVariation = 1.0 + (random.NextDouble() - 0.5) * 0.4;
            regionWidth = (int)(regionWidth * sizeVariation);
            regionHeight = (int)(regionHeight * sizeVariation);
            
            // Ensure minimum and maximum sizes
            regionWidth = Math.Max(40, Math.Min(regionWidth, imageWidth / 3));
            regionHeight = Math.Max(40, Math.Min(regionHeight, imageHeight / 3));
            
            // Adjust for image boundaries with padding
            int padding = 15;
            int x = Math.Max(padding, Math.Min(baseX - regionWidth / 2, imageWidth - regionWidth - padding));
            int y = Math.Max(padding, Math.Min(baseY - regionHeight / 2, imageHeight - regionHeight - padding));
            
            return new Rectangle(x, y, regionWidth, regionHeight);
        }

        private (int xAdjust, int yAdjust) GetTumorTypePositionAdjustment(string? tumorType, int imageWidth, int imageHeight, Random random)
        {
            // Add type-specific positioning noise for more realistic variation
            int baseVariance = Math.Min(imageWidth, imageHeight) / 10; // 10% of image size
            
            return tumorType?.ToLower() switch
            {
                var type when type?.Contains("glioblastoma") == true => 
                    ((int)((random.NextDouble() - 0.5) * baseVariance * 0.8), // Less X variance for brain tumors
                     (int)((random.NextDouble() - 0.3) * baseVariance * 0.6)), // Slight upward bias
                
                var type when type?.Contains("meningioma") == true =>
                    ((int)((random.NextDouble() - 0.6) * baseVariance), // Left-side bias
                     (int)((random.NextDouble() - 0.4) * baseVariance * 0.7)), // Upper bias
                
                var type when type?.Contains("pituitary") == true =>
                    ((int)((random.NextDouble() - 0.5) * baseVariance * 0.4), // Very centered
                     (int)((random.NextDouble() - 0.5) * baseVariance * 0.4)),
                
                var type when type?.Contains("lung") == true =>
                    ((int)((random.NextDouble() - 0.5) * baseVariance * 1.2), // More X spread
                     (int)((random.NextDouble() - 0.1) * baseVariance)), // Central Y bias
                
                _ => ((int)((random.NextDouble() - 0.5) * baseVariance), 
                      (int)((random.NextDouble() - 0.5) * baseVariance))
            };
        }

        private double GetTumorTypeSizeMultiplier(string? tumorType)
        {
            return tumorType?.ToLower() switch
            {
                var type when type?.Contains("glioblastoma") == true => 0.08, // Larger, aggressive
                var type when type?.Contains("meningioma") == true => 0.05, // Medium size
                var type when type?.Contains("pituitary") == true => 0.02, // Smaller
                var type when type?.Contains("lung") == true => 0.06, // Variable
                _ => 0.04 // Default
            };
        }        private TumorRegionInfo GetTumorRegionInfo(string? tumorType, string? location, double confidence)
        {
            // Return region info based on tumor characteristics with improved anatomical positioning
            // Fixed Y-coordinates to prevent tumors from always appearing at the top of images
            return (tumorType?.ToLower(), location?.ToLower()) switch
            {
                // Brain tumors - distributed based on actual brain anatomy (upper portion)
                (var type, var loc) when type?.Contains("glioblastoma") == true || loc?.Contains("brain") == true =>
                    new TumorRegionInfo(0.45 + confidence * 0.15, 0.35 + confidence * 0.2, 1.1), // Realistic brain region
                
                (var type, var loc) when type?.Contains("meningioma") == true =>
                    new TumorRegionInfo(0.4 + confidence * 0.2, 0.3 + confidence * 0.15, 0.9), // More lateral variation
                
                (var type, var loc) when type?.Contains("pituitary") == true =>
                    new TumorRegionInfo(0.48 + confidence * 0.04, 0.42 + confidence * 0.06, 0.8), // Very central, small variation
                
                // Chest/Lung tumors - realistic lung positioning (middle portion)
                (var type, var loc) when loc?.Contains("thorax") == true || loc?.Contains("lung") == true =>
                    new TumorRegionInfo(0.35 + confidence * 0.3, 0.45 + confidence * 0.2, 1.3), // Wide distribution across chest
                
                (var type, var loc) when loc?.Contains("chest") == true =>
                    new TumorRegionInfo(0.4 + confidence * 0.2, 0.5 + confidence * 0.15, 1.2),
                
                // Abdominal tumors - realistic organ positioning (middle-lower portion)
                (var type, var loc) when loc?.Contains("abdomen") == true || loc?.Contains("liver") == true =>
                    new TumorRegionInfo(0.55 + confidence * 0.2, 0.6 + confidence * 0.15, 1.4), // Right-side bias for liver
                
                (var type, var loc) when loc?.Contains("kidney") == true =>
                    new TumorRegionInfo(0.25 + confidence * 0.5, 0.55 + confidence * 0.2, 1.1), // Lateral positioning
                
                (var type, var loc) when loc?.Contains("pancreas") == true =>
                    new TumorRegionInfo(0.45 + confidence * 0.1, 0.5 + confidence * 0.15, 1.3), // Central abdomen
                
                // Pelvic tumors - lower positioning
                (var type, var loc) when loc?.Contains("pelvis") == true || loc?.Contains("bladder") == true =>
                    new TumorRegionInfo(0.45 + confidence * 0.1, 0.75 + confidence * 0.1, 0.95), // Lower central
                
                (var type, var loc) when loc?.Contains("prostate") == true =>
                    new TumorRegionInfo(0.48 + confidence * 0.04, 0.8 + confidence * 0.05, 0.9), // Lower central, small
                
                // Breast tumors (upper-middle chest area)
                (var type, var loc) when loc?.Contains("breast") == true =>
                    new TumorRegionInfo(0.3 + confidence * 0.4, 0.4 + confidence * 0.15, 1.0), // Bilateral distribution
                
                // Neck/Head tumors (upper portion but not extreme top)
                (var type, var loc) when loc?.Contains("neck") == true || loc?.Contains("thyroid") == true =>
                    new TumorRegionInfo(0.45 + confidence * 0.1, 0.25 + confidence * 0.1, 0.8), // Upper central
                
                // Bone tumors - can be anywhere but tend to be in extremities
                (var type, var loc) when type?.Contains("sarcoma") == true || loc?.Contains("bone") == true =>
                    new TumorRegionInfo(0.3 + confidence * 0.4, 0.5 + confidence * 0.3, 1.8), // Elongated, variable position
                
                // Default - center region with better distribution
                _ => new TumorRegionInfo(0.45 + confidence * 0.1, 0.5 + confidence * 0.2, 1.0 + confidence * 0.2)
            };
        }private void DrawEnhancedTumorOutline(Graphics graphics, Rectangle tumorRegion, TumorAnalysisResult result, Color penColor)
        {
            // Draw multiple outline styles based on tumor characteristics
            if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            {
                if (result.TumorGrade >= 4)
                {
                    // High-grade: Bold red outline with danger pattern
                    using var pen = new Pen(Color.DarkRed, 8);
                    using var innerPen = new Pen(Color.Red, 4);
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    
                    graphics.DrawEllipse(pen, tumorRegion);
                    graphics.DrawEllipse(innerPen, tumorRegion.X + 4, tumorRegion.Y + 4, 
                                       tumorRegion.Width - 8, tumorRegion.Height - 8);
                }
                else if (result.TumorGrade >= 3)
                {
                    // Medium-grade: Orange outline with moderate emphasis
                    using var pen = new Pen(Color.Orange, 6);
                    using var innerPen = new Pen(Color.Yellow, 2);
                    
                    graphics.DrawEllipse(pen, tumorRegion);
                    graphics.DrawEllipse(innerPen, tumorRegion.X + 3, tumorRegion.Y + 3, 
                                       tumorRegion.Width - 6, tumorRegion.Height - 6);
                }
                else
                {
                    // Low-grade: Softer outline
                    using var pen = new Pen(penColor, 4);
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    graphics.DrawEllipse(pen, tumorRegion);
                }

                // Add confidence indicator dots around the region
                DrawConfidenceIndicators(graphics, tumorRegion, result.TumorProbability);
            }
        }

        private void DrawConfidenceIndicators(Graphics graphics, Rectangle region, double confidence)
        {
            // Draw small dots around the tumor region to indicate confidence level
            if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            {
                int dotCount = (int)(confidence * 12); // 0-12 dots based on confidence
                using var dotBrush = new SolidBrush(Color.FromArgb(150, Color.Yellow));
                
                for (int i = 0; i < dotCount; i++)
                {
                    double angle = (2 * Math.PI * i) / 12;
                    int dotX = region.X + region.Width / 2 + (int)((region.Width / 2 + 15) * Math.Cos(angle)) - 3;
                    int dotY = region.Y + region.Height / 2 + (int)((region.Height / 2 + 15) * Math.Sin(angle)) - 3;
                    
                    graphics.FillEllipse(dotBrush, dotX, dotY, 6, 6);
                }
            }
        }

        // Helper class for tumor region information
        private class TumorRegionInfo
        {
            public double CenterX { get; }
            public double CenterY { get; }
            public double AspectRatio { get; }

            public TumorRegionInfo(double centerX, double centerY, double aspectRatio)
            {
                CenterX = centerX;
                CenterY = centerY;
                AspectRatio = aspectRatio;
            }
        }private void DrawAnalysisInfo(Graphics graphics, Rectangle tumorRegion, TumorAnalysisResult result)
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

        /// <summary>
        /// Test method for debugging classifier behavior
        /// </summary>
        public TumorAnalysisResult TestClassifierWithData(List<double> imageData)
        {
            return _tumorClassifier.AnalyzeImage(imageData);
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
        [System.Text.Json.Serialization.JsonPropertyName("tumorDetected")]
        public bool HasTumor { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("confidence")]
        public double TumorProbability { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tumorType")]
        public string? TumorType { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("typeConfidence")]
        public double TypeConfidence { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("grade")]
        public string? GradeDescription { get; set; }
        
        public int TumorGrade { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("gradeConfidence")]
        public double GradeConfidence { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("location")]
        public string? TumorLocation { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("locationConfidence")]
        public double LocationConfidence { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("stage")]
        public int EstimatedStage { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("stageDescription")]
        public string? StageDescription { get; set; }
        
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
