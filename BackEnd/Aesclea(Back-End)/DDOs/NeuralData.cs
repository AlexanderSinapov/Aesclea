namespace Aesclea_Back_End_.DDOs
{
    public class NeuralData
    {
        public List<List<List<double>>> Weights { get; set; } = new List<List<List<double>>>();
        public List<List<double>> Biases { get; set; } = new List<List<double>>();
        public NeuralData() { }
        public NeuralData(List<List<List<double>>> weights, List<List<double>> biases)
        {
            Weights = weights;
            Biases = biases;
        }
    }
}
