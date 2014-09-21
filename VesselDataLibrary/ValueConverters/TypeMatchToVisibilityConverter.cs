using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace VesselDataLibrary.ValueConverters
{
    public class TypeMatchToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility retval = Visibility.Collapsed;
            if (value != null && parameter != null)
            {
                string T = value.GetType().ToString();
                if (T.EndsWith(parameter.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    retval = Visibility.Visible;
                }
            }
            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
