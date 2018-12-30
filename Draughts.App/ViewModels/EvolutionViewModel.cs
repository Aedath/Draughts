using Draughs.NeuralNetwork.Services;
using Prism.Commands;
using Prism.Regions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Draughts.App.ViewModels
{
    internal class EvolutionViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;

        public EvolutionViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            ToggleProgressCommand = new DelegateCommand(async () => await OnToggleProgress());
        }

        private int _generationsLeft;

        public int GenerationsLeft
        {
            get => _generationsLeft;
            set => SetProperty(ref _generationsLeft, value);
        }

        public int GenePoolSize
        {
            get => _genePoolSize;
            set => SetProperty(ref _genePoolSize, value);
        }

        public int Generations
        {
            get => _generations;
            set => SetProperty(ref _generations, value);
        }

        public bool InProgress
        {
            get => _inProgress;
            set => SetProperty(ref _inProgress, value);
        }

        private string _text = "...";
        private int _genePoolSize = 2;
        private int _generations = 2;
        private bool _inProgress;

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public DelegateCommand ToggleProgressCommand { get; }

        private async Task OnToggleProgress()
        {
            InProgress = !InProgress;
            if (InProgress)
            {
                GenerationsLeft = Generations;
                await Evolve();
            }
        }

        private async Task Evolve()
        {
            if (File.Exists("../Network.txt"))
            {
                File.Copy("../Network.txt", $"../Network {DateTime.Now:yyyyMMddhhmmss}.txt");
            }
            await Task.Run(() =>
            {
                var service = new EvolutionService(GenePoolSize, Generations);
                foreach (var evolutionText in service.Evolve())
                {
                    Text = evolutionText;
                    GenerationsLeft--;
                };
                InProgress = false;
            });
        }
    }
}