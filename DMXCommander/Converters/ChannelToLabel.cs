using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DMXCommander.Converters
{
    public class ChannelToLabel : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return GeneralHelper.GetChannelLabel((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return GeneralHelper.GetLabelToInt(value as string);
        }
    }
}
