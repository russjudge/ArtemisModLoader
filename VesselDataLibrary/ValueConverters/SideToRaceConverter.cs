using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;
using VesselDataLibrary.Xml;

namespace VesselDataLibrary.ValueConverters
{

    public class SideToRaceConverter : IMultiValueConverter
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(SideToRaceConverter));
        
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string retVal = "Invalid Race";
            if (values != null)
            {
                HullRaceCollection races = values[0] as HullRaceCollection;
                if (races != null)
                {
                    int side = -1;
                    if (int.TryParse(values[1].ToString(), out side))
                    {
                        foreach (HullRace race in races)
                        {
                            if (race.ID == side)
                            {
                                retVal = race.Name;
                            }
                        }
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
