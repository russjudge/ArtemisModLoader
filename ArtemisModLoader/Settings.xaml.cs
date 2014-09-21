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
using System.Windows.Shapes;
using Microsoft.Win32;
using RussLibrary;
namespace ArtemisModLoader
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            ArtemisInstallLocation = Locations.ArtemisInstallPath;
            InitializeComponent();
            this.Title = Locations.AssemblyTitle + " Settings";
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog diag = new System.Windows.Forms.FolderBrowserDialog();
            diag.ShowNewFolderButton = false;
            
            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ArtemisInstallLocation = diag.SelectedPath;
                

            }
        }
        static void OnArtemisInstallLocationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Settings me = sender as Settings;
            if (me != null)
            {
                Locations.SetArtemisInstallPath(me.ArtemisInstallLocation);
                Locations.UseArtemisExtender = me.UseArtemisExtender;
                Locations.ArtemisExtenderPath = me.ArtemisExtenderPath;
            }
        }

        public static readonly DependencyProperty ArtemisInstallLocationProperty =
          DependencyProperty.Register("ArtemisInstallLocation", typeof(string),
          typeof(Settings), new PropertyMetadata(OnArtemisInstallLocationChanged));

        public string ArtemisInstallLocation
        {
            get
            {
                return (string)this.UIThreadGetValue(ArtemisInstallLocationProperty);

            }
            private set
            {
                this.UIThreadSetValue(ArtemisInstallLocationProperty, value);

            }
        }
        public static readonly DependencyProperty ArtemisExtenderPathProperty =
          DependencyProperty.Register("ArtemisExtenderPath", typeof(string),
          typeof(Settings), new PropertyMetadata(OnArtemisInstallLocationChanged));

        public string ArtemisExtenderPath
        {
            get
            {
                return (string)this.UIThreadGetValue(ArtemisExtenderPathProperty);

            }
            private set
            {
                this.UIThreadSetValue(ArtemisExtenderPathProperty, value);

            }
        }

        public static readonly DependencyProperty UseArtemisExtenderProperty =
         DependencyProperty.Register("UseArtemisExtender", typeof(bool),
         typeof(Settings), new PropertyMetadata(OnArtemisInstallLocationChanged));

        public bool UseArtemisExtender
        {
            get
            {
                return (bool)this.UIThreadGetValue(UseArtemisExtenderProperty);

            }
            private set
            {
                this.UIThreadSetValue(UseArtemisExtenderProperty, value);

            }
        }

        private void Extender_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = Locations.AssemblyTitle;
            diag.Multiselect = false;
            if (diag.ShowDialog() == true)
            {
                ArtemisExtenderPath = diag.FileName;


            }
        }
    }
}
