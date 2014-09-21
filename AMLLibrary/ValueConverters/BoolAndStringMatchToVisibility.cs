using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
namespace ArtemisModLoader.ValueConverters
{

    public class BoolAndStringMatchToVisibility :IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //First one is bool for show stock.
            // second is ID
            //parm is match.
            bool DoShowAlways = true;
            string valueToConsider = null;
            string parm = string.Empty;
            if (values != null && values.Length == 2)
            {
                DoShowAlways = !((values[0] as bool?) == false);
                valueToConsider = (string)values[1];
            }
            if (parameter != null)
            {
                parm = parameter.ToString();
            }
            Visibility retVal = Visibility.Visible;
            if (DoShowAlways || !valueToConsider.Contains(parm))
            {
                retVal = Visibility.Visible;
            }
            else
            {
                retVal = Visibility.Collapsed;
            }
            return retVal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
