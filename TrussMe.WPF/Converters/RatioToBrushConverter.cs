using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TrussMe.WPF.Converters
{
    public class RatioToBrushConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value!=null)
            {
                float ratio = float.Parse(value.ToString());
                return ratio > 1 ? Brushes.LightCoral : Brushes.LightGreen;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
