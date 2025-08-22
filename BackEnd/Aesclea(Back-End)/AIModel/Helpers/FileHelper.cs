using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Aesclea_Back_End_.DDOs;
using Newtonsoft.Json;

namespace Aesclea_Back_End_.AIModel.Helpers
{
    public class FileHelper
    {
        private string DataPath = string.Empty;
        private List<NeuralData> _neuralData = new List<NeuralData>();

        public void OpenFolder(string _path = null)
        {
            _path = _path ?? Path.Combine(TryGetSolutionDirectoryInfo().FullName, "NeuronData");

            if (Directory.Exists(_path))
            {
                DataPath = _path;
            }
            else
            {
                Directory.CreateDirectory(_path);
                DataPath = _path;
            }
        }

        public bool SaveNeuralData(NeuralData data, string name = null)
        {
            if (Directory.Exists(DataPath))
            {
                string fileName = name ?? DateTime.Now.ToString("yyyyMMddHHmmss");
                string filePath = Path.Combine(DataPath, $"{fileName}_NeuralData.wbn");

                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, json);

                return true;
            }
            else
            {
                Console.WriteLine($"[ERROR] Directory {DataPath} does not exist.");
                return false;
            }
        }

        public List<string> GetAvailableWeightFiles()
        {
            if (Directory.Exists(DataPath))
            {
                return Directory.GetFiles(DataPath, "*_NeuralData.wbn")
                    .Select(Path.GetFileName)
                    .ToList();
            }
            return new List<string>();
        }

        public NeuralData GetNeuralData(string filename)
        {
            if (Directory.Exists(DataPath))
            {
                string filePath = Path.Combine(DataPath, filename);

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"[ERROR] File {filePath} does not exist.");
                    return null;
                }

                var json = File.ReadAllText(filePath);
                dynamic modelData = JsonConvert.DeserializeObject(json);

                var allWeights = JsonConvert.DeserializeObject<List<List<List<double>>>>(modelData.Weights.ToString());
                var allBiases = JsonConvert.DeserializeObject<List<List<double>>>(modelData.Biases.ToString());

                NeuralData data = new NeuralData(allWeights, allBiases);


                return data;
            }
            else
            {
                Console.WriteLine($"[ERROR] Directory {DataPath} does not exist.");
                return null;
            }
        }

        public bool ConvertOldDataToNew(string inputFile, string outputName)
        {
            try
            {
                // Make sure we have valid paths
                if (string.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
                {
                    Console.WriteLine($"[ERROR] Input file {inputFile} does not exist.");
                    return false;
                }

                // Read the JSON file
                string jsonContent = File.ReadAllText(inputFile);

                // Parse the JSON
                using JsonDocument doc = JsonDocument.Parse(jsonContent);
                JsonElement root = doc.RootElement;

                // Get the weights and biases arrays
                JsonElement weightsElement = root.GetProperty("Weights");
                JsonElement biasesElement = root.GetProperty("Biases");

                NeuralData neuralData = new NeuralData();

                // Extract weights and biases
                ExtractWeights(weightsElement, neuralData.Weights);
                ExtractBiases(biasesElement, neuralData.Biases);

                // Save in the standard .wbn format
                SaveNeuralData(neuralData, outputName);

                Console.WriteLine($"Successfully converted {inputFile} to {outputName}_NeuralData.wbn");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting file: {ex.Message}");
                return false;
            }
        }

        private void ExtractWeights(JsonElement weightsElement, List<List<List<double>>> weights)
        {
            // Process each layer
            for (int i = 0; i < weightsElement.GetArrayLength(); i++)
            {
                var layerWeights = weightsElement[i];

                // Extract all weights from this layer
                if (layerWeights.GetArrayLength() > 0)
                {
                    List<List<double>> layerList = new List<List<double>>();

                    foreach (var neuronWeights in layerWeights.EnumerateArray())
                    {
                        List<double> neuronList = new List<double>();
                        foreach (var weight in neuronWeights.EnumerateArray())
                        {
                            neuronList.Add(weight.GetDouble());
                        }
                        layerList.Add(neuronList);
                    }

                    weights.Add(layerList);
                }
            }
        }

        private void ExtractBiases(JsonElement biasesElement, List<List<double>> biases)
        {
            // Process each layer
            for (int i = 0; i < biasesElement.GetArrayLength(); i++)
            {
                var layerBiases = biasesElement[i];

                // Each element in biasesElement is already an array of doubles
                // So we just need to convert it directly to a List<double>
                List<double> layerList = new List<double>();

                foreach (var b in layerBiases.EnumerateArray())
                {
                    layerList.Add(b.GetDouble());
                }

                biases.Add(layerList);
            }
        }

        public static DirectoryInfo TryGetSolutionDirectoryInfo(string currentPath = null)
        {
            var directory = new DirectoryInfo(
                currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            return directory;
        }
    }
}