namespace Aesclea_Back_End_.Services
{
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
}