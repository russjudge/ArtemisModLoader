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
using System.IO;
using Microsoft.Win32;
using ArtemisModLoader.Windows;
namespace ArtemisModLoader.Controls
{
    /// <summary>
    /// Interaction logic for ModDefiner.xaml
    /// </summary>
    public partial class ModDefiner : UserControl
    {
        public ModDefiner()
        {
            
            InitializeComponent();
            Configuration = new ModConfiguration();
        }

        public static readonly DependencyProperty ForDevelopmentProperty =
           DependencyProperty.Register("ForDevelopment", typeof(bool),
           typeof(ModDefiner));


        public bool ForDevelopment
        {
            get
            {
                return (bool)this.UIThreadGetValue(ForDevelopmentProperty);

            }
            set
            {
                this.UIThreadSetValue(ForDevelopmentProperty, value);

            }
        }


        //ID
        //zip package
        //Title
        //Description
        //Download source
        //Download Web page.

        static void OnConfigurationChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ModDefiner me = sender as ModDefiner;
            if (me != null && me.Configuration != null && me.Configuration.DependsOn != null && me.Configuration.DependsOn.Count > 0)
            {
                me.cbDependsOn.SelectedItem = me.Configuration.DependsOn[0];
            }
        }
        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(ModConfiguration),
            typeof(ModDefiner), new PropertyMetadata(OnConfigurationChange));


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

        private void NewGUID_click(object sender, RoutedEventArgs e)
        {
            Configuration.ID = "{" + Guid.NewGuid().ToString() + "}";
        }


        private void Filename_click(object sender, RoutedEventArgs e)
        {
            Configuration.ID = new FileInfo(Configuration.PackagePath).Name.Replace('.', '~');
        }

        private void AddDependsOn_Click(object sender, RoutedEventArgs e)
        {
            ModSelector win = new ModSelector();
            if (win.ShowDialog() == true)
            {
                if (win.SelectedConfiguration != null)
                {
                    Configuration.DependsOn.Add(new StringItem(win.SelectedConfiguration.ID));
                    cbDependsOn.SelectedItem = Configuration.DependsOn[0];
                }
            }
        }

        private void RemoveDependsOn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                StringItem dependsOn = btn.CommandParameter as StringItem;
                if (dependsOn != null)
                {
                    Configuration.DependsOn.Remove(dependsOn);
                }
            }
        }

      

      

    }
}