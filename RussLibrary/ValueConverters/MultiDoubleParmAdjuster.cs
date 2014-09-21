using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace RussLibrary.ValueConverters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parm"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
    public class MultiDoubleParmAdjuster : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double retVal = 0;
            double val = 0;
            double parm = 0;
            if (values != null)
            {
                foreach (object valx in values)
                {
                    if (double.TryParse(valx.ToString(), out val))
                    {
                        retVal += val;
                    }
                }
            }
            if (parameter != null)
            {
                if (!double.TryParse(parameter.ToString(), out parm))
                {
                    parm = 0;
                }
            }
            return retVal + parm;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
