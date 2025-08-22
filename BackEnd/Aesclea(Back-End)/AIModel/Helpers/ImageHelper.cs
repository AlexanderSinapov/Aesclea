using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Aesclea_Back_End_.AIModel
{
    public static class ImageHelper
    {
        private static Random random = new Random();

        /// <summary>
        /// Loads and preprocesses images from a folder into a format suitable for neural network input
        /// </summary>
        /// <param name="folderPath">Path to the folder containing images</param>
        /// <param name="imageSize">Size to resize images to (both width and height)</param>
        /// <param name="augment">Whether to apply data augmentation</param>
        /// <param name="augmentationCount">Number of augmented versions per image</param>
        /// <returns>List of normalized image data as flattened arrays</returns>
        public static List<List<double>> LoadImages(string folderPath, int imageSize = 128, bool augment = false, int augmentationCount = 0)
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
                            // Process original image
                            List<double> imageData = ProcessImage(bitmap, imageSize);
                            inputs.Add(imageData);
                            processedCount++;

                            // Apply augmentation if requested
                            if (augment && augmentationCount > 0)
                            {
                                for (int j = 0; j < augmentationCount; j++)
                                {
                                    List<double> augmentedData = AugmentImage(bitmap, imageSize, random);
                                    inputs.Add(augmentedData);
                                    processedCount++;
                                }
                            }
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
        /// Processes a Bitmap image into the format needed by the neural network
        /// </summary>
        /// <param name="bitmap">Source image</param>
        /// <param name="size">Target size</param>
        /// <returns>Processed image data</returns>
        public static List<double> ProcessImage(Bitmap bitmap, int size)
        {
            // Resize the image
            using (Bitmap resized = ResizeImage(bitmap, size, size))
            {
                // Convert to grayscale and normalize
                List<double> imageData = new List<double>(size * size);

                // Pre-allocate byte array for faster pixel access
                BitmapData bmpData = resized.LockBits(
                    new Rectangle(0, 0, resized.Width, resized.Height),
                    ImageLockMode.ReadOnly, resized.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(resized.PixelFormat) / 8;
                int byteCount = bmpData.Stride * resized.Height;
                byte[] pixels = new byte[byteCount];

                // Copy bitmap to byte array
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixels, 0, byteCount);
                resized.UnlockBits(bmpData);

                // Process pixels
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        int pos = y * bmpData.Stride + x * bytesPerPixel;
                        if (pos + 2 < pixels.Length)
                        {
                            // BGR format
                            byte b = pixels[pos];
                            byte g = pixels[pos + 1];
                            byte r = pixels[pos + 2];

                            // Standard grayscale conversion formula
                            double grayscaleValue = 0.299 * r + 0.587 * g + 0.114 * b;

                            // Apply normalization and contrast enhancement
                            double normalizedValue = grayscaleValue / 255.0;

                            // Z-score normalization around 0.5 with range compression
                            normalizedValue = (normalizedValue - 0.5) / 0.5; // Convert to [-1, 1] range

                            imageData.Add(normalizedValue);
                        }
                    }
                }

                return imageData;
            }
        }

        /// <summary>
        /// Resizes an image with high quality settings
        /// </summary>
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Creates an augmented version of the image with random transformations
        /// </summary>
        /// <param name="bitmap">Source image</param>
        /// <param name="size">Target size</param>
        /// <param name="random">Random number generator</param>
        /// <returns>Augmented image data</returns>
        private static List<double> AugmentImage(Bitmap bitmap, int size, Random random)
        {
            // Create a copy of the bitmap to avoid modifying the original
            using (Bitmap copy = new Bitmap(bitmap))
            {
                // Apply random augmentations
                int augmentationType = random.Next(5); // Choose one of 5 augmentation types

                switch (augmentationType)
                {
                    case 0: // Rotation
                        using (Bitmap rotated = RotateImage(copy, random.Next(-180, 180)))
                        {
                            return ProcessImage(rotated, size);
                        }

                    case 1: // Brightness adjustment
                        AdjustBrightness(copy, 0.8f + (float)random.NextDouble() * 0.4f); // 0.8 to 1.2
                        return ProcessImage(copy, size);

                    case 2: // Horizontal flip
                        copy.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        return ProcessImage(copy, size);

                    case 3: // Small zoom (crop and resize)
                        using (Bitmap zoomed = CropAndResize(copy, random))
                        {
                            return ProcessImage(zoomed, size);
                        }

                    case 4: // Small horizontal/vertical shift
                        using (Bitmap shifted = ShiftImage(copy, random))
                        {
                            return ProcessImage(shifted, size);
                        }

                    default:
                        return ProcessImage(copy, size);
                }
            }
        }

        /// <summary>
        /// Rotates an image by the specified angle
        /// </summary>
        private static Bitmap RotateImage(Bitmap bmp, float angle)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);
            rotatedImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Set the rotation point to the center of the image
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);

                g.DrawImage(bmp, new Point(0, 0));
            }

            return rotatedImage;
        }

        /// <summary>
        /// Adjusts the brightness of an image
        /// </summary>
        private static void AdjustBrightness(Bitmap bmp, float factor)
        {
            // Create brightness matrix
            float[][] matrixItems = {
                new float[] {factor, 0, 0, 0, 0},
                new float[] {0, factor, 0, 0, 0},
                new float[] {0, 0, factor, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            };

            ColorMatrix colorMatrix = new ColorMatrix(matrixItems);

            using (ImageAttributes attributes = new ImageAttributes())
            {
                attributes.SetColorMatrix(colorMatrix);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(bmp,
                        new Rectangle(0, 0, bmp.Width, bmp.Height),
                        0, 0, bmp.Width, bmp.Height,
                        GraphicsUnit.Pixel, attributes);
                }
            }
        }

        /// <summary>
        /// Crops a portion of the image and resizes it back to the original dimensions
        /// </summary>
        private static Bitmap CropAndResize(Bitmap bmp, Random random)
        {
            // Determine crop size (80-95% of original)
            float cropFactor = 0.8f + (float)random.NextDouble() * 0.15f;
            int cropWidth = (int)(bmp.Width * cropFactor);
            int cropHeight = (int)(bmp.Height * cropFactor);

            // Determine crop position
            int maxX = bmp.Width - cropWidth;
            int maxY = bmp.Height - cropHeight;
            int x = random.Next(maxX + 1);
            int y = random.Next(maxY + 1);

            // Crop the image
            Bitmap cropped = new Bitmap(cropWidth, cropHeight);
            using (Graphics g = Graphics.FromImage(cropped))
            {
                g.DrawImage(bmp,
                    new Rectangle(0, 0, cropWidth, cropHeight),
                    new Rectangle(x, y, cropWidth, cropHeight),
                    GraphicsUnit.Pixel);
            }

            // Resize back to original dimensions
            return ResizeImage(cropped, bmp.Width, bmp.Height);
        }

        /// <summary>
        /// Applies a small shift to the image
        /// </summary>
        private static Bitmap ShiftImage(Bitmap bmp, Random random)
        {
            // Calculate shift amount (up to 10% of dimensions)
            int shiftX = (int)(bmp.Width * 0.1 * (random.NextDouble() * 2 - 1));
            int shiftY = (int)(bmp.Height * 0.1 * (random.NextDouble() * 2 - 1));

            Bitmap shifted = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(shifted))
            {
                // Fill with black background
                g.Clear(Color.Black);

                // Calculate source and destination rectangles
                Rectangle srcRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                Rectangle destRect = new Rectangle(shiftX, shiftY, bmp.Width, bmp.Height);

                // Draw shifted image
                g.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel);
            }

            return shifted;
        }

        /// <summary>
        /// Saves image data to a bitmap file
        /// </summary>
        public static void SaveImageData(List<double> imageData, int width, int height, string outputPath)
        {
            if (imageData.Count != width * height)
            {
                throw new ArgumentException($"Image data length ({imageData.Count}) doesn't match dimensions ({width}x{height})");
            }

            // Create a new bitmap
            using (Bitmap bmp = new Bitmap(width, height))
            {
                // Set pixels from image data
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * width + x;
                        if (index < imageData.Count)
                        {
                            // Convert from [-1, 1] range back to [0, 255]
                            double normalizedValue = (imageData[index] * 0.5) + 0.5;
                            int pixelValue = (int)(normalizedValue * 255);
                            pixelValue = Math.Max(0, Math.Min(255, pixelValue));

                            // Set grayscale pixel
                            Color color = Color.FromArgb(pixelValue, pixelValue, pixelValue);
                            bmp.SetPixel(x, y, color);
                        }
                    }
                }

                // Save the bitmap
                bmp.Save(outputPath);
            }
        }

        /// <summary>
        /// Creates a set of standardized test predictions for visualization
        /// </summary>
        public static void GenerateModelPredictionVisualizations(NeuronNetwork network, List<List<double>> testInputs, List<List<double>> expectedOutputs, string outputFolder, int imageSize = 128)
        {
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            // Limit to a reasonable number of test images
            int samplesToVisualize = Math.Min(testInputs.Count, 10);

            for (int i = 0; i < samplesToVisualize; i++)
            {
                var input = testInputs[i];
                var expected = expectedOutputs[i];

                // Get model prediction
                var output = network.FeedForward(input);

                // Display input image
                string imageFilename = Path.Combine(outputFolder, $"test_image_{i}.png");
                SaveImageData(input, imageSize, imageSize, imageFilename);

                // Create prediction info file
                string infoFilename = Path.Combine(outputFolder, $"test_image_{i}_info.txt");
                string predictionText = $"Prediction: {output[0]:F4}\nExpected: {expected[0]:F4}\n";
                predictionText += $"Prediction Class: {(output[0] >= 0.5 ? "Positive" : "Negative")}\n";
                predictionText += $"Actual Class: {(expected[0] >= 0.5 ? "Positive" : "Negative")}\n";
                predictionText += $"Correct: {((output[0] >= 0.5) == (expected[0] >= 0.5))}\n";

                File.WriteAllText(infoFilename, predictionText);
            }

            Console.WriteLine($"Generated {samplesToVisualize} visualization samples in {outputFolder}");
        }
    }
}