using Draughs.NeuralNetwork.Evolution;
using System.Collections.Generic;

namespace Draughs.NeuralNetwork.Services
{
    public class EvolutionService
    {
        public IEnumerable<string> Evolve()
        {
            var evolver = new NeuralNetworkEvolver(40, new[] { 32, 40, 10, 1 }, 500, 0.05);
            foreach (var evolution in evolver.EvolvePlayer())
            {
                yield return evolution;
            }
            evolver.SaveBestGeneration();
            yield return "Evolution is finished";
        }

        public NeuralNetworkPlayer GetPlayer()
        {
            return new NeuralNetworkPlayer(new Gene());
        }
    }
}