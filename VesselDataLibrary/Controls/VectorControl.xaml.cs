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
namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for VectorControl.xaml
    /// </summary>
    public partial class VectorControl : UserControl
    {
        public VectorControl()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty VectorProperty =
            DependencyProperty.Register("Vector", typeof(VectorObject),
            typeof(VectorControl));
        
        public VectorObject Vector
        {
            get
            {
                return (VectorObject)this.UIThreadGetValue(VectorProperty);

            }
            set
            {
                this.UIThreadSetValue(VectorProperty, value);

            }
        }
    }
}
