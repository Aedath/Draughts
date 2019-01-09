using Draughs.NeuralNetwork.Evolution;
using System.Collections.Generic;

namespace Draughs.NeuralNetwork.Services
{
    public class EvolutionService
    {
        private readonly int _populationSize;
        private readonly int _generations;

        public EvolutionService(int populationSize, int generations)
        {
            _populationSize = populationSize;
            _generations = generations;
        }

        public IEnumerable<Evolution.NeuralNetwork> EvolveNew()
        {
            var evolver = new GeneticAlgorithm(_populationSize, new[] { 32, 40, 10, 1 }, _generations, 0.05, 0.5);
            foreach (var evolution in Evolve(evolver))
            {
                yield return evolution;
            }
        }

        public IEnumerable<Evolution.NeuralNetwork> EvolveExisting(double[] network)
        {
            var evolver = new GeneticAlgorithm(_populationSize, new[] { 32, 40, 10, 1 }, _generations, 0.05, 0.5, network);
            foreach (var evolution in Evolve(evolver))
            {
                yield return evolution;
            }
        }

        private bool _stop;

        public void Stop()
        {
            _stop = true;
        }

        private IEnumerable<Evolution.NeuralNetwork> Evolve(GeneticAlgorithm evolver)
        {
            try
            {
                foreach (var evolution in evolver.Evolve())
                {
                    yield return evolution;
                    if (_stop)
                    {
                        break;
                    }
                }
            }
            finally
            {
                _stop = false;
            }
        }

        public static NeuralNetworkPlayer GetPlayer(double[] network)
        {
            return new NeuralNetworkPlayer(new Evolution.NeuralNetwork(new[] { 32, 40, 10, 1 }, network));
        }
    }
}