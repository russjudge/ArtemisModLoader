using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace RussLibrary.ValueConverters
{
    public class MultiplierConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal val = 0.0M;
            decimal parm = 0.0M;

            decimal retVal = 0.0M;
            if (value != null)
            {
                if (!decimal.TryParse(value.ToString(), out val))
                {
                    val = 0;
                }
                if (parameter != null)
                {
                    if (!decimal.TryParse(parameter.ToString(), out parm))
                    {
                        parm = 0;
                    }
                }
            }
            retVal = val * parm;
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
