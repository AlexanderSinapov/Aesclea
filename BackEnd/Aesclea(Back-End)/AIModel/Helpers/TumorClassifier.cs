using System;
using System.Collections.Generic;
using System.Linq;
using Aesclea_Back_End_.AIModel.Helpers;

namespace Aesclea_Back_End_.AIModel
{
    public class TumorClassifier
    {
        private NeuronNetwork baseNetwork; // The existing tumor detection network
        private NeuronNetwork typeNetwork; // Network for tumor type classification
        private NeuronNetwork gradeNetwork; // Network for tumor grade classification
        private NeuronNetwork locationNetwork; // Network for tumor location classification
        private TumorHelper tumorHelper; // Helper with tumor metadata

        public TumorClassifier(NeuronNetwork baseNetwork)
        {
            this.baseNetwork = baseNetwork;
            this.tumorHelper = new TumorHelper();

            // Initialize specialized networks
            // Type network - outputs map to tumor types (larger network)
            this.typeNetwork = new NeuronNetwork(new int[] { 16384, 512, 256, 128, tumorHelper.TumorType.Length });

            // Grade network - outputs map to 4 tumor grades
            this.gradeNetwork = new NeuronNetwork(new int[] { 16384, 256, 64, 16, 4 });

            // Location network - outputs map to 7 location categories
            this.locationNetwork = new NeuronNetwork(new int[] { 16384, 256, 64, 32, 7 });
        }

        /// <summary>
        /// Analyzes an image to detect tumor and classify its properties if present
        /// </summary>
        /// <param name="imageData">Preprocessed image data</param>
        /// <returns>TumorAnalysisResult containing detection and classification data</returns>
        public TumorAnalysisResult AnalyzeImage(List<double> imageData)
        {
            // First use the base network to detect if there's a tumor
            var baseOutput = baseNetwork.FeedForward(imageData);
            double tumorProbability = baseOutput[0];

            var result = new TumorAnalysisResult
            {
                TumorProbability = tumorProbability,
                HasTumor = tumorProbability >= 0.5
            };

            // Only classify the tumor if probability is reasonably high
            if (result.HasTumor)
            {
                // Get tumor type
                var typeOutput = typeNetwork.FeedForward(imageData);
                int typeIndex = GetMaxIndex(typeOutput);
                result.TumorType = tumorHelper.TumorType[typeIndex];
                result.TypeConfidence = typeOutput[typeIndex];

                // Get tumor grade
                var gradeOutput = gradeNetwork.FeedForward(imageData);
                int gradeIndex = GetMaxIndex(gradeOutput);
                result.TumorGrade = gradeIndex + 1; // Grades are 1-indexed
                result.GradeDescription = tumorHelper.TumorGrade[gradeIndex + 1];
                result.GradeConfidence = gradeOutput[gradeIndex];

                // Get tumor location
                var locationOutput = locationNetwork.FeedForward(imageData);
                int locationIndex = GetMaxIndex(locationOutput);
                result.TumorLocation = tumorHelper.TumorLocation.Keys.ElementAt(locationIndex);
                result.LocationConfidence = locationOutput[locationIndex];

                // Determine stage from grade and other factors (simplified approach)
                // In a real system, this would use more complex analysis
                result.EstimatedStage = DetermineEstimatedStage(result.TumorGrade.ToString(), tumorProbability);
                result.StageDescription = tumorHelper.GeneralizedStage[result.EstimatedStage];
            }

            return result;
        }

        /// <summary>
        /// Train the specialized networks using the provided data
        /// </summary>
        public void TrainClassifiers(
            List<List<double>> inputs,
            List<string> types,
            List<int> grades,
            List<string> locations,
            int epochs,
            double learningRate)
        {
            Console.WriteLine("Training tumor type classifier...");
            TrainTypeNetwork(inputs, types, epochs, learningRate);

            Console.WriteLine("Training tumor grade classifier...");
            TrainGradeNetwork(inputs, grades, epochs, learningRate);

            Console.WriteLine("Training tumor location classifier...");
            TrainLocationNetwork(inputs, locations, epochs, learningRate);

            Console.WriteLine("Training complete for all classifiers.");
        }

        private void TrainTypeNetwork(List<List<double>> inputs, List<string> types, int epochs, double learningRate)
        {
            var outputs = new List<List<double>>();

            // Create one-hot encoded outputs for each tumor type
            foreach (string type in types)
            {
                var output = new List<double>(new double[tumorHelper.TumorType.Length]);
                int index = Array.IndexOf(tumorHelper.TumorType, type);
                if (index >= 0)
                    output[index] = 1.0;
                outputs.Add(output);
            }

            typeNetwork.Train(inputs, outputs, epochs, learningRate);
        }

