using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections;
using System.Windows.Controls;
using System.Windows;
using RussLibrary.WPF;

namespace VesselDataLibrary.ValueConverters
{

    public class ItemIndexMatchToBrush : IMultiValueConverter
    {
      //  static readonly ILog _log = LogManager.GetLogger(typeof(ItemIndexMatchToBrush));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //1st value is item.
            //2nd is selectedindex
            //3rd is collection
           //parm is color on match | color on no match.
            Brush retVal = null;
            if (values != null)
            {
                if (parameter != null)
                {
                    string[] parms = parameter.ToString().Split('|');
                    if (parms.Length >= 2)
                    {
                        BrushConverter cnv = new BrushConverter();

                        Brush colorOnMatch = cnv.ConvertFromInvariantString(parms[0]) as Brush;
                        Brush colorOnNomatch = cnv.ConvertFromInvariantString(parms[1]) as Brush;
                        if (colorOnMatch != null && colorOnNomatch != null)
                        {
                            if (values.Length >=3)
                            {
                                IList collection = values[2] as IList;
                                int selectedIndex = (int)values[1];
                                if (collection != null && collection.IndexOf(values[0]) == selectedIndex)
                                {
                                    retVal = colorOnMatch;
                                   
                                }
                                else
                                {
                                    retVal = colorOnNomatch;
                                }
                            }
                        }
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
