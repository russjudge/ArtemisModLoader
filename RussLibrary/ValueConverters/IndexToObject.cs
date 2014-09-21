using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;
using System.Collections;

namespace RussLibrary.ValueConverters
{

    public class IndexToObject : IMultiValueConverter
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(IndexToObject));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        #region IMultiValueConverter Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object retVal = null;
            if (values != null)
            {
                if (values.Length == 2)
                {
                    try
                    {
                        int index = (int)values[0];
                        IList collection = values[1] as IList;
                        if (collection != null)
                        {
                            if (collection.Count > index && index >= 0)
                            {
                                retVal = collection[index];
                            }
                        }
                    }
                    catch { }
                }
            }
            return retVal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
