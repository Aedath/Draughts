using Draughts.App.Infrastructure.Notifications.Messages;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Draughts.App.Infrastructure.Notifications.ViewModels
{
    internal class NotificationsViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private const int TimerDurationInSeconds = 10;

        private readonly IDictionary<INotification, DispatcherTimer> _timers = new Dictionary<INotification, DispatcherTimer>();
        private bool _isNotificationsRegionVisible;

        public ObservableCollection<object> Notifications { get; }

        public DelegateCommand<INotification> CloseCommand { get; }

        public bool IsNotificationsRegionVisible
        {
            get => _isNotificationsRegionVisible;
            set => SetProperty(ref _isNotificationsRegionVisible, value);
        }

        public NotificationsViewModel(INotificationService notificationService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            notificationService.NotificationHappened += OnNotificationHappened;

            Notifications = new ObservableCollection<object>();

            CloseCommand = new DelegateCommand<INotification>(OnCloseNotification);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsNotificationsRegionVisible = true;

            _eventAggregator.GetEvent<PubSubEvent<NotificationsPopupVisibilityChangedEventData>>()
                .Subscribe(OnNotificationsPopupVisibilityChanged);
        }

        private void OnNotificationsPopupVisibilityChanged(NotificationsPopupVisibilityChangedEventData data)
        {
            IsNotificationsRegionVisible = !data.IsVisible;
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

    public class NotificationsPopupVisibilityChangedEventData
    {
        public bool IsVisible { get; }

        public NotificationsPopupVisibilityChangedEventData(bool isVisible)
        {
            IsVisible = isVisible;
        }
    }
}