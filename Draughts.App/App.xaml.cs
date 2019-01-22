using Autofac;
using Draughts.App.Infrastructure.Notifications;
using System;
using System.Windows;

namespace Draughts.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Bootstrapper _bootstrapper;

        public App()
        {
            _bootstrapper = new Bootstrapper();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            DispatcherUnhandledException += OnException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _bootstrapper.Run();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var notificationService = _bootstrapper.Container.Resolve<INotificationService>();
            if (notificationService is null)
            {
                MessageBox.Show(((Exception) e.ExceptionObject).Message);
                return;
            }
            notificationService.NotifyError(((Exception)e.ExceptionObject).Message);
        }

        private void OnException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var notificationService = _bootstrapper.Container.Resolve<INotificationService>();
            if (notificationService is null)
            {
                MessageBox.Show(e.Exception.Message);
                return;
            }
            notificationService.NotifyError(e.Exception.Message);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _bootstrapper.Dispose();

            base.OnExit(e);
        }
    }
}