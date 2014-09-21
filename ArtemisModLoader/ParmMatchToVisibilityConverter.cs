using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Windows.Data;
using System.Windows;

namespace ArtemisModLoader
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parm"), ValueConversion(typeof(string), typeof(Visibility))]
    public class ParmMatchToVisibilityConverter : IValueConverter
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ParmMatchToVisibilityConverter));
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Visibility retVal = Visibility.Collapsed;
            Visibility VisibilityIfMatch = Visibility.Visible;
            Visibility VisibilityIfNotMatch = Visibility.Collapsed;
            if (value != null && parameter != null)
            {
                string val = value.ToString();
                string[] parm = parameter.ToString().Split('|');
                string match = parm[0];

                if (parm.Length > 1)
                {
                    if (parm[1].IndexOf("collapsed", StringComparison.OrdinalIgnoreCase) >  -1)
                    {
                        VisibilityIfMatch = Visibility.Collapsed;
                        VisibilityIfNotMatch = Visibility.Visible;
                    }
                    else if (parm[1].IndexOf("hidden", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        VisibilityIfMatch = Visibility.Hidden;
                        VisibilityIfNotMatch = Visibility.Visible;
                    }
                    else
                    {
                        VisibilityIfMatch = Visibility.Visible;
                        VisibilityIfNotMatch = Visibility.Collapsed;
                    }

                }

                retVal = val.Contains(match) ? VisibilityIfMatch : VisibilityIfNotMatch;
            }
            else
            {
                retVal = VisibilityIfNotMatch;
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

