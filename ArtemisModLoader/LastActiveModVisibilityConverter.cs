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

    public class LastActiveModVisibilityConverter : IValueConverter
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(LastActiveModVisibilityConverter));
       
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Visibility retVal = Visibility.Collapsed;
            if (value != null)
            {
                int seq = (int)value;
                if (seq == ActiveModConfigurations.Instance.Configurations.Count - 1)
                {
                    retVal = Visibility.Visible;
                }
                else
                {
                    retVal = Visibility.Collapsed;
                }
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
