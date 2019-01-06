using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public NeuralNetworkEvolver(int genePoolSize, int[] layers, int generations, double mutationRate, double[] network)
        {
            _genePoolSize = genePoolSize;
            _layers = layers;
            _generations = generations;
            _mutationRate = mutationRate;
            PopulateGenePool(network);
        }

        internal IEnumerable<Gene> EvolvePlayer()
        {
            for (var i = 0; i < _generations; i++)
            {
                EvaluateGenePool();
                BreedNextGeneration();
                yield return GetBestGene();
            }
        }

        private void BreedNextGeneration()
        {
            var newGen = new List<Gene>();

            for (var i = 0; i < _genePoolSize; i++)
            {
                newGen.Add(i < 5 ? _genePool[i] : Crossbreed(SelectParent(), SelectParent()));
            }

            _genePool = new List<Gene>(newGen);
        }

        public Gene GetBestGene()
        {
            var bestGene = _genePool.OrderByDescending(x => x.Score).ToList()[0];

            return bestGene;
        }

        public void SaveBestGeneration()
        {
            var bestGene = GetBestGene();
            using (var writer = new StreamWriter("../Network.txt", false) { AutoFlush = true })
            {
                foreach (var layer in _layers)
                {
                    writer.WriteLine(layer);
                }
                writer.WriteLine("__");
                foreach (var layer in bestGene.Network)
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

        private Gene Crossbreed(Gene gene1, Gene gene2)
        {
            var child = new Gene(_layers, 1);
            for (var layer = 0; layer < child.Network.Count; layer++)
            {
                for (var neuron = 0; neuron < child.Network[layer].Count; neuron++)
                {
                    for (var weight = 0; weight < child.Network[layer][neuron].Weights.Count; weight++)
                    {
                        var weight1 = gene1.Network[layer][neuron].Weights[weight];
                        var weight2 = gene2.Network[layer][neuron].Weights[weight];
                        if (_random.NextDouble() < _mutationRate)
                        {
                            child.Network[layer][neuron].Weights[weight] = RandomWeight(0.9, -0.9);
                        }
                        else if (_random.NextDouble() < 0.5)
                        {
                            child.Network[layer][neuron].Weights[weight] = weight1;
                        }
                        else
                        {
                            child.Network[layer][neuron].Weights[weight] = weight2;
                        }
                    }
                }
            }

            return child;
        }

        private double RandomWeight(double max, double min)
        {
            return _random.NextDouble() * (max - min) + min;
        }

        private Gene SelectParent()
        {
            var cumulatedPropobility = _random.NextDouble();

            return _genePool.First(x => (cumulatedPropobility -= x.Score) <= 0);
        }

        private void EvaluateGenePool()
        {
            Parallel.For(0, _genePoolSize - 1, (i) =>
            {
                Parallel.For(0, _genePoolSize - 1, (j) =>
                {
                    if (i == j)
                    {
                        return;
                    }

                    var gene1 = _genePool[i];
                    var gene2 = _genePool[j];
                    Play(ref gene1, ref gene2);
                });
            });

            RankGenePool();
        }

        private void RankGenePool()
        {
            _genePool = _genePool.OrderByDescending(x => x.Score).ToList();

            double min = ((_genePoolSize ^ 2) - _genePoolSize);
            var max = 2 * min;

            var genePoolMin = _genePool.Min(x => x.Score);
            var genePoolMax = _genePool.Max(x => x.Score);

            min = min < genePoolMin ? min : genePoolMin;
            max = max > genePoolMax ? max : genePoolMax;

            _genePool.ForEach(x => x.Score = (x.Score - min) / (max - min));

            var sum = _genePool.Sum(x => x.Score);

            _genePool.ForEach(x => x.Score /= sum);
        }

        private static void Play(ref Gene gene1, ref Gene gene2)
        {
            var player1 = new NeuralNetworkPlayer(gene1);
            var player2 = new NeuralNetworkPlayer(gene2);

            var game = new Game(player1, player2);
            if (game.Draw)
            {
                gene1.Score -= 10;
                gene2.Score -= 10;
            }
            else
            {
                ((NeuralNetworkPlayer)game.Winner).Gene.Score += game.PlayerPeaces(game.Winner) + (game.PlayerQueens(game.Winner) * 2) + 10;
                ((NeuralNetworkPlayer)game.Loser).Gene.Score -= 10;
            }
        }

        private void PopulateGenePool()
        {
            for (var i = 0; i < _genePoolSize; i++)
            {
                _genePool.Add(new Gene(_layers, i));
            }
        }

        private void PopulateGenePool(double[] network)
        {
            for (var i = 0; i < _genePoolSize; i++)
            {
                _genePool.Add(new Gene(_layers, network));
            }
        }

        private readonly int _genePoolSize;
        private readonly int[] _layers;
        private readonly int _generations;
        private readonly double _mutationRate;
        private List<Gene> _genePool = new List<Gene>();
        private readonly Random _random = new Random();
    }
}