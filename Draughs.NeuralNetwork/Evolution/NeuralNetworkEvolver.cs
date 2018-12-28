using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        internal IEnumerable<string> EvolvePlayer()
        {
            for (var i = 0; i < _generations; i++)
            {
                Console.WriteLine($"----------------------\n{i}");
                var s = new Stopwatch();
                s.Restart();
                EvaluateGenePool();
                BreedNextGeneration();
                s.Stop();
                var returnText =
                    $"{i + 1}/{_generations} generation evolved in {s.Elapsed:g}.\n ETA {GetEta(s.Elapsed, _generations - i - 1):g}";
                yield return returnText;
            }
            EvaluateGenePool();
        }

        private static TimeSpan GetEta(TimeSpan lastTimeSpan, int iterationsLeft)
        {
            var eta = new TimeSpan();
            for (var i = 0; i < iterationsLeft; i++)
            {
                eta = eta.Add(lastTimeSpan);
            }

            return eta;
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

        public void SaveBestGeneration()
        {
            var bestGene = _genePool.OrderByDescending(x => x.Score).ToList()[0];
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
            var layerCount = 0;
            foreach (var layer in gene1.Network)
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
                            SetWeight(child, layerCount, neuronCount, weightCount, gene1.Network[layerCount][neuronCount].Weights[weightCount]);
                        }
                        else
                        {
                            SetWeight(child, layerCount, neuronCount, weightCount, gene2.Network[layerCount][neuronCount].Weights[weightCount]);
                        }
                        weightCount++;
                    }
                    neuronCount++;
                }
                layerCount++;
            }

            return child;
        }

        private static void SetWeight(Gene gene, int layerId, int neuronId, int weightId, double weight)
        {
            gene.Network[layerId][neuronId].Weights[weightId] = weight;
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
            var gameCounter = 0;
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
                    gameCounter++;
                    //Debug.WriteLine($"Games played: {gameCounter}. Gene combination: {i} {j}");
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
                gene1.Score -= 1;
                gene2.Score -= 1;
            }
            else
            {
                ((NeuralNetworkPlayer)game.Winner).Gene.Score += 1;
                ((NeuralNetworkPlayer)game.Loser).Gene.Score -= 2;
            }
        }

        private void PopulateGenePool()
        {
            for (var i = 0; i < _genePoolSize; i++)
            {
                _genePool.Add(new Gene(_layers, i));
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