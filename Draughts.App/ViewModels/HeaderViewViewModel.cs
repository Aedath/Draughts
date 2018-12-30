using Draughts.App.Infrastructure.Services;
using Draughts.App.Views;
using Draughts.App.Views.Account;
using Prism.Commands;
using Prism.Regions;
using System.Threading.Tasks;

namespace Draughts.App.ViewModels
{
    internal class HeaderViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IAccessService _accessService;
        public override bool KeepAlive => true;

        public HeaderViewModel(IRegionManager regionManager, IAccessService accessService)
        {
            _regionManager = regionManager;
            _accessService = accessService;
            HomeCommand = new DelegateCommand(OnHome);
            LogoutCommand = new DelegateCommand(async () => await OnLogout());
        }

        private string _username;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public DelegateCommand HomeCommand { get; }

        private void OnHome() =>
            _regionManager.RequestNavigate("MainRegion", nameof(StartView));

        public DelegateCommand LogoutCommand { get; }

        public async Task OnLogout()
        {
            try
            {
                await _accessService.Logout();
                _regionManager.RequestNavigate("MainRegion", nameof(LogInView));
                _regionManager.Regions["HeaderRegion"].RemoveAll();
            }
            catch
            {
                //ignore
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            Username = navigationContext.Parameters["username"].ToString();
        }
    }
}