using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ArtemisModLoader.Xml;
using RussLibrary;

namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for BaseFileMappingControl.xaml
    /// </summary>
    public partial class BaseFileMappingControl : UserControl
    {
        public BaseFileMappingControl()
        {
            //Mappings = new ObservableCollection<FileMap>();
            InitializeComponent();
        }



        public static readonly DependencyProperty ForSubModProperty =
            DependencyProperty.Register("ForSubMod", typeof(bool),
            typeof(BaseFileMappingControl));

        public bool ForSubMod
        {
            get
            {
                return (bool)this.UIThreadGetValue(ForSubModProperty);

            }
            set
            {
                this.UIThreadSetValue(ForSubModProperty, value);

            }
        }




        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(ModConfiguration),
            typeof(BaseFileMappingControl));

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

        static void OnMappingsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }
        public static readonly DependencyProperty MappingsProperty =
            DependencyProperty.Register("Mappings", typeof(FileMapCollection),
            typeof(BaseFileMappingControl), new PropertyMetadata(OnMappingsChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public FileMapCollection  Mappings
        {
            get
            {
                return (FileMapCollection)this.UIThreadGetValue(MappingsProperty);

            }
            set
            {
                this.UIThreadSetValue(MappingsProperty, value);

            }
        }

        private void DeleteMapping_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                FileMap fm = btn.CommandParameter as FileMap;
                if (fm != null)
                {
                    Mappings.Remove(fm);
                }
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            FileMap fm = new FileMap(string.Empty, string.Empty, ForSubMod);
            Mappings.Add(fm);
        }

    }
}
