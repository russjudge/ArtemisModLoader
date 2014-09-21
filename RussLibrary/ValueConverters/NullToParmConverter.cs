using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;

namespace RussLibrary.ValueConverters
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parm")]
    public class NullToParmConverter : IValueConverter
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(NullToParmConverter));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        #region IValueConverter Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Decimal.TryParse(System.String,System.Decimal@)")]
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string[] parms = null;
            decimal resultIfNull = 0;
            decimal resultIfNotNull = 0;

            if (parameter != null)
            {
                parms = parameter.ToString().Split('|');
                decimal.TryParse(parms[0], out resultIfNull);
                if (parms.Length > 1)
                {
                    decimal.TryParse(parms[1], out resultIfNotNull);
                }
            }
          
            return (value == null) ? resultIfNull : resultIfNotNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
