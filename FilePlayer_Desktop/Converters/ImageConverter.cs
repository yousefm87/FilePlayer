using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FilePlayer.Converters
{
    //public class ImageConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType,
    //                          object parameter, CultureInfo culture)
    //    {
    //        try
    //        {
    //            BitmapImage bmp = new BitmapImage();
    //            bmp.BeginInit();
    //            bmp.UriSource = new Uri((string) value);
    //            bmp.CacheOption = BitmapCacheOption.OnLoad;
    //            bmp.EndInit();

    //            return bmp;
    //        }
    //        catch
    //        {
    //            return new BitmapImage();
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType,
    //                              object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

 
        /// <summary>
        /// This converter facilitates a couple of requirements around images. Firstly, it automatically disposes of image streams as soon as images
        /// are loaded, thus avoiding file access exceptions when attempting to delete images. Secondly, it allows images to be decoded to specific
        /// widths and / or heights, thus allowing memory to be saved where images will be scaled down from their original size.
        /// </summary>
        public sealed class ImageConverter : IValueConverter
        {
            //doubles purely to facilitate easy data binding
            private double _decodePixelWidth;
            private double _decodePixelHeight;

            public double DecodePixelWidth
            {
                get
                {
                    return _decodePixelWidth;
                }
                set
                {
                    _decodePixelWidth = value;
                }
            }

            public double DecodePixelHeight
            {
                get
                {
                    return _decodePixelHeight;
                }
                set
                {
                    _decodePixelHeight = value;
                }
            }

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                string path = value as string;

                if ((path != null) && (path != ""))
                {
                    try
                    {
                        //create new stream and create bitmap frame
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = new FileStream(path, FileMode.Open, FileAccess.Read);

                    //bitmapImage.DecodePixelWidth = (int)_decodePixelWidth;
                        //bitmapImage.DecodePixelHeight = (int)_decodePixelHeight;
                        //load the image now so we can immediately dispose of the stream
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();

                        //clean up the stream to avoid file access exceptions when attempting to delete images
                        bitmapImage.StreamSource.Dispose();

                        return bitmapImage;
                    }
                    catch(Exception)
                    {
                        return DependencyProperty.UnsetValue;
                    }
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }




