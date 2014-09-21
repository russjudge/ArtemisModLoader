using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows;
using System.Windows.Data;

namespace RussLibrary.ValueConverters
{
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumNameMatchToVisibility : IValueConverter
    {
        //        static readonly ILog _log = LogManager.GetLogger(typeof(EnumNameMatchToBool));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        #region IValueConverter Members
        /// <summary>
        /// Parameter syntax:
        /// ConverterParameter='MatchValue|VisibilityOnMatch|VisibilityOnMismatch'
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            var ParameterString = parameter as string;
            string EnumNameMatch = string.Empty;
            Visibility returnOnMatch = Visibility.Visible;
            Visibility returnOnMismatch = Visibility.Collapsed;
            Visibility retVal = returnOnMatch;
            if (ParameterString != null)
            {
                string[] parms = ParameterString.Split('|');
                EnumNameMatch = parms[0];
                if (parms.Length > 1)
                {
                    
                    switch (parms[1].ToUpperInvariant())
                    {
                        case "VISIBLE":
                            returnOnMatch = Visibility.Visible;
                            break;
                        case "COLLAPSED":
                            returnOnMatch = Visibility.Collapsed;
                            returnOnMismatch = Visibility.Visible;
                            break;
                        case "HIDDEN":
                            returnOnMatch = Visibility.Hidden;
                            returnOnMismatch = Visibility.Visible;
                            break;
                    }
                }
                if (parms.Length > 2)
                {
                    switch (parms[2].ToUpperInvariant())
                    {
                        case "VISIBLE":
                            returnOnMismatch = Visibility.Visible;
                            break;
                        case "COLLAPSED":
                            returnOnMismatch = Visibility.Collapsed;
                            break;
                        case "HIDDEN":
                            returnOnMismatch = Visibility.Hidden;
                            break;
                    }
                }
            }
            if (value == null)
            {

                retVal = string.IsNullOrEmpty(EnumNameMatch) ? returnOnMatch : returnOnMismatch;

            }
            else
            {
                string[] matchItems = EnumNameMatch.Split('~');
                foreach (string match in matchItems)
                {
                    if (value.ToString() == match)
                    {
                        return returnOnMatch;
                    }
                }
                return returnOnMismatch;


            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ParameterString = parameter as string;


            string EnumNameMatch = string.Empty;
            bool returnOnMatch = true;
            if (ParameterString == null)
            {
                return DependencyProperty.UnsetValue;
            }
            else
            {
                string[] parms = ParameterString.Split('|');
                EnumNameMatch = parms[0];
                if (parms.Length > 1)
                {
                    if (!bool.TryParse(parms[1], out returnOnMatch))
                    {
                        returnOnMatch = true;
                    }
                }


                if (Enum.IsDefined(targetType, EnumNameMatch))
                {
                    if (returnOnMatch == (bool)value)
                    {
                        return Enum.Parse(targetType, ParameterString);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion
    }
}