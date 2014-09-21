using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArtemisModLoader.Xml;
using RussLibrary;
namespace ArtemisModLoader.Controls
{
    /// <summary>
    /// Interaction logic for ModDescriptorControl.xaml
    /// </summary>
    public partial class ModDescriptorControl : UserControl
    {
        public ModDescriptorControl()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(ModConfiguration),
            typeof(ModDescriptorControl));

        public ModConfiguration Configuration
        {
            get
            {
                return (ModConfiguration)this.UIThreadGetValue(ConfigurationProperty);

            }
            set
            {
                this.UIThreadSetValue(ConfigurationProperty, value);

            }
        }
    }
}