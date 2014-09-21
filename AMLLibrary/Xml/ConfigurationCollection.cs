using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.WPF;
using RussLibrary;
using RussLibrary.Xml;
using System.Collections.ObjectModel;
using System.Windows;

namespace ArtemisModLoader.Xml
{
    public class ConfigurationCollection : ChangeDependencyObject
    {
        public ConfigurationCollection()
        {
            Configurations = new ObservableCollection<ModConfiguration>();
        }
        public static readonly DependencyProperty ConfigurationsProperty =
            DependencyProperty.Register("Configurations", typeof(ObservableCollection<ModConfiguration>),
            typeof(ActiveModConfigurations), new UIPropertyMetadata(OnItemChanged));
       [XmlConversion("ModConfiguration")]
        public ObservableCollection<ModConfiguration> Configurations
        {
            get
            {
                return (ObservableCollection<ModConfiguration>)this.UIThreadGetValue(ConfigurationsProperty);

            }
            private set
            {
                this.UIThreadSetValue(ConfigurationsProperty, value);

            }
        }
    }
}
