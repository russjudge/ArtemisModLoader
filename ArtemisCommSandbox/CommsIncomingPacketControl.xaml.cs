using ArtemisComm;
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

namespace ArtemisCommSandbox
{
    /// <summary>
    /// Interaction logic for CommsIncomingPacketControl.xaml
    /// </summary>
    public partial class CommsIncomingPacketControl : UserControl, IPackageSelector
    {
        public CommsIncomingPacketControl()
        {
            
            Package = new ArtemisComm.Packet(new CommsIncomingPacket());
            InitializeComponent();
        }

        public static readonly DependencyProperty PackageProperty =
         DependencyProperty.Register("Package", typeof(ArtemisComm.Packet),
             typeof(CommsIncomingPacketControl));

        public ArtemisComm.Packet Package
        {
            get
            {
                return (ArtemisComm.Packet)this.GetValue(PackageProperty);

            }
            set
            {
                this.SetValue(PackageProperty, value);

            }
        }

    }
}
