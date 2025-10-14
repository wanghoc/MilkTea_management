using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.IO;

namespace MilkTeaShop.Presentation.Converters
{
    public class TabVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int selectedIndex && parameter is string tabIndexStr)
            {
                if (int.TryParse(tabIndexStr, out int tabIndex))
                {
                    return selectedIndex == tabIndex ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && parameter.ToString() == "Reverse")
            {
                // Show when null (for placeholder)
                return value == null || string.IsNullOrEmpty(value.ToString()) ? Visibility.Visible : Visibility.Collapsed;
            }
            
            // Default behavior: Show when not null
            return value != null && !string.IsNullOrEmpty(value.ToString()) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ObjectEqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return false;

            return ReferenceEquals(values[0], values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return null;

            var imagePath = value.ToString();
            try
            {
                // Handle relative paths from Images folder
                if (imagePath.StartsWith("Images"))
                {
                    var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
                    if (File.Exists(fullPath))
                    {
                        // Create BitmapImage with cache disabled to ensure fresh load
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.None;
                        bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                        bitmap.EndInit();
                        return bitmap;
                    }
                }
                // Handle absolute paths
                else if (File.Exists(imagePath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.None;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.EndInit();
                    return bitmap;
                }
            }
            catch
            {
                // Return null if image cannot be loaded
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}