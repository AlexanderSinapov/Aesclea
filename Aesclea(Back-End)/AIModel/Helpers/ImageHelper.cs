using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Aesclea_Back_End_.AIModel
{
    public static class ImageHelper
    {
        /// <summary>
        /// Loads and preprocesses images from a folder into a format suitable for neural network input
        /// </summary>
        /// <param name="folderPath">Path to the folder containing images</param>
        /// <param name="imageSize">Size to resize images to (both width and height)</param>
        /// <returns>List of normalized image data as flattened arrays</returns>
        public static List<List<double>> LoadImages(string folderPath, int imageSize = 512)
        {
            List<List<double>> inputs = new List<List<double>>();

            try
            {
                string[] supportedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                var files = Directory.GetFiles(folderPath, "*.*")
                    .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                    .ToArray();

                Console.WriteLine($"Found {files.Length} images in {folderPath}");

                int processedCount = 0;
                int errorCount = 0;

                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    try
                    {
                        // Show progress percentage
                        if (files.Length > 10)
                        {
                            Console.Write($"\rLoading images: {(int)((i + 1) / (double)files.Length * 100)}% ({i + 1}/{files.Length})");
                        }

                        using (Bitmap bitmap = new Bitmap(file))
                        {
                            // Resize to specified dimensions
                            Bitmap resized = new Bitmap(bitmap, new Size(imageSize, imageSize));

                            // Convert to grayscale and normalize
                            List<double> imageData = new List<double>(imageSize * imageSize);
                            for (int y = 0; y < resized.Height; y++)
                            {
                                for (int x = 0; x < resized.Width; x++)
                                {
                                    Color pixelColor = resized.GetPixel(x, y);
                                    // Standard grayscale conversion formula
                                    double grayscaleValue = 0.299 * pixelColor.R + 0.587 * pixelColor.G + 0.114 * pixelColor.B;
                                    imageData.Add(grayscaleValue / 255.0); // Normalize to [0, 1]
                                }
                            }
                            inputs.Add(imageData);
                            processedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        Console.WriteLine($"\nError processing image {file}: {ex.Message}");
                    }
                }

                Console.WriteLine($"\nSuccessfully processed {processedCount} images. {errorCount} images had errors.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing folder {folderPath}: {ex.Message}");
            }

            return inputs;
        }

        /// <summary>
        /// Preprocesses a single image for neural network input
        /// </summary>
        /// <param name="image">The image to process</param>
        /// <param name="size">Target size (both width and height)</param>
        /// <returns>Normalized image data as a flattened array</returns>
        public static List<double> PreprocessImage(Bitmap image, int size = 512)
        {
            // Resize the image
            using (Bitmap resized = new Bitmap(image, new Size(size, size)))
            {
                List<double> imageData = new List<double>(size * size);

                Console.WriteLine("Processing image...");

                for (int y = 0; y < size; y++)
                {
                    // Show progress for large images
                    if (y % 50 == 0)
                    {
                        Console.Write($"\rProcessing: {(int)((y + 1) / (double)size * 100)}%");
                    }

                    for (int x = 0; x < size; x++)
                    {
                        Color pixelColor = resized.GetPixel(x, y);
                        double grayscaleValue = 0.299 * pixelColor.R + 0.587 * pixelColor.G + 0.114 * pixelColor.B;
                        imageData.Add(grayscaleValue / 255.0);
                    }
                }

                Console.WriteLine("\rProcessing: 100% - Complete!    ");
                return imageData;
            }
        }
    }
}