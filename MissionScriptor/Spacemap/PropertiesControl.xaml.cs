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
using System.Collections.ObjectModel;


namespace MissionStudio.Spacemap
{
    /// <summary>
    /// Interaction logic for PropertiesControl.xaml
    /// </summary>
    public partial class PropertiesControl : UserControl
    {
        public PropertiesControl()
        {
            InitializeComponent();
        }
        static void OnPropertyCollectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
        public static readonly DependencyProperty PropertyCollectionProperty =
         DependencyProperty.Register("PropertyCollection", typeof(ObservableCollection<PropertyItem>),
         typeof(PropertiesControl), new PropertyMetadata(OnPropertyCollectionChanged));
        public ObservableCollection<PropertyItem> PropertyCollection
        {
            get
            {
                return (ObservableCollection<PropertyItem>)this.UIThreadGetValue(PropertyCollectionProperty);

            }
            set
            {
                this.UIThreadSetValue(PropertyCollectionProperty, value);

            }
        }
    }
}
