using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;
using RussLibrary.WPF;

namespace RussLibrary.ValueConverters
{
    [ValueConversion(typeof(ValidationObjectCollection), typeof(ValidationObject))]
    public class PropertyToValidationConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Parameter contains name of property to return validation on.
            //Value must be the ValidationObjectCollection.
            ValidationObject retVal = null;
            ValidationObjectCollection val = value as ValidationObjectCollection;
            if (val != null && parameter != null)
            {
                retVal = val.GetValidationResult(parameter.ToString());
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
