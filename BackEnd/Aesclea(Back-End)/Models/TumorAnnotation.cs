using System.ComponentModel.DataAnnotations;

namespace Aesclea_Back_End_.Models
{
    /// <summary>
    /// Represents a tumor annotation with detailed geometric and medical information
    /// </summary>
    public class TumorAnnotation
    {
        public int Id { get; set; }
        public string AnalysisId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        
        // Region coordinates and geometry
        public List<AnnotationRegion> Regions { get; set; } = new();
        
        // Annotation properties
        public AnnotationType Type { get; set; }
        public AnnotationColor Color { get; set; }
        public int StrokeWidth { get; set; } = 3;
        public double Opacity { get; set; } = 0.7;
        
        // Medical information
        public string? TumorType { get; set; }
        public int? TumorGrade { get; set; }
        public double? Confidence { get; set; }
        public string? Notes { get; set; }
        
        // Metadata
        public string OriginalImagePath { get; set; } = string.Empty;
        public string? AnnotatedImagePath { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        
        // Statistics
        public AnnotationStatistics Statistics { get; set; } = new();
    }

    /// <summary>
    /// Represents a specific region within an annotation
    /// </summary>
    public class AnnotationRegion
    {
        public int Id { get; set; }
        public int AnnotationId { get; set; }
        
        // Geometric data
        public RegionType Type { get; set; }
        public List<AnnotationPoint> Points { get; set; } = new();
        public Rectangle BoundingBox { get; set; }
        
        // Properties
        public string Label { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string? Description { get; set; }
        
        // Calculated properties
        public double Area => CalculateArea();
        public double Perimeter => CalculatePerimeter();
        public AnnotationPoint Centroid => CalculateCentroid();
        
        private double CalculateArea()
        {
            if (Points.Count < 3) return 0;
            
            double area = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                int j = (i + 1) % Points.Count;
                area += Points[i].X * Points[j].Y;
                area -= Points[j].X * Points[i].Y;
            }
            return Math.Abs(area) / 2.0;
        }
        
        private double CalculatePerimeter()
        {
            if (Points.Count < 2) return 0;
            
            double perimeter = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                int j = (i + 1) % Points.Count;
                double dx = Points[j].X - Points[i].X;
                double dy = Points[j].Y - Points[i].Y;
                perimeter += Math.Sqrt(dx * dx + dy * dy);
            }
            return perimeter;
        }
        
        private AnnotationPoint CalculateCentroid()
        {
            if (Points.Count == 0) return new AnnotationPoint { X = 0, Y = 0 };
            
            double x = Points.Average(p => p.X);
            double y = Points.Average(p => p.Y);
            return new AnnotationPoint { X = x, Y = y };
        }
    }

    /// <summary>
    /// Represents a point in the annotation coordinate system
    /// </summary>
    public class AnnotationPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        
        // Normalized coordinates (0-1)
        public double NormalizedX { get; set; }
        public double NormalizedY { get; set; }
    }

    /// <summary>
    /// Request model for creating annotations
    /// </summary>
    public class CreateAnnotationRequest
    {
        [Required]
        public string AnalysisId { get; set; } = string.Empty;
        
        public List<CreateRegionRequest> Regions { get; set; } = new();
        public AnnotationType Type { get; set; } = AnnotationType.AutoDetected;
        public AnnotationColor Color { get; set; } = AnnotationColor.Auto;
        public int StrokeWidth { get; set; } = 3;
        public double Opacity { get; set; } = 0.7;
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Request model for creating annotation regions
    /// </summary>
    public class CreateRegionRequest
    {
        public RegionType Type { get; set; }
        public List<AnnotationPoint> Points { get; set; } = new();
        public string Label { get; set; } = string.Empty;
        public double Confidence { get; set; } = 1.0;
        public string? Description { get; set; }
    }

    /// <summary>
    /// Enhanced annotation response with detailed analysis
    /// </summary>
    public class AnnotationResponse
    {
        public TumorAnnotation Annotation { get; set; } = new();
        public string AnnotatedImageUrl { get; set; } = string.Empty;
        public AnnotationStatistics Statistics { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Statistical information about annotations
    /// </summary>
    public class AnnotationStatistics
    {
        public int TotalRegions { get; set; }
        public double TotalArea { get; set; }
        public double LargestRegionArea { get; set; }
        public double SmallestRegionArea { get; set; }
        public double AverageConfidence { get; set; }
        public AnnotationPoint CombinedCentroid { get; set; } = new();
        public Dictionary<string, int> RegionTypeDistribution { get; set; } = new();
    }

    // Enums

    public enum AnnotationType
    {
        AutoDetected = 0,
        ManuallyDrawn = 1,
        AIAssisted = 2,
        Combined = 3
    }

    public enum RegionType
    {
        Circle = 0,
        Rectangle = 1,
        Ellipse = 2,
        Polygon = 3,
        Freehand = 4,
        Arrow = 5,
        Text = 6
    }

    public enum AnnotationColor
    {
        Auto = 0,
        Red = 1,
        Orange = 2,
        Yellow = 3,
        Green = 4,
        Blue = 5,
        Purple = 6,
        Pink = 7,
        Cyan = 8,
        Magenta = 9,
        Lime = 10,
        DarkRed = 11,
        DarkBlue = 12,
        DarkGreen = 13
    }

    /// <summary>
    /// Structure for rectangle representation
    /// </summary>
    public struct Rectangle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        
        public int Right => X + Width;
        public int Bottom => Y + Height;
        public AnnotationPoint Center => new() { X = X + Width / 2.0, Y = Y + Height / 2.0 };
    }
}
