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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parm")]
    public class LessThanParmVisibilityConverter : IValueConverter
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(LessThanParmVisibilityConverter));
        #region IValueConverter Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Decimal.TryParse(System.String,System.Decimal@)")]
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            decimal parm = 0;
            if (parameter != null)
            {

                decimal.TryParse(parameter.ToString(), out parm);
            }
            Visibility retVal = Visibility.Collapsed;
            if (value != null)
            {

                decimal val = 0;
                decimal.TryParse(value.ToString(), out val);
                retVal = (val < parm) ? Visibility.Visible : Visibility.Collapsed;
            }
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
