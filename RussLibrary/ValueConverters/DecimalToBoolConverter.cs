using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;

namespace RussLibrary.ValueConverters
{
    [ValueConversion(typeof(decimal), typeof(bool))]
    public class DecimalToBoolConverter :IValueConverter
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(DecimalToBoolConverter));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        #region IValueConverter Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Decimal.TryParse(System.String,System.Decimal@)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)")]
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal val = 0;
            bool retVal = false;
            if (value != null)
            {
                decimal.TryParse(value.ToString(), out val);
            }
            if (parameter != null)
            {
                //#|true (or false): if value > # what to return. opposite returned on nomatch.
                string[] parms = parameter.ToString().Split('|');
                decimal match = 0;
                decimal.TryParse(parms[0], out match);
                bool returnOnMatch = true;
                if (parms.Length > 1)
                {
                    bool.TryParse(parms[1], out returnOnMatch);
                }
                retVal = (val > match) ? returnOnMatch : !returnOnMatch;
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
