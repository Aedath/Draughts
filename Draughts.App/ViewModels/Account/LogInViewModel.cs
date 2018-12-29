using System;
using Prism.Commands;
using System.Security;
using System.Threading.Tasks;
using Draughts.App.Infrastructure.Services;
using Draughts.App.Views;
using Prism.Regions;

namespace Draughts.App.ViewModels.Account
{
    internal class LogInViewModel : ViewModelBase
    {
        private readonly IAccessService _accessService;
        private readonly IRegionManager _regionManager;

        public LogInViewModel(IAccessService accessService, IRegionManager regionManager)
        {
            SignInCommand = new DelegateCommand(async () => await SignIn(), CanSignIn)
                .ObservesProperty(() => Username)
                .ObservesProperty(() => Password);
            RegisterCommand = new DelegateCommand(async () => await Register(), CanRegister)
                .ObservesProperty(() => Username)
                .ObservesProperty(() => Email)
                .ObservesProperty(() => Password)
                .ObservesProperty(() => ConfirmPassword);
            ClearCommand = new DelegateCommand(OnClear);
            _accessService = accessService;
            _regionManager = regionManager;
        }

        private string _username;

        private SecureString _password;

        private SecureString _confirmPassword;
        private bool _inProgress;
        private string _email;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public SecureString Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public SecureString ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public bool InProgress
        {
            get => _inProgress;
            private set => SetProperty(ref _inProgress, value);
        }

        public DelegateCommand SignInCommand { get; }

        private async Task SignIn()
        {
            try
            {
                InProgress = true;
                await _accessService.LogIn(Username, Password);
                InProgress = false;
                ClearCommand.Execute();
                _regionManager.RequestNavigate("MainRegion", nameof(StartView));
            }
            catch (Exception ex)
            {
                
            }
        }

        private bool CanSignIn() => !string.IsNullOrWhiteSpace(Username) && Password != null && Password.Length > 0;

        public DelegateCommand RegisterCommand { get; }

        private async Task Register()
        {
            try
            {
                InProgress = true;
                await _accessService.Register(Username, Email, Password, ConfirmPassword);
                InProgress = false;
                ClearCommand.Execute();
            }
            catch (Exception ex)
            {
            }
        }

        private bool CanRegister() => CanSignIn() && !string.IsNullOrWhiteSpace(Email) && ConfirmPassword != null && ConfirmPassword.Length == Password.Length;

        public DelegateCommand ClearCommand { get; }

        private void OnClear()
        {
            Username = string.Empty;
            Email = string.Empty;
            Password = null;
            Password = new SecureString();
            ConfirmPassword = null;
            ConfirmPassword = new SecureString();
        }
    }
}
