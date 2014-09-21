using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;

namespace ArtemisEngineeringPresets
{

    public class ShipSystemTextConverter : IMultiValueConverter
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ShipSystemTextConverter));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string[] systems = { "Beam", "Torpedo", "Sensors", "Maneuvering", "Impulse", "Warp/Jump", "Front Shield", "Rear Shield" };
            string retVal = string.Empty;
            if (values != null)
            {

                SystemLevel sys = values[0] as SystemLevel;
                if (sys != null)
                {
                    Preset p = values[1] as Preset;
                    if (p != null)
                    {
                        retVal = systems[p.SystemLevels.IndexOf(sys)];
                    }
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
