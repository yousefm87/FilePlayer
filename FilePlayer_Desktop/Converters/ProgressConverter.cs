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
            string name = "";

            switch ((string)parameter)
            {
                case "Progress":
                    string numerator = "?";
                    string denominator = "?";

                    if ((values[1] != null) && (!values[1].Equals(DependencyProperty.UnsetValue)))
                        numerator = (string) values[1];
                    if ((values[2] != null) && (!values[2].Equals(DependencyProperty.UnsetValue)))
                        denominator = (string) values[2];

                    name = numerator + "/" + denominator ;
                    break;

            }

            return name;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

