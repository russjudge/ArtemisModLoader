using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections;

namespace RussLibrary.ValueConverters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parm")]
    public class ItemIndexMatchToParm : IMultiValueConverter
    {
        //  static readonly ILog _log = LogManager.GetLogger(typeof(ItemIndexMatchToBrush));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        #region IMultiValueConverter Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Decimal.TryParse(System.String,System.Decimal@)")]
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //1st value is item.
            //2nd is selectedindex
            //3rd is collection
            //parm is color on match | color on no match.
            decimal retVal = 0;
            if (values != null)
            {
                if (parameter != null)
                {
                   

                        decimal colorOnMatch = 0;
                        decimal.TryParse(parameter.ToString(), out colorOnMatch);
                      
                            if (values.Length >= 3)
                            {
                                IList collection = values[2] as IList;
                                int selectedIndex = (int)values[1];
                                if (collection != null && collection.IndexOf(values[0]) == selectedIndex)
                                {
                                    retVal = colorOnMatch;

                                }
                                else
                                {
                                    retVal = collection.IndexOf(values[0]);
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