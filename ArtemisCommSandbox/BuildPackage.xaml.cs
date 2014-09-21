using ArtemisComm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArtemisCommSandbox
{
    /// <summary>
    /// Interaction logic for BuildPackage.xaml
    /// </summary>
    public partial class BuildPackage : Window
    {
        public BuildPackage()
        {
            PacketList = new ObservableCollection<KeyValuePair<string, IPackageSelector>>();
            //PacketList.Add(new KeyValuePair<string,Packet>("CommsIncomingPacket", ))

            foreach (Type t in Assembly.GetAssembly(this.GetType()).GetTypes())
            {
                if (t.Name.EndsWith("PacketControl"))
                {
                    ConstructorInfo constructor = t.GetConstructor(new Type[0]);
                    object obj = constructor.Invoke(new object[0]);

                    PacketList.Add(new KeyValuePair<string, IPackageSelector>(t.Name, (IPackageSelector)obj));
                }
            }

            InitializeComponent();
        }
  

        public static readonly DependencyProperty SendToServerProperty =
          DependencyProperty.Register("SendToServer", typeof(bool),
              typeof(BuildPackage));

        public bool SendToServer
        {
            get
            {
                return (bool)this.GetValue(SendToServerProperty);

            }
            set
            {
                this.SetValue(SendToServerProperty, value);

            }
        }

        public static readonly DependencyProperty PacketListProperty =
          DependencyProperty.Register("PacketList", typeof(ObservableCollection<KeyValuePair<string, IPackageSelector>>),
              typeof(BuildPackage));

        public ObservableCollection<KeyValuePair<string, IPackageSelector>> PacketList
        {
            get
            {
                return (ObservableCollection<KeyValuePair<string, IPackageSelector>>)this.GetValue(PacketListProperty);

            }
            set
            {
                this.SetValue(PacketListProperty, value);

            }
        }

        private void OnSend(object sender, RoutedEventArgs e)
        {

        }
    }
}
