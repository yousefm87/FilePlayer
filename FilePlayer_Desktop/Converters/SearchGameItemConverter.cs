using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FilePlayer.Converters
{
    class SearchGameItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {

                //if ((values[1] != null) && (!values[1].Equals(DependencyProperty.UnsetValue)))
                //    numerator = (string)values[1];
                //if ((values[2] != null) && (!values[2].Equals(DependencyProperty.UnsetValue)))
                //    denominator = (string)values[2];

                //name = numerator + "/" + denominator;
                //BitmapImage bmp = new BitmapImage();
                //bmp.BeginInit();
                //bmp.UriSource = new Uri((string)values[1]);
                //bmp.CacheOption = BitmapCacheOption.OnLoad;
                //bmp.EndInit();

                //return bmp;
                return null;
            }
            catch
            {
                return new BitmapImage();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
