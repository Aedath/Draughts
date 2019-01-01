using Draughts.App.Infrastructure.Services;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;

namespace Draughts.App.ViewModels
{
    internal class StatisticsViewModel : ViewModelBase
    {
        private readonly IAccessService _accessService;

        public StatisticsViewModel(IAccessService accessService)
        {
            _accessService = accessService;
        }

        public ObservableCollection<StatisticsModel> Statistics { get; set; } = new ObservableCollection<StatisticsModel>();

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            var results = await _accessService.GetGameResults();
            Statistics.Clear();
            var statistics = results
                .GroupBy(x => x.Generation)
                .Select(x => new StatisticsModel(x.Key,
                    x.Count(r => r.Score == 1),
                    x.Count(r => r.Score == 0)))
                .ToList();

            Statistics.AddRange(statistics);
        }
    }

    public class StatisticsModel : BindableBase
    {
        public StatisticsModel(int generation, int wins, int losses)
        {
            Generation = generation;
            Wins = wins;
            Losses = losses;
        }

        private int _generation;
        private int _wins;
        private int _losses;

        public int Generation
        {
            get => _generation;
            set => SetProperty(ref _generation, value);
        }

        public int Wins
        {
            get => _wins;
            set => SetProperty(ref _wins, value);
        }

        public int Losses
        {
            get => _losses;
            set => SetProperty(ref _losses, value);
        }
    }
}