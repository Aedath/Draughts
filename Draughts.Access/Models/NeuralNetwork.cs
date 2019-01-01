using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Draughts.Access.Models
{
    public class NeuralNetwork
    {
        [Key]
        public int NeuralNetworkId { get; set; }

        public int GenePoolSize { get; set; }
        public int Generation { get; set; }
        public string Network { get; set; }

        public NeuralNetworkViewModel ToViewModel()
        {
            var network = Network.Split('\n').Select(double.Parse).ToArray();
            return new NeuralNetworkViewModel(Generation, network);
        }
    }

    public class NeuralNetworkViewModel
    {
        public NeuralNetworkViewModel(int generation, double[] network)
        {
            Generation = generation;
            Network = network;
        }

        public int Generation { get; set; }
        public double[] Network { get; set; }
    }
}