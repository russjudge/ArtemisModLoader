using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;
using System.Windows;

namespace RussLibrary.ValueConverters
{

    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToHiddenConverter : IValueConverter
    {


        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parm = parameter as string;
            Visibility VisibilityIfNull = Visibility.Collapsed;
            
            if (!string.IsNullOrEmpty(parm))
            {
                if (parm.ToUpperInvariant() == "VISIBLE")
                {
                    VisibilityIfNull = Visibility.Visible;
                }
            }
            Visibility retVal = VisibilityIfNull;
            if (value == null)
            {
                retVal = VisibilityIfNull;
            }
            else
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    retVal = VisibilityIfNull;
                }
                else
                {
                    if (VisibilityIfNull == Visibility.Collapsed)
                    {
                        retVal = Visibility.Visible;
                    }
                    else
                    {
                        retVal = Visibility.Collapsed;
                    }
                }
            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}