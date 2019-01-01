using Draughs.NeuralNetwork.Services;
using Draughts.App.Infrastructure.Services;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Draughts.App.ViewModels
{
    internal class EvolutionViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IAccessService _accessService;

        public EvolutionViewModel(IRegionManager regionManager, IAccessService accessService)
        {
            _regionManager = regionManager;
            _accessService = accessService;
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

        public ObservableCollection<int> ExistingGenerations { get; set; } = new ObservableCollection<int>();

        public int SelectedGeneration
        {
            get => _selectedGeneration;
            set => SetProperty(ref _selectedGeneration, value);
        }

        private string _text = "...";
        private int _genePoolSize = 2;
        private int _generations = 2;
        private bool _inProgress;
        private int _selectedGeneration = -1;

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
                File.Delete("../Network.txt");
            }

            if (SelectedGeneration == -1)
            {
                await Task.Run(() => EvolveNew());
                return;
            }

            var network = await _accessService.GetByGeneration(SelectedGeneration);
            await Task.Run(() => EvolveExisting(network.Network));
        }

        private void EvolveNew()
        {
            var service = new EvolutionService(GenePoolSize, Generations);
            foreach (var evolutionText in service.EvolveNew())
            {
                Text = evolutionText;
                GenerationsLeft--;
            };
            InProgress = false;
        }

        private void EvolveExisting(double[] network)
        {
            var service = new EvolutionService(GenePoolSize, Generations);
            foreach (var evolutionText in service.EvolveExisting(network))
            {
                Text = evolutionText;
                GenerationsLeft--;
            };
            InProgress = false;
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            var generations = await _accessService.GetGenerations();

            ExistingGenerations.Clear();
            ExistingGenerations.AddRange(generations);
        }
    }
}