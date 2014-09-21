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
using VesselDataLibrary.Xml;
using RussLibrary;
namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for BeamPortDataEntryControl.xaml
    /// </summary>
    public partial class BeamPortDataEntryControl : UserControl
    {
        public BeamPortDataEntryControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty BeamProperty =
            DependencyProperty.Register("Beam", typeof(BeamPort),
            typeof(BeamPortDataEntryControl));

        public BeamPort Beam
        {
            get
            {
                return (BeamPort)this.UIThreadGetValue(BeamProperty);

            }
            set
            {
                this.UIThreadSetValue(BeamProperty, value);

            }
        }
    }
}
