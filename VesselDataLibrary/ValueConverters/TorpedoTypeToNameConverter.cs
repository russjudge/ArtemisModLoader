using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace VesselDataLibrary.ValueConverters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class TorpedoTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int val = -1;
            string retVal = string.Empty;

            if (value != null)
            {
                if (!int.TryParse(value.ToString(), out val))
                {
                    val = -1;
                }
            }
            switch (val)
            {
    //                 <torpedo_storage type="0" amount="8" />  <!-- Type 1 Homing"-->
    //<torpedo_storage type="1" amount="2" />  <!-- Type 4 LR Nuke-->
    //<torpedo_storage type="2" amount="6" />  <!-- Type 6 Mine"-->
    //<torpedo_storage type="3" amount="4" />  <!-- Type 9 ECM"-->
                case 0:
                    retVal = "Homing";
                    break;
                case 1:
                    retVal = "LR Nuke";
                    break;
                case 2:
                    retVal = "Mine";
                    break;
                case 3:
                    retVal = "ECM";
                    break;
            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
