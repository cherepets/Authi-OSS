using Microsoft.UI.Xaml.Data;
using System;

namespace Authi.App.WinUI.Converters
{
    public class InverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }
    }
}
