using System;
using System.Windows.Data;
using log4net;
using System.Reflection;

namespace ArtemisModLoader
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class OppositeBoolConverter : IValueConverter
    {

        static readonly ILog _log = LogManager.GetLogger(typeof(OppositeBoolConverter));
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = !(bool)value;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        #endregion
    }
}
