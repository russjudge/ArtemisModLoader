using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;
using VesselDataLibrary.Xml;
using System.Windows;

namespace VesselDataLibrary.ValueConverters
{

    public class HullRaceFilterConverter : IMultiValueConverter
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(HullRaceFilterConverter));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            HullRace selectedRace = null;
            int vesselSide = -1;
            Visibility retVal = Visibility.Visible;
            if (values != null)
            {
                if (values.Length > 0)
                {
                    selectedRace = values[0] as HullRace;
                }
                if (values.Length > 1)
                {
                    if (!int.TryParse(values[1].ToString(), out vesselSide))
                    {
                        vesselSide = -1;
                    }
                }
                if (selectedRace == null || selectedRace.ID < 0 || vesselSide == selectedRace.ID)
                {
                    retVal = Visibility.Visible;

                }
                else
                {
                    retVal = Visibility.Collapsed;
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
