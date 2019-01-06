using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughs.NeuralNetwork.Evolution
{
    public class Gene
    {
        public double Score { get; set; } = 0;
        public List<List<Neuron>> Network = new List<List<Neuron>>();

        public Gene(int[] layers, int seed)
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

        public Gene(int[] layers, double[] network)
        {
            var node = 0;
            for (var j = 1; j < layers.Count(); j++)
            {
                var layer = new List<Neuron>();
                for (var i = 0; i < layers[j]; i++)
                {
                    var neuron = new Neuron(layers[j - 1], 0, 0.1, 0.1);
                    for (var k = 0; k < neuron.Weights.Count; k++)
                    {
                        var weight = network[node];
                        neuron.Weights[k] = weight;
                        node++;
                    }
                    layer.Add(neuron);
                }
                Network.Add(layer);
            }
        }

        public List<double> GetNetworkResult(List<double> board)
        {
            var input = new List<double>(board)
            {
                1
            };
            var output = new List<double>();
            foreach (var layer in Network)
            {
                foreach (var neuron in layer)
                {
                    double sum = 0;
                    for (var i = 0; i < input.Count; i++)
                    {
                        neuron.Inputs[i] = input[i];
                        sum += neuron.Weights[i] * input[i];
                    }

                    sum = Sygmoid(sum);
                    neuron.Output = sum;
                }

                if (ReferenceEquals(layer, Network.Last()))
                {
                    output.AddRange(layer.Select(x => x.Output));
                }
                else
                {
                    input = new List<double>(layer.Select(x => x.Output))
                    {
                        1 //bias
                    };
                }
            }

            return output;
        }

        private static double Sygmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }
    }
}