using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Draughs.NeuralNetwork.Evolution
{
    public class Gene
    {
        private int[] _layers;
        private int _seed;

        public double Score { get; set; } = 0;
        public List<List<Neuron>> Network = new List<List<Neuron>>();

        public Gene(int[] layers, int seed)
        {
            _layers = layers;
            _seed = seed;

            var rnd = new Random(seed);

            for(int j = 1; j < layers.Length; j++)
            {
                var layer = new List<Neuron>();
                for (int i = 0; i < layers[j]; i++)
                {
                    layer.Add(new Neuron(layers[j - 1], rnd.Next(), -0.2, 0.2));
                }
                Network.Add(layer);
            }
        }

        public Gene()
        {
            using (StreamReader reader = new StreamReader("../Network.txt"))
            {
                string line;
                var layerSizes = new List<int>();
                while ((line = reader.ReadLine()) != "__")
                {
                    Int32.TryParse(line, out int layerSize);
                    layerSizes.Add(layerSize);
                }
                _layers = layerSizes.ToArray();

                for (int j = 1; j < _layers.Length; j++)
                {
                    var layer = new List<Neuron>();
                    for (int i = 0; i < _layers[j]; i++)
                    {
                        var neuron = new Neuron(_layers[j - 1], 0, 0.1, 0.1);
                        for (var k = 0; k < neuron.Weights.Count; k++)
                        {
                            double.TryParse(reader.ReadLine(), out double weight);
                            neuron.Weights[k] = weight;
                        }
                        layer.Add(neuron);
                    }
                    Network.Add(layer);
                }
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
                var sums = new List<double>(layer.Count);
                foreach (var neuron in layer)
                {
                    double sum = 0;
                    for (int i = 0; i < input.Count; i++)
                    {
                        neuron.Inputs[i] = input[i];
                        sum += neuron.Weights[i] * input[i];
                    }

                    sum = Sygmoid(sum, false);
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

        private double Sygmoid(double x, bool derivative)
        {
            return derivative ? x * (1 - x) : 1 / (1 + Math.Exp(-x));
        }
    }
}