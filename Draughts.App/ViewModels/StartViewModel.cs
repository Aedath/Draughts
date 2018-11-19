using Draughts.App.Views;
using Prism.Commands;
using Prism.Regions;
using System.Windows;

namespace Draughts.App.ViewModels
{
    internal class StartViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        public DelegateCommand StartNewGameCommand { get; set; }
        public DelegateCommand EvolveNetworkCommand { get; set; }

        public StartViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            StartNewGameCommand = new DelegateCommand(OnStartNewGame);
            EvolveNetworkCommand = new DelegateCommand(OnEvolveNewNework);
        }

        private void OnStartNewGame()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _regionManager.RequestNavigate("MainRegion", nameof(GameView));
            });
        }

        private void OnEvolveNewNework()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _regionManager.RequestNavigate("MainRegion", nameof(EvolutionView));
            });
        }
    }
}