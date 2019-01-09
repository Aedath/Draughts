using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Draughs.NeuralNetwork.Evolution
{
    internal class GeneticAlgorithm
    {
        private readonly int _populationSize;
        private readonly int[] _layers;
        private readonly int _generations;
        private readonly double _mutationRate;
        private readonly double _crossoverRate;
        private List<NeuralNetwork> _population = new List<NeuralNetwork>();
        private readonly Random _random = new Random();

        public GeneticAlgorithm(int populationSize, int[] layers, int generations, double mutationRate, double crossoverRate)
        {
            _populationSize = populationSize;
            _layers = layers;
            _generations = generations;
            _mutationRate = mutationRate;
            _crossoverRate = crossoverRate;
            Populate();
        }

        public GeneticAlgorithm(int populationSize, int[] layers, int generations, double mutationRate, double crossoverRate, double[] network)
        {
            _populationSize = populationSize;
            _layers = layers;
            _generations = generations;
            _mutationRate = mutationRate;
            _crossoverRate = crossoverRate;
            Populate(network);
        }

        internal IEnumerable<NeuralNetwork> Evolve()
        {
            for (var i = 0; i < _generations; i++)
            {
                EvaluatePopulation();
                BreedNextGeneration();
                if(i < _generations - 1)
                {
                    yield return GetBestIndividual();
                }
            }
            EvaluatePopulation();
            yield return GetBestIndividual();
        }

        private void BreedNextGeneration()
        {
            var newGen = new List<NeuralNetwork>();

            for (var i = 0; i < _populationSize; i++)
            {
                newGen.Add(i < 5 ? _population[i] : Crossbreed(SelectParent(), SelectParent()));
            }

            _population = new List<NeuralNetwork>(newGen);
        }

        private NeuralNetwork GetBestIndividual()
        {
            var bestGene = _population.OrderByDescending(x => x.Fitness).ToList()[0];

            return bestGene;
        }

        private NeuralNetwork Crossbreed(NeuralNetwork mother, NeuralNetwork father)
        {
            var child = new NeuralNetwork(_layers, 1);
            for (var layer = 0; layer < child.Network.Count; layer++)
            {
                for (var neuron = 0; neuron < child.Network[layer].Count; neuron++)
                {
                    for (var weight = 0; weight < child.Network[layer][neuron].Weights.Count; weight++)
                    {
                        var weight1 = father.Network[layer][neuron].Weights[weight];
                        var weight2 = mother.Network[layer][neuron].Weights[weight];
                        if (_random.NextDouble() < _mutationRate)
                        {
                            child.Network[layer][neuron].Weights[weight] = RandomWeight(0.9, -0.9);
                        }
                        else if (_random.NextDouble() < _crossoverRate)
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

        private NeuralNetwork SelectParent()
        {
            var cumulativeProbability = _random.NextDouble();

            return _population.First(x => (cumulativeProbability -= x.Fitness) <= 0);
        }

        private void EvaluatePopulation()
        {
            Parallel.For(0, _populationSize - 1, (i) =>
            {
                Parallel.For(0, _populationSize - 1, (j) =>
                {
                    if (i == j)
                    {
                        return;
                    }

                    var individual1 = _population[i];
                    var individual2 = _population[j];
                    CalculateFitness(ref individual1, ref individual2);
                });
            });

            RankPopulation();
        }

        private void RankPopulation()
        {
            _population = _population.OrderByDescending(x => x.Fitness).ToList();

            double min = (_populationSize ^ 2) - _populationSize;
            var max = 2 * min;

            var populationMin = _population.Min(x => x.Fitness);
            var populationMax = _population.Max(x => x.Fitness);

            min = min < populationMin ? min : populationMin;
            max = max > populationMax ? max : populationMax;

            _population.ForEach(x => x.Fitness = (x.Fitness - min) / (max - min));

            var sum = _population.Sum(x => x.Fitness);

            _population.ForEach(x => x.Fitness /= sum);
        }

        private static void CalculateFitness(ref NeuralNetwork individual1, ref NeuralNetwork individual2)
        {
            var player1 = new NeuralNetworkPlayer(individual1);
            var player2 = new NeuralNetworkPlayer(individual2);

            var game = new Game(player1, player2);
            if (game.Draw)
            {
                individual1.Fitness -= 10;
                individual2.Fitness -= 10;
            }
            else
            {
                ((NeuralNetworkPlayer)game.Winner).Network.Fitness += game.PlayerPeaces(game.Winner) + (game.PlayerQueens(game.Winner) * 2) + 10;
                ((NeuralNetworkPlayer)game.Loser).Network.Fitness -= 10;
            }
        }

        private void Populate()
        {
            for (var i = 0; i < _populationSize; i++)
            {
                _population.Add(new NeuralNetwork(_layers, i));
            }
        }

        private void Populate(double[] network)
        {
            for (var i = 0; i < _populationSize; i++)
            {
                _population.Add(new NeuralNetwork(_layers, network));
            }
        }
    }
}