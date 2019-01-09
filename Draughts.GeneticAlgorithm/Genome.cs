using System;

namespace Draughts.GeneticAlgorithm
{
    internal class Genome
    {
        private readonly int _count;
        private static Random _random = new Random();

        public Genome(int count, bool createGenes)
        {
            _count = count;
            Genes = new double[count];
            if (createGenes)
            {
                CreateGenes(); 
            }
        }

        public Genome(double[] genes)
        {
            _count = genes.Length;
            Genes = genes;
        }

        public double MutationRate { get; set; }
        public double Fitness { get; set; }
        public double[] Genes { get; }

        public Genome DeepCopy()
        {
            Genome g = new Genome(_count, false);
            Array.Copy(Genes, g.Genes, _count);
            return g;
        }

        private void CreateGenes()
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                Genes[i] = (_random.NextDouble() + _random.Next(-20, 20));
            }
        }

        public void Crossover(ref Genome genome2, out Genome child1, out Genome child2)
        {
            int pos = (int)(_random.NextDouble() * _count);
            child1 = new Genome(_count, false);
            child2 = new Genome(_count, false);
            for (int i = 0; i < _count; i++)
            {
                if (i < pos)
                {
                    child1.Genes[i] = Genes[i];
                    child2.Genes[i] = genome2.Genes[i];
                }
                else
                {
                    child1.Genes[i] = genome2.Genes[i];
                    child2.Genes[i] = Genes[i];
                }
            }
        }

        public void Mutate()
        {
            for (int pos = 0; pos < _count; pos++)
            {
                if (_random.NextDouble() < MutationRate)
                    Genes[pos] = (Genes[pos] + (_random.NextDouble() + _random.Next(-20, 20))) / 2.0;
            }
        }
    }
}
