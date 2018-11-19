using Draughs.NeuralNetwork.Services;
using Draughts.App.Views;
using Prism.Regions;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Draughts.App.ViewModels
{
    internal class EvolutionViewModel : ViewModelBase
    {
        public EvolutionViewModel(IRegionManager regionManager)
        {
            Task.Run(async () =>
            {
                if (File.Exists("../Network.txt"))
                {
                    File.Copy("../Network.txt", $"../Network {DateTime.Now:yyyy MM dd}.txt");
                }
                File.Delete("../Network.txt");
                Text = "Genetic evolution in progress...";
                await Task.Run(() =>
                {
                    var service = new EvolutionService();
                    foreach (var evolutionText in service.Evolve())
                    {
                        Text = evolutionText;
                    };
                });
                Application.Current.Dispatcher.Invoke(() =>
                {
                    regionManager.RequestNavigate("MainRegion", nameof(GameView));
                });
            });
        }

        private string _text = "...";

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
    }
}