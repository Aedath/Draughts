using Draughs.NeuralNetwork.Services;
using Draughts.App.Infrastructure.Services;
using Prism.Commands;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Draughts.App.ViewModels
{
    internal class EvolutionViewModel : ViewModelBase
    {
        private readonly IAccessService _accessService;

        public EvolutionViewModel(IAccessService accessService)
        {
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

        private int _genePoolSize = 20;
        private int _generations = 2;
        private bool _inProgress;
        private int _selectedGeneration = -1;

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
            if (SelectedGeneration == -1)
            {
                await Task.Run(async ()=> await EvolveNew());
                return;
            }

            var network = await _accessService.GetByGeneration(SelectedGeneration);
            await Task.Run(async () => await EvolveExisting(network.Network));
        }

        private async Task EvolveNew()
        {
            var service = new EvolutionService(GenePoolSize, Generations);
            List<double> lastGene = new List<double>();
            foreach (var evolution in service.EvolveNew())
            {
                lastGene = evolution.Network.SelectMany(n => n.SelectMany(l => l.Weights)).ToList();
                GenerationsLeft--;
            }

            await _accessService.AddNeuralNetwork(Generations, lastGene.ToArray());

            InProgress = false;
        }

        private async Task EvolveExisting(double[] network)
        {
            var service = new EvolutionService(GenePoolSize, Generations);
            List<double> lastGene = new List<double>();
            foreach (var evolution in service.EvolveExisting(network))
            {
                lastGene = evolution.Network.SelectMany(n => n.SelectMany(l => l.Weights)).ToList();
                GenerationsLeft--;
            }
            await _accessService.AddNeuralNetwork(Generations + SelectedGeneration, lastGene.ToArray());
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