        private void TrainGradeNetwork(List<List<double>> inputs, List<int> grades, int epochs, double learningRate)
        {
            var outputs = new List<List<double>>();

            // Create one-hot encoded outputs for each tumor grade
            foreach (int grade in grades)
            {
                var output = new List<double>(new double[4]); // 4 possible grades
                if (grade >= 1 && grade <= 4)
                    output[grade - 1] = 1.0;
                outputs.Add(output);
            }

            gradeNetwork.Train(inputs, outputs, epochs, learningRate);
        }

        private void TrainLocationNetwork(List<List<double>> inputs, List<string> locations, int epochs, double learningRate)
        {
            var outputs = new List<List<double>>();
            var locationKeys = tumorHelper.TumorLocation.Keys.ToArray();

            // Create one-hot encoded outputs for each location
            foreach (string location in locations)
            {
                var output = new List<double>(new double[locationKeys.Length]);
                int index = Array.IndexOf(locationKeys, location);
                if (index >= 0)
                    output[index] = 1.0;
                outputs.Add(output);
            }

            locationNetwork.Train(inputs, outputs, epochs, learningRate);
        }

        /// <summary>
        /// Finds the index of the maximum value in a list
        /// </summary>
        private int GetMaxIndex(List<double> values)
        {
            double maxValue = double.MinValue;
            int maxIndex = 0;

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] > maxValue)
                {
                    maxValue = values[i];
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        /// <summary>
        /// Simple estimation of tumor stage based on grade and detection confidence
        /// </summary>
        private int DetermineEstimatedStage(string grade, double probability)
        {
            // This is a simplified approach - in a real system, staging would require
            // multiple imaging studies and clinical data
            if (int.Parse(grade) == 1 && probability < 0.7 || grade == "Pituitary adenoma")
                return 1; // Localized
            else if (int.Parse(grade) <= 2 && probability < 0.85 || grade == "Atypical adenoma")
                return 2; // Locally advanced
            else if (int.Parse(grade) <= 3 || grade == "Pituitary carcinoma")
                return 3; // More advanced local spread
            else
                return 4; // Possible metastasis
        }

        /// <summary>
        /// Save all classifier network weights
        /// </summary>
        public void SaveWeights(FileHelper fileHelper, string baseName)
        {
            // Save each specialized network with appropriate suffix
            fileHelper.SaveNeuralData(typeNetwork.GetNeuralNetworkData(), baseName + "_type");
            fileHelper.SaveNeuralData(gradeNetwork.GetNeuralNetworkData(), baseName + "_grade");
            fileHelper.SaveNeuralData(locationNetwork.GetNeuralNetworkData(), baseName + "_location");
        }

        /// <summary>
        /// Load all classifier network weights
        /// </summary>
        public void LoadWeights(FileHelper fileHelper, string baseName)
        {
            // Load each specialized network
            var typeData = fileHelper.GetNeuralData(baseName + "_type_NeuralData.wbn");
            if (typeData != null)
                typeNetwork.SetNeuralNetworkData(typeData);

            var gradeData = fileHelper.GetNeuralData(baseName + "_grade_NeuralData.wbn");
            if (gradeData != null)
                gradeNetwork.SetNeuralNetworkData(gradeData);

            var locationData = fileHelper.GetNeuralData(baseName + "_location_NeuralData.wbn");
            if (locationData != null)
                locationNetwork.SetNeuralNetworkData(locationData);
        }
    }

    /// <summary>
    /// Stores the results of tumor analysis
    /// </summary>
    public class TumorAnalysisResult
    {
        // Base tumor detection
        public double TumorProbability { get; set; }
        public bool HasTumor { get; set; }

        // Tumor classification
        public string TumorType { get; set; }
        public double TypeConfidence { get; set; }

        public int TumorGrade { get; set; }
        public string GradeDescription { get; set; }
        public double GradeConfidence { get; set; }

        public string TumorLocation { get; set; }
        public double LocationConfidence { get; set; }

        public int EstimatedStage { get; set; }
        public string StageDescription { get; set; }

        /// <summary>
        /// Returns a formatted summary of the analysis results
        /// </summary>
        public string GetSummary()
        {
            if (!HasTumor)
            {
                return $"Analysis complete. No tumor detected (confidence: {(1 - TumorProbability) * 100:F2}%)";
            }

            return $"TUMOR ANALYSIS RESULTS:\n" +
                   $"Detection Confidence: {TumorProbability * 100:F2}%\n" +
                   $"Type: {TumorType} (confidence: {TypeConfidence * 100:F2}%)\n" +
                   $"Grade: {TumorGrade} - {GradeDescription} (confidence: {GradeConfidence * 100:F2}%)\n" +
                   $"Location: {TumorLocation} (confidence: {LocationConfidence * 100:F2}%)\n" +
                   $"Estimated Stage: {EstimatedStage} - {StageDescription}";
        }
    }
}