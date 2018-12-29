using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Draughts.App.Infrastructure
{
    public class NonZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new Exception("Invalid target type");

            int i = (int)value;

            return i > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}