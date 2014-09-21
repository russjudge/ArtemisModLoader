using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Data;
using ArtemisModLoader.Xml;
using System.Globalization;
using System.Collections;

namespace ArtemisModLoader.ValueConverters
{
    [ValueConversion(typeof(string), typeof(string))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID"), ValueConversion(typeof(string), typeof(string))]
    public class IDToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string id = null;
            string title = string.Empty;
            IList<StringItem> l = value as IList<StringItem>;
            if (l != null)
            {
                if (l.Count > 0)
                {
                    id = l[0].Text;
                }
            }
            else
            {
                id = value as string;
            }


            if (!string.IsNullOrEmpty(id))
            {
                title = AMLResources.Properties.Resources.DependsOnNotInstalled;
                bool found = false;
                foreach (ModConfiguration config in InstalledModConfigurations.Current.Configurations.Configurations)
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
                    foreach (ModConfiguration configID in ModManagement.GetPredefinedMods().Values)
                    {
                        if (configID.ID == id)
                        {
                            found = true;
                            title = string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.DependsOn, configID.Title);
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
