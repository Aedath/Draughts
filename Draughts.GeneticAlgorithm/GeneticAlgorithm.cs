using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Draughts.GeneticAlgorithm
{
    public class GeneticAlgorithm
    {
        private readonly double _crossoverRate;
        private readonly double _mutationRate;
        private readonly int _populationSize;
        private readonly int _epochLength;
        private readonly int _weights;
        private readonly bool _elitism;
        private readonly Func<double[], double> _fitnessFunction;
        private List<double> _fitnessTable;
        private List<Genome> _currentGeneration;
        private List<Genome> _nextGeneration;
        private double _totalFitness;
        private static Random _random = new Random();

        public GeneticAlgorithm(double crossoverRate, double mutationRate, int populationSize, int epochLength, int weights, Func<double[], double> fitnessFunction, bool elitism = false)
        {
            _crossoverRate = crossoverRate;
            _mutationRate = mutationRate;
            _populationSize = populationSize;
            _epochLength = epochLength;
            _weights = weights;
            _elitism = elitism;
            _fitnessFunction = fitnessFunction;
        }

        public void Evolve()
        {
            _fitnessTable = new List<double>();
            _currentGeneration = new List<Genome>(_epochLength);
            _nextGeneration = new List<Genome>(_epochLength);

            CreateGenomes();
            RankPopulation();

            for (int i = 0; i < _epochLength; i++)
            {
                CreateNextGeneration();
                var fitness = RankPopulation();

                if (i % 100 == 0)
                {
                    Console.WriteLine("Generation " + i + ", Best Fitness: " + fitness);
                }
            }
        }
        private void CreateGenomes()
        {
            for (int i = 0; i < _populationSize; i++)
            {
                var genome = new Genome(_epochLength, true) {MutationRate = _mutationRate};
                _currentGeneration.Add(genome);
            }
        }

        private double RankPopulation()
        {
            _totalFitness = 0.0;
            foreach (var genome in _currentGeneration)
            {
                genome.Fitness = _fitnessFunction(genome.Genes);
                _totalFitness += genome.Fitness;
            }

            _currentGeneration.OrderByDescending(x => x.Fitness);

            double fitness = 0.0;
            _fitnessTable.Clear();

            foreach (Genome t in _currentGeneration)
            {
                fitness += t.Fitness;
                _fitnessTable.Add(t.Fitness);
            }

            return _fitnessTable[_fitnessTable.Count - 1];
        }

        private void CreateNextGeneration()
        {
            _nextGeneration.Clear();
            Genome genome = null;
            if (_elitism)
            {
                genome = _currentGeneration[_populationSize - 1].DeepCopy();
            }

            for (int i = 0; i < _populationSize; i += 2)
            {
                int pidx1 = RouletteSelection();
                int pidx2 = RouletteSelection();
                var parent1 = _currentGeneration[pidx1];
                var parent2 = _currentGeneration[pidx2];

                Genome child1, child2;
                if (_random.NextDouble() < _crossoverRate)
                {
                    parent1.Crossover(ref parent2, out child1, out child2);
                }
                else
                {
                    child1 = parent1;
                    child2 = parent2;
                }
                child1.Mutate();
                child2.Mutate();

                _nextGeneration.Add(child1);
                _nextGeneration.Add(child2);
            }
            if (_elitism && genome != null)
            {
                _nextGeneration[0] = genome;
            }

            _currentGeneration.Clear();
            foreach (Genome newGenome in _nextGeneration)
            {
                _currentGeneration.Add(newGenome);
            }
        }

        private int RouletteSelection()
        {
            var randomFitness = _random.NextDouble() * _totalFitness;
            var idx = -1;
            var first = 0;
            var last = _populationSize - 1;
            var mid = (last - first) / 2;

            while (idx == -1 && first <= last)
            {
                if (randomFitness < _fitnessTable[mid])
                {
                    last = mid;
                }
                else if (randomFitness > _fitnessTable[mid])
                {
                    first = mid;
                }
                mid = (first + last) / 2;

                if ((last - first) == 1)
                {
                    idx = last;
                }
            }
            return idx;
        }
    }
}
