using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughs.NeuralNetwork.Evolution
{
    public class Neuron
    {
        public Neuron()
        {
        }

        public Neuron(int layerSize, int seed, double min, double max)
        {
            var neuronSeed = new Random(seed);
            for (var i = 0; i < layerSize; i++)
            {
                Weights.Add(neuronSeed.NextDouble() * (max - min) + min);
            }
            Weights.Add(neuronSeed.NextDouble());
        }

        public List<double> Weights { get; internal set; } = new List<double>();
        public double Output { get; internal set; }

        public void SetInput(List<double> input)
        {
            var sum = input.Select((t, i) => Weights[i] * t).Sum();

            Output = Sygmoid(sum);
        }

        private static double Sygmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }
    }
}