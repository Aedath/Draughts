using Autofac;
using System.Linq;
using System.Reflection;

namespace Draughts.App
{
    internal class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == "Draughts.App.ViewModels").ToList()
                    .ForEach(x => builder.RegisterType(x).AsSelf());

            Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == "Draughts.App.Views").ToList()
                    .ForEach(x => builder.RegisterType(x).Named<object>(x.Name));

            RegisterServices(builder);
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            //builder.RegisterType<Infrastructure.ScreenService>().As<Infrastructure.IScreenService>();
        }
    }
}