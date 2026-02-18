using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Aesclea_Back_End_.AIModel.Helpers;

namespace Aesclea_Back_End_.AIModel
{
    /// <summary>
    /// Service for training the medical diagnosis AI model with LLM datasets
    /// </summary>
    public class TrainingService
    {
        private readonly MedicalDiagnosisClassifier _classifier;
        private readonly string _datasetBasePath;

        public TrainingService(MedicalDiagnosisClassifier classifier, string datasetBasePath)
        {
            _classifier = classifier;
            _datasetBasePath = datasetBasePath;
        }

        /// <summary>
        /// Training configuration
        /// </summary>
        public class TrainingConfig
        {
            public int Epochs { get; set; } = 10;
            public double LearningRate { get; set; } = 0.01;
            public int BatchSize { get; set; } = 64; // Increased from 32 for parallel processing
            public double ValidationSplit { get; set; } = 0.2;
            public bool UseMedicalSamples { get; set; } = true;
            public bool UseTextbooks { get; set; } = true;
            public bool UsePubMedQA { get; set; } = true;
            public bool UsePlainTextFiles { get; set; } = true; // NEW: Enable plain text file training
            public int MaxSamplesPerSource { get; set; } = 10000; // Limit for performance
            public bool EnableParallelProcessing { get; set; } = true; // Use all CPU cores
            public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount; // CPU cores to use
            public bool EnableConcurrentNetworkTraining { get; set; } = false; // Run diagnostic/severity/urgency training concurrently with controlled allocation
        }

        /// <summary>
        /// Training progress callback
        /// </summary>
        public class TrainingProgress
        {
            public int CurrentEpoch { get; set; }
            public int TotalEpochs { get; set; }
            public int SamplesProcessed { get; set; }
            public int TotalSamples { get; set; }
            public double CurrentLoss { get; set; }
            public double ValidationAccuracy { get; set; }
            public string Phase { get; set; } = "";
            public string Message { get; set; } = "";
        }

