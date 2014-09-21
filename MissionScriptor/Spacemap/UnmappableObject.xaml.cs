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
using System.Collections.ObjectModel;
using RussLibrary;
namespace MissionStudio.Spacemap
{
    /// <summary>
    /// Interaction logic for UnmappableObject.xaml
    /// </summary>
    public partial class UnmappableObject : UserControl
    {
        public UnmappableObject()
        {
            InitializeComponent();
        }
  

        static void OnCommandNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UnmappableObject me = sender as UnmappableObject;
            if (me != null)
            {
                me.Attributes = new ObservableCollection<PropertyItem>();
                foreach (PropertyItem item in PropertyItem.GetCommandProperties(me.CommandName, (SpaceObjectType)0))
                {
                    me.Attributes.Add(item);
                }
            }
        }
        public static readonly DependencyProperty CommandNameProperty =
           DependencyProperty.Register("CommandName", typeof(string),
           typeof(UnmappableObject), new PropertyMetadata(OnCommandNameChanged));
        public string CommandName
        {
            get
            {
                return (string)this.UIThreadGetValue(CommandNameProperty);

            }
            set
            {
                this.UIThreadSetValue(CommandNameProperty, value);

            }
        }

        public static readonly DependencyProperty MappableObjectProperty =
          DependencyProperty.Register("MappableObject", typeof(SpaceObject),
          typeof(UnmappableObject));
        public SpaceObject MappableObject
        {
            get
            {
                return (SpaceObject)this.UIThreadGetValue(MappableObjectProperty);

            }
            set
            {
                this.UIThreadSetValue(MappableObjectProperty, value);

            }
        }
        public static readonly DependencyProperty AttributesProperty =
            DependencyProperty.Register("Attributes", typeof(ObservableCollection<PropertyItem>),
            typeof(UnmappableObject));

        public ObservableCollection<PropertyItem> Attributes
        {
            get
            {
                return (ObservableCollection<PropertyItem>)this.UIThreadGetValue(AttributesProperty);

            }
            set
            {
                this.UIThreadSetValue(AttributesProperty, value);

            }
        }

    }
}
