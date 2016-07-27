using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FilePlayer.Converters
{
    public class ProgressConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string currentItemString = "";

            string itemType = (string) parameter;
   
            string numerator = "?";
            string denominator = "?";
            string itemName = "?";

            if ((values[0] != null) && (!values[0].Equals(DependencyProperty.UnsetValue)))
                numerator = (string) values[0];
            if ((values[1] != null) && (!values[1].Equals(DependencyProperty.UnsetValue)))
                denominator = (string) values[1];
            if ((values[2] != null) && (!values[2].Equals(DependencyProperty.UnsetValue)))
                itemName = (string) values[2];

            currentItemString = itemType + " (" + numerator + "/" + denominator + ") - " + itemName;

            return currentItemString;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

