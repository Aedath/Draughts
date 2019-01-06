using Draughs.NeuralNetwork.Evolution;
using System.Collections.Generic;

namespace Draughs.NeuralNetwork.Services
{
    public class EvolutionService
    {
        private readonly int _genePoolSize;
        private readonly int _generations;

        public EvolutionService(int genePoolSize, int generations)
        {
            _genePoolSize = genePoolSize;
            _generations = generations;
        }

        public IEnumerable<Gene> EvolveNew()
        {
            var evolver = new NeuralNetworkEvolver(_genePoolSize, new[] { 32, 40, 10, 1 }, _generations, 0.05);
            foreach (var evolution in Evolve(evolver))
            {
                yield return evolution;
            }
        }

        public IEnumerable<Gene> EvolveExisting(double[] network)
        {
            var evolver = new NeuralNetworkEvolver(_genePoolSize, new[] { 32, 40, 10, 1 }, _generations, 0.05, network);
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

        private IEnumerable<Gene> Evolve(NeuralNetworkEvolver evolver)
        {
            try
            {
                foreach (var evolution in evolver.EvolvePlayer())
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
            return new NeuralNetworkPlayer(new Gene(new[] { 32, 40, 10, 1 }, network));
        }
    }
}