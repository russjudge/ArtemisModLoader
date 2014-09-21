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

namespace ArtemisModLoader.Windows
{
    /// <summary>
    /// Interaction logic for ModSelector.xaml
    /// </summary>
    public partial class ModSelector : Window
    {
        public ModSelector()
        {
            InitializeComponent();
            ImageBrush brsh = ArtemisModLoader.Helpers.FileHelper.GetRandomSkybox();
            if (brsh != null)
            {
                this.Background = brsh;
            }
        }

        public static readonly DependencyProperty ShowStockProperty =
           DependencyProperty.Register("ShowStock", typeof(bool),
           typeof(ModSelector), new PropertyMetadata(false));

        public bool ShowStock
        {
            get
            {
                return (bool)this.UIThreadGetValue(ShowStockProperty);

            }
            set
            {
                this.UIThreadSetValue(ShowStockProperty, value);

            }
        }
        public static readonly DependencyProperty SelectedConfigurationProperty =
            DependencyProperty.Register("SelectedConfiguration", typeof(ModConfiguration),
            typeof(ModSelector));

        public ModConfiguration SelectedConfiguration
        {
            get
            {
                return (ModConfiguration)this.UIThreadGetValue(SelectedConfigurationProperty);

            }
            set
            {
                this.UIThreadSetValue(SelectedConfigurationProperty, value);

            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedConfiguration == null)
            {
                Locations.MessageBoxShow(AMLResources.Properties.Resources.PleaseSelect, MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            else
            {
                DialogResult = true;
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        

       
        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedConfiguration != null)
            {
                DialogResult = true;

            }
            this.Close();
        }
    }
}
