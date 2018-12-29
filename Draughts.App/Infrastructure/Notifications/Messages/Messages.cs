using System;

namespace Draughts.App.Infrastructure.Notifications.Messages
{
    public interface INotification
    {
        string Message { get; }
        DateTime Timestamp { get; }
    }

    public class ErrorNotification : INotification
    {
        public string Message { get; }
        public DateTime Timestamp { get; }

        public ErrorNotification(string message)
        {
            Message = message;

            Timestamp = DateTime.Now;
        }
    }

    public class InfoNotification : INotification
    {
        public string Message { get; }
        public DateTime Timestamp { get; }

        public InfoNotification(string message)
        {
            Message = message;

            Timestamp = DateTime.Now;
        }
    }

    public class SuccessNotification : INotification
    {
        public string Message { get; }
        public DateTime Timestamp { get; }

        public SuccessNotification(string message)
        {
            Message = message;

            Timestamp = DateTime.Now;
        }
    }

    public class WarningNotification : INotification
    {
        public string Message { get; }
        public DateTime Timestamp { get; }

        public WarningNotification(string message)
        {
            Message = message;

            Timestamp = DateTime.Now;
        }
    }
}