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
using VesselDataLibrary.Xml;
using System.Collections.ObjectModel;

namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for TubeStorageControl.xaml
    /// </summary>
    public partial class TubeStorageControl : UserControl
    {

    //     <torpedo_storage type="0" amount="8" />  <!-- Type 1 Homing"-->
    //<torpedo_storage type="1" amount="2" />  <!-- Type 4 LR Nuke-->
    //<torpedo_storage type="2" amount="6" />  <!-- Type 6 Mine"-->
    //<torpedo_storage type="3" amount="4" />  <!-- Type 9 ECM"-->


        public TubeStorageControl()
        {
            InitializeComponent();
        }



        public static readonly DependencyProperty TorpedosProperty =
         DependencyProperty.Register("Torpedos", typeof(TorpedoStorageCollection),
         typeof(TubeStorageControl));

        public TorpedoStorageCollection Torpedos
        {
            get
            {
                return (TorpedoStorageCollection)this.UIThreadGetValue(TorpedosProperty);
            }
            set
            {
                this.UIThreadSetValue(TorpedosProperty, value);
            }
        }

    }
}
