using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;
using System.Collections;

namespace ArtemisEngineeringPresets
{

    public class ToIndexConverter : IMultiValueConverter
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(ToIndexConverter));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int retVal = -1;

            if (values != null)
            {
                object val = values[0];
                IList collection = values[1] as IList;
                if (val != null && collection != null)
                {
                    retVal = collection.IndexOf(val);
                }
            }
            if (++retVal > 9)
            {
                retVal = 0;
            }
            string xx = retVal.ToString(culture);
            return xx;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
