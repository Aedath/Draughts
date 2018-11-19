using System;
using System.Collections.Generic;

namespace Draughs.NeuralNetwork.Evolution
{
    public class Neuron
    {
        public List<double> Weights = new List<double>();

        public Neuron(int layerSize, int seed, double min, double max)
        {
            var neuronSeed = new Random(seed);
            Inputs = new List<double>();
            for (int i = 0; i < layerSize; i++)
            {
                Weights.Add(neuronSeed.NextDouble() * (max - min) + min);
                Inputs.Add(0);
            }
            Weights.Add(neuronSeed.NextDouble());
            Inputs.Add(0);
        }

        public List<double> Inputs { get; internal set; }
        public double Output { get; internal set; }
    }
}