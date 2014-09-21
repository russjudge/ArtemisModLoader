using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;

namespace RussLibrary.ValueConverter
{
    [ValueConversion(typeof(string), typeof(string))]
    public class FilenameFormatter : IValueConverter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string val = value as string;
            string retVal = val;
            if (!string.IsNullOrEmpty(val))
            {
                try
                {
                    FileInfo f = new FileInfo(val);
                    retVal = f.Name;
                }
                catch { }
            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
