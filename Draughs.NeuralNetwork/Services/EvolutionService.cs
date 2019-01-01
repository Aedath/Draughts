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

        public IEnumerable<string> EvolveNew()
        {
            var evolver = new NeuralNetworkEvolver(_genePoolSize, new[] { 32, 40, 10, 1 }, _generations, 0.05);
            foreach (var evolution in evolver.EvolvePlayer())
            {
                yield return evolution;
            }
            evolver.SaveBestGeneration();
        }

        public IEnumerable<string> EvolveExisting(double[] network)
        {
            var evolver = new NeuralNetworkEvolver(_genePoolSize, new[] { 32, 40, 10, 1 }, _generations, 0.05, network);
            foreach (var evolution in evolver.EvolvePlayer())
            {
                yield return evolution;
            }
            evolver.SaveBestGeneration();
        }

        public static NeuralNetworkPlayer GetPlayer(double[] network)
        {
            return new NeuralNetworkPlayer(new Gene(new[] { 32, 40, 10, 1 }, network));
        }
    }
}