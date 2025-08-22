namespace Aesclea_Back_End_.DDOs
{
    public class RecurrentNeuralData
    {
        // Parameters for recurrent layers
        public List<List<List<double>>> RecurrentLayerWeights { get; set; }
        public List<List<double>> RecurrentLayerBiases { get; set; }
        public List<List<List<double>>> RecurrentWeights { get; set; }

        // Parameters for output layers
        public List<List<List<double>>> OutputLayerWeights { get; set; }
        public List<List<double>> OutputLayerBiases { get; set; }

        public RecurrentNeuralData()
        {
            RecurrentLayerWeights = new List<List<List<double>>>();
            RecurrentLayerBiases = new List<List<double>>();
            RecurrentWeights = new List<List<List<double>>>();
            OutputLayerWeights = new List<List<List<double>>>();
            OutputLayerBiases = new List<List<double>>();
        }

        public RecurrentNeuralData(
            List<List<List<double>>> recurrentLayerWeights,
            List<List<double>> recurrentLayerBiases,
            List<List<List<double>>> recurrentWeights,
            List<List<List<double>>> outputLayerWeights,
            List<List<double>> outputLayerBiases)
        {
            RecurrentLayerWeights = recurrentLayerWeights;
            RecurrentLayerBiases = recurrentLayerBiases;
            RecurrentWeights = recurrentWeights;
            OutputLayerWeights = outputLayerWeights;
            OutputLayerBiases = outputLayerBiases;
        }
    }
}
