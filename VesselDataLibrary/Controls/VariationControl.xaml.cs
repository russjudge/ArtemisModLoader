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
using RussLibrary;
using ArtemisModLoader.Xml;

namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for VariationsControl.xaml
    /// </summary>
    public partial class VariationControl : UserControl
    {
        public VariationControl()
        {
            InitializeComponent();
        }



        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(ModConfiguration),
            typeof(VariationControl));

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




        public static readonly DependencyProperty VariationProperty =
           DependencyProperty.Register("Variation", typeof(SubMod),
           typeof(VariationControl));

        
        public SubMod Variation
        {
            get
            {
                return (SubMod)this.UIThreadGetValue(VariationProperty);

            }
            set
            {
                this.UIThreadSetValue(VariationProperty, value);

            }
        }



    }
}
