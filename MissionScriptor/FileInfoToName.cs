using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;
using System.IO;

namespace MissionStudio
{

    public class FileInfoToName : IValueConverter
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(FileInfoToName));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string retVal = string.Empty;
            if (value != null)
            {
                FileInfo f = value as FileInfo;
                if (f != null)
                {
                    retVal = f.Name.Substring(0, f.Name.Length - f.Extension.Length);
                }
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
