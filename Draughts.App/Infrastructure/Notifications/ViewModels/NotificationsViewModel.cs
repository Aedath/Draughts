using Draughts.App.Infrastructure.Notifications.Messages;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Draughts.App.Infrastructure.Notifications.ViewModels
{
    internal class NotificationsViewModel : ViewModelBase
    {
        private const int TimerDurationInSeconds = 10;

        private readonly IDictionary<INotification, DispatcherTimer> _timers = new Dictionary<INotification, DispatcherTimer>();

        public ObservableCollection<INotification> Notifications { get; } = new ObservableCollection<INotification>();

        public DelegateCommand<INotification> CloseCommand { get; }

        public NotificationsViewModel(INotificationService notificationService)
        {
            notificationService.NotificationHappened += OnNotificationHappened;
            CloseCommand = new DelegateCommand<INotification>(OnCloseNotification);
        }

        private void OnCloseNotification(INotification notification)
        {
            if (!_timers.TryGetValue(notification, out var timer))
            {
                return;
            }

            timer.Stop();

            Notifications.Remove(notification);
            _timers.Remove(notification);
        }

        private void OnNotificationHappened(INotification notification)
        {
            if (_timers.ContainsKey(notification))
            {
                return;
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Notifications.Add(notification);

                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(TimerDurationInSeconds) };
                _timers.Add(notification, timer);

                timer.Tick += (sender, args) => OnCloseNotification(notification);
                timer.Start();
            });
        }
    }
}