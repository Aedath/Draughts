using Draughs.NeuralNetwork.Evolution;

namespace Draughs.NeuralNetwork.Services
{
    public class EvolutionService
    {
        public void Evolve(int genes, int[] layers, int generations, double mutationRate)
        {
            var evolver = new NeuralNetworkEvolver(40, new int[] { 32, 40, 10, 1 }, 500, 0.05);
            evolver.EvolvePlayer();
            evolver.SaveBestGeneration();
        }

        public Evaluator GetNetwork()
        {
            return new Evaluator();
        }
    }
}