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

        public IEnumerable<string> Evolve()
        {
            var evolver = new NeuralNetworkEvolver(_genePoolSize, new[] { 32, 40, 10, 1 }, _generations, 0.05);
            foreach (var evolution in evolver.EvolvePlayer())
            {
                yield return evolution;
            }
            evolver.SaveBestGeneration();
        }

        public static NeuralNetworkPlayer GetPlayer()
        {
            return new NeuralNetworkPlayer(new Gene());
        }
    }
}