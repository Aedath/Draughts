using Autofac;
using Draughts.App.Views;
using Prism.Autofac;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace Draughts.App
{
    internal class Bootstrapper : AutofacBootstrapper, IDisposable
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();

            var regionManager = Container.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion("MainRegion", () => Container.Resolve<GameView>());
            regionManager.RegisterViewWithRegion("BoardRegion", () => Container.Resolve<BoardView>());
        }

        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            base.ConfigureContainerBuilder(builder);

            builder.RegisterModule<Module>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.FullName;
                viewName = viewName.Replace(".Views.", ".ViewModels.");
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
                var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}{1}", viewName, suffix);

                var assembly = viewType.GetTypeInfo().Assembly;
                var type = assembly.GetType(viewModelName, true);

                return type;
            });
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}