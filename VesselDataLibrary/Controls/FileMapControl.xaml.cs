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
using ArtemisModLoader.Helpers;
using ArtemisModLoader;
using System.Collections.ObjectModel;
namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for FileMapControl.xaml
    /// </summary>
    public partial class FileMapControl : UserControl
    {
        public FileMapControl()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty SearchPrefixesProperty =
          DependencyProperty.Register("SearchPrefixes", typeof(ObservableCollection<string>),
          typeof(FileMapControl));

        public ObservableCollection<string> SearchPrefixes
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(SearchPrefixesProperty);

            }
            set
            {
                this.UIThreadSetValue(SearchPrefixesProperty, value);

            }
        }


        static void OnConfigurationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FileMapControl v = sender as FileMapControl;
            if (v != null && v.Configuration != null)
            {
                v.SearchPrefixes = new ObservableCollection<string>(ModManagement.SearchPrefixes(v.Configuration));
            }
        }
        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(ModConfiguration),
            typeof(FileMapControl), new PropertyMetadata(OnConfigurationChanged));

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

        public static readonly DependencyProperty FileMappingProperty =
            DependencyProperty.Register("FileMapping", typeof(FileMap),
            typeof(FileMapControl));

        public FileMap FileMapping
        {
            get
            {
                return (FileMap)this.UIThreadGetValue(FileMappingProperty);

            }
            set
            {
                this.UIThreadSetValue(FileMappingProperty, value);

            }
        }

        static void OnUseWildcardChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FileMapControl me = sender as FileMapControl;
            if (me != null)
            {
                if (me.UseWildcard)
                {
                    string f = me.FileMapping.Source;
                    int i = f.LastIndexOf('\\');
                    if (i >= 0)
                    {
                        f = f.Substring(0, i + 1) + "*";
                    }
                    else
                    {
                        f = "*";
                    }
                    me.FileMapping.Source = f;
                }
            }
        }
        public static readonly DependencyProperty UseWildcardProperty =
            DependencyProperty.Register("UseWildcard", typeof(bool),
            typeof(FileMapControl), new PropertyMetadata(OnUseWildcardChanged));

        public bool UseWildcard
        {
            get
            {
                return (bool)this.UIThreadGetValue(UseWildcardProperty);

            }
            set
            {
                this.UIThreadSetValue(UseWildcardProperty, value);

            }
        }


        private void OnInvalidFilePath(object sender, RoutedEventArgs e)
        {
            FileHelper.FileSelectionControl_InvalidFilePath(sender, e, Configuration);
        }

        private void OnInvalidTargetPath(object sender, RoutedEventArgs e)
        {
            Locations.MessageBoxShow("Invalid target destination.\r\nPlease select a new location.", MessageBoxButton.OK, MessageBoxImage.Hand);
        }

        private void OnFileChanged(object sender, RoutedEventArgs e)
        {
            string f = e.OriginalSource as string;
            if (string.IsNullOrEmpty(FileMapping.Target))
            {
                FileMapping.Target = f;
            }
            if (!string.IsNullOrEmpty(f) && f.EndsWith("*"))
            {
                UseWildcard = true;
            }
            else
            {
                UseWildcard = false;
            }
        }

      
    }
}
