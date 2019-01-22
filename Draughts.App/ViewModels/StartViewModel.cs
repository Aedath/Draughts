using Draughts.App.Views;
using Prism.Commands;
using Prism.Regions;

namespace Draughts.App.ViewModels
{
    internal class StartViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        public DelegateCommand DemoGameCommand { get; }
        public DelegateCommand StartNewGameCommand { get; }
        public DelegateCommand EvolveNetworkCommand { get; }
        public DelegateCommand StatisticsCommand { get; }

        public StartViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            DemoGameCommand = new DelegateCommand(() =>
                _regionManager.RequestNavigate("MainRegion", nameof(DemoView)));
            StartNewGameCommand = new DelegateCommand(() =>
                _regionManager.RequestNavigate("MainRegion", nameof(GameView)));
            EvolveNetworkCommand = new DelegateCommand(() =>
                _regionManager.RequestNavigate("MainRegion", nameof(EvolutionView)));
            StatisticsCommand = new DelegateCommand(() =>
                _regionManager.RequestNavigate("MainRegion", nameof(StatisticsView)));
        }
    }
}