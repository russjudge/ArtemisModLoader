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
    public class ConfigurationGroup : DependencyObject
    {
        public ConfigurationGroup()
        {
            
            Configurations = new ObservableCollection<ModConfiguration>();
            
        }
        public static readonly DependencyProperty ConfigurationsProperty =
            DependencyProperty.Register("Configurations", typeof(ObservableCollection<ModConfiguration>),
            typeof(ConfigurationGroup));
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
