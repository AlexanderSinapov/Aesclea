using System.Drawing;
using System.Drawing.Imaging;

namespace Aesclea_Back_End_.Services
{
    public class ImageProcessingService
    {
        private readonly ILogger<ImageProcessingService> _logger;

        public ImageProcessingService(ILogger<ImageProcessingService> logger)
        {
            _logger = logger;
        }        public Task<List<double>> ProcessImageForAnalysisAsync(string imagePath)
        {
            try
            {
                using var image = Image.FromFile(imagePath);
                
                // Resize to standard input size (e.g., 128x128 = 16384 pixels)
                using var resizedImage = ResizeImage(image, 128, 128);
                
                // Convert to grayscale and normalize
                var imageData = ConvertToNormalizedGrayscale(resizedImage);
                
                return Task.FromResult(imageData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image for analysis");
                throw;
            }
        }

        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var resized = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(resized);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(image, 0, 0, width, height);
            return resized;
        }

        private List<double> ConvertToNormalizedGrayscale(Bitmap image)
        {
            var pixels = new List<double>();
            
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(x, y);
                    // Convert to grayscale and normalize to [0, 1]
                    var gray = (pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114) / 255.0;
                    pixels.Add(gray);
                }
            }
            
            return pixels;
        }
    }
}