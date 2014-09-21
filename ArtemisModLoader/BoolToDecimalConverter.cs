using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;

namespace ArtemisModLoader
{
    [ValueConversion(typeof(bool), typeof(decimal))]
    public class BoolToDecimalConverter : IValueConverter
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(BoolToDecimalConverter));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        #region IValueConverter Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Decimal.TryParse(System.String,System.Decimal@)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)")]
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string parm = string.Empty;
            decimal falseVal = 0;
            decimal trueVal = 0;
            if (parameter != null)
            {
                parm = parameter.ToString();
                string[] parms = parm.Split('|');
                decimal.TryParse(parms[0], out trueVal);
                if (parm.Length > 1)
                {
                    decimal.TryParse(parms[1], out falseVal);
                }

            }
            bool val = false;
            if (value != null)
            {
                bool.TryParse(value.ToString(), out val);
            }
            decimal retVal =  val ? trueVal : falseVal;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
