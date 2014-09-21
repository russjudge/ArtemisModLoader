using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace RussLibrary.ValueConverters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parm"), ValueConversion(typeof(double), typeof(double))]
    public class DoubleParmAdjuster : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double val = 0;
            double parm = 0;
            if (value != null)
            {
                if (!double.TryParse(value.ToString(), out val))
                {
                    return value;
                }
            }
            if (parameter != null)
            {
                if (!double.TryParse(parameter.ToString(), out parm))
                {
                    parm = 0;
                }
            }
            return val + parm;


        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
