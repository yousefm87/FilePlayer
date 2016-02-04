using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FilePlayer.Converters
{
    public class ReleaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            if (value.Equals(""))
            {
                return value;
            }
            else
            {
                return "Release Date: " + value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}
