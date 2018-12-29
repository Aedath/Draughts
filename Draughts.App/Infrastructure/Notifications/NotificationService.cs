using Draughts.App.Infrastructure.Notifications.Messages;
using System.Collections.Generic;
using System.Windows;

namespace Draughts.App.Infrastructure.Notifications
{
    public delegate void NotificationHappenedHandler(INotification notification);

    public interface INotificationService
    {
        event NotificationHappenedHandler NotificationHappened;

        void NotifyInfo(string messsage);

        void NotifyError(string messsage);

        void NotifyWarning(string messsage);

        void NotifySuccess(string message);

        void DismissNotification(INotification notification);

        IEnumerable<INotification> GetNotifications();
    }

    internal class NotificationService : INotificationService
    {
        public event NotificationHappenedHandler NotificationHappened = notification => { };

        private readonly List<INotification> _notifications;

        public NotificationService()
        {
            _notifications = new List<INotification>();
        }

        public void NotifyInfo(string message)
        {
            Notify(new InfoNotification(message));
        }

        public void NotifyError(string message)
        {
            Notify(new ErrorNotification(message));
        }

        public void NotifyWarning(string message)
        {
            Notify(new WarningNotification(message));
        }

        public void NotifySuccess(string message)
        {
            Notify(new SuccessNotification(message));
        }

        private void Notify(INotification notification)
        {
            _notifications.Add(notification);

            Application.Current.Dispatcher.Invoke(() =>
            {
                NotificationHappened(notification);
            });
        }

        public void DismissNotification(INotification notification)
        {
            _notifications.Remove(notification);
        }

        public IEnumerable<INotification> GetNotifications()
        {
            return _notifications;
        }
    }
}