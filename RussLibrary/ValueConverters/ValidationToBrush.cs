using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;
using System.Windows.Media;
using RussLibrary.WPF;

namespace RussLibrary.ValueConverters
{

    public class ValidationToBrush : IValueConverter
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(ValidationToBrush));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //parameter = "propertyMatch|Color If success|Color if Warning|Color if Error
            Brush retVal = null;
            ValidationObjectCollection validation = null;
            if (value != null)
            {
                validation = value as ValidationObjectCollection;

                if (parameter != null)
                {
                    string[] parms = parameter.ToString().Split('|');
                    string key = parms[0];
                    ValidationObject val = validation.GetValidationResult(key);
                    string colorOnSuccess = parms[1];
                    string colorOnWarn = parms[2];
                    string colorOnError = parms[3];
                    BrushConverter cnv = new BrushConverter();
                    switch (val.Code)
                    {
                        case ValidationValue.IsValid:
                            retVal = cnv.ConvertFromInvariantString(colorOnSuccess) as Brush;
                            break;
                        case ValidationValue.IsWarnState:
                            retVal = cnv.ConvertFromInvariantString(colorOnWarn) as Brush;
                            break;
                        case ValidationValue.IsError:
                            retVal = cnv.ConvertFromInvariantString(colorOnError) as Brush;
                            break;
                    }

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
