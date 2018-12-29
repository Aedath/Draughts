using Autofac;
using Draughts.App.Infrastructure.Notifications;
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
                .Where(x => x.Name.EndsWith("ViewModel")).ToList()
                    .ForEach(x => builder.RegisterType(x).AsSelf());

            Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.Name.EndsWith("View")).ToList()
                    .ForEach(x => builder.RegisterType(x).Named<object>(x.Name));

            RegisterServices(builder);
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<Infrastructure.Services.AccessService>().As<Infrastructure.Services.IAccessService>().SingleInstance();
            builder.RegisterType<NotificationService>().As<INotificationService>().SingleInstance();
        }
    }
}