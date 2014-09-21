using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;
using System.Windows;

namespace ArtemisModLoader
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
       
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility VisibilityIfTrue = Visibility.Visible;
            Visibility VisibilityIfFalse = Visibility.Collapsed;

            if (parameter != null)
            {
                string[] parms = parameter.ToString().Split('|');
                if (parms[0].StartsWith("Visible", StringComparison.OrdinalIgnoreCase))
                {
                    VisibilityIfTrue = Visibility.Visible;
                }
                else if (parms[0].StartsWith("Collapsed", StringComparison.OrdinalIgnoreCase))
                {
                    VisibilityIfTrue = Visibility.Collapsed;
                }
                else if (parms[0].StartsWith("Hidden", StringComparison.OrdinalIgnoreCase))
                {
                    VisibilityIfTrue = Visibility.Hidden;
                }
                if (parms.Length > 1)
                {
                    if (parms[1].StartsWith("Visible", StringComparison.OrdinalIgnoreCase))
                    {
                        VisibilityIfFalse = Visibility.Visible;
                    }
                    else if (parms[1].StartsWith("Collapsed", StringComparison.OrdinalIgnoreCase))
                    {
                        VisibilityIfFalse = Visibility.Collapsed;
                    }
                    else if (parms[1].StartsWith("Hidden", StringComparison.OrdinalIgnoreCase))
                    {
                        VisibilityIfFalse = Visibility.Hidden;
                    }
                }
            }
            if (value == null)
            {
                return VisibilityIfFalse;
            }
            else
            {
                bool val = (bool)value;
                return val ? VisibilityIfTrue : VisibilityIfFalse;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
