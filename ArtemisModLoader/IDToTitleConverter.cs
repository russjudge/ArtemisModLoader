using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace ArtemisModLoader
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID"), ValueConversion(typeof(string), typeof(string))]
    public class IDToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string id = value as string;
            string title = string.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                title = AMLResources.Properties.Resources.DependsOnNotInstalled;
                bool found = false;
                foreach (ModConfiguration config in InstalledModConfigurations.Instance.Configurations)
                {
                    if (config.ID == id)
                    {
                        title = string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.DependsOn, config.Title);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    foreach (string configID in PredefinedMods.PredefinedModDictionary.Keys)
                    {
                        if (configID == id)
                        {
                            found = true;
                            title = string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.DependsOn, PredefinedMods.PredefinedModDictionary[configID].Title);
                            break;
                        }
                    }
                }
            }
            return title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
