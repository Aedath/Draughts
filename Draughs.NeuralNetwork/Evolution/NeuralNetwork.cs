using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughs.NeuralNetwork.Evolution
{
    public class NeuralNetwork
    {
        public double Fitness { get; set; } = 0;
        public List<List<Neuron>> Network = new List<List<Neuron>>();

        public NeuralNetwork(int[] layers, int seed)
        {
            var rnd = new Random(seed);

            for (var j = 1; j < layers.Length; j++)
            {
                var layer = new List<Neuron>();
                for (var i = 0; i < layers[j]; i++)
                {
                    layer.Add(new Neuron(layers[j - 1], rnd.Next(), -0.2, 0.2));
                }
                Network.Add(layer);
            }
        }

        public NeuralNetwork(int[] layers, double[] weights)
        {
            var node = 0;
            for (var j = 1; j < layers.Length; j++)
            {
                var layer = new List<Neuron>();
                for (var i = 0; i < layers[j]; i++)
                {
                    var neuron = new Neuron();
                    for (var k = 0; k < layers[j - 1]; k++)
                    {
                        var weight = weights[node];
                        neuron.Weights.Add(weight);
                        node++;
                    }
                    neuron.Weights.Add(1);
                    layer.Add(neuron);
                }
                Network.Add(layer);
            }
        }

        public List<double> GetNetworkResult(List<double> board)
        {
            var input = new List<double>(board)
            {
                1 //bias
            };
            foreach (var layer in Network)
            {
                foreach (var neuron in layer)
                {
                    neuron.SetInput(input);
                }

                input = new List<double>(layer.Select(x => x.Output))
                {
                    1 //bias
                };
            }

            return Network.Last().Select(x => x.Output).ToList();
        }
    }
}