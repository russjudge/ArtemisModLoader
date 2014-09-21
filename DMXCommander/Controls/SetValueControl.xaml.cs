using DMXCommander.Xml;
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
namespace DMXCommander.Controls
{
    /// <summary>
    /// Interaction logic for SetValueControl.xaml
    /// </summary>
    public partial class SetValueControl : UserControl
    {
        public SetValueControl()
        {
            ChannelList = GeneralHelper.GetChannelList();
            InitializeComponent();
            NumberList = GeneralHelper.Get0thru255();
            GeneralHelper.ChannelListChanged += new EventHandler(GeneralHelper_ChannelListChanged);
           
        }

        void GeneralHelper_ChannelListChanged(object sender, EventArgs e)
        {
            ChannelList = GeneralHelper.GetChannelList();
        }

      
        public static readonly DependencyProperty NumberListProperty =
           DependencyProperty.Register("NumberList", typeof(ObservableCollection<byte>),
           typeof(SetValueControl));

        public ObservableCollection<byte> NumberList
        {
            get
            {
                return (ObservableCollection<byte>)this.UIThreadGetValue(NumberListProperty);

            }
            set
            {
                this.UIThreadSetValue(NumberListProperty, value);

            }
        }



        public static readonly DependencyProperty ChannelListProperty =
           DependencyProperty.Register("ChannelList", typeof(ListCollectionView),
           typeof(SetValueControl), new PropertyMetadata());

        public ListCollectionView ChannelList
        {
            get
            {
                return (ListCollectionView)this.UIThreadGetValue(ChannelListProperty);

            }
            set
            {
                this.UIThreadSetValue(ChannelListProperty, value);

            }
        }
        

        public static readonly DependencyProperty DataProperty =
           DependencyProperty.Register("Data", typeof(SetValue),
           typeof(SetValueControl));

        public SetValue Data
        {
            get
            {
                return (SetValue)this.UIThreadGetValue(DataProperty);

            }
            set
            {
                this.UIThreadSetValue(DataProperty, value);

            }
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

      
      

      
    }
}
