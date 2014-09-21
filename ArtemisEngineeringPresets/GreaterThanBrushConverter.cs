using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace ArtemisEngineeringPresets
{
    public class GreaterThanBrushConverter : IValueConverter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Int32.TryParse(System.String,System.Int32@)")]
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Brush retVal = null;
            if (value != null && parameter != null)
            {
                int val = (int)value;
                string[] parms = parameter.ToString().Split('|');
                int match = 0;
                Brush brushIfMatch = null;
                Brush brushIfNoMatch = null;
                int.TryParse(parms[0], out match);
                if (parms.Length > 1)
                {
                    brushIfMatch = (new BrushConverter()).ConvertFromInvariantString(parms[1]) as Brush;
                    if (parms.Length > 2)
                    {
                        brushIfNoMatch = (new BrushConverter()).ConvertFromInvariantString(parms[2]) as Brush;
                    }
                }
                retVal = (val > match) ? brushIfMatch : brushIfNoMatch;

            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