        /// <summary>
        /// Trains the model with all available datasets
        /// </summary>
        public void TrainModel(TrainingConfig config, Action<TrainingProgress>? progressCallback = null)
        {
            try
            {
                Console.WriteLine("=== Starting Medical AI Model Training ===");
                Console.WriteLine($"Dataset Base Path: {_datasetBasePath}");
                Console.WriteLine($"Epochs: {config.Epochs}, Learning Rate: {config.LearningRate}");
                Console.WriteLine();

                ReportDiskReadThroughput();
                Console.WriteLine();

                // Step 1: Load all datasets
                var allTrainingData = LoadAllDatasets(config, progressCallback);

                if (allTrainingData.Count == 0)
                {
                    Console.WriteLine("ERROR: No training data loaded!");
                    return;
                }

                Console.WriteLine($"Total training samples: {allTrainingData.Count}");
                Console.WriteLine();

                // Step 2: Shuffle and split data
                var (trainingSet, validationSet) = SplitData(allTrainingData, config.ValidationSplit);

                Console.WriteLine($"Training set: {trainingSet.Count} samples");
                Console.WriteLine($"Validation set: {validationSet.Count} samples");
                Console.WriteLine();

                // Step 3: Train the model
                TrainWithData(trainingSet, validationSet, config, progressCallback);

                // Step 4: Save trained model to NeuronData folder (auto-loaded by chatbot)
                var fileHelper = new FileHelper();
                fileHelper.OpenFolder(); // Opens default NeuronData folder
                _classifier.SaveWeights(fileHelper, "medical_diagnosis");
                
                var neuronDataPath = Path.Combine(FileHelper.TryGetSolutionDirectoryInfo().FullName, "NeuronData");
                Console.WriteLine($"✓ Model saved to: {neuronDataPath}");
                Console.WriteLine($"✓ Files created:");
                Console.WriteLine($"  - medical_diagnosis_diagnostic_NeuralData.wbn");
                Console.WriteLine($"  - medical_diagnosis_severity_NeuralData.wbn");
                Console.WriteLine($"  - medical_diagnosis_urgency_NeuralData.wbn");
                Console.WriteLine($"✓ Model will be automatically loaded by AI chatbot on next restart");
                Console.WriteLine();

                Console.WriteLine("=== Training Complete ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR during training: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void ReportDiskReadThroughput(int maxCharactersToRead = 5_000_000)
        {
            try
            {
                if (!Directory.Exists(_datasetBasePath))
                {
                    Console.WriteLine("⚠️  Disk benchmark skipped: dataset directory not found.");
                    return;
                }

                var eligibleExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    ".txt", ".csv", ".json", ".jsonl", ".md"
                };

                var filesToSample = Directory.EnumerateFiles(_datasetBasePath, "*.*", SearchOption.AllDirectories)
                    .Where(path => eligibleExtensions.Contains(Path.GetExtension(path)))
                    .Take(10)
                    .ToList();

                if (filesToSample.Count == 0)
                {
                    Console.WriteLine("⚠️  Disk benchmark skipped: no text-based dataset files found.");
                    return;
                }

                long totalBytes = 0;
                long totalChars = 0;
                int filesRead = 0;

                var encoding = Encoding.UTF8;
                var stopwatch = Stopwatch.StartNew();

                var byteBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(1 << 15);
                var charBuffer = System.Buffers.ArrayPool<char>.Shared.Rent(1 << 15);

                try
                {
                    foreach (var file in filesToSample)
                    {
                        using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, byteBuffer.Length, FileOptions.SequentialScan);
                        var decoder = encoding.GetDecoder();

                        int bytesRead;
                        while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
                        {
                            totalBytes += bytesRead;
                            totalChars += decoder.GetChars(byteBuffer, 0, bytesRead, charBuffer, 0, flush: false);

                            if (maxCharactersToRead > 0 && totalChars >= maxCharactersToRead)
                            {
                                break;
                            }
                        }

                        filesRead++;

                        if (maxCharactersToRead > 0 && totalChars >= maxCharactersToRead)
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    stopwatch.Stop();
                    System.Buffers.ArrayPool<byte>.Shared.Return(byteBuffer);
                    System.Buffers.ArrayPool<char>.Shared.Return(charBuffer);
                }

                if (totalChars == 0 || stopwatch.Elapsed.TotalSeconds <= 0)
                {
                    Console.WriteLine("⚠️  Disk benchmark inconclusive (no data read).");
                    return;
                }

                double seconds = stopwatch.Elapsed.TotalSeconds;
                double charsPerSecond = totalChars / seconds;
                double mbPerSecond = (totalBytes / (1024.0 * 1024.0)) / seconds;

                Console.WriteLine("📀 Disk throughput benchmark (approximate)");
                Console.WriteLine($"   Files sampled: {filesRead}, Characters read: {totalChars:N0}");
                Console.WriteLine($"   Sustained read speed: {charsPerSecond:N0} chars/s (~{mbPerSecond:F2} MB/s)");
                Console.WriteLine("   Note: Measurement limited to first few files and approximates characters via UTF-8 decoding.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Disk benchmark failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads all datasets based on configuration
        /// </summary>
        private List<TrainingData> LoadAllDatasets(TrainingConfig config, Action<TrainingProgress>? progressCallback)
        {
            var allData = new List<TrainingData>();

            // Load Medical Samples (mtsamples.csv)
            if (config.UseMedicalSamples)
            {
                progressCallback?.Invoke(new TrainingProgress
                {
                    Phase = "Loading Data",
                    Message = "Loading medical transcription samples..."
                });

                var csvPath = Path.Combine(_datasetBasePath, "mtsamples.csv");
                var medicalSamples = DataLoaders.MedicalSampleLoader.LoadFromCsv(csvPath);
                var medicalTrainingData = DataLoaders.MedicalSampleLoader.ConvertToTrainingData(medicalSamples);

                if (medicalTrainingData.Count > config.MaxSamplesPerSource)
                {
                    medicalTrainingData = medicalTrainingData
                        .OrderBy(x => Guid.NewGuid())
                        .Take(config.MaxSamplesPerSource)
                        .ToList();
                }

                allData.AddRange(medicalTrainingData);
                Console.WriteLine($"✓ Loaded {medicalTrainingData.Count} medical case samples");
            }

            // Load Textbooks
            if (config.UseTextbooks)
            {
                progressCallback?.Invoke(new TrainingProgress
                {
                    Phase = "Loading Data",
                    Message = "Loading medical textbooks..."
                });

                var textbookPath = Path.Combine(_datasetBasePath, "en");
                var textbookChunks = DataLoaders.TextbookLoader.LoadFromDirectory(textbookPath);
                var textbookTrainingData = DataLoaders.TextbookLoader.ConvertToTrainingData(textbookChunks);

                if (textbookTrainingData.Count > config.MaxSamplesPerSource)
                {
                    textbookTrainingData = textbookTrainingData
                        .OrderBy(x => Guid.NewGuid())
                        .Take(config.MaxSamplesPerSource)
                        .ToList();
                }

                allData.AddRange(textbookTrainingData);
                Console.WriteLine($"✓ Loaded {textbookTrainingData.Count} textbook chunks");
            }

            // Load PubMed QA
            if (config.UsePubMedQA)
            {
                progressCallback?.Invoke(new TrainingProgress
                {
                    Phase = "Loading Data",
                    Message = "Loading PubMed Q&A dataset..."
                });

                var pubmedPath = Path.Combine(_datasetBasePath, "pubmedqa-master", "data", "ori_pqal.json");
                var groundTruthPath = Path.Combine(_datasetBasePath, "pubmedqa-master", "data", "test_ground_truth.json");
                
                var pubmedQuestions = DataLoaders.PubMedQALoader.LoadFromJson(pubmedPath, groundTruthPath);
                var pubmedTrainingData = DataLoaders.PubMedQALoader.ConvertToTrainingData(pubmedQuestions);

                if (pubmedTrainingData.Count > config.MaxSamplesPerSource)
                {
                    pubmedTrainingData = pubmedTrainingData
                        .OrderBy(x => Guid.NewGuid())
                        .Take(config.MaxSamplesPerSource)
                        .ToList();
                }

                allData.AddRange(pubmedTrainingData);
                Console.WriteLine($"✓ Loaded {pubmedTrainingData.Count} PubMed Q&A samples");
            }

            // Load Plain Text Files (TXT) - NEW!
            if (config.UsePlainTextFiles)
            {
                progressCallback?.Invoke(new TrainingProgress
                {
                    Phase = "Loading Data",
                    Message = "Loading plain text files (ICD-11, medical texts, etc.)..."
                });

                // Scan multiple subdirectories for TXT files
                var textDirectories = new[]
                {
                    Path.Combine(_datasetBasePath, "textbooks"),
                    Path.Combine(_datasetBasePath, "clinical_cases"),
                    Path.Combine(_datasetBasePath, "medical_qa"),
                    Path.Combine(_datasetBasePath, "safety_examples"),
                    Path.Combine(_datasetBasePath, "uncertainty_calibration"),
                    _datasetBasePath // Also check root directory
                };

                var parsingConfig = new DataLoaders.PlainTextLoader.ParsingConfig
                {
                    MinSectionLength = 50,
                    MaxSectionLength = 2000,
                    DetectMedicalCodes = true,
                    ExtractHeaders = true,
                    CleanText = true,
                    ChunkOverlap = 100
                };

                var allTextDocuments = new List<DataLoaders.PlainTextLoader.TextDocument>();

                foreach (var dir in textDirectories)
                {
                    if (Directory.Exists(dir))
                    {
                        var documents = DataLoaders.PlainTextLoader.LoadFromDirectory(dir, parsingConfig);
                        allTextDocuments.AddRange(documents);
                    }
                }

                if (allTextDocuments.Count > 0)
                {
                    var plainTextTrainingData = DataLoaders.PlainTextLoader.ConvertToTrainingData(allTextDocuments);

                    if (plainTextTrainingData.Count > config.MaxSamplesPerSource)
                    {
                        plainTextTrainingData = plainTextTrainingData
                            .OrderBy(x => Guid.NewGuid())
                            .Take(config.MaxSamplesPerSource)
                            .ToList();
                    }

                    allData.AddRange(plainTextTrainingData);
                    Console.WriteLine($"✓ Loaded {plainTextTrainingData.Count} training samples from {allTextDocuments.Count} plain text files");
                }
                else
                {
                    Console.WriteLine("ℹ No plain text (.txt) files found in dataset directories");
                }
            }

            return allData;
        }

        /// <summary>
        /// Splits data into training and validation sets
        /// </summary>
        private (List<TrainingData> training, List<TrainingData> validation) SplitData(
            List<TrainingData> data, double validationSplit)
        {
            // Shuffle data
            var shuffled = data.OrderBy(x => Guid.NewGuid()).ToList();

            var validationSize = (int)(shuffled.Count * validationSplit);
            var trainingSize = shuffled.Count - validationSize;

            var trainingSet = shuffled.Take(trainingSize).ToList();
            var validationSet = shuffled.Skip(trainingSize).ToList();

            return (trainingSet, validationSet);
        }

        /// <summary>
        /// Trains the model with the prepared data
        /// </summary>
        private void TrainWithData(
            List<TrainingData> trainingSet,
            List<TrainingData> validationSet,
            TrainingConfig config,
            Action<TrainingProgress>? progressCallback)
        {
            Console.WriteLine($"Training with {trainingSet.Count} samples");

            // Prepare batch data for training
            var medicalTexts = trainingSet.Select(s => s.InputText).ToList();
            var diagnosticCategories = trainingSet.Select(s => GetCategoryName(s.DiagnosisCategory)).ToList();
            var severityLevels = trainingSet.Select(s => Math.Min(Math.Max(s.SeverityLevel, 1), 5)).ToList();
            var urgencyLevels = trainingSet.Select(s => Math.Min(Math.Max(s.UrgencyLevel, 0), 2)).ToList();

            // Train the classifier using its batch training method
            if (config.EnableConcurrentNetworkTraining && config.MaxDegreeOfParallelism > 1)
            {
                Console.WriteLine("🔥 CONCURRENT NETWORK TRAINING MODE 🔥");
                Console.WriteLine("Training 3 networks simultaneously with separate progress lines:");
                Console.WriteLine();
                
                // Controlled concurrent training: divide cores across networks to avoid oversubscription
                int total = Math.Max(1, config.MaxDegreeOfParallelism);

                int baseAlloc = total / 3;
                int remainder = total % 3;

                // Assign remainder to diagnostic (largest) then severity then urgency
                int diagAlloc = Math.Max(1, baseAlloc + (remainder > 0 ? 1 : 0));
                int sevAlloc = Math.Max(1, baseAlloc + (remainder > 1 ? 1 : 0));
                int urgAlloc = Math.Max(1, baseAlloc);

                // If allocations somehow sum to more than total due to Max(1,..), normalize
                int sum = diagAlloc + sevAlloc + urgAlloc;
                if (sum > total)
                {
                    // reduce urgency first
                    int over = sum - total;
                    urgAlloc = Math.Max(1, urgAlloc - over);
                }

                _classifier.SetPerNetworkParallelism(diagAlloc, sevAlloc, urgAlloc);
                
                // Enable multi-line progress display - reserve 3 lines for 3 networks
                // Print placeholder lines first
                Console.WriteLine("[DIAGNOSTIC] Initializing...".PadRight(120));
                Console.WriteLine("[SEVERITY  ] Initializing...".PadRight(120));
                Console.WriteLine("[URGENCY   ] Initializing...".PadRight(120));
                
                // Configure networks to use separate console lines
                _classifier.diagnosticNetwork.ConsoleLineOffset = 2; // 2 lines up from current
                _classifier.severityNetwork.ConsoleLineOffset = 1;   // 1 line up
                _classifier.urgencyNetwork.ConsoleLineOffset = 0;    // Current line

                // Run three training tasks concurrently. Each internal Train* will use its assigned threads.
                var tasks = new List<System.Threading.Tasks.Task>();

                tasks.Add(System.Threading.Tasks.Task.Run(() =>
                {
                    _classifier.TrainDiagnosticNetworkConcurrent(medicalTexts, diagnosticCategories, config.Epochs, config.LearningRate);
                }));

                tasks.Add(System.Threading.Tasks.Task.Run(() =>
                {
                    _classifier.TrainSeverityNetworkConcurrent(medicalTexts, severityLevels, config.Epochs, config.LearningRate);
                }));

                tasks.Add(System.Threading.Tasks.Task.Run(() =>
                {
                    _classifier.TrainUrgencyNetworkConcurrent(medicalTexts, urgencyLevels, config.Epochs, config.LearningRate);
                }));

                System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
                
                // Reset to normal console mode
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("✓ All networks trained concurrently");
            }
            else
            {
                // Configure classifier parallelism so internal training uses reasonable per-network threads (sequential mode)
                int perNetworkParallelism = Math.Max(1, config.MaxDegreeOfParallelism / 3);
                _classifier.SetTrainingParallelism(perNetworkParallelism);

                _classifier.TrainDiagnosticNetworks(
                    medicalTexts,
                    diagnosticCategories,
                    severityLevels,
                    urgencyLevels,
                    config.Epochs,
                    config.LearningRate
                );
            }

            // Validate final model
            var accuracy = ValidateModel(validationSet);
            Console.WriteLine($"Final Validation Accuracy: {accuracy:F2}%");
            Console.WriteLine();

            progressCallback?.Invoke(new TrainingProgress
            {
                CurrentEpoch = config.Epochs,
                TotalEpochs = config.Epochs,
                SamplesProcessed = trainingSet.Count,
                TotalSamples = trainingSet.Count,
                ValidationAccuracy = accuracy,
                Phase = "Complete",
                Message = $"Training complete - Final Accuracy: {accuracy:F2}%"
            });
        }

        /// <summary>
        /// Gets category name from index
        /// </summary>
        private string GetCategoryName(int categoryIndex)
        {
            var categories = new[] {
                "Cardiovascular", "Neurological", "Respiratory", "Gastrointestinal",
                "Musculoskeletal", "Oncology", "Endocrine", "Renal", "Dermatological", "Other"
            };

            return categoryIndex >= 0 && categoryIndex < categories.Length 
                ? categories[categoryIndex] 
                : "Other";
        }

        /// <summary>
        /// Validates the model on validation set
        /// </summary>
        private double ValidateModel(List<TrainingData> validationSet)
        {
            int correct = 0;
            int total = 0;

            foreach (var sample in validationSet.Take(100)) // Sample for speed
            {
                try
                {
                    var result = _classifier.AnalyzeMedicalText(sample.InputText);

                    // Check if diagnosis category matches
                    var predictedCategory = GetTopCategory(result.DiagnosticConfidences);
                    if (predictedCategory == sample.DiagnosisCategory)
                    {
                        correct++;
                    }

                    total++;
                }
                catch
                {
                    // Skip validation errors
                }
            }

            return total > 0 ? (correct * 100.0 / total) : 0;
        }

        private int GetTopCategory(Dictionary<string, double> scores)
        {
            if (scores == null || scores.Count == 0) return 0;

            var topScore = scores.OrderByDescending(s => s.Value).First();
            
            // Map category name to index
            var categories = new[] {
                "Cardiovascular", "Neurological", "Respiratory", "Gastrointestinal",
                "Musculoskeletal", "Oncology", "Endocrine", "Renal", "Dermatological", "Other"
            };

            for (int i = 0; i < categories.Length; i++)
            {
                if (topScore.Key.Contains(categories[i]))
                    return i;
            }

            return 9; // Default to "Other"
        }
    }
}
