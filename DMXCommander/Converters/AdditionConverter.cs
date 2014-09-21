using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DMXCommander.Converters
{
    public class AdditionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? val = value as int?;
            if (val != null)
            {
                int parm = 0;
                if (parameter != null)
                {
                    if (!int.TryParse(parameter.ToString(), out parm))
                    {
                        parm = 0;
                    }
                }
                return val.Value + parm;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? val = value as int?;
            if (val != null)
            {
                int parm = 0;
                if (parameter != null)
                {
                    if (!int.TryParse(parameter.ToString(), out parm))
                    {
                        parm = 0;
                    }
                }
                return val.Value - parm;
            }
            return 0;
        }
    }
}
