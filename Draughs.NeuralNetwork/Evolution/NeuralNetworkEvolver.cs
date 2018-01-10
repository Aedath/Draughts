using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Draughs.NeuralNetwork.Evolution
{
    internal class NeuralNetworkEvolver
    {
        public NeuralNetworkEvolver(int genePoolSize, int[] layers, int generations, double mutationRate)
        {
            _genePoolSize = genePoolSize;
            _layers = layers;
            _generations = generations;
            _mutationRate = mutationRate;
            PopulateGenePool();
        }

        internal void EvolvePlayer()
        {
            for (int i = 0; i < _genePoolSize; i++)
            {
                EvaluateGenePool();
                BreedNextGeneration();
            }
            EvaluateGenePool();
        }

        private void BreedNextGeneration()
        {
            var newGen = new List<Evaluator>();

            for (int i = 0; i < _genePoolSize; i++)
            {
                if (i < 5)
                {
                    newGen.Add(_genePool[i]);
                }
                else
                {
                    newGen.Add(Crossbreed(SelectParent(), SelectParent()));
                }
            }
        }

        public void SaveBestGeneration()
        {
            var bestGene = _genePool.OrderByDescending(x => x.Score).ToList()[0];
            using (StreamWriter writer = new StreamWriter("../Network.txt", false) { AutoFlush = true })
            {
                foreach (var layer in _layers)
                {
                    writer.WriteLine(layer);
                }
                writer.WriteLine("__");
                foreach(var layer in bestGene.Network)
                {
                    foreach (var neuron in layer)
                    {
                        foreach (var weight in neuron.Weights)
                        {
                            writer.WriteLine(weight);
                        }
                    }
                }
            }
        }

        private Evaluator Crossbreed(Evaluator evaluator1, Evaluator evaluator2)
        {
            var child = new Evaluator(_layers, 1);
            var layerCount = 0;
            foreach (var layer in evaluator1.Network)
            {
                var neuronCount = 0;
                foreach (var neuron in layer)
                {
                    var weightCount = 0;
                    foreach (var weight in neuron.Weights)
                    {
                        if (_random.NextDouble() < _mutationRate)
                        {
                            SetWeight(child, layerCount, neuronCount, weightCount, RandomWeight(0.9, -0.9));
                        }
                        else if (_random.NextDouble() < 0.5)
                        {
                            SetWeight(child, layerCount, neuronCount, weightCount, evaluator1.Network[layerCount][neuronCount].Weights[weightCount]);
                        }
                        else
                        {
                            SetWeight(child, layerCount, neuronCount, weightCount, evaluator2.Network[layerCount][neuronCount].Weights[weightCount]);
                        }
                        weightCount++;
                    }
                    neuronCount++;
                }
                layerCount++;
            }

            return child;
        }

        private void SetWeight(Evaluator evaluator, int layerId, int neuronId, int weightId, double weight)
        {
            evaluator.Network[layerId][neuronId].Weights[weightId] = weight;
        }

        private double RandomWeight(double max, double min)
        {
            return _random.NextDouble() * (max - min) + min;
        }

        private Evaluator SelectParent()
        {
            var cumulatedPropobility = _random.NextDouble();

            return _genePool.First(x => (cumulatedPropobility -= x.Score) <= 0);
        }

        private void EvaluateGenePool()
        {
            for (int i = 0; i < _genePoolSize; i++)
            {
                for (int j = 0; j < _genePoolSize; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var evaluator1 = _genePool[i];
                    var evaluator2 = _genePool[j];
                    Debug.WriteLine($"----------------------\nPlaying {i} {j} game\n---------------------");
                    Play(ref evaluator1, ref evaluator2);
                }
            }

            RankGenePool();
        }

        private void RankGenePool()
        {
            _genePool.OrderByDescending(x => x.Score);

            double min = ((_genePoolSize ^ 2) - _genePoolSize);
            double max = 2 * min;

            double genePoolMin = _genePool.Min(x => x.Score);
            double genePoolMax = _genePool.Max(x => x.Score);

            min = min < genePoolMin ? min : genePoolMin;
            max = max > genePoolMax ? max : genePoolMax;

            _genePool.ForEach(x => x.Score = (x.Score - min) / (max - min));

            var sum = _genePool.Sum(x => x.Score);

            _genePool.ForEach(x => x.Score /= sum);
        }
        
        private void Play(ref Evaluator evaluator1, ref Evaluator evaluator2)
        {
            var player1 = new NeuralNetworkPlayer(evaluator1);
            var player2 = new NeuralNetworkPlayer(evaluator2);

            var game = new Game(player1, player2);
            if(game.Draw)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Debug.Write(game.Board[8 * i + j]);
                    }
                    Debug.WriteLine("");
                }
                evaluator1.Score -= 1;
                evaluator2.Score -= 1;
            }
            else
            {
                (game.Winner as NeuralNetworkPlayer).Evaluator.Score += 1;
                (game.Loser as NeuralNetworkPlayer).Evaluator.Score -= 2;
            }
        }

        private void PopulateGenePool()
        {
            for (int i = 0; i < _genePoolSize; i++)
            {
                _genePool.Add(new Evaluator(_layers, i));
            }
        }

        private int _genePoolSize { get; }
        private int[] _layers { get; }
        private int _generations { get; }
        private double _mutationRate { get; }
        private List<Evaluator> _genePool = new List<Evaluator>();
        private Random _random = new Random();
    }
}