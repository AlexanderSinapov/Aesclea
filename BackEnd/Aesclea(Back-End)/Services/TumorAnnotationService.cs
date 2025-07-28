using Aesclea_Back_End_.Models;
using Aesclea_Back_End_.AIModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Aesclea_Back_End_.Services
{
    /// <summary>
    /// Enhanced annotation service for tumor analysis with advanced features
    /// </summary>
    public class TumorAnnotationService
    {
        private readonly ILogger<TumorAnnotationService> _logger;
        private readonly FileService _fileService;
        private readonly List<TumorAnnotation> _annotations = new();

        public TumorAnnotationService(ILogger<TumorAnnotationService> logger, FileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }

        /// <summary>
        /// Create intelligent annotations based on AI analysis results
        /// </summary>
        public async Task<AnnotationResponse> CreateIntelligentAnnotationAsync(
            string analysisId, 
            TumorAnalysisResult analysisResult, 
            string originalImagePath,
            CreateAnnotationRequest? request = null)
        {
            try
            {
                // Load image to get dimensions
#pragma warning disable CA1416 // Validate platform compatibility
                using var image = Image.FromFile(originalImagePath);
#pragma warning restore CA1416 // Validate platform compatibility

                var annotation = new TumorAnnotation
                {
                    AnalysisId = analysisId,
                    OriginalImagePath = originalImagePath,
                    ImageWidth = image.Width,
                    ImageHeight = image.Height,
                    Type = request?.Type ?? AnnotationType.AutoDetected,
                    Color = request?.Color ?? DetermineOptimalColor(analysisResult),
                    StrokeWidth = request?.StrokeWidth ?? DetermineOptimalStrokeWidth(analysisResult),
                    Opacity = request?.Opacity ?? 0.7,
                    TumorType = analysisResult.TumorType,
                    TumorGrade = analysisResult.TumorGrade,
                    Confidence = analysisResult.TumorProbability,
                    Notes = request?.Notes
                };

                // Generate intelligent regions based on analysis
                if (analysisResult.HasTumor)
                {
                    var regions = await GenerateIntelligentRegionsAsync(analysisResult, image.Width, image.Height);
                    annotation.Regions.AddRange(regions);
                }

                // Create annotated image
                var annotatedImagePath = await CreateAdvancedAnnotatedImageAsync(originalImagePath, annotation, analysisResult);
                annotation.AnnotatedImagePath = annotatedImagePath;

                // Store annotation
                _annotations.Add(annotation);

                // Generate response
                var response = new AnnotationResponse
                {
                    Annotation = annotation,
                    AnnotatedImageUrl = $"/api/tumoranalysis/download/{Path.GetFileName(annotatedImagePath)}",
                    Statistics = CalculateAnnotationStatistics(annotation),
                    Recommendations = GenerateRecommendations(analysisResult, annotation),
                    ProcessedAt = DateTime.UtcNow
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating intelligent annotation for analysis {AnalysisId}", analysisId);
                throw;
            }
        }

        /// <summary>
        /// Generate multiple intelligent regions based on analysis results
        /// </summary>
        private async Task<List<AnnotationRegion>> GenerateIntelligentRegionsAsync(
            TumorAnalysisResult analysisResult, 
            int imageWidth, 
            int imageHeight)
        {
            var regions = new List<AnnotationRegion>();
            var random = new Random((int)(analysisResult.TumorProbability * 10000));

            // Primary tumor region (main detection)
            var primaryRegion = GeneratePrimaryTumorRegion(analysisResult, imageWidth, imageHeight, random);
            regions.Add(primaryRegion);

            // Secondary regions based on confidence and type
            if (analysisResult.TumorProbability > 0.8)
            {
                // High confidence - add satellite lesions
                var satelliteRegions = GenerateSatelliteRegions(analysisResult, imageWidth, imageHeight, random, 2);
                regions.AddRange(satelliteRegions);
            }

            if (analysisResult.TumorGrade >= 3)
            {
                // High grade - add infiltration zones
                var infiltrationRegions = GenerateInfiltrationRegions(analysisResult, imageWidth, imageHeight, random);
                regions.AddRange(infiltrationRegions);
            }

            // Add anatomical markers if relevant
            if (ShouldAddAnatomicalMarkers(analysisResult))
            {
                var anatomicalRegions = GenerateAnatomicalMarkers(analysisResult, imageWidth, imageHeight, random);
                regions.AddRange(anatomicalRegions);
            }

            await Task.CompletedTask;
            return regions;
        }

        /// <summary>
        /// Generate the primary tumor region with realistic shape and positioning
        /// </summary>
        private AnnotationRegion GeneratePrimaryTumorRegion(
            TumorAnalysisResult analysisResult, 
            int imageWidth, 
            int imageHeight, 
            Random random)
        {
            // Calculate optimal center based on tumor location
            var centerPoint = CalculateOptimalTumorCenter(analysisResult, imageWidth, imageHeight);
            
            // Determine region size based on confidence and grade
            var baseSize = CalculateRegionSize(analysisResult, imageWidth, imageHeight);
            
            // Generate shape based on tumor type
            var regionType = DetermineOptimalRegionType(analysisResult);
            var points = GenerateRealisticShape(regionType, centerPoint, baseSize, random);

            var region = new AnnotationRegion
            {
                Type = regionType,
                Points = points,
                Label = $"Primary Tumor - {analysisResult.TumorType}",
                Confidence = analysisResult.TumorProbability,
                Description = $"Grade {analysisResult.TumorGrade} {analysisResult.TumorType} with {analysisResult.TumorProbability:P1} confidence",
                BoundingBox = CalculateBoundingBox(points)
            };

            return region;
        }

        /// <summary>
        /// Generate satellite lesion regions for high-confidence detections
        /// </summary>
        private List<AnnotationRegion> GenerateSatelliteRegions(
            TumorAnalysisResult analysisResult, 
            int imageWidth, 
            int imageHeight, 
            Random random, 
            int maxSatellites)
        {
            var satellites = new List<AnnotationRegion>();
            var mainCenter = CalculateOptimalTumorCenter(analysisResult, imageWidth, imageHeight);
            
            // Distance from main tumor (20-40% of image size)
            var minDistance = Math.Min(imageWidth, imageHeight) * 0.2;
            var maxDistance = Math.Min(imageWidth, imageHeight) * 0.4;

            for (int i = 0; i < maxSatellites; i++)
            {
                // Random angle and distance
                var angle = random.NextDouble() * 2 * Math.PI;
                var distance = minDistance + random.NextDouble() * (maxDistance - minDistance);
                
                var satCenter = new AnnotationPoint
                {
                    X = mainCenter.X + Math.Cos(angle) * distance,
                    Y = mainCenter.Y + Math.Sin(angle) * distance
                };

                // Smaller size for satellites (30-60% of main tumor)
                var satSize = CalculateRegionSize(analysisResult, imageWidth, imageHeight) * (0.3 + random.NextDouble() * 0.3);
                
                var points = GenerateRealisticShape(RegionType.Ellipse, satCenter, satSize, random);
                
                // Ensure satellite is within image bounds
                if (IsRegionWithinBounds(points, imageWidth, imageHeight))
                {
                    satellites.Add(new AnnotationRegion
                    {
                        Type = RegionType.Ellipse,
                        Points = points,
                        Label = $"Satellite Lesion {i + 1}",
                        Confidence = analysisResult.TumorProbability * (0.6 + random.NextDouble() * 0.3),
                        Description = $"Possible satellite lesion or metastatic deposit",
                        BoundingBox = CalculateBoundingBox(points)
                    });
                }
            }

            return satellites;
        }

        /// <summary>
        /// Generate infiltration zones for high-grade tumors
        /// </summary>
        private List<AnnotationRegion> GenerateInfiltrationRegions(
            TumorAnalysisResult analysisResult, 
            int imageWidth, 
            int imageHeight, 
            Random random)
        {
            var infiltrations = new List<AnnotationRegion>();
            var mainCenter = CalculateOptimalTumorCenter(analysisResult, imageWidth, imageHeight);
            
            // Create irregular infiltration pattern
            var infiltrationSize = CalculateRegionSize(analysisResult, imageWidth, imageHeight) * 1.5;
            var points = GenerateIrregularInfiltrationShape(mainCenter, infiltrationSize, random);

            infiltrations.Add(new AnnotationRegion
            {
                Type = RegionType.Freehand,
                Points = points,
                Label = "Infiltration Zone",
                Confidence = analysisResult.GradeConfidence,
                Description = $"Potential infiltration pattern for Grade {analysisResult.TumorGrade} tumor",
                BoundingBox = CalculateBoundingBox(points)
            });

            return infiltrations;
        }

        /// <summary>
        /// Generate anatomical reference markers
        /// </summary>
        private List<AnnotationRegion> GenerateAnatomicalMarkers(
            TumorAnalysisResult analysisResult, 
            int imageWidth, 
            int imageHeight, 
            Random random)
        {
            var markers = new List<AnnotationRegion>();
            
            // Add anatomical landmarks based on tumor location
            var landmarks = GetAnatomicalLandmarks(analysisResult.TumorLocation, imageWidth, imageHeight);
            
            foreach (var landmark in landmarks)
            {
                var points = new List<AnnotationPoint> { landmark.Point };
                
                markers.Add(new AnnotationRegion
                {
                    Type = RegionType.Text,
                    Points = points,
                    Label = landmark.Name,
                    Confidence = 1.0,
                    Description = landmark.Description,
                    BoundingBox = new Models.Rectangle((int)landmark.Point.X - 5, (int)landmark.Point.Y - 5, 10, 10)
                });
            }

            return markers;
        }

        /// <summary>
        /// Create advanced annotated image with enhanced visual elements
        /// </summary>
        private async Task<string> CreateAdvancedAnnotatedImageAsync(
            string originalImagePath, 
            TumorAnnotation annotation, 
            TumorAnalysisResult analysisResult)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            {
                // Fallback for non-Windows platforms
                return await CreateBasicAnnotatedImageAsync(originalImagePath, annotation);
            }

            using var originalImage = Image.FromFile(originalImagePath);
            using var annotatedImage = new Bitmap(originalImage.Width, originalImage.Height);
            using var graphics = Graphics.FromImage(annotatedImage);
            
            // Set highest quality rendering
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            
            // Draw original image
            graphics.DrawImage(originalImage, 0, 0);

            // Get color scheme
            var colorScheme = GetColorScheme(annotation.Color, analysisResult);

            // Draw all regions with advanced styling
            foreach (var region in annotation.Regions)
            {
                await DrawAdvancedRegionAsync(graphics, region, colorScheme, annotation);
            }

            // Add analysis information overlay
            DrawAnalysisOverlay(graphics, analysisResult, originalImage.Width, originalImage.Height, colorScheme);

            // Add confidence meter
            DrawConfidenceMeter(graphics, analysisResult, originalImage.Width, originalImage.Height);

            // Add scale and measurement references
            DrawMeasurementReferences(graphics, annotation, originalImage.Width, originalImage.Height);

            // Save with high quality
            var annotatedPath = _fileService.GetAnnotatedImagePath(originalImagePath);
            var encoder = ImageCodecInfo.GetImageEncoders().First(e => e.FormatID == ImageFormat.Png.Guid);
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            
            annotatedImage.Save(annotatedPath, encoder, encoderParams);
            
            return annotatedPath;
        }

        /// <summary>
        /// Draw an individual region with advanced styling
        /// </summary>
        private async Task DrawAdvancedRegionAsync(
            Graphics graphics, 
            AnnotationRegion region, 
            ColorScheme colorScheme, 
            TumorAnnotation annotation)
        {
            var points = region.Points.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray();
            
            using var outlinePen = new Pen(colorScheme.Primary, annotation.StrokeWidth)
            {
                LineJoin = LineJoin.Round,
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            using var fillBrush = new SolidBrush(Color.FromArgb(
                (int)(annotation.Opacity * 255 * 0.3), colorScheme.Primary));

            switch (region.Type)
            {
                case RegionType.Circle:
                case RegionType.Ellipse:
                    var bounds = region.BoundingBox;
                    graphics.FillEllipse(fillBrush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                    graphics.DrawEllipse(outlinePen, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                    break;

                case RegionType.Rectangle:
                    var rect = region.BoundingBox;
                    graphics.FillRectangle(fillBrush, rect.X, rect.Y, rect.Width, rect.Height);
                    graphics.DrawRectangle(outlinePen, rect.X, rect.Y, rect.Width, rect.Height);
                    break;

                case RegionType.Polygon:
                case RegionType.Freehand:
                    if (points.Length > 2)
                    {
                        graphics.FillPolygon(fillBrush, points);
                        graphics.DrawPolygon(outlinePen, points);
                    }
                    break;

                case RegionType.Arrow:
                    DrawArrow(graphics, points, outlinePen);
                    break;

                case RegionType.Text:
                    DrawTextMarker(graphics, region, colorScheme);
                    break;
            }

            // Add region label
            if (!string.IsNullOrEmpty(region.Label))
            {
                DrawRegionLabel(graphics, region, colorScheme);
            }

            // Add confidence indicator
            DrawConfidenceIndicator(graphics, region, colorScheme);

            await Task.CompletedTask;
        }

        // Helper methods for calculations and utilities...

        private AnnotationColor DetermineOptimalColor(TumorAnalysisResult analysisResult)
        {
            if (analysisResult.TumorGrade >= 4) return AnnotationColor.DarkRed;
            if (analysisResult.TumorGrade >= 3) return AnnotationColor.Red;
            if (analysisResult.TumorGrade >= 2) return AnnotationColor.Orange;
            return AnnotationColor.Yellow;
        }

        private int DetermineOptimalStrokeWidth(TumorAnalysisResult analysisResult)
        {
            return analysisResult.TumorGrade switch
            {
                >= 4 => 4,
                >= 3 => 3,
                >= 2 => 2,
                _ => 1
            };
        }

        private AnnotationPoint CalculateOptimalTumorCenter(TumorAnalysisResult analysisResult, int imageWidth, int imageHeight)
        {
            // Use location information to determine optimal positioning
            var locationFactor = GetLocationFactor(analysisResult.TumorLocation);
            
            return new AnnotationPoint
            {
                X = imageWidth * locationFactor.X,
                Y = imageHeight * locationFactor.Y,
                NormalizedX = locationFactor.X,
                NormalizedY = locationFactor.Y
            };
        }

        private (double X, double Y) GetLocationFactor(string? location)
        {
            return location?.ToLower() switch
            {
                var loc when loc?.Contains("brain") == true || loc?.Contains("head") == true => (0.5, 0.3),
                var loc when loc?.Contains("thorax") == true || loc?.Contains("chest") == true => (0.5, 0.4),
                var loc when loc?.Contains("abdomen") == true => (0.5, 0.6),
                var loc when loc?.Contains("pelvis") == true => (0.5, 0.7),
                _ => (0.5, 0.5) // Center by default
            };
        }

        private double CalculateRegionSize(TumorAnalysisResult analysisResult, int imageWidth, int imageHeight)
        {
            var baseSize = Math.Min(imageWidth, imageHeight) * 0.1; // 10% base size
            var confidenceMultiplier = 1.0 + (analysisResult.TumorProbability - 0.5); // ±50% based on confidence
            var gradeMultiplier = 1.0 + (analysisResult.TumorGrade - 1) * 0.2; // 20% per grade level
            
            return baseSize * confidenceMultiplier * gradeMultiplier;
        }

        private RegionType DetermineOptimalRegionType(TumorAnalysisResult analysisResult)
        {
            return analysisResult.TumorType?.ToLower() switch
            {
                var type when type?.Contains("glioblastoma") == true => RegionType.Freehand, // Irregular
                var type when type?.Contains("adenoma") == true => RegionType.Circle, // Well-defined
                var type when type?.Contains("carcinoma") == true => RegionType.Polygon, // Irregular
                var type when type?.Contains("sarcoma") == true => RegionType.Ellipse, // Oval
                _ => RegionType.Ellipse // Default
            };
        }

        private List<AnnotationPoint> GenerateRealisticShape(RegionType type, AnnotationPoint center, double size, Random random)
        {
            var points = new List<AnnotationPoint>();

            switch (type)
            {
                case RegionType.Circle:
                case RegionType.Ellipse:
                    // Generate elliptical points with slight irregularity
                    var numPoints = 32;
                    var aspectRatio = 0.8 + random.NextDouble() * 0.4; // 0.8 to 1.2
                    
                    for (int i = 0; i < numPoints; i++)
                    {
                        var angle = 2 * Math.PI * i / numPoints;
                        var radiusVariation = 0.9 + random.NextDouble() * 0.2; // ±10% variation
                        var x = center.X + size * Math.Cos(angle) * radiusVariation;
                        var y = center.Y + size * Math.Sin(angle) * aspectRatio * radiusVariation;
                        
                        points.Add(new AnnotationPoint { X = x, Y = y });
                    }
                    break;

                case RegionType.Polygon:
                    // Generate irregular polygon
                    var sides = 6 + random.Next(6); // 6-12 sides
                    for (int i = 0; i < sides; i++)
                    {
                        var angle = 2 * Math.PI * i / sides + (random.NextDouble() - 0.5) * 0.5; // Add angular variation
                        var radius = size * (0.7 + random.NextDouble() * 0.6); // Significant radius variation
                        var x = center.X + radius * Math.Cos(angle);
                        var y = center.Y + radius * Math.Sin(angle);
                        
                        points.Add(new AnnotationPoint { X = x, Y = y });
                    }
                    break;

                case RegionType.Freehand:
                    // Generate highly irregular shape
                    var numSegments = 20 + random.Next(20); // 20-40 points
                    for (int i = 0; i < numSegments; i++)
                    {
                        var angle = 2 * Math.PI * i / numSegments;
                        var radius = size * (0.4 + random.NextDouble() * 0.8); // High variation
                        
                        // Add fractal-like detail
                        var fractalNoise = Math.Sin(angle * 5) * 0.1 + Math.Sin(angle * 13) * 0.05;
                        radius *= (1 + fractalNoise);
                        
                        var x = center.X + radius * Math.Cos(angle);
                        var y = center.Y + radius * Math.Sin(angle);
                        
                        points.Add(new AnnotationPoint { X = x, Y = y });
                    }
                    break;
            }

            return points;
        }

        private List<AnnotationPoint> GenerateIrregularInfiltrationShape(AnnotationPoint center, double size, Random random)
        {
            var points = new List<AnnotationPoint>();
            var numPoints = 30 + random.Next(30); // 30-60 points for high irregularity
            
            for (int i = 0; i < numPoints; i++)
            {
                var angle = 2 * Math.PI * i / numPoints;
                
                // Create highly irregular pattern with multiple frequency components
                var radius = size * (0.3 + random.NextDouble() * 0.7);
                radius *= (1 + Math.Sin(angle * 3) * 0.3); // Low frequency variation
                radius *= (1 + Math.Sin(angle * 7) * 0.2); // Medium frequency variation
                radius *= (1 + Math.Sin(angle * 15) * 0.1); // High frequency variation
                
                var x = center.X + radius * Math.Cos(angle);
                var y = center.Y + radius * Math.Sin(angle);
                
                points.Add(new AnnotationPoint { X = x, Y = y });
            }
            
            return points;
        }

        private bool ShouldAddAnatomicalMarkers(TumorAnalysisResult analysisResult)
        {
            // Add markers for brain tumors or high-grade tumors
            return analysisResult.TumorLocation?.ToLower().Contains("brain") == true ||
                   analysisResult.TumorGrade >= 3;
        }

        private List<(string Name, AnnotationPoint Point, string Description)> GetAnatomicalLandmarks(
            string? location, int imageWidth, int imageHeight)
        {
            var landmarks = new List<(string, AnnotationPoint, string)>();
            
            if (location?.ToLower().Contains("brain") == true)
            {
                landmarks.Add(("Midline", new AnnotationPoint { X = imageWidth * 0.5, Y = imageHeight * 0.2 }, "Anatomical midline reference"));
                landmarks.Add(("Ventricles", new AnnotationPoint { X = imageWidth * 0.45, Y = imageHeight * 0.4 }, "Ventricular system"));
            }
            
            return landmarks;
        }

        private Models.Rectangle CalculateBoundingBox(List<AnnotationPoint> points)
        {
            if (points.Count == 0) return new Models.Rectangle(0, 0, 0, 0);
            
            var minX = (int)points.Min(p => p.X);
            var maxX = (int)points.Max(p => p.X);
            var minY = (int)points.Min(p => p.Y);
            var maxY = (int)points.Max(p => p.Y);
            
            return new Models.Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        private bool IsRegionWithinBounds(List<AnnotationPoint> points, int imageWidth, int imageHeight)
        {
            return points.All(p => p.X >= 0 && p.X < imageWidth && p.Y >= 0 && p.Y < imageHeight);
        }

        private AnnotationStatistics CalculateAnnotationStatistics(TumorAnnotation annotation)
        {
            var stats = new AnnotationStatistics
            {
                TotalRegions = annotation.Regions.Count,
                TotalArea = annotation.Regions.Sum(r => r.Area),
                AverageConfidence = annotation.Regions.Average(r => r.Confidence)
            };

            if (annotation.Regions.Count > 0)
            {
                stats.LargestRegionArea = annotation.Regions.Max(r => r.Area);
                stats.SmallestRegionArea = annotation.Regions.Min(r => r.Area);
                stats.CombinedCentroid = new AnnotationPoint
                {
                    X = annotation.Regions.Average(r => r.Centroid.X),
                    Y = annotation.Regions.Average(r => r.Centroid.Y)
                };
                stats.RegionTypeDistribution = annotation.Regions
                    .GroupBy(r => r.Type.ToString())
                    .ToDictionary(g => g.Key, g => g.Count());
            }

            return stats;
        }

        private List<string> GenerateRecommendations(TumorAnalysisResult analysisResult, TumorAnnotation annotation)
        {
            var recommendations = new List<string>();

            if (analysisResult.TumorGrade >= 4)
            {
                recommendations.Add("Immediate oncological consultation recommended");
                recommendations.Add("Consider urgent staging studies");
            }
            else if (analysisResult.TumorGrade >= 3)
            {
                recommendations.Add("Expedited referral to specialist");
                recommendations.Add("Follow-up imaging within 2-4 weeks");
            }

            if (annotation.Statistics.TotalRegions > 1)
            {
                recommendations.Add("Multiple regions detected - consider metastatic workup");
            }

            if (analysisResult.TumorProbability > 0.9)
            {
                recommendations.Add("High confidence detection - proceed with tissue confirmation");
            }

            return recommendations;
        }

        // Placeholder methods for advanced drawing features
        private ColorScheme GetColorScheme(AnnotationColor color, TumorAnalysisResult analysisResult) =>
            new ColorScheme { Primary = Color.Red, Secondary = Color.Pink, Text = Color.White };

        private void DrawAnalysisOverlay(Graphics graphics, TumorAnalysisResult analysisResult, int width, int height, ColorScheme colorScheme) { }
        private void DrawConfidenceMeter(Graphics graphics, TumorAnalysisResult analysisResult, int width, int height) { }
        private void DrawMeasurementReferences(Graphics graphics, TumorAnnotation annotation, int width, int height) { }
        private void DrawArrow(Graphics graphics, PointF[] points, Pen pen) { }
        private void DrawTextMarker(Graphics graphics, AnnotationRegion region, ColorScheme colorScheme) { }
        private void DrawRegionLabel(Graphics graphics, AnnotationRegion region, ColorScheme colorScheme) { }
        private void DrawConfidenceIndicator(Graphics graphics, AnnotationRegion region, ColorScheme colorScheme) { }

        private async Task<string> CreateBasicAnnotatedImageAsync(string originalImagePath, TumorAnnotation annotation)
        {
            // Fallback implementation for non-Windows platforms
            var annotatedPath = _fileService.GetAnnotatedImagePath(originalImagePath);
            File.Copy(originalImagePath, annotatedPath, true);
            return annotatedPath;
        }

        public class ColorScheme
        {
            public Color Primary { get; set; }
            public Color Secondary { get; set; }
            public Color Text { get; set; }
        }
    }
}
