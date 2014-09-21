using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;
using System.Globalization;

namespace RussLibrary.ValueConverters
{
    [ValueConversion(typeof(double), typeof(string))]
    public class NumericConverter : IValueConverter
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(NumericConverter));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double val = double.NaN;
            if (value != null)
            {
                val = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            
            string format = string.Empty;
            if (parameter != null)
            {
                format = parameter.ToString();
            }
            
            return val.ToString(format, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string val = value as string;
            if (!string.IsNullOrEmpty(val))
            {
                string v = val.Replace(",", string.Empty);
                decimal d = 0;
                if (decimal.TryParse(v, out d))
                {
                    return d;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}
