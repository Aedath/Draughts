using System.ComponentModel.DataAnnotations;

namespace Draughts.Access.Models
{
    public class NeuralNetwork
    {
        [Key]
        public int NeuralNetworkId { get; set; }

        public int GenePoolSize { get; set; }
        public int Generation { get; set; }
        public string Network { get; set; }
    }
}